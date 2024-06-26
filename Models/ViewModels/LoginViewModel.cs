using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetBlog.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage="El  Correo electro es obligatorio")]
    [StringLength(100,ErrorMessage="El campo Correo debe de tener un máximo {1} carácteres")]
    [DataType(DataType.EmailAddress)]
    public string ? Correo { get; set; }

    [Required(ErrorMessage="El campo Contraseña es obligatorio")]
    // [StringLength(50,ErrorMessage="El campo Nombre debe de tener un máximo {1} carácteres")]
    [StringLength(50, MinimumLength=8,ErrorMessage="El campo Contraseña debe de tener 8 carácteres como mínimo")]
    [DataType(DataType.Password)]
    public string ? Contrasenya { get; set; }
    
    [Display(Name="Recuérdame")]
    public bool MantenerActivo { get; set; }
}
