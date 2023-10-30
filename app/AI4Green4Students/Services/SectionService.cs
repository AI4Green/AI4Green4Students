using AI4Green4Students.Data;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class SectionService
{
  private readonly ApplicationDbContext _db;

  public SectionService(ApplicationDbContext db)
  {
    _db = db;
  }

  /// <summary>
  /// Get all sections for a specific project and a specific user.
  /// </summary>
  /// <param name="experimentId"></param>
  /// <param name="userId"></param>
  /// <returns></returns>
  public async Task<List<SectionModel>> List(int experimentId, string userId)
  {
    var sections = _db.Sections.Where(x => x.Experiment.Id == experimentId)
      .Include(section => section.Fields)
      .Select(x => new SectionModel
    {
      Id = x.Id,
      Name = x.Name,
      Approved = x.Approved
    }).ToList();

foreach(var s  in sections) 
    {
      var fields = _db.Fields.Where(x => x.Section.Id == s.Id)
                                         .Include(x => x.FieldResponses)
                                         .ThenInclude(x => x.Conversation)
                                         .ThenInclude(x => x.Comments).ToList();

     s.Comments = fields.SelectMany(field => field.FieldResponses)
                                               .Select(fieldResponse => fieldResponse.Conversation)
                                               .Sum(conversation => conversation.Comments.Count);                                          
    }
    return sections;
  }
}
