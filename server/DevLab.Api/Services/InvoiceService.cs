using DevLab.Api.Data;
using DevLab.Api.Models;

namespace DevLab.Api.Services;

/// <summary>
/// servicio de facturacion a nivel de aplicacion
/// valida reglas basicas y delega en el repositorio
/// </summary>
public sealed class InvoiceService : IInvoiceService
{
    /// <summary>
    /// repositorio de facturas
    /// </summary>
    private readonly IInvoiceRepository _repo;

    /// <summary>
    /// inyeccion del repositorio de facturas
    /// </summary>
    public InvoiceService(IInvoiceRepository repo) => _repo = repo;

    /// <summary>
    /// crea una factura y retorna su id
    /// valida que exista al menos un item
    /// </summary>
    /// <param name="req">datos de la factura</param>
    /// <returns>id generado</returns>
    public Task<int> CreateAsync(CreateInvoiceRequest req)
    {
        if (req.Items.Count == 0)
            throw new InvalidOperationException("Invoice requires at least one item.");
        return _repo.CreateAsync(req);
    }
}
