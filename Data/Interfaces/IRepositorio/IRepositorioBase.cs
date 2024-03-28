using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetBlog.Models;

namespace NetBlog.Data.Interfaces.IRepositorio;

public interface IRepositorioBase
{
    public void InsertPost(Post ent);
    public void UpdatePost(Post post);
    public void DeletePost(int id);
    public Post ObtenerPostId(int id);
    public List<Post> ObtenerPostCategoria(int id);
    public List <Post> ObtenerPostTitulo(string titulo);
    public void AgregarComentario(int  postId,string comentario,int ? userId,int ? comentarioPadreId);
    public List<Post> ObtenerTodos();
    public List<Comentario> ObtenerComentariosPorPostId(int id);
    public List<Comentario> ObtenerComentariosHijos(List<Comentario> comments);
    public List<Comentario> ObtenerComentariosNietos(List<Comentario> comments);



}
