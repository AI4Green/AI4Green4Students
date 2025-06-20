using AI4Green4Students.Constants;
using AI4Green4Students.Data.Entities.SectionTypeData;

namespace AI4Green4Students.Utilities;

public static class SectionTypeHelper
{
  /// <summary>
  /// Get section data type name from an entity type.
  /// </summary>
  /// <returns>Section data type name.</returns>
  public static string GetSectionTypeName<T>() where T : CoreSectionTypeData => typeof(T).Name switch
  {
    nameof(Plan) => SectionTypes.Plan,
    nameof(Note) => SectionTypes.Note,
    nameof(Report) => SectionTypes.Report,
    nameof(LiteratureReview) => SectionTypes.LiteratureReview,
    _ => throw new InvalidOperationException($"Unsupported section type: {typeof(T).Name}")
  };
}
