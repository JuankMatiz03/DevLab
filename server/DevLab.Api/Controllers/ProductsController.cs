using DevLab.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace DevLab.Api.Controllers;

/// <summary>
/// Endpoints de catálogo de productos (lookup para combos).
/// </summary>
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ILookupRepository _repo;

    /// <summary>
    /// inyeccion del repositorio de lookups via SP
    /// </summary>
    public ProductsController(ILookupRepository repo) => _repo = repo;

    /// <summary>
    /// get /api/products
    /// lista de productos activos ordenados por nombre
    /// uso, poblar datos de productos en el front.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _repo.GetProductsAsync());
}
