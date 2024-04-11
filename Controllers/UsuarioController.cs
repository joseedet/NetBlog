using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetBlog.Data;
using NetBlog.Data.Interfaces.IUsuario;
using NetBlog.Data.Servicios;
using NetBlog.Models;

namespace NetBlog.Controllers;

[Route("[controller]")]
public class UsuarioController : Controller
{
    //private readonly Contexto _context;
    private readonly IUsuario _usuario;

    public UsuarioController(Contexto contexto, IUsuario usuario)
    {
            usuario = new UsuarioServicio(contexto);
            _usuario = usuario;
    
    }

    [Authorize]
    public IActionResult Perfil()
    {
        int userId = 0;

        var userClaim = User.FindFirst("UsuarioId");

        if (userClaim != null && int.TryParse(userClaim.Value, out int parsedUserId))
            userId = parsedUserId;
        Usuario user = _usuario.ObtenerUsuarioPorId(userId);
        return View();
    }

    [HttpPost]
    public IActionResult ActualizarPerfil(Usuario model)
    {
        _usuario.ActualizarPerfil(model);

        return RedirectToAction("Perfil");
    }

    [HttpPost]
    public IActionResult EliminarCuenta(int id)
    {
        int userId = 0;

        var userClaim = User.FindFirst("UsuarioId");

        if (userClaim != null && int.TryParse(userClaim.Value, out int parsedUserId))
            userId = parsedUserId;
        Usuario user = _usuario.ObtenerUsuarioPorId(userId);

        _usuario.EliminarCuenta(user.UsuarioId);
         HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
         
        return RedirectToAction("Index", "Home");
    }
}
