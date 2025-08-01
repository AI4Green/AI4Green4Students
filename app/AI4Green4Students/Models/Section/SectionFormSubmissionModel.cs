using System.Text.Json;

namespace AI4Green4Students.Models.Section;

/// <summary>
/// Payload sent from the client to save a section.
/// </summary>
public class SectionFormPayloadModel
{
  /// <summary>
  /// Id of the section that is being saved.
  /// </summary>
  public int SectionId { get; set; }
  
  /// <summary>
  /// Id of the record that is being saved, which could be PlanId, LiteratureReviewId.
  /// </summary>
  public int RecordId { get; set; } 
  
  /// <summary>
  /// JSON string representing the field responses that are being saved. This string is used for updating existing field responses.
  /// </summary>
  /// <remarks>
  /// The JSON string contains an array of field responses, where each field response is represented as an object with properties "id" and "value".
  /// The "id" property corresponds to the field response id.
  /// </remarks>
  public string FieldResponses { get; set; } = String.Empty;
  
  /// <summary>
  /// JSON string representing the field responses that are being saved. This string is used for creating new field responses.
  /// </summary>
  /// <remarks>
  /// The JSON string contains an array of field responses, where each field response is represented as an object with properties "id" and "value".
  /// The "id" property corresponds to the field id for which the field response is being created.
  /// </remarks>
  public string NewFieldResponses { get; set; } = String.Empty;
  
  /// <summary>
  /// List of files to be uploaded.
  /// </summary>
  public List<IFormFile> Files { get; set; } = new();
  
  /// <summary>
  /// JSON string of the metadata of the files to be uploaded. This metadata is used for updating existing field responses.
  /// <remarks>
  /// The JSON string contains an array of field responses, where each field response is represented as an object with properties "id" and "value".
  /// The "id" property corresponds to the field response id.
  /// The "value" property corresponds to the metadata of the file.
  /// Ensure that the order of metadata entries matches the order of files in Files.
  /// </remarks>
  /// </summary>
  public string FileFieldResponses { get; set; } = String.Empty;
  
  /// <summary>
  /// List of files to be uploaded.
  /// </summary>
  public List<IFormFile> NewFiles { get; set; } = new();
  
  /// <summary>
  /// JSON string of the metadata of the files to be uploaded. This metadata is used for creating new field responses.
  /// <remarks>
  /// The JSON string contains an array of field responses, where each field response is represented as an object with properties "id" and "value".
  /// The "id" property corresponds to the field id. The "value" property corresponds to the metadata of the file.
  /// Ensure that the order of metadata entries matches the order of files in NewFiles.
  /// </remarks>
  /// </summary>
  public string NewFileFieldResponses { get; set; } = String.Empty;
}

/// <summary>
/// Payload that has been transformed to be used for saving a section.
/// </summary>
public class SectionFormSubmissionModel
{
  public int SectionId { get; set; }
  public List<CreateFieldResponseModel> FieldResponses { get; set; } = new();
  public List<CreateFieldResponseModel> NewFieldResponses { get; set; } = new();
  public int RecordId { get; set; }
}

/// <summary>
/// Model representing a field response.
/// <param name="Id">Field id or field response id.</param>
/// <param name="Value">JSON string of the field response.</param>
/// </summary>
public record CreateFieldResponseModel(int Id, string Value);

/// <summary>
/// Model for JSON string of field response.
/// </summary>
/// <param name="Id">Field id or field response id.</param>
/// <param name="Value">JSON string of the field response.</param>
public record FieldResponseJsonModel(int Id, JsonElement Value);
