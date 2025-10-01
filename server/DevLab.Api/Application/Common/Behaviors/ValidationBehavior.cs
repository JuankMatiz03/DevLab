using FluentValidation;
using MediatR;

namespace DevLab.Api.Application.Common.Behaviors;

/// <summary>
/// validaciOn para el pipeline de MediatR
/// ejecuta todos los validadores de FluentValidation antes del handler
/// si hay errores, lanza <see cref="ValidationException"/>, de lo contrario, continua la cadena.
/// </summary>
/// <typeparam name="TRequest">tipo de la solicitud</typeparam>
/// <typeparam name="TResponse">tipo de la respuesta</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// inyecta la coleccion de validadores aplicables a <typeparamref name="TRequest"/>.
    /// </summary>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    /// <summary>
    /// ejecuta validaciones, si son validas, delega en el siguiente handler del pipeline
    /// </summary>
    /// <param name="request">solicitud a validar</param>
    /// <param name="next">delegado para continuar con el pipeline</param>
    /// <param name="ct">token de cancelacion</param>
    /// <returns>respuesta generada por el siguiente handler si la validación es exitosa</returns>
    /// <exception cref="ValidationException">cuando existen errores de validacion</exception>
    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        // si no hay validadores registrados continua
        if (_validators.Any())
        {
            // contexto de validacion para la request actual
            var ctx = new ValidationContext<TRequest>(request);

            // ejecuta todos los validadores en paralelo y junta sus errores
            var errors = (
                await Task.WhenAll(_validators.Select(v => v.ValidateAsync(ctx, ct)))
            )
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

            // si hay errores se interrumpe el pipeline
            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
        }

        // sin errores, continua con el siguiente paso del pipeline
        return await next();
    }
}
