using Domain.Enums;

namespace Application.Models;

public class SemesterInfo
{
    public int Year { get; set; }
    
    public Semester Semester { get; set; }
}