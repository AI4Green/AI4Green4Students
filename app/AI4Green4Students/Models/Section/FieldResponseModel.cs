using System.Text.Json;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Utilities;

namespace AI4Green4Students.Models.Section;

public record FieldResponseModel(
  int Id,
  string FieldType,
  int FieldResponseId,
  JsonElement? Value,
  bool IsApproved,
  int EntityId
)
{
  public FieldResponseModel(FieldResponse entity, int id)
    : this(
      entity.Field.Id,
      entity.Field.InputType.Name,
      entity.Id,
      SerializerHelper.DeserializeOrDefault<JsonElement>(
        entity.FieldResponseValues.MaxBy(x => x.ResponseDate)?.Value
        ?? JsonSerializer.Serialize(entity.Field.DefaultResponse)),
      entity.Approved,
      id
    )
  { }
}