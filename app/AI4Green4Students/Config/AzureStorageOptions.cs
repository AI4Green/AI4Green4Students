namespace AI4Green4Students.Config;

public class AzureStorageOptions
{
  public string ExperimentBlobContainer { get; set; } = "experiments";
  public string AI4GreenHttpEndpoint { get; set; } = string.Empty;
}
