using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movielogger.dal.entities;

[Table("genres")]
public class Genre
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("title")]
    public string Title { get; set; } = default!;

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}