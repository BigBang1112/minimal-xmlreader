namespace MinimalXmlReader;

/// <summary>
/// Represents the type of the end element.
/// </summary>
public enum EndType
{
    /// <summary>
    /// None
    /// </summary>
    None,
    /// <summary>
    /// Self-closed
    /// </summary>
    SelfClosed,
    /// <summary>
    /// Processing instruction
    /// </summary>
    ProcessingInstruction
}
