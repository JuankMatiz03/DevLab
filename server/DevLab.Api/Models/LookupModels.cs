namespace DevLab.Api.Models;

public record CustomerDto(int Id, string Name);
public record ProductDto(int Id, string Name, decimal Price, string? ImageUrl);
