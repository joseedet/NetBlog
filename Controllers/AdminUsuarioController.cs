using Microsoft.AspNetCore.Mvc;
using NetBlog.Data.Interfaces.IUsuario;

namespace NetBlog.Controllers;

[Route("[controller]")]
public class AdminUsuarioController : Controller
{
    private readonly IUsuario _usuario;

    public AdminUsuarioController(IUsuario usuario)
    {

        _usuario = usuario;
    }
}
