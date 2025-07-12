using AI4Green4Students.Data;
using AI4Green4Students.Data.DefaultExperimentSeeding;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Identity;

namespace AI4Green4Students.Startup.Web;

public static class WebInitialisation
{
    /// <summary>
    ///  Do any app initialisation after the app has been built (and DI services are locked down)
    ///
    /// e.g. Internal Data Seeding on App Startup
    /// </summary>
    /// <param name="app"></param>
    public static async Task Initialise(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var db = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var roles = scope.ServiceProvider
          .GetRequiredService<RoleManager<IdentityRole>>();

        var registrationRule = scope.ServiceProvider
          .GetRequiredService<RegistrationRuleService>();

        var config = scope.ServiceProvider
          .GetRequiredService<IConfiguration>();

        var users = scope.ServiceProvider
          .GetRequiredService<UserManager<ApplicationUser>>();

        var passwordHasher = scope.ServiceProvider
          .GetRequiredService<IPasswordHasher<ApplicationUser>>();

        var inputTypes = scope.ServiceProvider
          .GetRequiredService<InputTypeService>();

        var sectionTypes = scope.ServiceProvider
          .GetRequiredService<SectionTypeService>();

        var sections = scope.ServiceProvider
          .GetRequiredService<SectionService>();

        var fields = scope.ServiceProvider
          .GetRequiredService<FieldService>();

        var seeder = new DataSeeder(db, roles, registrationRule, users, passwordHasher, config, inputTypes, sectionTypes);
        await seeder.SeedRoles();
        await seeder.SeedRegistrationRules();
        await seeder.SeedAdminUser();
        await seeder.SeedInputTypes();
        await seeder.SeedSectionTypes();
        await seeder.SeedStageType();
        await seeder.SeedStage();
        await seeder.SeedStagePermission();

        //todo - move this to a CLI command for creating default experiment, complete with fields
        //We may keep this seeding option in as an example experiment for users to look at
        var defaultExperimentSeeder = new DefaultExperimentDataSeeder(db, sections, inputTypes, fields, sectionTypes, users);
        await defaultExperimentSeeder.SeedDefaultExperiment();
        
    }
    
}
