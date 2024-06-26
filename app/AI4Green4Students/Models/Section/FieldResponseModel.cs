using System.Text.Json;
using AI4Green4Students.Data.Entities;

namespace AI4Green4Students.Models.Section;

public class FieldResponseModel
{
  
  public FieldResponseModel(FieldResponse entity)
  {
    Id = entity.Field.Id;
    FieldType = entity.Field.InputType.Name;
    FieldResponseId = entity.Id;
    IsApproved = entity.Approved;
  }
  
  
  public int Id { get; set; }
  public string FieldType { get; set; } = string.Empty;
  public int FieldResponseId { get; set; }
  public JsonElement? Value { get; set; }
  public bool IsApproved { get; set; } 
}
