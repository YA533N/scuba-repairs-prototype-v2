using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public TMP_Text oxygenText;
    public TMP_Text hullText;

    void Update()
    {
        if (OxygenAndHull.Instance == null) return;
        oxygenText.text = $"Oxygen: {OxygenAndHull.Instance.oxygen:F0}%";
        hullText.text = $"Hull: {OxygenAndHull.Instance.hullTime:F0}s";
    }
}