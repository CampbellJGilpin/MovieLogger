using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movielogger.dal.entities;

[Table("viewings")]
public class Viewing
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("user_movie_id")]
    public int UserMovieId { get; set; }

    [Required]
    [Column("date_viewed")]
    public DateTime DateViewed { get; set; }

    [ForeignKey("user_movie_id")]
    public UserMovie UserMovie { get; set; } = default!;

    public Review? Review { get; set; }
}