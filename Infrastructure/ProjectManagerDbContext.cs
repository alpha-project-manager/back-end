using Domain.Entities;
using Domain.Entities.TelegramBot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public class ProjectManagerDbContext : DbContext
{
    private readonly string _connectionString;

    public DbSet<ApplicationMessage> ApplicationMessages { get; init; }
    public DbSet<CalendarSettings> CalendarSettings { get; init; }
    public DbSet<ControlPoint> ControlPoints { get; init; }
    public DbSet<ControlPointInProject> ControlPointInProjects { get; init; }
    public DbSet<Meeting> Meetings { get; init; }
    public DbSet<Project> Projects { get; init; }
    public DbSet<ProjectApplication> ProjectApplications { get; init; }
    public DbSet<ProjectCase> ProjectCases { get; init; }
    public DbSet<Student> Students { get; init; }
    public DbSet<StudentInProject> StudentInProjects { get; init; }
    public DbSet<StudentRole> StudentRoles { get; init; }
    public DbSet<TodoTask> TodoTasks { get; init; }
    public DbSet<Tutor> Tutors { get; init; }
    public DbSet<User> Users { get; init; }
    
    public DbSet<CaseVote> CaseVotes { get; init; }
    public DbSet<StudentAttendance> StudentAttendances { get; init; }
    public DbSet<TutorAttendance> TutorAttendances { get; init; }
    
    public DbSet<ApplicationQuestion> ApplicationQuestions { get; init; }
    public DbSet<ApplicationQuestionAnswer> ApplicationQuestionAnswers { get; init; }
    
    public ProjectManagerDbContext(IConfiguration configuration)
    {
        var readConnString = configuration.GetConnectionString("DefaultConnection");
        _connectionString = readConnString ?? throw new Exception("Connection string \"DefaultConnection\" wasn't found in appsettings.json");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.UseNpgsql(_connectionString);
    }
}