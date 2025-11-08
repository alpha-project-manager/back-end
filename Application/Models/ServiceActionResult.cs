namespace Application.Models;

public class ServiceActionResult
{
    public required bool Completed { get; set; }

    public string Comment { get; set; } = "";
}