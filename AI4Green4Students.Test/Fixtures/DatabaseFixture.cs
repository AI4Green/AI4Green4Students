using AI4Green4Students.Data;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Tests.Fixtures;
public class DatabaseFixture
{
  public readonly ApplicationDbContext DbContext;

  public DatabaseFixture()
  {
    DbContext = CreateNewContext();
  }

  public ApplicationDbContext CreateNewContext()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    return new ApplicationDbContext(options);
  }
}
