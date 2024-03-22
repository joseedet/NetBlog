using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetBlog.Models.ViewModels;

public class PostDetallesViewModel
{
    public Post ? Post { get; set; }
    public List<Comentario>? ComentariosPrincipales { get; set; }
    public List<Comentario>? ComentariosHijos { get; set; }
    public List<Comentario>? ComentariosNietos { get; set; }
    public List<Post>? PostRecientes { get; set; }

}
