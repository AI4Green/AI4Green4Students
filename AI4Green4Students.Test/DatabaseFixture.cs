using AI4Green4Students.Data;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Tests;
public class DatabaseFixture
{
  public readonly ApplicationDbContext DbContext;

  public DatabaseFixture()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase")
        .Options;

    DbContext = new ApplicationDbContext(options);
  }

}
