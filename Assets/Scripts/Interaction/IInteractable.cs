public interface IInteractable
{
    /// <summary>
    /// Is called whenever the player looks at the object.
    /// </summary>
    public void OnStartLook();

    /// <summary>
    /// Is called whenever the player presses interact on the object.
    /// </summary>
    public void OnInteract();

    /// <summary>
    /// Is called whenever the player stops looking at the object.
    /// </summary>
    public void OnEndLook();
}
