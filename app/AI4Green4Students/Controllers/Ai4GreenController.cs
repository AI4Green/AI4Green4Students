using System.Text.Json;
using AI4Green4Students.Config;
using AI4Green4Students.Models.ReactionTable;
using AI4Green4Students.Services;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class Ai4GreenController : ControllerBase
{
  private readonly AZOptions _azConfig;
  private readonly ReactionTableService _reactionTable;

  public Ai4GreenController(IOptions<AZOptions> azConfig,
    ReactionTableService reactionTable)
  {
    _azConfig = azConfig.Value;
    _reactionTable = reactionTable;
  }

  /// <summary>
  /// Get reaction data from AI4Green
  /// </summary>
  /// <param name="reactants"></param>
  /// <param name="products"></param>
  /// <param name="reactionSmiles"></param>
  /// <returns>Reaction data</returns>
  [HttpGet("_Process")]
  public async Task<ActionResult> GetReactionData(string reactants, string products, string reactionSmiles)
  {
    var ai4GreenAZHttpTriggerUrl = _azConfig.AI4GreenHttpEndpoint
      .SetQueryParams(new
      {
        reactants,
        products,
        reactionSmiles
      });

    try
    {
      var jsonResponse = await ai4GreenAZHttpTriggerUrl.GetStringAsync();
      var jsonDocument = JsonDocument.Parse(jsonResponse);

      if (!jsonDocument.RootElement.TryGetProperty("data", out var dataElement))
        return NotFound("No data found or invalid response");
      
      var reactionData = JsonSerializer.Deserialize<ReactionDataModel>(dataElement.GetRawText(),
        new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });
      return Ok(_reactionTable.GetInitialTableData(reactionData));
    }
    catch (FlurlHttpException e)
    {
      var statusCode = e.Call.Response?.StatusCode ?? (int)System.Net.HttpStatusCode.InternalServerError;

      switch (statusCode)
      {
        // For now, just checking bad request and internal server error
        case (int)System.Net.HttpStatusCode.BadRequest:
          return BadRequest(new { message = "Invalid Request" });
        case (int)System.Net.HttpStatusCode.InternalServerError:
          return StatusCode(statusCode, new { message = "Internal Server Error" });
        default:
          var errorResponse = await e.GetResponseJsonAsync<object>() 
                              ?? new { message = "An error occurred, and the details could not be parsed." };
          return StatusCode(statusCode, errorResponse);
      }
    }
    catch (Exception e)
    {
      return StatusCode(500, new { message = $"Internal Server Error: {e.Message}" });
    }
  }

  /// <summary>
  /// Get compound list starting with queryName
  /// </summary>
  /// <param name="queryName">name to search for</param>
  /// <returns>list of compound names</returns>
  [HttpGet("ListCompounds")]
  public async Task<List<PartialReagentModel>> ListCompounds(string queryName)
    => await _reactionTable.ListCompounds(queryName);

  /// <summary>
  /// Get compound list starting with queryName
  /// </summary>
  /// <returns>list of compound names</returns>
  [HttpGet("ListSolvents")]
  public async Task<List<PartialSolventModel>> ListSolvents()
    => await _reactionTable.ListSolvents();

  /// <summary>
  /// Get reagent data
  /// </summary>
  /// <param name="reagentName">name to search for</param>
  /// <returns>Reagent data</returns>
  [HttpGet("Reagent")]
  public async Task<ActionResult<CompoundModel>> GetReagent(string reagentName)
  {
    try
    {
      return await _reactionTable.GetReagent(reagentName);
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
  }

  /// <summary>
  /// Get solvent data
  /// </summary>
  /// <param name="solventName">name to search for</param>
  /// <returns>Solvent data</returns>
  [HttpGet("Solvent")]
  public async Task<ActionResult<SolventModel>> GetSolvent(string solventName)
  {
    try
    {
      return await _reactionTable.GetSolvent(solventName);
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
  }
}
