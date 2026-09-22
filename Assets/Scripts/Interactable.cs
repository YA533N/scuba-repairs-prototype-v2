using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

    public string prompt = "Interact"; // The prompt to display when the player can interact with this object

    public abstract void Interact(PlayerInteraction Player);

}
