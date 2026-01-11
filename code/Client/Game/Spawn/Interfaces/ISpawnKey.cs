/// <summary>
/// Marker interface for runtime visual identity keys.
/// <para>
/// Implementations represent stable identifiers used by the client
/// to track spawned visual instances during runtime.
/// </para>
/// <para>
/// Visual keys are:
/// - Client-side only
/// - Runtime-only
/// - Not serialized
/// - Not authoritative
/// </para>
/// </summary>
public interface ISpawnKey
{
}