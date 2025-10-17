namespace Application.Utils;

public static class DateTimeToTimestamp
{
    public static long ConvertToTimestamp(this DateTime dateTime)
    {
        var utcDateTime = dateTime.ToUniversalTime();
        var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var elapsedTime = utcDateTime - unixEpoch;
        return (long)elapsedTime.TotalSeconds;
    }
}