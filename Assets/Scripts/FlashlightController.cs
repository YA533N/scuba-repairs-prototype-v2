using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("References")]
    public Light flashlightBeam;

    [Header("Input")]
    public KeyCode toggleKey = KeyCode.F;

    [Header("Battery")]
    public float batteryMax = 100f;
    public float drainPerSecond = 0.6f;   // ~2.5 min of full-on use

    private float battery;
    private bool isOn = true;

    public float BatteryPercent => battery / batteryMax;
    public bool IsOn => isOn;

    void Awake()
    {
        battery = batteryMax;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey) && battery > 0f)
            isOn = !isOn;

        if (isOn && battery > 0f)
        {
            battery -= drainPerSecond * Time.deltaTime;
            if (battery <= 0f)
            {
                battery = 0f;
                isOn = false;
            }
        }

        ApplyLightState();
    }

    void ApplyLightState()
    {
        if (flashlightBeam == null) return;

        bool shouldBeOn = isOn && battery > 0f;
        flashlightBeam.enabled = shouldBeOn;

        if (!shouldBeOn) return;

        // Dim as battery falls below 25%
        float low = Mathf.InverseLerp(0f, batteryMax * 0.25f, battery);
        flashlightBeam.intensity = Mathf.Lerp(0.6f, 4f, low);
    }

    public void AddBattery(float amount)
    {
        battery = Mathf.Clamp(battery + amount, 0f, batteryMax);
    }
}