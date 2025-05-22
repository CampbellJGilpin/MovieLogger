using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movielogger.dal.entities;

[Table("user_movies")]
public class UserMovie
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("movie_id")]
    public int MovieId { get; set; }

    [Column("favourite")]
    public bool Favourite { get; set; } = false;

    [Column("owns_movie")]
    public bool OwnsMovie { get; set; } = false;

    [Column("upcoming_view_date")]
    public DateTime? UpcomingViewDate { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = default!;

    [ForeignKey(nameof(MovieId))]
    public Movie Movie { get; set; } = default!;

    public ICollection<Viewing> Viewings { get; set; } = new List<Viewing>();
}