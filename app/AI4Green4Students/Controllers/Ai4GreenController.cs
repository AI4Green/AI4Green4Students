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
  /// Get compound list starting with queryName
  /// </summary>
  /// <param name="queryName">name to search for</param>
  /// <returns>list of compound names</returns>
  [HttpGet("ListCompounds")]
  public async Task<List<PartialModel>> ListCompounds(string queryName)
    => await _reactionTable.ListCompounds(queryName);
  
  /// <summary>
  /// Get compound list starting with queryName
  /// </summary>
  /// <returns>list of compound names</returns>
  [HttpGet("ListSolvents")]
  public async Task<List<PartialModel>> ListSolvents()
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
