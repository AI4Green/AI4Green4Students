using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using Org.BouncyCastle.Crypto.Macs;

namespace AI4Green4Students.Tests;
public static class StringConstants
{
  public const string GmailDomain = "@gmail.com";
  public const string MailDomain = "@mail.com";

  public const string AllowedMailEmail = "allowed@mail.com";
  public const string GoodGmailEmail = "GoodEmail@gmail.com";
  public const string HelloMailEmail = "hello@mail.com";
  public const string BlockedGmailEmail = "blocked@gmail.com";
  public const string SomeoneGmailEmail = "someone@gmail.com";
  public const string ValidGmailEmail = "valid@gmail.com";
  public const string ExampleMailEmail = "example@mail.com";

  public const string Wildcard = "*";
  
  // Project name
  public const string FirstProject = "Test Project";

  // plan sections
  public const string PlanFirstSection = "Plan First Section";
  public const string PlanSecondSection = "Plan Second Section";
  public const string PlanThirdSection = "Plan Third Section";
  public const string PlanFourthSection = "Plan Fourth Section";

  // report sections
  public const string ReportFirstSection = "Report First Section";
  public const string ReportSecondSection = "Report Second Section";
  public const string ReportThirdSection = "Report Third Section";
  
  // note sections
  public const string NoteFirstSection = "Note First Section";
  public const string NoteSecondSection = "Note Second Section";
  
  // project group
  public const string FirstProjectGroup = "Test Project Group";

  // Fields
  public const string FirstField = "Example Field One";
  public const string SecondField = "Example Field Two";
  public const string ThirdField = "Example Field Three";
  
  public const string CreatedField = "This is a 3rd field, created in the test.";

  public const string TriggerCause = "This value causes a new field to trigger";
  public const string TriggerField = "This is a triggered field!";
  
  // Field options
  public const string FirstOption = "This is the first option for a field!";
  public const string SecondOption = "And another one!";
  public const string ThirdOption = "And yet another...";
  
  // users
  public const string StudentUserOne = "Bob Tester";
  public const string StudentUserTwo = "Elon Tester";
  public const string InstructorUser = "Example Instructor";

  // Field responses
  public const string FirstResponse = "Here is a text response. La la la.";
  public const string SecondResponse = "This is an old response. Do not show me by default.";
  public const string ThirdResponse = "This is a new response. Show me by default.";
  public const string DefaultResponse = "This is the default response for a field.";
  public const string ApprovedResponse = "This is an approved response for a field.";

  // Comments
  public const string FirstComment = "This is terrible. Write it again!";
  public const string SecondComment = "You make an interesting point, however, please make reference to X";
  public const string InstructorComment = "This is a new comment left by a investigator";
  public const string StudentComment = "This is a new comment left by a student";

  // Plan
  public const string PlanOne = "Plan One";
  public const string PlanTwo = "Plan Two";
  public const string PlanThree = "Plan Three";
  public const string PlanFour = "Plan Four";
  
  public static string UnansweredField = "Field with no answer in place";

}
