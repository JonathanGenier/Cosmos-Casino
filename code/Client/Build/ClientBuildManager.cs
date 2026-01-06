using CosmosCasino.Core.Build;
using CosmosCasino.Core.Console.Logging;

/// <summary>
/// Initializes the client build manager with access to required
/// core services through the client bootstrap.
/// </summary>
/// <param name="bootstrap">
/// Bootstrap context providing initialized services.
/// </param>
public sealed partial class ClientBuildManager(ClientBootstrap bootstrap) : ClientManager(bootstrap)
{
    /// <summary>
    /// Executes the specified build intent against the authoritative
    /// build system and logs the per-cell operation outcomes.
    /// </summary>
    /// <param name="buildIntent">
    /// The build intent to execute.
    /// </param>
    public void ExecuteBuildIntent(BuildIntent buildIntent)
    {
        BuildResult buildResult = CoreServices.BuildManager.ApplyBuildOperations(buildIntent);

        ConsoleLog.Info(nameof(ClientBuildManager), $"Number of cells: {buildIntent.Cells.Count.ToString()}");

        foreach (var result in buildResult.Results)
        {
            ConsoleLog.Info(nameof(ClientBuildManager), $"Cell {result.Cell.ToString()} -> Outcome : {result.Outcome.ToString()}");
        }
    }
}