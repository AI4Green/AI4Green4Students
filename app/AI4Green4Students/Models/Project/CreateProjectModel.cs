using System.Globalization;

namespace AI4Green4Students.Models.Project;

public record CreateProjectModel
{
  public string Name { get; init; } = string.Empty;
  public string StartDate { get; init; } = string.Empty;
  public string PlanningDeadline { get; init; } = string.Empty;
  public string ExperimentDeadline { get; init; } = string.Empty;
  
  public DateTimeOffset ParseDateOrDefault(string dateString)
  {
    return DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
      out var date)
      ? new DateTimeOffset(date, TimeSpan.Zero)
      : DateTimeOffset.MaxValue;
  }
};
