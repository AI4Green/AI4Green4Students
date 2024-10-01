using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Data.Config;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddDataDbContext(this IServiceCollection s, IConfiguration c)
  {
    s.AddDbContext<ApplicationDbContext>(o =>
    {
      var connectionString = c.GetConnectionString("Default");
      if (string.IsNullOrWhiteSpace(connectionString))
        o.UseNpgsql();
      else
        o.UseNpgsql(connectionString,
          o => o.EnableRetryOnFailure());
    });

    return s;
  }

  public static IServiceCollection AddAI4GreenDbContext(this IServiceCollection s, IConfiguration c)
  {
    s.AddDbContext<AI4GreenDbContext>(o =>
    {
      var connectionString = c.GetConnectionString("AI4Green");
      if (string.IsNullOrWhiteSpace(connectionString))
        o.UseNpgsql();
      else
        o.UseNpgsql(connectionString,
          o => o.EnableRetryOnFailure());
    });

    return s;
  }
}
