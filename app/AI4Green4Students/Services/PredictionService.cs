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
  private readonly WorkerOptions _worker;

  public PredictionService(IOptions<WorkerOptions> worker) => _worker = worker.Value;

  /// <summary>
  /// Predict products based on reaction smiles.
  /// </summary>
  /// <param name="smiles">Reaction smiles.</param>
  /// <param name="firstN">Number of products to return.</param>
  /// <returns>Predicted products.</returns>
  public async Task<List<PredictedProductModel>> PredictProducts(string smiles, int firstN = 5)
  {
    var url = _worker.ApiUrl.TrimEnd('/') + "/api"
      .AppendPathSegment("predict-products")
      .SetQueryParams(new
      {
        smiles, firstN
      });

    var response = await url.WithHeader("x-functions-key", _worker.ApiKey).GetStringAsync();
    var data = JsonSerializer.Deserialize<List<PredictedProductDataModel>>(response, DefaultJsonOptions.Serializer);

    return data?.Select(x => new PredictedProductModel(
      x.Product,
      Math.Round(x.Score, 2),
      x.ReactionImage,
      x.IupacName,
      x.Synonyms
    )).ToList() ?? new List<PredictedProductModel>();
  }
}
