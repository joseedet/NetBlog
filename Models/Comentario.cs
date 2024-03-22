using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetBlog.Models;

public class Comentario
{
    public int ComentarioId { get; set; }
    public string ? Contenido { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int UsuarioId { get; set; }
    public int PostId { get; set; }
    public int ? ComentarioPadreId { get; set; }
    public List<Comentario> ? ComentariosHijos { get; set; }
    [NotMapped]
    public string ? NombreUsuario{get;set;}
    public int ? ComentarioAbueloId {get;set;}

}
