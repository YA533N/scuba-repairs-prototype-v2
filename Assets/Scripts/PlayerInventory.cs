using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static event System.Action OnInventoryChanged;

    private readonly HashSet<string> cards = new HashSet<string>();

    public bool HasCard(string id) => cards.Contains(id);

    public void AddCard(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        if (cards.Add(id)) OnInventoryChanged?.Invoke();
    }

    public IEnumerable<string> Cards => cards;
}