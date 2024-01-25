
namespace AI4Green4Students.Models.Stage;

public class StageModel
{
  private Data.Entities.Stage? _nextStage;

  public StageModel(Data.Entities.Stage? nextStage)
  {
    _nextStage = nextStage;
  }
}
