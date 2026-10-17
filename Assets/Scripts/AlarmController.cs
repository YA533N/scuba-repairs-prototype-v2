using System;
using UnityEngine;

public class AlarmController : MonoBehaviour
{
    public static AlarmController Instance { get; private set; }
    public static event Action<bool> OnAlarmChanged;

    [SerializeField] private KeyCode testKey = KeyCode.K;

    public bool IsActive { get; private set; }

    private void Awake() { Instance = this; }

    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            if (IsActive) Deactivate(); else Activate();
        }
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        OnAlarmChanged?.Invoke(true);
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        OnAlarmChanged?.Invoke(false);
    }
}