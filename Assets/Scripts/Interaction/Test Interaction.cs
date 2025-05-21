using UnityEngine;

public class TestInteraction : MonoBehaviour, IInteractable
{
    public void OnStartLook()
    {
        Debug.Log("Looking at: " + this.gameObject.name);
    }

    public void OnInteract()
    {
        Debug.Log("Interacted with: " + this.gameObject.name);
    }

    public void OnEndLook()
    {
        Debug.Log("Stopped looking at: " + this.gameObject.name);
    }
}
