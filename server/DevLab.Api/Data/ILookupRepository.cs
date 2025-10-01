using DevLab.Api.Models;

namespace DevLab.Api.Data;

/// <summary>
/// repositorio de catálogos lookups .
/// implementación SP
/// </summary>
public interface ILookupRepository
{
    /// <summary>
    /// Bbtiene la lista de clientes activos ordenada por nombre
    /// Usa el SP <c>dbo.usp_Customers_List</c>
    /// </summary>
    /// <returns>Listado de <see cref="CustomerDto"/> para combos</returns>
    Task<IReadOnlyList<CustomerDto>> GetCustomersAsync();

    /// <summary>
    /// obtiene la lista de productos activos ordenada por nombre
    /// usa el SP <c>dbo.usp_Products_List</c>
    /// </summary>
    /// <returns>listado de <see cref="ProductDto"/></returns>
    Task<IReadOnlyList<ProductDto>> GetProductsAsync();
}
