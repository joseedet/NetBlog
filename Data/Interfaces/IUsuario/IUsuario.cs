using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetBlog.Models;

namespace NetBlog.Data.Interfaces.IUsuario;

public interface IUsuario
{
    public void RegistrarUsuario(string nombre,string apellidos,string correo,string contrasenya
    ,string nombreUsuario,DateTime fechaExpiracion);

    public Usuario ValidarUsuario(string correo);
    public int ActivarCuenta(string token, DateTime fechaexpiracion);
 
}
