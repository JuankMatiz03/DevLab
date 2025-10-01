using DevLab.Api.Models;

namespace DevLab.Api.Services;

/// <summary>
/// servicio de facturacion a nivel de aplicacion
/// expone operacion para crear factura
/// </summary>
public interface IInvoiceService
{
    /// <summary>
    /// crea una factura y retorna su id
    /// </summary>
    Task<int> CreateAsync(CreateInvoiceRequest req);
}
