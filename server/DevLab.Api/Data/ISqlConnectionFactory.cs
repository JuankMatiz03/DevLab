using Microsoft.Data.SqlClient;

namespace DevLab.Api.Data;

/// <summary>
/// fabrica de conexiones sql para la aplicacion
/// provee un metodo para obtener SqlConnection basado en la configuracion
/// </summary>
public interface ISqlConnectionFactory
{
    /// <summary>
    /// crea y retorna una instancia de SqlConnection usando la cadena configurada
    /// la conexion se retorna sin abrir
    /// </summary>
    SqlConnection Create();
}

/// <summary>
/// implementacion de la fabrica de conexiones usando IConfiguration
/// lee la cadena de conexion llamada LabDev
/// </summary>
public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    /// <summary>
    /// cadena de conexion obtenida de la configuracion
    /// </summary>
    private readonly string _cs;

    /// <summary>
    /// constructor que inyecta IConfiguration y toma la cadena LabDev
    /// </summary>
    public SqlConnectionFactory(IConfiguration cfg)
    {
        _cs = cfg.GetConnectionString("LabDev")!;
    }

    /// <summary>
    /// crea una nueva instancia de SqlConnection con la cadena configurada
    /// </summary>
    public SqlConnection Create() => new SqlConnection(_cs);
}
