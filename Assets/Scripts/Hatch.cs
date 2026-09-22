using UnityEngine;

public class Hatch : Interactable
{
    public Vector3 openOffset = new Vector3(0f, 2.5f, 0f);
    public float slideSpeed = 3f;

    Vector3 closedPos;
    Vector3 openPos;
    bool isOpen;

    void Start()
    {
        closedPos = transform.localPosition;
        openPos = closedPos + openOffset;
    }

    public override void Interact(PlayerInteraction player)
    {
        isOpen = !isOpen;
    }

    void Update()
    {
        Vector3 target = isOpen ? openPos : closedPos;
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition, target, slideSpeed * Time.deltaTime);
    }
}