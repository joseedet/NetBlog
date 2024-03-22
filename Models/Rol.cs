using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetBlog.Models;

public class Rol
{
    public int RolId { get; set; }
    public string ? Nombre { get; set; }
    public ICollection<Usuario> ? Usuarios { get; set; }
}
