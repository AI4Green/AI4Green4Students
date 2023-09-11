using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Emails;
using AI4Green4Students.Models.User;
using AI4Green4Students.Services.EmailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize (nameof(AuthPolicies.CanViewAllUsers))]
public class UsersController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _users;
  private readonly AccountEmailService _accountEmail;

  public UsersController(
    UserManager<ApplicationUser> users,
    AccountEmailService accountEmail) 
  {
    _users = users;
    _accountEmail = accountEmail;

  }

  /// <summary>
  /// Get users list
  /// </summary>
  /// <returns>users list with their associated roles</returns>
  [HttpGet]
  public async Task<List<UserModel>> List()
  {
    var list = await _users.Users.ToListAsync();
    
    var usersList = new List<UserModel>();
    foreach (var x in list)
    {
      var roles = await _users.GetRolesAsync(x); // Get user roles
      var user = new UserModel
      {
        Id = x.Id,
        FullName = x.FullName,
        Email = x.Email,
        EmailConfirmed = x.EmailConfirmed,
        Roles = new List<string>(roles), // Assign list of roles
      };
      usersList.Add(user);
    }
    return usersList; // return users list
  }
  
  /// <summary>
  /// Get user
  /// </summary>
  /// <param name="id">user id</param>
  /// <returns>user matching the id</returns>
  [HttpGet("{id}")]
  public async Task<UserModel> Get(string id)
  {
    var userFound = await _users.FindByIdAsync(id);
    var roles = await _users.GetRolesAsync(userFound); // Get user roles
    var user = new UserModel
    {
      Id = userFound.Id,
      FullName = userFound.FullName,
      Email = userFound.Email,
      EmailConfirmed = userFound.EmailConfirmed,
      Roles = new List<string>(roles) // Assign list of roles
    };
    return user; // return user
  }
  
  /// <summary>
  /// Delete User by ID
  /// </summary>
  /// <param name="id"></param>
  /// <param name="userModel"></param>
  [Authorize (nameof(AuthPolicies.CanDeleteUsers))]
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete (string id, [FromBody] UserModel userModel)
  {
    var user = await _users.FindByIdAsync(id);
    if (user is null) return NotFound();
    await _users.DeleteAsync(user);
    if (userModel.SendUpdateEmail) // Check if send update email is true
      await _accountEmail.SendDeleteUpdate(
        new EmailAddress(userModel.Email){ Name = user.FullName });
    return NoContent();
  }
}
