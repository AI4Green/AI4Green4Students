namespace AI4Green4Students.Controllers;

using System.Net;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
  private readonly PredictionService _prediction;

  public PredictionController(PredictionService prediction) => _prediction = prediction;

  /// <summary>
  /// List predicted products based on smiles.
  /// </summary>
  /// <param name="smiles">Reaction Smiles.</param>
  /// <returns>Predicted products.</returns>
  [HttpGet("products")]
  public async Task<ActionResult> PredictProducts(string smiles)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest();
    }

    try
    {
      var predictions = await _prediction.PredictProducts(smiles);
      return Ok(predictions);
    }
    catch (FlurlHttpException e)
    {
      var statusCode = e.Call.Response?.StatusCode ?? (int)HttpStatusCode.InternalServerError;

      switch (statusCode)
      {
        case (int)HttpStatusCode.BadRequest:
          return BadRequest(new
          {
            message = "Invalid Request"
          });
        case (int)HttpStatusCode.InternalServerError:
          return StatusCode(statusCode, new
          {
            message = "Internal Server Error"
          });
        default:
          var errorResponse = await e.GetResponseJsonAsync<object>()
                              ?? new
                              {
                                message = "An error occurred, and the details could not be parsed."
                              };
          return StatusCode(statusCode, errorResponse);
      }
    }
    catch (Exception e)
    {
      return StatusCode(500, new
      {
        message = $"Internal Server Error: {e.Message}"
      });
    }
  }
}
