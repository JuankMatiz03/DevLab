using DevLab.Api.Models;
using MediatR;

namespace DevLab.Api.Application.Invoices.Queries.GetInvoiceByNumber;

/// <summary>
/// consulta para obtener una factura por su numero
/// </summary>
/// <param name="Number">numero de la factura a buscar</param>
/// <returns>encabezado y detalle de la factura</returns>
public sealed record GetInvoiceByNumberQuery(int Number) : IRequest<SearchByNumberResponse>;
