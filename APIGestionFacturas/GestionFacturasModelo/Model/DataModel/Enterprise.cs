using GestionFacturasModelo.Model.Templates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFacturasModelo.Model.DataModel
{
    public class Enterprise : BaseEntity
    {
        public Enterprise()
        {
        }

        public Enterprise (EnterpriseEditable data)
        {
            Name = data.Name;
            User = data.User;
            UserId = data.UserId;
        }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;
        public User? User { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
    }
}
