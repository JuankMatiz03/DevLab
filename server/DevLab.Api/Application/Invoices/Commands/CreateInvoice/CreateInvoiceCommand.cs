using DevLab.Api.Models;
using MediatR;

namespace DevLab.Api.Application.Invoices.Commands.CreateInvoice;

/// <summary>
/// comando para crear una factura
/// </summary>
/// <param name="Number">numero de factura.</param>
/// <param name="CustomerId">id del cliente asociado</param>
/// <param name="Items">items de la factura, producto, cantidad, precio unitario</param>
/// <returns>id de la factura creada</returns>
/// <remarks>
/// se envía a través de MediatR y pasa por el pipeline de validación FluentValidation
/// </remarks>
public sealed record CreateInvoiceCommand(
    int Number,
    int CustomerId,
    List<InvoiceItemDto> Items
) : IRequest<int>;
