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
    public void RegistrarUsuario(string nombre, string apellidos, string correo, string contrasenya, int rolId,
     string nombreUsuario, bool estado, string token, DateTime fechaExpiracion)
    {
       using (var connection = new SqlConnection(_contexto.Conexion))
       {

            connection.Open();
            using(var cmd=new SqlCommand ("RegistrarUsuario",connection))
            {
                cmd.Parameters.AddWithValue("@Nombre",nombre);
                cmd.Parameters.AddWithValue("@Apellidos", apellidos);
                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Contrasenya",contrasenya);
                cmd.Parameters.AddWithValue("@RolId",rolId);
                cmd.Parameters.AddWithValue("@NombreUsuario",nombreUsuario);
                cmd.Parameters.AddWithValue("@Estado",estado);
                cmd.Parameters.AddWithValue("@Token",token);
                cmd.Parameters.AddWithValue("@FechaExpiracion",fechaExpiracion);

                cmd.ExecuteNonQuery();


            }


       }
    }

    public Usuario ValidarUsuario(string correo)
    {
        throw new NotImplementedException();
    }
}
