using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scouting_service.src.Scoutings.Core.Domain.Entities;

[Table("users")]
public class User
{
    [Key]
    [Column("id_user")]
    public int Id_User { get; set; }
    
    [Column("name")]
    [MaxLength(20)]
    public string? Name { get; set; }
    
    [Column("surname")]
    [MaxLength(20)]
    public string? Surname { get; set; }
    
    [Column("email")]
    [MaxLength(50)]
    public string? Email { get; set; }
    
    [Column("phone")]
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [Column("password")]
    [MaxLength(50)]
    public string? Password { get; set; }
    
    [Column("type")]
    [MaxLength(50)]
    public string? Type { get; set; }
}
