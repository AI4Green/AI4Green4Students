namespace AI4Green4Students.Models.Emails
{
  public record EmailAddress(string Address)
  {
    public string? Name { get; init; }
  }
}
