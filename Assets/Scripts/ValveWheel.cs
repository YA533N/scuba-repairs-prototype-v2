using UnityEngine;

public class ValveWheel : Interactable
{
    public Vector3 turnAxis = Vector3.forward;
    public float degreesPerPress = 180f;
    public float turnSpeed = 180f;

    public bool IsOpen { get; private set; } = true;

    Quaternion targetRotation;

    void Start()
    {
        targetRotation = transform.localRotation;
    }

    public override void Interact(PlayerInteraction player)
    {
        IsOpen = !IsOpen;
        targetRotation *= Quaternion.AngleAxis(degreesPerPress, turnAxis);
    }

    void Update()
    {
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, turnSpeed * Time.deltaTime);
    }
}