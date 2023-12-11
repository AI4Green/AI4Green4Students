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

  public const string FirstSection = "First Section";
  public const string SecondSection = "Second String";

  public const string FirstProject = "Test Project";

  public const string StudentUser = "Bob Tester";

  public const string FirstProjectGroup = "Test Project Group";

  public const string FirstExperiment = "First Experiment";

  public const string TextInput = "Texty";

  public const string FirstField = "Example Field";
  public const string SecondField = "Example Approved Field";

  public const string FirstResponse = "Here is a text response. La la la.";
  public const string SecondResponse = "This is an old response. Do not show me by default.";

  public const string FirstComment = "This is terrible. Write it again!";
  public const string SecondComment = "You make an interesting point, however, please make reference to X";

  public const string TriggerCause = "This value causes a new field to trigger";
  public const string TriggerField = "This is a triggered field!";
  public const string CreatedField = "This is a 3rd field, created in the test.";

  public const string FirstOption = "This is the first option for a field!";
  public const string SecondOption = "And another one!";
  public const string ThirdOption = "And yet another...";
}
