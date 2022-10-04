using GestionFacturasModelo.Model.DataModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFacturasModelo.Model.Templates
{
    public class EnterpriseEditable
    {
        [StringLength(50)]
        public string? Name { get; set; } = String.Empty;
        
        public User? User { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set;}
        
    }
}
