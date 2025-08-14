namespace AI4Green4Students.Services;

using System.Text.Json;
using Config;
using Constants;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Models.Prediction;

public class PredictionService
{
  private readonly PredictorOptions _predictor;

  public PredictionService(IOptions<PredictorOptions> predictor) => _predictor = predictor.Value;

  /// <summary>
  /// Forward prediction based on reaction smiles.
  /// </summary>
  /// <param name="smiles">Reaction smiles.</param>
  /// <returns>Results.</returns>
  public async Task<List<ForwardPredictionModel>> ForwardPrediction(string smiles)
  {
    var url = _predictor.ForwardPredictionApiUrl.TrimEnd('/') + "/api"
      .AppendPathSegment("predict");

    var requestBody = new { smiles };

    var response = await url
      .WithHeader("X-API-Key", _predictor.ForwardPredictionApiKey)
      .PostJsonAsync(requestBody)
      .ReceiveString();

    var data = JsonSerializer.Deserialize<ForwardPredictionResultDataModel>(response, DefaultJsonOptions.Serializer);

    return data?.Result.Select(x => new ForwardPredictionModel(
      x.Product,
      Math.Round(x.Score, 2),
      x.ReactionImage,
      x.IupacName,
      x.Synonyms
    )).ToList() ?? new List<ForwardPredictionModel>();
  }
}
