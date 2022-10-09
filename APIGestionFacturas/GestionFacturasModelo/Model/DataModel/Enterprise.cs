using GestionFacturasModelo.Model.Templates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFacturasModelo.Model.DataModel
{
    public class Enterprise : BaseEntity
    {
        // Generate default Enterprise
        public Enterprise() { }

        // Generate default Enterprise, but replace values with data
        public Enterprise (EnterpriseEditable data)
        {
            Name = data.Name;
            UserId = data.UserId;
        }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;    // Name of the enterprise
        public User? User { get; set; }                     // User that manages the enterprise
        [ForeignKey("User")]
        public int? UserId { get; set; }                    // User Id
    }
}
