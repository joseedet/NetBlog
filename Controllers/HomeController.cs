using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NetBlog.Data;
using NetBlog.Data.Interfaces.IRepositorio;
using NetBlog.Data.Interfaces.IUsuario;
using NetBlog.Models;

namespace NetBlog.Controllers;

public class HomeController : Controller
{
    //private readonly Contexto _context;
    private readonly IRepositorioBase _postServicio;

    public HomeController(IRepositorioBase postServicio)
    {

        //_context = contexto;
        _postServicio = postServicio;
    }
    public IActionResult Index(int categoria,string buscar,int? pagina)
    {


        return View();

    }

    
}
