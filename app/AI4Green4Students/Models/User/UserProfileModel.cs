namespace AI4Green4Students.Models.User;

public record BaseUserProfileModel(
  string Email,
  string FullName,
  string UICulture,
  List<string> Permissions
);

public record UserProfileModel(
  string Email,
  string FullName,
  string UICulture,
  List<string> Permissions

)
  : BaseUserProfileModel(
      Email,
      FullName,
      UICulture,
     Permissions);

