using System.ComponentModel.DataAnnotations;

namespace GestionFacturasModelo.Model.DataModel
{
    public class UserEditableTemplate
    {
        [StringLength(20)]
        public string? Name { get; set; } = string.Empty;
        [StringLength(50)]
        public string? Email { get; set; } = string.Empty;
        [StringLength(30)]
        public string? Password { get; set; } = string.Empty;
        public UserRol? Rol { get; set; } = UserRol.USUARIO;
    }
}
