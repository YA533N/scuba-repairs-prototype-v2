using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float reach = 3f;
    public float holdDistance = 1.6f;
    public float carryStrength = 15f;
    public float maxCarrySpeed = 12f;
    public float rotateSpeed = 120f;

    Pickup held;
    Rigidbody heldRb;
    CharacterController playerCC;

    public Pickup Held => held;

    void Start()
    {
        playerCC = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)){ TryInteract(); }
        if (Input.GetMouseButtonDown(0))
        {
            if (held == null) TryGrab(); else Drop();
        }
        if (held != null)
            holdDistance = Mathf.Clamp(holdDistance + Input.mouseScrollDelta.y * 0.2f, 1f, reach);
    }

    void FixedUpdate()
    {
        if (held == null) return;

        Vector3 target = transform.position + transform.forward * holdDistance;
        Vector3 v = Vector3.ClampMagnitude((target - heldRb.position) * carryStrength, maxCarrySpeed);
        #if UNITY_6000_0_OR_NEWER
                heldRb.linearVelocity = v;
        #else
                heldRb.velocity = v;
        #endif
        float yaw = Input.GetKey(KeyCode.R) ? rotateSpeed : 0f;
        float pitch = Input.GetKey(KeyCode.T) ? rotateSpeed : 0f;
        heldRb.angularVelocity = new Vector3(pitch, yaw, 0f) * Mathf.Deg2Rad;
    }

    void TryGrab()
    {
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, reach)) return;
        Pickup p = hit.collider.GetComponentInParent<Pickup>();
        if (p == null) return;

        held = p;
        heldRb = p.GetComponent<Rigidbody>();
        heldRb.useGravity = false;
        SetIgnorePlayer(true);
    }

    void Drop()
    {
        heldRb.useGravity = true;
        SetIgnorePlayer(false);
        held = null;
        heldRb = null;
    }

    void SetIgnorePlayer(bool ignore)
    {
        if (playerCC == null) return;
        foreach (Collider c in held.GetComponentsInChildren<Collider>())
            Physics.IgnoreCollision(playerCC, c, ignore);
    }

    void TryInteract()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, reach);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (held != null && hit.collider.GetComponentInParent<Pickup>() == held) continue;

            Interactable target = hit.collider.GetComponentInParent<Interactable>();

            if (target != null)
            {
                target.Interact(this);
                return;
            }

            if (!hit.collider.isTrigger) return;
        }
    }
}