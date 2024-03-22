using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetBlog.Models;

public class Categoria
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; }
    ICollection <Post> Post {get;set;}
}
