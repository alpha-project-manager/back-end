namespace Application.Models;

public class ServiceActionResult
{
    public required bool Completed { get; set; }

    public string Comment { get; set; } = "";

    public static ServiceActionResult Failed(string comment)
    {
        return new ServiceActionResult
        {
            Completed = false,
            Comment = comment
        };
    }
}