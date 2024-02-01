using AI4Green4Students.Config;
using AI4Green4Students.Models.ReactionTable;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Ai4GreenController : ControllerBase
{
  private readonly AZOptions _azConfig;
  private readonly ReactionTableService _reactionTable;

  public Ai4GreenController(IOptions<AZOptions> azConfig, ReactionTableService reactionTable)
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
  [HttpGet("_process")]
  public async Task<ActionResult> GetReactionData(string reactants, string products, string reactionSmiles)
  {
    var httpClient = new HttpClient();
    var ai4GreenAZHttpTriggerUrl =
      $"{_azConfig.AI4GreenHttpEndpoint}?reactants={reactants}&products={products}&reactionSmiles={reactionSmiles}";

    try
    {
      var response = await httpClient.GetAsync(ai4GreenAZHttpTriggerUrl);

      if (response.IsSuccessStatusCode) return Ok(await response.Content.ReadAsStringAsync());

      if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) return BadRequest();

      // if we get here, return the status code and the response body
      return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
    }
    catch (Exception e)
    {
      return StatusCode(500, $"Internal Server Error: {e.Message}");
    }
  }

  /// <summary>
  /// Get partial reagents i.e. list of reagent names that start with the query name
  /// </summary>
  /// <param name="queryName">name to search for</param>
  /// <returns>list of reagent names</returns>
  [HttpGet("partialReagents")]
  public async Task<List<ReagentPartialModel>> ListPartialReagents(string queryName)
    => await _reactionTable.ListPartialReagents(queryName);

  /// <summary>
  /// Get reagent data
  /// </summary>
  /// <param name="reagentName">name to search for</param>
  /// <returns>list of reagent names</returns>
  [HttpGet("reagent")]
  public async Task<ReagentModel> GetReagent(string reagentName)
    => await _reactionTable.GetReagent(reagentName);
}
