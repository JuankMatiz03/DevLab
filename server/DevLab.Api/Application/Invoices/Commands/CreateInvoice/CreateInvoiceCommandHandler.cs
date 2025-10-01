using DevLab.Api.Data;
using MediatR;

namespace DevLab.Api.Application.Invoices.Commands.CreateInvoice;

/// <summary>
/// manejador del comando <see cref="CreateInvoiceCommand"/>
/// delega en el repositorio que invoca el SP <c>dbo.usp_Invoices_Create</c>
/// la validacion FluentValidation ocurre antes en el pipeline
/// </summary>
public sealed class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, int>
{
    private readonly IInvoiceRepository _repo;

    /// <summary>
    /// inyecta el repositorio de facturas
    /// </summary>
    public CreateInvoiceCommandHandler(IInvoiceRepository repo) => _repo = repo;

    /// <summary>
    /// ejecuta la creacion de la factura
    /// </summary>
    /// <param name="request">datos de la factura, numero, cliente, items</param>
    /// <param name="ct">token de cancelacion</param>
    /// <returns>id de la factura creada</returns>
    public Task<int> Handle(CreateInvoiceCommand request, CancellationToken ct)
    {
        // mapea el comando al DTO del repositorio y ejecuta el SP:
        //  - valida numero unico
        //  - calcula subtotal-IVA-total
        //  - inserta encabezado y detalle
        var req = new DevLab.Api.Models.CreateInvoiceRequest
        {
            Number     = request.Number,
            CustomerId = request.CustomerId,
            Items      = request.Items
        };

        return _repo.CreateAsync(req);
    }
}
