using UnityEngine;

public class AlarmUnit : MonoBehaviour
{
    [SerializeField] private Light[] spotLights;      // SpotL, SpotR
    [SerializeField] private Behaviour[] rotators;    // the rotation script (or Animator)
    [SerializeField] private AudioSource alarmAudio;  // alarm_warning

    private void Awake()
    {
        if (alarmAudio != null) alarmAudio.loop = true;
        SetState(false);
    }

    private void OnEnable()  { AlarmController.OnAlarmChanged += SetState; }
    private void OnDisable() { AlarmController.OnAlarmChanged -= SetState; }

    private void Start()
    {
        // sync with the current state, and stop any Play On Awake that fired late
        SetState(AlarmController.Instance != null && AlarmController.Instance.IsActive);
    }

    private void SetState(bool on)
    {
        foreach (Light l in spotLights)
            if (l != null) l.enabled = on;

        foreach (Behaviour b in rotators)
            if (b != null) b.enabled = on;

        if (alarmAudio != null)
        {
            if (on) alarmAudio.Play();
            else alarmAudio.Stop();
        }
    }
}