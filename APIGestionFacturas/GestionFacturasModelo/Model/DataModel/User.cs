using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestionFacturasModelo.Model.DataModel
{
    public enum UserRol{ ADMINISTRATOR = 0, USER = 10 }

    [Index(nameof(Email), IsUnique = true)]
    public class User : BaseEntity
    {
        User() { }
        public User(UserEditable data)
        {
            Name = data.Name;
            Email = data.Email;
            Password = data.Password;
            Rol = (UserRol)data.Rol;
        }

        [Required, StringLength(20)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(50)]
        public string Email { get; set; } = string.Empty;
        [Required, StringLength(30)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public UserRol Rol { get; set; } = UserRol.USER;
    }
}