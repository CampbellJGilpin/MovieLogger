using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movielogger.dal.entities;

[Table("reviews")]
public class Review
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_movie_viewing_id")]
    public int UserMovieViewingId { get; set; }

    [Column("review_text", TypeName = "text")]
    public string? ReviewText { get; set; }

    [Column("score")]
    public int? Score { get; set; }

    [ForeignKey(nameof(UserMovieViewingId))]
    public UserMovieViewing UserMovieViewing { get; set; } = default!;
}