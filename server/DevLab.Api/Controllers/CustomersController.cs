using DevLab.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace DevLab.Api.Controllers;

/// <summary>
/// endpoints de catalogo de clientes lookup.
/// </summary>
[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ILookupRepository _repo;

    /// <summary>
    /// inyeccion del repositorio de lookups via SP
    /// </summary>
    public CustomersController(ILookupRepository repo) => _repo = repo;

    /// <summary>
    /// get /api/customers
    /// lista de clientes activos ordenados por nombre
    /// uso, data de clientes para el front
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _repo.GetCustomersAsync());
}
