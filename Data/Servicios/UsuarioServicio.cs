using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using NetBlog.Data.Interfaces.IUsuario;
using NetBlog.Models;
using NetBlog.Models.ViewModels;

namespace NetBlog.Data.Servicios;

public class UsuarioServicio : IUsuario
{
    private readonly Contexto _contexto;
    public string ? ViewBag;
    

    public UsuarioServicio(Contexto contexto)
    {
        _contexto = contexto;
    }

    //Activacion cuenta usuario
    public int ActivarCuenta(string token, DateTime fechaexpiracion)
    {
        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            using (var cmd = new SqlCommand("ActivarCuenta", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Token", token);
                DateTime FechaExpiracion = DateTime.UtcNow.AddMinutes(5);
                cmd.Parameters.AddWithValue("@FechaExpiracion", FechaExpiracion);
                connection.Open();
                var resultado = cmd.ExecuteNonQuery();

                int activado = Convert.ToInt32(resultado);

                connection.Close();

                return activado;
            }
        }
    }

    public void ActualizarToken(string correo, DateTime fechaexpiracion, string Token)
    {
        throw new NotImplementedException();
    }

    public bool Login(LoginViewModel model)
    {
        bool passwordMatch = false;
        try
        {
            using (var connection = new SqlConnection(_contexto.Conexion))
            {
                using (var cmd = new SqlCommand("ValidarUsuario", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Correo", model.Correo);
                    //DateTime FechaExpiracion = DateTime.UtcNow.AddMinutes(5);
                    //cmd.Parameters.AddWithValue("@FechaExpiracion",FechaExpiracion);
                    connection.Open();

                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                passwordMatch = BCrypt.Net.BCrypt.Verify(
                                    model.Contrasenya,
                                    reader["Contrasenya"].ToString()
                                );

                                if (passwordMatch)
                                {
                                    DateTime fechaExpiracion = DateTime.UtcNow;

                                    if (
                                        !(bool)reader["Estado"]
                                        && reader["FechaExpiracion"].ToString()
                                            != fechaExpiracion.ToString()
                                    )
                                    {
                                        if (model.Correo != null)
                                        {
                                            ActualizarToken(model.Correo, fechaExpiracion);

                                            ViewBag = "Su cuenta no ha sido activada, se ha enviado un correo de activación verifique su bandeja de correo";
                                        }
                                    }
                                    else if (!(bool)reader["Estado"])

                                        ViewBag = "Su cuenta no ha sido activada, revise su bandeja de correo";
                                    else
                                    {
                                        string ? nombreusuario = reader["NombreUsuario"].ToString();
                                        int usuarioId = (int)reader["UsuarioId"];
                                        if(nombreusuario!=null)
                                        {
                                            var claims = new List<Claim>()
                                            {

                                                new Claim(ClaimTypes.NameIdentifier,nombreusuario),
                                                new Claim("UsuarioId",usuarioId.ToString())

                                            };
                                            int rolId = (int)reader["RolId"];
                                            string rolNombre = rolId == 1 ? "Administrador" : "Usuario";


                                        }
                                    }
                                        


                                    
                                }
                            }
                        }
                        connection.Close();
                        return passwordMatch;
                    }
                    catch (System.Exception ex)
                    {
                        connection.Close();
                        return passwordMatch;
                    }

                    //return resultado;
                }
            }
        }
        catch (System.Exception ex)
        {
            return passwordMatch;
        }
    }

    //Registrar Usuario
    public void RegistrarUsuario(
        string nombre,
        string apellidos,
        string correo,
        string contrasenya,
        string nombreUsuario,
        DateTime fechaExpiracion
    )
    {
        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();
            using (var cmd = new SqlCommand("RegistrarUsuario", connection))
            {
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Apellidos", apellidos);
                cmd.Parameters.AddWithValue("@Correo", correo);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(contrasenya);
                cmd.Parameters.AddWithValue("@Contrasenya", hashedPassword);
                cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                var token = Guid.NewGuid();
                cmd.Parameters.AddWithValue("@Token", token);
                DateTime FechaExpiracion = DateTime.UtcNow.AddMinutes(5);
                cmd.Parameters.AddWithValue("@FechaExpiracion", FechaExpiracion);

                cmd.ExecuteNonQuery();
            }
        }
    }

    // Validacion de usuario
    public Usuario ValidarUsuario(string correo)
    {
        throw new NotImplementedException();
    }
}
