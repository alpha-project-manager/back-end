using Domain.Entities;
using Domain.Enums;

namespace AlphaProjectManager.Controllers.ApplicationMessages.Responses;

public class MessageResponse
{
    public required Guid Id { get; set; }
    
    public required long Timestamp { get; set; }
    
    public required string Content { get; set; }
    
    public required bool IsRead { get; set; }
    
    public required bool FromStudents { get; set; }

    public static MessageResponse FromApplicationMessage(ApplicationMessage msg)
    {
        return new MessageResponse
        {
            Id = msg.Id,
            Timestamp = msg.Timestamp,
            Content = msg.Content,
            IsRead = msg.IsRead,
            FromStudents = msg.Direction == ApplicationMsgDirection.FromStudents
        };
    }
}