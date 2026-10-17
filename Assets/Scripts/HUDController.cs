using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public TMP_Text oxygenText;
    public TMP_Text hullText;

    [Header("Keycards")]
    public UnityEngine.UI.Text keycardText;

    void Start()
    {
        PlayerInventory.OnInventoryChanged += RefreshKeycards;
        RefreshKeycards();
    }
    
    
    void Update()
    {
        if (OxygenAndHull.Instance != null)
            oxygenText.text = $"Oxygen: {OxygenAndHull.Instance.oxygen:F0}%";

        if (HullManager.Instance != null)
            hullText.text = $"Hull: {HullManager.Instance.SecondsRemaining:F0}s";
    }

    void OnDestroy()
    {
        PlayerInventory.OnInventoryChanged -= RefreshKeycards;
    }

    void RefreshKeycards()
    {
        if (keycardText == null) return;
        var inv = FindObjectOfType<PlayerInventory>();
        if (inv == null) { keycardText.text = "Keycards: —"; return; }

        int n = 0;
        foreach (var _ in inv.Cards) n++;
        keycardText.text = n == 0 ? "Keycards: —" : $"Keycards: {n}";
    }
}