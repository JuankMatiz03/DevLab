using DevLab.Api.Data;
using DevLab.Api.Models;
using MediatR;

namespace DevLab.Api.Application.Invoices.Queries.GetInvoicesByCustomer;

/// <summary>
/// handler de la consulta <see cref="GetInvoicesByCustomerQuery"/>
/// obtiene las facturas de un cliente, ssolo encabezados en orden descendente por fecha
/// delega en el repositorio SP: <c>dbo.usp_Invoices_Search</c> con @CustomerId
/// </summary>
public sealed class GetInvoicesByCustomerHandler
    : IRequestHandler<GetInvoicesByCustomerQuery, SearchByCustomerResponse>
{
    private readonly IInvoiceRepository _repo;

    /// <summary>
    /// inyeccion del repositorio de facturas via SP
    /// </summary>
    public GetInvoicesByCustomerHandler(IInvoiceRepository repo) => _repo = repo;

    /// <summary>
    /// ejecuta la busqueda por id de cliente
    /// </summary>
    /// <param name="request">consulta con el identificador del cliente</param>
    /// <param name="ct">token de cancelacion</param>
    /// <returns>lista de encabezados de facturas del cliente</returns>
    public Task<SearchByCustomerResponse> Handle(
        GetInvoicesByCustomerQuery request,
        CancellationToken ct
    ) => _repo.SearchByCustomerAsync(request.CustomerId);
}
