using System.CommandLine;

namespace AI4Green4Students.Startup.Cli;

public class CliEntrypoint : RootCommand
{
    public CliEntrypoint() : base("AI4Green4Students CLI")
    {
        AddGlobalOption(new Option<string>(new[] { "--environment", "-e" }));

        // Add Commands here
    }
}
