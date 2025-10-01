using FluentValidation;

namespace DevLab.Api.Application.Invoices.Commands.CreateInvoice;

/// <summary>
/// validador de <see cref="CreateInvoiceCommand"/> con FluentValidation
/// reglas minimas, numeros positivos y al menos un item valido
/// </summary>
public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        // cabecera
        RuleFor(x => x.Number).GreaterThan(0);
        RuleFor(x => x.CustomerId).GreaterThan(0);

        // Debe traer al menos un item
        RuleFor(x => x.Items).NotEmpty();

        // reglas por item: campos requeridos y > 0
        RuleForEach(x => x.Items).ChildRules(i =>
        {
            i.RuleFor(y => y.ProductId).GreaterThan(0);
            i.RuleFor(y => y.Quantity).GreaterThan(0);
            i.RuleFor(y => y.UnitPrice).GreaterThan(0);
        });
    }
}
