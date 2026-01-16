using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("favorites")]
    public class Favorite
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("userId")]
        public int UserId { get; set; }

        [Required]
        [Column("functionId")]
        public int FunctionId { get; set; }

        [Column("order")]
        public int? Order { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("FunctionId")]
        public virtual Function Function { get; set; }
    }
}
