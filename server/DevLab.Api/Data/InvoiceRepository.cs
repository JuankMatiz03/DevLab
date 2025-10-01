using DevLab.Api.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevLab.Api.Data;

/// <summary>
/// repositorio de facturas con ado net y procedimientos almacenados
/// </summary

public sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly ISqlConnectionFactory _factory;

    // inyeccion de la fabrica de conexiones
    public InvoiceRepository(ISqlConnectionFactory factory) => _factory = factory;

    // crea una factura y retorna su id
    // usa el sp de creacion de factura
    public async Task<int> CreateAsync(CreateInvoiceRequest req)
    {
        using var cn = _factory.Create();
        await cn.OpenAsync();

        // arma tvp compatible con el tipo dbo InvoiceItemType
        var tvp = new DataTable();
        tvp.Columns.Add("ProductId", typeof(int));
        tvp.Columns.Add("Quantity", typeof(int));
        tvp.Columns.Add("UnitPrice", typeof(decimal));
        foreach (var i in req.Items)
            tvp.Rows.Add(i.ProductId, i.Quantity, i.UnitPrice);

        // configura comando para el sp de creacion
        using var cmd = new SqlCommand("dbo.usp_Invoices_Create", cn)
        { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Number", req.Number);
        cmd.Parameters.AddWithValue("@CustomerId", req.CustomerId);
        var p = cmd.Parameters.AddWithValue("@Items", tvp);
        p.SqlDbType = SqlDbType.Structured;
        p.TypeName = "dbo.InvoiceItemType";

        // ejecuta y lee el id generado
        using var rd = await cmd.ExecuteReaderAsync();
        int id = 0;
        if (await rd.ReadAsync()) id = rd.GetInt32(0);
        return id;
    }

    // busca facturas por id de cliente y retorna solo encabezados
    public async Task<SearchByCustomerResponse> SearchByCustomerAsync(int customerId)
    {
        using var cn = _factory.Create();
        using var cmd = new SqlCommand("dbo.usp_Invoices_Search", cn)
        { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@CustomerId", customerId);

        await cn.OpenAsync();
        var res = new SearchByCustomerResponse();

        // recorre el primer result set y agrega encabezados
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            res.Items.Add(new InvoiceHeaderDto(
                rd.GetInt32(0), rd.GetInt32(1), rd.GetString(2),
                rd.GetDecimal(3), rd.GetDecimal(4), rd.GetDecimal(5),
                rd.GetDateTime(6)
            ));
        }
        return res;
    }

    // busca una factura por su numero y retorna encabezado y detalle
    public async Task<SearchByNumberResponse> SearchByNumberAsync(int number)
    {
        using var cn = _factory.Create();
        using var cmd = new SqlCommand("dbo.usp_Invoices_Search", cn)
        { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Number", number);

        await cn.OpenAsync();
        var res = new SearchByNumberResponse();

        // primer result set encabezado
        using var rd = await cmd.ExecuteReaderAsync();
        if (await rd.ReadAsync())
        {
            res.Header = new InvoiceHeaderDto(
                rd.GetInt32(0), rd.GetInt32(1), rd.GetString(2),
                rd.GetDecimal(3), rd.GetDecimal(4), rd.GetDecimal(5),
                rd.GetDateTime(6)
            );
        }

        // segundo result set detalle si existe
        if (await rd.NextResultAsync())
        {
            while (await rd.ReadAsync())
            {
                res.Details.Add(new InvoiceDetailDto(
                    rd.GetInt32(0), rd.GetString(1), rd.GetInt32(2),
                    rd.GetDecimal(3), rd.GetDecimal(4),
                    rd.IsDBNull(5) ? null : rd.GetString(5)
                ));
            }
        }
        return res;
    }
}
