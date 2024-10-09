namespace AI4Green4Students.Models.Emails
{
    /// <summary>
    /// View model used for sending a feedback request email to an instructor.
    /// It contains details such as the project name, the student requesting feedback, a URL link to the note, and the instructor's name.
    /// </summary>
    public record SendNoteFeedBackRequestModel(string ProjectName, string Student, string NoteUrl, string InstructorName, string planName);
}
