using UnityEngine;

public class Hatch : Interactable
{
    [Header("Slide")]
    public Vector3 openOffset = new Vector3(0f, 2.5f, 0f);
    public float slideSpeed = 3f;

    [Header("Wheel (optional)")]
    public Transform [] wheels;                 // child mesh that spins
    public float wheelSpinDegrees = 720f;   // two full turns
    public float unlockTime = 2f;           // seconds to spin

    [Header("Lock (optional)")]
    public bool requiresKeycard = false;
    public string requiredCardId = "main";

    [Header("Wheel Settings")]
    public Vector3 wheelSpinAxis = Vector3.forward;   // door normal

    Vector3 closedPos;
    Vector3 openPos;
    Quaternion[] wheelClosedRots;
    bool isOpen;
    float spinProgress;   // 0..1
    

    void Start()
    {
        closedPos = transform.localPosition;
        openPos = closedPos + openOffset;

        if (wheels != null)
        {
            wheelClosedRots = new Quaternion[wheels.Length];
            for (int i = 0; i < wheels.Length; i++)
                if (wheels[i] != null)
                    wheelClosedRots[i] = wheels[i].localRotation;
        }
    }

    public override void Interact(PlayerInteraction player)
    {
        if (!isOpen && requiresKeycard)
        {
            var inv = FindAnyObjectByType<PlayerInventory>();
            if (inv == null || !inv.HasCard(requiredCardId))
            {
                Debug.Log("Hatch locked. Needs keycard: " + requiredCardId);
                return;   // stay shut
            }
        }

        isOpen = !isOpen;
        if (!isOpen) spinProgress = 0f;   // reset for next open
    }

    void Update()
    {
        if (isOpen && spinProgress < 1f)
        {
            spinProgress = Mathf.Min(1f, spinProgress + Time.deltaTime / unlockTime);
            SpinWheels(wheelSpinDegrees * spinProgress);
            return;
        }

        Vector3 target = isOpen ? openPos : closedPos;
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition, target, slideSpeed * Time.deltaTime);

        if (!isOpen) UnwindWheels();
    }

    void SpinWheels(float degrees)
    {
        if (wheels == null) return;
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i] == null) continue;
            wheels[i].localRotation = wheelClosedRots[i] *
                Quaternion.AngleAxis(degrees, wheelSpinAxis);
        }
    }

    void UnwindWheels()
    {
        if (wheels == null) return;
        float speed = wheelSpinDegrees / unlockTime * Time.deltaTime;
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i] == null) continue;
            wheels[i].localRotation = Quaternion.RotateTowards(
                wheels[i].localRotation, wheelClosedRots[i], speed);
        }
    }
}