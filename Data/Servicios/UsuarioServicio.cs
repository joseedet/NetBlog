using System.Data.SqlClient;
using NetBlog.Data.Interfaces.IUsuario;
using NetBlog.Models;

namespace NetBlog.Data.Servicios;

public class UsuarioServicio : IUsuario
{
    private readonly Contexto _contexto;

    public UsuarioServicio(Contexto contexto)
    {
        _contexto = contexto;
    }

    //Activacion cuenta usuario
    public int ActivarCuenta(string token, DateTime fechaexpiracion)
    {
       using (var connection = new SqlConnection(_contexto.Conexion))
       {

             using(var cmd=new SqlCommand ("ActivarCuenta",connection))
            {                   
                
                cmd.Parameters.AddWithValue("@Token",token);
                DateTime FechaExpiracion = DateTime.UtcNow.AddMinutes(5);
                cmd.Parameters.AddWithValue("@FechaExpiracion",FechaExpiracion);
                connection.Open();           
                var resultado=cmd.ExecuteNonQuery();

                int activado = Convert.ToInt32(resultado);

                connection.Close();

                return activado;


            }


       }
    }

    //Registrar Usuario
    public void RegistrarUsuario(string nombre, string apellidos, string correo, string contrasenya, 
     string nombreUsuario,DateTime fechaExpiracion)
    {
       using (var connection = new SqlConnection(_contexto.Conexion))
       {

            connection.Open();
            using(var cmd=new SqlCommand ("RegistrarUsuario",connection))
            {
                
                cmd.Parameters.AddWithValue("@Nombre",nombre);
                cmd.Parameters.AddWithValue("@Apellidos", apellidos);
                cmd.Parameters.AddWithValue("@Correo", correo);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(contrasenya);
                cmd.Parameters.AddWithValue("@Contrasenya",hashedPassword);               
                cmd.Parameters.AddWithValue("@NombreUsuario",nombreUsuario);
                var token = Guid.NewGuid();             
                cmd.Parameters.AddWithValue("@Token",token);
                DateTime FechaExpiracion = DateTime.UtcNow.AddMinutes(5);
                cmd.Parameters.AddWithValue("@FechaExpiracion",FechaExpiracion);

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
