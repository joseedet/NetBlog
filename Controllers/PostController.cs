using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetBlog.Data;
using NetBlog.Data.Servicios;
using NetBlog.Models;
using NetBlog.Models.ViewModels;

namespace NetBlog.Controllers;

[Route("[controller]")]
public class PostController : Controller
{
    private readonly Contexto _contexto;
    private readonly PostServicio _postServicio;

    //private readonly  Data.Interfaces.IRepositorio  _repo;

    public PostController(
        Contexto contexto /*,PostServicio postServicio */
    )
    {
        _contexto = contexto;
        //_postServicio= new PostServicio(contexto);
        _postServicio = new PostServicio(contexto);
        // _repo=repo;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public IActionResult Create(Post post)
    {
        /* using (var connection = new SqlConnection(_contexto.Conexion))
         {
             connection.Open();
             using (var command = new SqlCommand("AgregarPost", connection))
             {
                 command.CommandType = CommandType.StoredProcedure;
                 command.Parameters.AddWithValue("@Titulo", post.Titulo);
                 command.Parameters.AddWithValue("@Contenido", post.Contenido);
                 command.Parameters.AddWithValue("@CategoriaId", post.CategoriaId);
                 DateTime fc = DateTime.UtcNow;
                 command.Parameters.AddWithValue("@FechaCreacion", fc);
                 command.ExecuteNonQuery();
             }
         }*/
        _postServicio.InsertPost(post);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public IActionResult Update(int id)
    {
        //var post=_;
        return View(_postServicio.ObtenerPostId(id));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public IActionResult Update(Post post)
    {
        /* using (var connection= new SqlConnection(_contexto.Conexion))
         {
             connection.Open();
             using(var command=new SqlCommand("ActualizarPost",connection))
             {
 
                 command.CommandType=CommandType.StoredProcedure;
                 command.Parameters.AddWithValue("@PostId", post.PostId);
                 command.Parameters.AddWithValue("@Titulo",post.Titulo);
                 command.Parameters.AddWithValue("@Contenido",post.Contenido);
                 command.Parameters.AddWithValue("@CategoriaId",post.CategoriaId);
                 command.ExecuteNonQuery();
 
 
             }
 
         }*/
        _postServicio.UpdatePost(post);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Delete(int id)
    {
        /* using (var connection= new SqlConnection(_contexto.Conexion))
         {
             connection.Open();
             using(var command=new SqlCommand("EliminarPost",connection))
             {

                 command.CommandType=CommandType.StoredProcedure;
                 command.Parameters.AddWithValue("@PostId", id);
                 command.ExecuteNonQuery();


             }

         }*/
        _postServicio.DeletePost(id);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Details(int id)
    {
        var post = _postServicio.ObtenerPostId(id);
        var comentarios = _postServicio.ObtenerComentariosPorPostId(id);
        comentarios = _postServicio.ObtenerComentariosHijos(comentarios);
        comentarios = _postServicio.ObtenerComentariosNietos(comentarios);

        var model = new PostDetallesViewModel
        {
            Post = post,
            ComentariosPrincipales = comentarios
                .Where(c =>c.ComentarioPadreId == null && c.ComentarioAbueloId == null)
                .ToList(),
            ComentariosHijos = comentarios
                .Where(c => c.ComentarioPadreId != null && c.ComentarioAbueloId == null)
                .ToList(),
            ComentariosNietos = comentarios.Where(c => c.ComentarioAbueloId != null).ToList(),
            PostRecientes = _postServicio.ObtenerTodos().Take(10).ToList()
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult AgregarComentario(int postId, string comentario, int ? comentarioPadreId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(comentario))
            {
                ViewBag.Error = "El comentario no puede estar vacio";
                return RedirectToAction("Details", "Post", new { id = postId });
            }
            int? userId = null;
            var userIdClaim = User.FindFirst("UsuarioId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUsuarioId))
                userId = parsedUsuarioId;

            DateTime FechaPublicacion = DateTime.UtcNow;
            
            _postServicio.AgregarComentario(postId, comentario,(int) userId, comentarioPadreId);

            return RedirectToAction("Details", "Post", new { id = postId });
        }
        catch (System.Exception e)
        {
            ViewBag.Error = e.Message;
            return RedirectToAction("Details", "Post", new { id = postId });
        }
    }
}
