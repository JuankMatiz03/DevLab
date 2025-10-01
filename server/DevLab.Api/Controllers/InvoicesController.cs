using DevLab.Api.Data;
using DevLab.Api.Models;
using DevLab.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevLab.Api.Controllers;

/// <summary>
/// endpoints de facturacion, creación y busqueda de facturas.
/// </summary>
[ApiController]
[Route("api/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceRepository _repo;
    private readonly IInvoiceService _svc;

    /// <summary>
    /// inyeccion de repositorio, consultas y servicio creacion-validacion
    /// </summary>
    public InvoicesController(IInvoiceRepository repo, IInvoiceService svc)
    {
        _repo = repo; _svc = svc;
    }

    /// <summary>
    /// post /api/invoices
    /// crea una factura con sus items
    /// </summary>
    /// <param name="req">numero, cliente y items producto, cantidad, precio</param>
    /// <returns>id de la factura creada o error de validacion</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest req)
    {
        try
        {
            var id = await _svc.CreateAsync(req);
            return Ok(new { id });
        }
        catch (Exception ex)
        {
            // respuesta del problema 
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// get /api/invoices/search
    /// busqueda de facturas por cliente o por numero
    /// </summary>
    /// <param name="customerId">id de cliente, devuelve lista de encabezados</param>
    /// <param name="number">numero de factura, devuelve encabezado + detalle</param>
    /// <returns>
    /// si viene <c>customerId</c>, lista de facturas del cliente
    /// si viene <c>number</c>: factura especifica con detalle
    /// si no viene ninguno: error 400 solicitud invalida
    /// </returns>
    // /api/invoices/search?customerId=1  OR  /api/invoices/search?number=1001
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] int? customerId, [FromQuery] int? number)
    {
        if (customerId.HasValue)
            return Ok(await _repo.SearchByCustomerAsync(customerId.Value));

        if (number.HasValue)
            return Ok(await _repo.SearchByNumberAsync(number.Value));

        return BadRequest("Provide either customerId or number.");
    }
}
