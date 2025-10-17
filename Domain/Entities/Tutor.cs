using System.ComponentModel.DataAnnotations;
using System.Text;
using Domain.Interfaces;

namespace Domain.Entities;

public class Tutor : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required string FirstName { get; set; }

    public string? LastName { get; set; } = "";
    
    public string? Patronymic { get; set; } = "";

    public string GetFio()
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(LastName))
        {
            sb.Append(LastName);
            sb.Append(' ');
        }

        sb.Append(FirstName);
        if (!string.IsNullOrWhiteSpace(Patronymic))
        {
            sb.Append(' ');
            sb.Append(Patronymic);
        }

        return sb.ToString();
    }
}