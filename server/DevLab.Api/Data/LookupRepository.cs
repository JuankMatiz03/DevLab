using DevLab.Api.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevLab.Api.Data;

/// <summary>
/// repositorio de catalogos lookups con ado net y procedimientos almacenados
/// </summary>
public sealed class LookupRepository : ILookupRepository
{
    private readonly ISqlConnectionFactory _factory;

    /// <summary>
    /// inyecta la fabrica de conexiones
    /// </summary>
    public LookupRepository(ISqlConnectionFactory factory) => _factory = factory;

    /// <summary>
    /// obtiene lista de clientes activos ordenada por nombre
    /// retorna lista de solo lectura de customer dto
    /// </summary>
    public async Task<IReadOnlyList<CustomerDto>> GetCustomersAsync()
    {
        using var cn = _factory.Create();
        using var cmd = new SqlCommand("dbo.usp_Customers_List", cn)
        { CommandType = CommandType.StoredProcedure };
        await cn.OpenAsync();

        var list = new List<CustomerDto>();
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
            list.Add(new CustomerDto(rd.GetInt32(0), rd.GetString(1)));
        return list;
    }

    /// <summary>
    /// obtiene lista de productos activos ordenada por nombre 
    /// incluye precio e imagen cuando aplica
    /// retorna lista de solo lectura de product dto
    /// </summary>
    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync()
    {
        using var cn = _factory.Create();
        using var cmd = new SqlCommand("dbo.usp_Products_List", cn)
        { CommandType = CommandType.StoredProcedure };
        await cn.OpenAsync();

        var list = new List<ProductDto>();
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
            list.Add(new ProductDto(
                rd.GetInt32(0),
                rd.GetString(1),
                rd.GetDecimal(2),
                rd.IsDBNull(3) ? null : rd.GetString(3)
            ));
        return list;
    }
}
