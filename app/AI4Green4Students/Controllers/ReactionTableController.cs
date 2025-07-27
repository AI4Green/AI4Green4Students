namespace AI4Green4Students.Controllers;

using System.Net;
using System.Text.Json;
using Config;
using Constants;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.ReactionTable;
using Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReactionTableController : ControllerBase
{
  private readonly ReactionTableService _reactionTable;
  private readonly WorkerOptions _worker;

  public ReactionTableController(IOptions<WorkerOptions> worker,
    ReactionTableService reactionTable)
  {
    _worker = worker.Value;
    _reactionTable = reactionTable;
  }

  /// <summary>
  /// Get data for generating a reaction table.
  /// </summary>
  /// <param name="reactants">Reactant Smiles.</param>
  /// <param name="products">Product Smiles.</param>
  /// <param name="smiles">Reaction Smiles.</param>
  /// <returns>Reaction data</returns>
  [HttpGet("data")]
  public async Task<ActionResult> GetReactionData(string reactants, string products, string smiles)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest();
    }

    var url = _worker.ApiUrl.TrimEnd('/') + "/api"
      .AppendPathSegment("reaction-table")
      .SetQueryParams(new
      {
        reactants, products, smiles
      });

    try
    {
      var response = await url.WithHeader("x-functions-key", _worker.ApiKey).GetStringAsync();
      var data = JsonSerializer.Deserialize<ReactionTableDataModel>(response, DefaultJsonOptions.Serializer);
      return Ok(data?.Compounds
        .Select(x => new CompoundModel
        {
          Name = x.Name,
          MolecularWeight = x.MolecularWeight,
          Density = x.Density,
          Hazards = x.Hazards,
          Smiles = x.Smiles,
          SubstanceType = x.SubstanceType
        })
        .ToList());
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

  /// <summary>
  /// Get compound list starting with queryName
  /// </summary>
  /// <param name="query">name to search for</param>
  /// <returns>list of compound names</returns>
  [HttpGet("compounds")]
  public async Task<List<PartialReagentModel>> ListCompounds(string query)
    => await _reactionTable.ListCompounds(query);

  /// <summary>
  /// Get compound list starting with queryName
  /// </summary>
  /// <returns>list of compound names</returns>
  [HttpGet("solvents")]
  public async Task<List<PartialSolventModel>> ListSolvents()
    => await _reactionTable.ListSolvents();

  /// <summary>
  /// Get reagent.
  /// </summary>
  /// <param name="name">Reagent name to search for.</param>
  /// <returns>Reagent.</returns>
  [HttpGet("reagent")]
  public async Task<ActionResult<CompoundModel>> GetReagentByName(string name)
  {
    try
    {
      return await _reactionTable.GetReagent(name);
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
  }

  /// <summary>
  /// Get solvent.
  /// </summary>
  /// <param name="name">Solvent name to search for.</param>
  /// <returns>Solvent.</returns>
  [HttpGet("solvent")]
  public async Task<ActionResult<SolventModel>> GetSolventByName(string name)
  {
    try
    {
      return await _reactionTable.GetSolvent(name);
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
  }
}
