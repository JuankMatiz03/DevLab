namespace DevLab.Api.Models;

public class InvoiceItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateInvoiceRequest
{
    public int Number { get; set; }
    public int CustomerId { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
}

public record InvoiceHeaderDto(
    int Id,
    int Number,
    string Customer,
    decimal Subtotal,
    decimal Tax,
    decimal Total,
    DateTime CreatedAt
);

public record InvoiceDetailDto(
    int ProductId,
    string Name,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal,
    string? ImageUrl
);

public class SearchByNumberResponse
{
    public InvoiceHeaderDto? Header { get; set; }
    public List<InvoiceDetailDto> Details { get; set; } = new();
}

public class SearchByCustomerResponse
{
    public List<InvoiceHeaderDto> Items { get; set; } = new();
}
