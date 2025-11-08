using Application.Models;
using Domain.Enums;

namespace Application.Utils;

public static class CurrentSemesterHelper
{
    public static SemesterInfo GetCurrentSemester()
    {
        return new SemesterInfo
        {
            Year = DateTime.Now.Year,
            Semester = DateTime.Now.Month > 7 || DateTime.Now.Month < 2 ? Semester.Autumn : Semester.Spring
        };
    }
}