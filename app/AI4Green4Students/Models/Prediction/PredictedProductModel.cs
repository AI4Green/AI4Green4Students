namespace AI4Green4Students.Models.Prediction;

using System.Text.Json.Serialization;

public record PredictedProductModel(
  string Product,
  double Score,
  string ReactionImage,
  string IupacName,
  List<string> Synonyms
);

public record PredictedProductDataModel(
  string Product,
  double Score,
  [property: JsonPropertyName("reaction_image")]
  string ReactionImage,
  [property: JsonPropertyName("iupac_name")]
  string IupacName,
  List<string> Synonyms
);
