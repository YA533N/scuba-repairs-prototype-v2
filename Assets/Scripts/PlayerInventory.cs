using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<string> cards = new List<string>();

    public bool HasCard(string id) => cards.Contains(id);

    public void AddCard(string id)
    {
        if (!cards.Contains(id)) cards.Add(id);
    }
}