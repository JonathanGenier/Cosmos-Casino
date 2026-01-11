/// <summary>
/// Defines the contract for an input module that participates in input processing within a specific phase and scope.
/// </summary>
/// <remarks>Implementations of this interface are used to handle input events in a modular and extensible manner.
/// The processing order of input modules is determined by their assigned phase. Modules can be enabled or disabled
/// dynamically, and their scope determines which input events they receive.</remarks>
public interface IInputModule
{
    /// <summary>
    /// Gets the current input phase for the associated input module.
    /// </summary>
    InputPhase Phase { get; }

    /// <summary>
    /// Gets the input scope that defines the expected type of user input for the associated app state.
    /// </summary>
    InputScope Scope { get; }

    /// <summary>
    /// Gets a value indicating whether the module is enabled.
    /// </summary>
    bool IsEnabled { get; }
}