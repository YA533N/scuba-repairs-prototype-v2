using UnityEngine;

public class Keycard : Interactable
{
    public string cardId = "main";

    public override void Interact(PlayerInteraction player)
    {
        var inv = FindAnyObjectByType<PlayerInventory>();
        if (inv == null)
        {
            Debug.LogWarning("No PlayerInventory on the player.");
            return;
        }

        inv.AddCard(cardId);
        gameObject.SetActive(false);   // disappears once collected
    }
}