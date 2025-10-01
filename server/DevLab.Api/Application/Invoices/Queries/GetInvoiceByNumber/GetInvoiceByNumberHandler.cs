using DevLab.Api.Data;
using DevLab.Api.Models;
using MediatR;

namespace DevLab.Api.Application.Invoices.Queries.GetInvoiceByNumber;

/// <summary>
/// handler de la consulta <see cref="GetInvoiceByNumberQuery"/>
/// busca una factura por su numero y retorna encabezado + detalle
/// delega en el repositorio SP: <c>dbo.usp_Invoices_Search</c>
/// </summary>
public sealed class GetInvoiceByNumberHandler
    : IRequestHandler<GetInvoiceByNumberQuery, SearchByNumberResponse>
{
    private readonly IInvoiceRepository _repo;

    /// <summary>
    /// inyeccion del repositorio de facturas via SP
    /// </summary>
    public GetInvoiceByNumberHandler(IInvoiceRepository repo) => _repo = repo;

    /// <summary>
    /// Ejecuta la bUsqueda por nUmero de factura
    /// </summary>
    /// <param name="request">Consulta con el nUmero de factura</param>
    /// <param name="ct">Token de cancelacion</param>
    /// <returns>encabezado y detalle de la factura encontrada</returns>
    public Task<SearchByNumberResponse> Handle(
        GetInvoiceByNumberQuery request,
        CancellationToken ct
    ) => _repo.SearchByNumberAsync(request.Number);
}
