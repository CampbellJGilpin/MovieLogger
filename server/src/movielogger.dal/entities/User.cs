using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movielogger.dal.entities;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("user_name")]
    public string UserName { get; set; } = default!;

    [Required]
    [MaxLength(255)]
    [Column("email")]
    public string Email { get; set; } = default!;

    [Required]
    [Column("password")]
    public string Password { get; set; } = default!;

    [Column("is_admin")]
    public bool IsAdmin { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    public ICollection<UserMovie> Library { get; set; } = new List<UserMovie>();
    public ICollection<List> Lists { get; set; } = new List<List>();
    public ICollection<UserMovieViewing> UserMovieViewings { get; set; } = new List<UserMovieViewing>();
}
