using Domain.Entities;
using Domain.Enums;

namespace AlphaProjectManager.Controllers.ProjectApplications.Requests;

public class UpdateApplicationRequest
{
    public ApplicationStatus Status { get; set; }

    public void ApplyToApplication(ProjectApplication application)
    {
        application.Status = Status;
    }
}