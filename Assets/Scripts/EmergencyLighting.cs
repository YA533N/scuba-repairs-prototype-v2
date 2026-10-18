using UnityEngine;
using System.Collections.Generic;

public class EmergencyLighting : MonoBehaviour
{
    [Header("Lights to control")]
    [SerializeField] private Transform[] lightsParents;
    [SerializeField] private bool includeInactive = true;

    [Header("Per-light randomness")]
    [SerializeField] [Range(0f, 0.3f)] private float thresholdJitter = 0.1f;
    [SerializeField] [Range(0f, 0.5f)] private float speedJitter = 0.4f;

    [Header("Color shift")]
    [SerializeField] private Color emergencyColor = new Color(1f, 0.1f, 0.05f);
    [SerializeField] [Range(0f, 1f)] private float shiftStartsAt = 0.5f;   // hull % where white starts turning red
    [SerializeField] [Range(0f, 1f)] private float fullRedAt = 0.15f;      // hull % where it is fully red

    [Header("Flicker")]
    [SerializeField] [Range(0f, 1f)] private float flickerStartsAt = 0.3f;
    [SerializeField] private float flickerSpeed = 8f;
    [SerializeField] [Range(0f, 1f)] private float flickerStrength = 0.6f;

    [Header("Alarm response")]
    [SerializeField] private float alarmFadeSeconds = 4f;
    [SerializeField] [Range(0f, 1f)] private float alarmDimLevel = 0.5f;      // lights drop to this fraction
    [SerializeField] [Range(0f, 1f)] private float alarmBaseFlicker = 0.35f;  // flicker even at full hull
    [SerializeField] [Range(0f, 1f)] private float alarmRedTint = 0.3f;

    private bool alarmOn;
    private float alertBlend;   // 0 = calm, 1 = full alarm

    private void OnEnable()  { AlarmController.OnAlarmChanged += HandleAlarm; }
    private void OnDisable() { AlarmController.OnAlarmChanged -= HandleAlarm; }
    private void HandleAlarm(bool on) => alarmOn = on;

    private Light[] lights;
    private Color[] baseColors;
    private float[] baseIntensities;
    private float[] noiseOffsets;
    private float[] speedMults;
    private float[] thresholdOffsets;
    private float hullPercent = 1f;

    private void Awake()
    {
        HashSet<Light> found = new HashSet<Light>();
        foreach (Transform parent in lightsParents)
        {
            if (parent == null) continue;
            foreach (Light l in parent.GetComponentsInChildren<Light>(includeInactive))
                if (l.GetComponentInParent<AlarmUnit>(true) == null)
                    found.Add(l);
        }
        lights = new Light[found.Count];
        found.CopyTo(lights);

        int n = lights.Length;
        baseColors = new Color[n];
        baseIntensities = new float[n];
        noiseOffsets = new float[n];
        speedMults = new float[n];
        thresholdOffsets = new float[n];

        for (int i = 0; i < n; i++)
        {
            baseColors[i] = lights[i].color;
            baseIntensities[i] = lights[i].intensity;

            noiseOffsets[i] = Random.Range(0f, 1000f);
            speedMults[i] = 1f + Random.Range(-speedJitter, speedJitter);
            thresholdOffsets[i] = Random.Range(-thresholdJitter, thresholdJitter);
        }
    }

    private void Start()
    {
        if (HullManager.Instance != null)
            HullManager.Instance.OnHullChanged += HandleHullChanged;
    }

    private void OnDestroy()
    {
        if (HullManager.Instance != null)
            HullManager.Instance.OnHullChanged -= HandleHullChanged;
    }

    private void HandleHullChanged(float percent) => hullPercent = percent;

    private void Update()
    {
        alertBlend = Mathf.MoveTowards(alertBlend, alarmOn ? 1f : 0f,
                                    Time.deltaTime / alarmFadeSeconds);

        float hullDanger = Mathf.InverseLerp(shiftStartsAt, fullRedAt, hullPercent) * alertBlend;
        float danger = Mathf.Max(hullDanger, alertBlend * alarmRedTint);

        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] == null) continue;
            lights[i].color = Color.Lerp(baseColors[i], emergencyColor, danger);
        }

        ApplyFlicker();
    }

    private void ApplyFlicker()
    {
        float dim = Mathf.Lerp(1f, alarmDimLevel, alertBlend);

        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] == null) continue;

            float start = Mathf.Clamp01(flickerStartsAt + thresholdOffsets[i]);

            float hullEffect = hullPercent < start ? Mathf.InverseLerp(start, 0f, hullPercent) : 0f;
            float effect = Mathf.Max(hullEffect * alertBlend, alertBlend * alarmBaseFlicker);

            float flicker = 1f;
            if (effect > 0f)
            {
                float noise = Mathf.PerlinNoise(Time.time * flickerSpeed * speedMults[i], noiseOffsets[i]);
                flicker = Mathf.Lerp(1f, noise, flickerStrength * effect);

                if (Random.value < 0.01f * effect)
                    flicker *= 0.1f;
            }

            lights[i].intensity = baseIntensities[i] * flicker * dim;
        }
    }
}