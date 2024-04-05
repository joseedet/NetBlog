using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NetBlog.Data;
using NetBlog.Data.Interfaces.IUsuario;
using NetBlog.Models;
using NetBlog.Models.ViewModels;

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
                    /*model.Nombre,
                    model.Apellidos,
                    model.Correo,
                    model.Contrasenya,                    
                    model.NombreUsuario,
                    fecha*/ model             
                
                    
                );
                 Email email = new();
                if(model.Correo!=null)
                    email.Enviar(model.Correo, model.Token.ToString());
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

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        // bool passwordMatch = false;
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
                                var passwordMatch = BCrypt.Net.BCrypt.Verify(
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
                                           _usuario.ActualizarToken(model.Correo);

                                            ViewBag.Error =
                                                "Su cuenta no ha sido activada, se ha enviado un correo de activación verifique su bandeja de correo";
                                        }
                                    }
                                    else if (!(bool)reader["Estado"])
                                        ViewBag.Error =
                                            "Su cuenta no ha sido activada, revise su bandeja de correo";
                                    else
                                    {
                                        string? nombreusuario = reader["NombreUsuario"].ToString();
                                        int usuarioId = (int)reader["UsuarioId"];
                                        if (nombreusuario != null)
                                        {
                                            var claims = new List<Claim>()
                                            {
                                                new Claim(ClaimTypes.NameIdentifier, nombreusuario),
                                                new Claim("UsuarioId", usuarioId.ToString())
                                            };
                                            int rolId = (int)reader["RolId"];
                                            string rolNombre =
                                                rolId == 1 ? "Administrador" : "Usuario";
                                            claims.Add(new Claim(ClaimTypes.Role, rolNombre));

                                            var identity = new ClaimsIdentity(
                                                claims,
                                                CookieAuthenticationDefaults.AuthenticationScheme
                                            );
                                            var propiedades = new AuthenticationProperties
                                            {
                                                AllowRefresh = true,
                                                IsPersistent = model.MantenerActivo,
                                                ExpiresUtc = DateTimeOffset.UtcNow.Add(
                                                    model.MantenerActivo
                                                        ? TimeSpan.FromDays(1)
                                                        : TimeSpan.FromMinutes(5)
                                                )
                                            };
                                            await HttpContext.SignInAsync(
                                                CookieAuthenticationDefaults.AuthenticationScheme,
                                                new ClaimsPrincipal(identity),
                                                propiedades
                                            );
                                            return RedirectToAction("Index", "Home");
                                        }
                                    }
                                }
                            }
                            else
                            {

                                ViewBag.Error = "Correo no registrado.";
                                reader.Close();
                            }
                        }
                    }
                    finally 
                    {
                        if (cmd != null)
                            cmd.Dispose();
                    }

                    //return resultado;
                }
            }
        }
        catch (System.Exception ex) {

            ViewBag.Error = ex.Message;
         }
        return View(model);

    }
    public async Task<IActionResult>CerrarSesion()
    {

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");


    }
}
