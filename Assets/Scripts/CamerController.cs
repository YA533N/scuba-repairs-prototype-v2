using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public static event System.Action OnCinematicEnded;

    // Higher number = Brain prefers this camera.
    // Only ONE vcam should hold the highest value at a time.
    public const int PriorityInactive    = 0;
    public const int PriorityFirstPerson = 10;
    public const int PriorityThirdPerson = 20;
    public const int PriorityCinematic   = 100;

    [Header("Virtual Cameras")]
    public CinemachineCamera firstPersonVcam;
    public CinemachineCamera thirdPersonVcam;
    public CinemachineCamera cinematicVcam;

    [Header("Input")]
    public KeyCode thirdPersonToggleKey = KeyCode.V;


    private bool isThirdPerson = false;
    private bool isCinematicActive = false;
    private float cinematicTimer = 0f;
    
    public bool IsCinematicActive => isCinematicActive;
    public static bool InputLocked => Instance != null && Instance.isCinematicActive;
    public static bool IsFirstPersonActive => Instance != null && 
                                                       !Instance.isThirdPerson && 
                                                       !Instance.isCinematicActive;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        SetFirstPerson();
    }

    private void Update()
    {
        if (Input.GetKeyDown(thirdPersonToggleKey) && !isCinematicActive)
        {
            if (isThirdPerson) SetFirstPerson();
            else SetThirdPerson();
        }

        if (isCinematicActive)
        {
            cinematicTimer -= Time.deltaTime;
            if (cinematicTimer <= 0f) RestoreDefaultView();
        }
    }

    public void SetFirstPerson()
    {
        isThirdPerson = false;
        firstPersonVcam.Priority = PriorityFirstPerson;
        thirdPersonVcam.Priority = PriorityInactive;
        cinematicVcam.Priority   = PriorityInactive;
    }

    public void SetThirdPerson()
    {
        isThirdPerson = true;
        firstPersonVcam.Priority = PriorityInactive;
        thirdPersonVcam.Priority = PriorityThirdPerson;
        cinematicVcam.Priority   = PriorityInactive;
    }

    private void RestoreDefaultView()
    {
        isCinematicActive = false;
        OnCinematicEnded?.Invoke();
        if (isThirdPerson) SetThirdPerson();
        else SetFirstPerson();
    }

        public void PlayCinematic(float duration)
    {
        if (isCinematicActive) return;   // don't stack
        isCinematicActive = true;
        cinematicTimer = duration;
        cinematicVcam.Priority = PriorityCinematic;
    }
}