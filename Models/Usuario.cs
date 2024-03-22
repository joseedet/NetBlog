using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace NetBlog.Models;

public class Usuario
{
    public int UsuarioId { get; set; }

    [Required(ErrorMessage="El campo Nombre es obligatorio")]
    [StringLength(50,ErrorMessage="El campo Nombre debe de tener un máximo {1} carácteres")]
    public string ? Nombre { get; set; }

    [Required(ErrorMessage="El campo Apellidos es obligatorio")]
    [StringLength(50,ErrorMessage="El campo Nombre debe de tener un máximo {1} carácteres")]
    public string ? Apellidos{ get; set; }

    [Required(ErrorMessage="El campo Correo es obligatorio")]
    [StringLength(100,ErrorMessage="El campo Correo debe de tener un máximo {1} carácteres")]
    [DataType(DataType.EmailAdress)]
    public string ? Correo { get; set; }

    [Required(ErrorMessage="El campo Contraseña es obligatorio")]
    // [StringLength(50,ErrorMessage="El campo Nombre debe de tener un máximo {1} carácteres")]
    [DataType(DataType.Password)]
    public string ? Contrasenya { get; set; }
    public int RolId  { get; set; }
    public Rol ? Rol { get; set; }

    [Required(ErrorMessage="El campo NombreUsuario es obligatorio")]
    [StringLength(50,ErrorMessage="El campo NombreUsuario debe de tener un máximo {1} carácteres")]
    public string ? NombreUsuario { get; set; }

    public boolean Estado { get; set; }

    [Required(ErrorMessage="El campo Token es obligatorio")]
    //[StringLength(50,ErrorMessage="El campo Nombre debe de tener un máximo {1} carácteres")]
    public string ? Token { get; set; }

    public DateTime ? FechaExpiracion { get; set; }

}
