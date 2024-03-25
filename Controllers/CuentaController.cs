using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NetBlog.Data;
using NetBlog.Data.Interfaces.IUsuario;
using NetBlog.Models;

namespace NetBlog.Controllers;

public class CuentaController : Controller
{
    private readonly Contexto _contexto;
    private readonly IUsuario _usuario;

    public CuentaController(Contexto con, IUsuario usuario)
    {
        _contexto = con;
        _usuario = usuario;
    }

    public IActionResult Registrar()
    {
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Registrar(Usuario model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                DateTime fecha = DateTime.UtcNow;
                _usuario.RegistrarUsuario(
                    model.Nombre,
                    model.Apellidos,
                    model.Correo,
                    model.Contrasenya,
                    model.RolId,
                    model.NombreUsuario,
                    model.Estado,
                    model.Token,
                    fecha
                );
                return RedirectToAction("Token");

                //Programar envio de correo al usuario.
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                    ViewBag.Error =
                        "El correo eletrónico y/ nombre de usuario ya se encuentra registrado";
                else
                    ViewBag.Error =
                        "Ocurrió un error al intentar registrar al usuario" + ex.Message;
                throw;
            }
        }

        return View(model);
    }

    public IActionResult Token()
    {
        string? token = Request.Query["valor"];

        if (token != null)
        {
            try
            {
                DateTime fechaExpiracion = DateTime.UtcNow;
                if (_usuario.ActivarCuenta(token, fechaExpiracion) == 1)
                    ViewData["mensaje"] = "Su cuenta ha sido validado exitosamente.";
                else
                    ViewData["mensaje"] = "El enlace de activacón ha expirado.";
            }
            catch (System.Exception ex)
            {
                ViewData["mensaje"] = ex.Message;
                return View();
            }
        }
        else
        {
            ViewData["mensaje"] = "Verifique su correo para activar su cuenta.";
            return View();
        }
        return View();
    }

    public IActionResult Login()
    {
        ClaimsPrincipal c = HttpContext.User;

        if (c.Identity != null)
        {
            if (c.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
        }
        return View();
    }
}
