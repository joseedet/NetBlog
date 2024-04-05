using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetBlog.Models;
using NetBlog.Models.ViewModels;

namespace NetBlog.Data.Interfaces.IUsuario;

public interface IUsuario
{
    public void RegistrarUsuario(/*string ? nombre,string ? apellidos,string ? correo,string ? contrasenya
    ,string ? nombreUsuario,DateTime ? fechaExpiracion,string token*/ Usuario model);

    public Usuario ValidarUsuario(string correo);
    public int ActivarCuenta(string token, DateTime fechaexpiracion);
    //public bool Login(LoginViewModel login);
    public void ActualizarToken(string correo);
    public List<Rol> ListarRoles();
    public Usuario ObtenerUsuarioPorId(int id);
    public void ActualizarPerfil(Usuario model);
    public void EliminarCuenta(int id);

 
}
