using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NetBlog.Data;
using NetBlog.Data.Interfaces.IRepositorio;
using NetBlog.Data.Interfaces.IUsuario;
using NetBlog.Models;
using X.PagedList;

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
    public IActionResult Index(int ? categoria,string buscar,int? pagina)
    {
        var post = new List<Post>();

        if (categoria == null && string.IsNullOrEmpty(buscar))
            _postServicio.ObtenerTodos();
        else if (categoria != null)
            post = _postServicio.ObtenerPostCategoria((int)categoria);
            var nombreCategoria=_postServicio.ObtenerNombreCategoria((int)categoria);
            if(post.Count==0)
                ViewBag.Error=$"No se encontraron publicaciones en la categoría {nombreCategoria}";
        else if(!string.IsNullOrEmpty(buscar))
            post=_postServicio.ObtenerPostTitulo(buscar);
            
            if(post.Count==0)
                ViewBag.Error=$"No se encontraron publicaciones en la categoría {buscar}.";

        int pageSize = 6;
        int pageNumber = (pagina ?? 1);

        string descripcioncategoria = !string.IsNullOrEmpty(nombreCategoria.ToString()) ? _postServicio.ObtenerNombreCategoria((int)categoria).ToString() : "Todas las denas";

        ViewBag.CategoriaDescripcion = descripcioncategoria;
        return View(post.ToPagedList(pageNumber,pageSize));

    }

    
}
