using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NetBlog.Data;
using NetBlog.Data.Interfaces.IUsuario;
using NetBlog.Data.Servicios;

namespace NetBlog.Controllers;

[Route("[controller]")]
public class AdminUsuarioController : Controller
{
    private readonly IUsuario _usuario;
    private readonly Contexto _contexto;

    public AdminUsuarioController(IUsuario usuario, Contexto contexto)
    {

        _usuario = usuario;
        _contexto = contexto;
        _usuario=new UsuarioServicio(contexto);

    }
    public IActionResult  Index( string buscar, int? pagina)
    {


        var usuarios = _usuario.ListarUsuarios();
  

        if(!String.IsNullOrEmpty(buscar))
               usuarios = usuarios.Where(u => u.Correo != null && u.Correo.Contains(buscar) ||
            u.NombreUsuario != null && u.NombreUsuario.Contains(buscar)).ToList();

        usuarios = usuarios.OrderBy(u => u.NombreUsuario).ToList();
        List<SelectListItem> roles= _usuario.ListarRoles().Select();


    }


}
