using System.ComponentModel.DataAnnotations;

namespace GestionFacturasModelo.Model.DataModel
{
    public enum RolUsuario { ADMINSITRADOR = 0, USUARIO = 10 }
    public class User : BaseEntity
    {
        [Required, StringLength(20)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public string Email { get; set; } = string.Empty;
        [Required, StringLength(30)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public ICollection<Enterprise> Enterprises { get; set; } = new List<Enterprise>();
    }
}