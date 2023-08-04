using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Models.User;

public class UserModel
{
  public string Id { get; set; } = string.Empty;

  [Required] [EmailAddress] public string Email { get; set; } = string.Empty; 
  
  public string FullName { get; set; } = string.Empty;
  
  public bool EmailConfirmed { get; set; }

  public string UICulture { get; set; } = string.Empty;
  
  [Required] public List<string> Roles { get; set; } = new();
  
  public bool SendUpdateEmail { get; set; } // Applicable to sending email to the user when their account is updated
  
}
