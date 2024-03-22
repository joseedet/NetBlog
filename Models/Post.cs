using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace NetBlog.Models;

public class Post
{
    public int PostId { get; set; }

    [Required(ErrorMessage="El titulo es requerido.")]
    [StringLength(100,MinimumLength =5,ErrorMessage="Minimo 5 carácteres y máximo de 100")]
    public string ? Titulo { get; set; }

    [Required(ErrorMessage="El contenido es requerido.")]
    [StringLength(500,MinimumLength =5,ErrorMessage="Minimo 50 carácteres y máximo de 500")]
    public string ?  Contenido { get; set; }

    [Required(ErrorMessage="Debes de elegir una categoría.")]
    public int CategoriaId { get; set; }

    
    public DateTime FechaCreacion { get; set; }=DateTime.Now;

    public Categoria ? Categoria {get;set;}
}
