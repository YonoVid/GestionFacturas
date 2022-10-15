using GestionFacturasModelo.Model.Templates;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestionFacturasModelo.Model.DataModel
{
    // User rol
    public enum UserRol{ ADMINISTRATOR = 0, USER = 10 }

    [Index(nameof(Email), IsUnique = true)]
    public class User : BaseEntity
    {
        // Generate default User
        public User() { }
        // Generate default User, but replace values with data
        public User(UserEditable data)
        {
            Name = data.Name;
            Email = data.Email;
            Password = data.Password;
            Rol = (UserRol)data.Rol;
        }
        // Generate default User, but replace values with data
        public User(UserAuthorization data)
        {
            Name = data.Name;
            Email = data.Email;
            Password = data.Password;
        }

        [Required, StringLength(20)]
        public string Name { get; set; } = string.Empty;        // Name of the user
        [Required, StringLength(50)]
        public string Email { get; set; } = string.Empty;       // Email of the user
        [Required, StringLength(30)]
        public string Password { get; set; } = string.Empty;    // Password of the user account
        [Required]
        public UserRol Rol { get; set; } = UserRol.USER;        // User rol (enum)
    }
}