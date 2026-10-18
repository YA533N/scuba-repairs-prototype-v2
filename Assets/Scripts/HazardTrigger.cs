using UnityEngine;

public class HazardTrigger : MonoBehaviour
{
    [SerializeField] private float cinematicSeconds = 4f;
    [SerializeField] private GameObject burstLeak;   // a Leak prefab placed at the pipe, starts disabled

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        if (burstLeak != null) burstLeak.SetActive(true);

        CameraController.OnCinematicEnded += HandleCinematicEnded;
        CameraController.Instance.PlayCinematic(cinematicSeconds);
    }

    private void HandleCinematicEnded()
    {
        CameraController.OnCinematicEnded -= HandleCinematicEnded;

        if (AlarmController.Instance != null)
            AlarmController.Instance.Activate();
    }

    private void OnDisable()
    {
        CameraController.OnCinematicEnded -= HandleCinematicEnded;
    }
}