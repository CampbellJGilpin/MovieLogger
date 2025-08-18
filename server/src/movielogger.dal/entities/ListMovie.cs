using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movielogger.dal.entities;

[Table("list_movies")]
public class ListMovie
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("list_id")]
    public int ListId { get; set; }

    [Column("movie_id")]
    public int MovieId { get; set; }

    [Column("added_date")]
    public DateTime AddedDate { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(ListId))]
    public List List { get; set; } = default!;

    [ForeignKey(nameof(MovieId))]
    public Movie Movie { get; set; } = default!;
}