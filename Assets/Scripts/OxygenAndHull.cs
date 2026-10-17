using UnityEngine;
using System.Collections.Generic;

public class OxygenAndHull : MonoBehaviour
{
    public static OxygenAndHull Instance;

    public float oxygen = 100f;
    public float repairHullBonus = 15f;

    List<Hazard> activeHazards = new List<Hazard>();

    void Awake() { Instance = this; }

    void Update()
    {
        activeHazards.RemoveAll(h => h == null || !h.IsActive);

        float drain = 0f;
        foreach (Hazard h in activeHazards) drain += h.oxygenDrainPerSecond;
        oxygen = Mathf.Max(0f, oxygen - drain * Time.deltaTime);
    }

    public void RegisterHazard(Hazard h) => activeHazards.Add(h);

    public void OnHazardFixed()
    {
        if (HullManager.Instance != null)
        {
            HullManager.Instance.AddTime(repairHullBonus);
        }
    }
}