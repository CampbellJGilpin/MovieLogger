using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movielogger.dal.entities;

[Table("user_movie_viewings")]
public class UserMovieViewing
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [Column("movie_id")]
    public int MovieId { get; set; }

    [Required]
    [Column("date_viewed")]
    public DateTime DateViewed { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = default!;

    [ForeignKey(nameof(MovieId))]
    public Movie Movie { get; set; } = default!;

    public Review? Review { get; set; }
}