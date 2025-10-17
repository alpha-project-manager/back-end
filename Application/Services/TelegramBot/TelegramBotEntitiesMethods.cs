using Application.DataQuery;
using Application.Utils;
using Domain.Entities;
using Domain.Entities.TelegramBot;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace Application.Services.TelegramBot;

public partial class TelegramBotBackgroundService
{
    private async Task<bool> DeleteApplicationAsync(long chatId)
    {
        var scope = _services.CreateScope();
        var applicationService = scope.ServiceProvider.GetRequiredService<BaseService<ProjectApplication>>();
        var answerService = scope.ServiceProvider.GetRequiredService<BaseService<ApplicationQuestionAnswer>>();
        var messageService = scope.ServiceProvider.GetRequiredService<BaseService<ApplicationMessage>>();

        var applicationsFound = await applicationService.GetAsync(new DataQueryParams<ProjectApplication>
        {
            Expression = a => a.ChatId == chatId
        });
        if (applicationsFound.Length == 0)
        {
            return false;
        }

        var application = applicationsFound[0];
        var answers = await answerService.GetAsync(new DataQueryParams<ApplicationQuestionAnswer>
        {
            Expression = ans => ans.ApplicationId == application.Id
        });
        var messages = await messageService.GetAsync(new DataQueryParams<ApplicationMessage>
        {
            Expression = msg => msg.ApplicationId == application.Id
        });
        await answerService.RemoveRangeAsync(answers);
        await messageService.RemoveRangeAsync(messages);
        return await applicationService.TryRemoveAsync(application.Id);
    }
    
    private async Task<ProjectApplication> CreateNewApplicationAsync(Guid caseId, long chatId, string userName)
    {
        var scope = _services.CreateScope();
        var applicationService = scope.ServiceProvider.GetRequiredService<BaseService<ProjectApplication>>();
        var application = new ProjectApplication
        {
            Id = Guid.NewGuid(),
            CaseId = caseId,
            ChatId = chatId,
            TeamTitle = "",
            Status = ApplicationStatus.InProgress,
            CurrentQuestionId = null,
            TelegramUsername = userName,
        };
        var firstQuestion = await TryGetFirstQuestion();
        application.CurrentQuestionId = firstQuestion?.Id;
        await applicationService.CreateAsync(application);
        return application;
    }
    
    private async Task<ProjectCase[]> GetAvailableProjectCasesAsync()
    {
        var scope = _services.CreateScope();
        var caseService = scope.ServiceProvider.GetRequiredService<BaseService<ProjectCase>>();
        var foundActiveCases = await caseService.GetAsync(new DataQueryParams<ProjectCase>
        {
            Expression = prCase => prCase.IsActive,
            Filters = [prCase => prCase.MaxTeams > prCase.AcceptedTeams]
        });
        return foundActiveCases;
    }

    private async Task<ProjectCase?> TryGetProjectCaseByIdAsync(Guid caseId)
    {
        var scope = _services.CreateScope();
        var caseService = scope.ServiceProvider.GetRequiredService<BaseService<ProjectCase>>();
        var foundActiveCases = await caseService.GetAsync(new DataQueryParams<ProjectCase>
        {
            Expression = prCase => prCase.Id == caseId
        });
        return foundActiveCases.Length > 0 ? foundActiveCases[0] : null;
    }
    
    private async Task<ProjectApplication?> TryGetApplicationByChatIdAsync(long chatId)
    {
        var scope = _services.CreateScope();
        var applicationService = scope.ServiceProvider.GetRequiredService<BaseService<ProjectApplication>>();
        var projectApplications = await applicationService.GetAsync(new DataQueryParams<ProjectApplication>
        {
            Expression = prCase => prCase.ChatId == chatId
        });
        return projectApplications.Length > 0 ? projectApplications[0] : null;
    }
    
    private async Task<ApplicationQuestion?> TryGetFirstQuestion()
    {
        var scope = _services.CreateScope();
        var questionService = scope.ServiceProvider.GetRequiredService<BaseService<ApplicationQuestion>>();
        var nextQuestionFound = await questionService.GetAsync(new DataQueryParams<ApplicationQuestion>
        {
            Expression = q => q.PrevQuestionId == null
        });
        return nextQuestionFound.Length > 0 ? nextQuestionFound[0] : null;
    }
    
    private async Task<(ApplicationQuestion? CurrentQuestion, ApplicationQuestion? NextQuestion)> TryGetQuestionForApplicationAsync(ProjectApplication application)
    {
        var scope = _services.CreateScope();
        var questionService = scope.ServiceProvider.GetRequiredService<BaseService<ApplicationQuestion>>();
        if (application.CurrentQuestionId == null)
        {
            return (null, null);
        }
        
        var currentQuestion = await questionService.GetAsync(new DataQueryParams<ApplicationQuestion>
        {
            Expression = q => q.Id == application.CurrentQuestionId.Value,
            IncludeParams = new IncludeParams<ApplicationQuestion>
            {
                IncludeProperties = [q => q.NextQuestion]
            }
        });
        
        return (currentQuestion[0], currentQuestion[0].NextQuestion);
    }

    private async Task CreateNewMessageFromStudentsAsync(ProjectApplication application, Message msg)
    {
        var scope = _services.CreateScope();
        var messageService = scope.ServiceProvider.GetRequiredService<BaseService<ApplicationMessage>>();
        var message = new ApplicationMessage
        {
            Id = Guid.NewGuid(),
            ApplicationId = application.Id,
            Content = msg.Text!,
            Direction = ApplicationMsgDirection.FromStudents,
            Timestamp = msg.Date.ConvertToTimestamp()
        };
        await messageService.CreateAsync(message);
    }
}