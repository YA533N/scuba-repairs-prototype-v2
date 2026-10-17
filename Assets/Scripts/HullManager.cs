using System;
using UnityEngine;

public class HullManager : MonoBehaviour
{
    public static HullManager Instance { get; private set; }

    [Header("Hull Timer")]
    [SerializeField] private float totalSeconds = 20f;   // 20 seconds
    [SerializeField] private bool startRunning = true;

    [Header("Debug (read-only)")]
    [SerializeField] private float debugSecondsRemaining;

    public float SecondsRemaining { get; private set; }
    public float HullPercent => SecondsRemaining / totalSeconds;   // 1 = full, 0 = gone
    public bool IsRunning { get; private set; }
    public bool IsDepleted { get; private set; }

    public event Action<float> OnHullChanged;   // passes HullPercent
    public event Action OnHullDepleted;

    private void Awake()
    {
        Instance = this;
        SecondsRemaining = totalSeconds;
        IsRunning = startRunning;
    }

    private void Update()
    {
        if (!IsRunning || IsDepleted) return;

        SecondsRemaining -= Time.deltaTime;

        debugSecondsRemaining = SecondsRemaining;

        if (SecondsRemaining <= 0f)
        {
            SecondsRemaining = 0f;
            IsDepleted = true;
            IsRunning = false;
            OnHullChanged?.Invoke(HullPercent);
            OnHullDepleted?.Invoke();
            Debug.Log("Hull depleted!");
            return;
        }

        OnHullChanged?.Invoke(HullPercent);
    }

    public void AddTime(float seconds)
    {
        if (IsDepleted) return;
        SecondsRemaining = Mathf.Min(SecondsRemaining + seconds, totalSeconds);
        OnHullChanged?.Invoke(HullPercent);
    }

    public void StartTimer() => IsRunning = true;
    public void PauseTimer() => IsRunning = false;
}