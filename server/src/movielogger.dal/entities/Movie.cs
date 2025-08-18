using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movielogger.dal.entities;

[Table("movies")]
public class Movie
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    [Column("title")]
    public string Title { get; set; } = default!;

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("release_date")]
    public DateTime? ReleaseDate { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [Column("genre_id")]
    public int GenreId { get; set; }

    [ForeignKey(nameof(GenreId))]
    public Genre Genre { get; set; } = default!;

    [Column("poster_path")]
    public string? PosterPath { get; set; }

    public ICollection<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
    public ICollection<ListMovie> ListMovies { get; set; } = new List<ListMovie>();
}