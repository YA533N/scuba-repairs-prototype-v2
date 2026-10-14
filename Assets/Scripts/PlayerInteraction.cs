using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform lookPivot;

    public float reach = 3f;
    public float holdDistance = 1.6f;
    public float carryStrength = 15f;
    public float maxCarrySpeed = 12f;
    public float rotateSpeed = 120f;

    Pickup held;
    Rigidbody heldRb;
    CharacterController playerCC;
    Renderer[] heldRenderers;
    bool [] heldRendererOriginalEnabled;

    public Pickup Held => held;

    void Start()
    {
        playerCC = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        if (held != null ) { SetHeldVisible(CameraController.IsFirstPersonActive); }
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

        Vector3 target = lookPivot.position + lookPivot.forward * holdDistance;
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
        if (!Physics.Raycast(lookPivot.position, lookPivot.forward, out RaycastHit hit, reach)) return;
        Pickup p = hit.collider.GetComponentInParent<Pickup>();
        if (p == null) return;

        held = p;
        CacheHeldRenderers(held.gameObject);
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
        RaycastHit[] hits = Physics.RaycastAll(lookPivot.position, lookPivot.forward, reach);
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

    void CacheHeldRenderers(GameObject obj)
    {
        heldRenderers = obj.GetComponentsInChildren<Renderer>(true);
        heldRendererOriginalEnabled = new bool[heldRenderers.Length];
        for (int i = 0; i < heldRenderers.Length; i++)
            heldRendererOriginalEnabled[i] = heldRenderers[i].enabled;
    }

    void SetHeldVisible(bool visible)
    {
        if (heldRenderers == null) return;
        for (int i = 0; i < heldRenderers.Length; i++)
        {
            bool want = visible && heldRendererOriginalEnabled[i];
            if (heldRenderers[i].enabled != want)
                heldRenderers[i].enabled = want;
        }
    }
}