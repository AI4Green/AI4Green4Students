namespace AI4Green4Students.Models.Emails
{
    /// <summary>
    /// View model used for sending a feedback completion email to a student.
    /// It contains details such as the project name, the student receiving the feedback, a URL link to the note, 
    /// the instructor's name, and the plan name.
    /// </summary>
    public record SendNoteFeedbackCompletionModel(string ProjectName, string Student, string NoteUrl, string InstructorName, string PlanName);
}
