using System.ComponentModel.DataAnnotations;

namespace FunctionApp1.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string HashContraseña { get; set; }

        [StringLength(255)]
        public string Sal { get; set; }
    }
}