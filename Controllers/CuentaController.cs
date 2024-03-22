using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetBlog.Data;
using NetBlog.Models;

namespace NetBlog.Controllers;


public class CuentaController : Controller
{
    private readonly Contexto _contexto;
    

    public CuentaController(Contexto con)
    {
        _contexto = con;

    }

    public IActionResult Registrar()
    {

        return View();

    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Registrar(Usuario model)
    {
        if(ModelState.IsValid)
        {
            try
            {


            }
            catch(SqlException ex)
            {

                if (ex.Number == 2627) 
                    ViewBag.Error = "El correo eletrónico y/ nombre de usuario ya se encuentra registrado";              
                else
                    ViewBag.Error = "Ocurrió un error al intentar registrar al usuario"+ ex.Message;
                throw;
            }

        }

        return View(model);

    }

   
}
