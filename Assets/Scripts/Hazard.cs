using UnityEngine;

public class Hazard : MonoBehaviour
{
    public enum HazardType { Leak, Fire }
    public HazardType type;
    public string requiredTool = "Wrench";
    public float oxygenDrainPerSecond = 1f;

    public bool IsActive { get; private set; } = true;

    void Start()
    {
        OxygenAndHull.Instance.RegisterHazard(this);
    }

    void OnTriggerEnter(Collider other)
    {
        Pickup p = other.GetComponent<Pickup>();
        if (p != null && p.toolType == requiredTool)
        {
            Extinguish();
        }
    }
    void Extinguish()
    {
        IsActive = false;
        gameObject.SetActive(false);
        OxygenAndHull.Instance.OnHazardFixed();
    }
}