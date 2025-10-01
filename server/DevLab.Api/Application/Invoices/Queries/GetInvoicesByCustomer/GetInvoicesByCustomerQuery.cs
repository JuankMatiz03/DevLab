using DevLab.Api.Models;
using MediatR;

namespace DevLab.Api.Application.Invoices.Queries.GetInvoicesByCustomer;

/// <summary>
/// consulta para obtener las facturas de un cliente solo encabezados
/// </summary>
/// <param name="CustomerId">identificador del cliente</param>
/// <returns>lista de encabezados de facturas del cliente</returns>
public sealed record GetInvoicesByCustomerQuery(int CustomerId) : IRequest<SearchByCustomerResponse>;
