using System.Resources;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NetBlog.Data.Interfaces.IUsuario;
using NetBlog.Models;
using NetBlog.Models.ViewModels;

namespace NetBlog.Data.Servicios;

public class UsuarioServicio : IUsuario
{
    private readonly Contexto _contexto;
    public string? ViewBag;

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

    public void ActualizarPerfil(Usuario model)
    {
        //Usuario usuario = new Usuario();
        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            using (var cmd = new SqlCommand("ActualizarPerfil", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UsuarioId", model.UsuarioId);
                cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                cmd.Parameters.AddWithValue("@Apellidos", model.Apellidos);
                cmd.Parameters.AddWithValue("@Correo", model.Correo);

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }

    public void ActualizarToken(string correo)
    {
        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            using (var cmd = new SqlCommand("ActualizarToken", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Correo", correo);
                DateTime Fecha = DateTime.UtcNow.AddMinutes(5);
                cmd.Parameters.AddWithValue("@Fecha", Fecha);
                var token = Guid.NewGuid();
                cmd.Parameters.AddWithValue("@Token", token.ToString());
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

                //INSERTAR ENVÍO DE CORREO

                Email email = new();
                if (correo != null)
                    email.Enviar(correo, token.ToString());
            }
        }
    }

    
    public void EliminarCuenta(int id)
    {
       using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();

            using (var command = new SqlCommand("EliminarUsuario", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UsuarioId", id);
                command.ExecuteNonQuery();
            }
        }
    }

    public List<Rol> ListarRoles()
    {
        var roles = new List<Rol>();

        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();

            using (var command = new SqlCommand("ListarRoles", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var rol = new Rol();

                        rol.RolId = Convert.ToInt32(reader["PostId"]);
                        rol.Nombre = Convert.ToString(reader["Nombre"]);

                        roles.Add(rol);
                    }
                    //reader.Close();
                }
            }
        }
        return roles;
    }

    public List<Usuario> ListarUsuarios()
    {
        //throw new NotImplementedException();
        var usuarios = new List<Usuario>();

        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();

            using (var command = new SqlCommand("ListarUsuarios", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var usuario = new Usuario();

                        usuario.UsuarioId = Convert.ToInt32(reader["UsuarioId"]);
                        usuario.Nombre = Convert.ToString(reader["Nombre"]);
                        usuario.Apellidos = reader["Apellidos"].ToString();
                        usuario.Correo = reader["Correo"].ToString();
                        usuario.RolId = (int)reader["RolId"];
                        usuario.NombreUsuario = reader["NombreUsuario"].ToString();
                        usuario.Estado = (Boolean)reader["Estado"];
                        usuario.Token = reader["Token"].ToString();
                        usuario.FechaExpiracion = Convert.ToDateTime(reader["FechaExpiracion"]);

                        usuarios.Add(usuario);
                    }
                    //reader.Close();
                }
            }
        }
        return usuarios;

    }

    public Usuario ObtenerUsuarioPorId(int id)
    {
        Usuario usuario = new Usuario();
        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            using (var command = new SqlCommand("ObtenerUsuarioPorId", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UsuarioId", id);
                connection.Open();

                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    usuario = new Usuario
                    {
                        UsuarioId = id,
                        Nombre = reader["Nombre"].ToString(),
                        Apellidos = reader["Apellidos"].ToString(),
                        Correo = reader["Correo"].ToString(),
                        Contrasenya = reader["Contrasenya"].ToString(),
                        RolId = (int)reader["RolId"],
                        NombreUsuario = reader["NombreUsuario"].ToString(),
                        Estado = (Boolean)reader["Estado"],
                        Token = reader["Token"].ToString(),
                        FechaExpiracion = Convert.ToDateTime(reader["FechaExpiracion"])
                    };
                }
                //reader.Close();
            }
        }
        return usuario;
    }

    //Registrar Usuario
    public void RegistrarUsuario(
        Usuario model
    /*string? nombre,
    string? apellidos,
    string? correo,
    string? contrasenya,
    string? nombreUsuario,
    DateTime? fechaExpiracion*/
    )
    // Cambiar parametros por model.
    {
        using (var connection = new SqlConnection(_contexto.Conexion))
        {
            connection.Open();
            using (var cmd = new SqlCommand("RegistrarUsuario", connection))
            {
                cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                cmd.Parameters.AddWithValue("@Apellidos", model.Apellidos);
                cmd.Parameters.AddWithValue("@Correo", model.Correo);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Contrasenya);
                cmd.Parameters.AddWithValue("@Contrasenya", hashedPassword);
                cmd.Parameters.AddWithValue("@NombreUsuario", model.NombreUsuario);
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
