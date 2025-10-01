using DevLab.Api.Models;

namespace DevLab.Api.Data;

/// <summary>
///  repositorio de facturas via SP
/// operaciones, crear y consultar facturas
/// </summary>
public interface IInvoiceRepository
{
    /// <summary>
    /// crea una factura y retorna su id
    /// usa el SP <c>dbo.usp_Invoices_Create</c>.
    /// </summary>
    /// <param name="req">numero, cliente y items de la factura</param>
    /// <returns>id generado para la factura</returns>
    Task<int> CreateAsync(CreateInvoiceRequest req);

    /// <summary>
    /// busca facturas por id de cliente solo encabezados
    /// usa el SP <c>dbo.usp_Invoices_Search</c> con <c>@CustomerId</c>
    /// </summary>
    /// <param name="customerId">identificador del cliente</param>
    /// <returns>listado de encabezados de facturas del cliente</returns>
    Task<SearchByCustomerResponse> SearchByCustomerAsync(int customerId);

    /// <summary>
    /// busca una factura por su numero, encabezado + detalle
    /// usa el SP <c>dbo.usp_Invoices_Search</c> con <c>@Number</c>
    /// </summary>
    /// <param name="number">numero de la factura</param>
    /// <returns>factura encontrada con encabezado y detalle</returns>
    Task<SearchByNumberResponse> SearchByNumberAsync(int number);
}
