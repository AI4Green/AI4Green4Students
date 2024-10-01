using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using AI4Green4Students.Commands.Helpers;
using AI4Green4Students.Startup.Cli;
using AI4Green4Students.Startup.EfCoreMigrations;
using AI4Green4Students.Startup.Web;

// Enable EF Core tooling to get a DbContext configuration
EfCoreMigrationsEntrypoint.BootstrapDbContext(args);

// Global App Startup stuff here

// Initialise the command line parser and run the appropriate entrypoint
await new CommandLineBuilder(new CliEntrypoint())
  .UseDefaults()
  .UseRootCommandBypass(args, WebEntrypoint.Run)
  .UseCliHostDefaults(args)
  .Build()
  .InvokeAsync(args);
