using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastInteractor : MonoBehaviour
{
    public float interactRange = 3.5f;
    public LayerMask interactLayer;
    public Transform rayOriginFromPlayer;
    private IInteractable currentTarget;
    private IInteractable previousTarget;

    void Update()
    {
        if (Camera.main == null) return;

        List<Ray> rays = new List<Ray>();

        // 🔁 ยิงจากกล้อง: กลางจอ และต่ำลงเล็กน้อย
        rays.Add(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)));
        rays.Add(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.4f, 0f)));

        // 🧍 ยิงจากตัวละครไปตามกล้อง
        if (rayOriginFromPlayer != null)
        {
            rays.Add(new Ray(rayOriginFromPlayer.position, Camera.main.transform.forward));
        }

        float closestDistance = float.MaxValue;
        IInteractable bestHit = null;

        foreach (Ray ray in rays)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, interactRange, interactLayer);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                // เช็คว่าชนอะไรที่ Block ไว้หรือไม่
                if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit blockCheckHit, hit.distance, ~interactLayer))
                {
                    // มีวัตถุอื่นมากั้นก่อน
                    break;
                }

                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null && interactable.CanBeInteracted() && hit.distance < closestDistance)
                {
                    bestHit = interactable;
                    closestDistance = hit.distance;
                    break; // หยุดเมื่อเจอ interactable ที่ไม่ถูกบล็อค
                }
            }
            Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.green);
        }

        if (bestHit != null)
        {
            if (bestHit != currentTarget)
            {
                previousTarget?.SetHighlighted(false);

                currentTarget = bestHit;

                if (currentTarget.CanBeInteracted())
                    currentTarget.SetHighlighted(true);

                previousTarget = currentTarget;
            }

            if (!InspectManager.Instance.IsInspecting())
            {
                if (Mouse.current?.leftButton.wasPressedThisFrame == true ||
                    Gamepad.current?.buttonSouth.wasPressedThisFrame == true)
                {
                    currentTarget.Interact();
                }
            }
        }
        else
        {
            // ถ้าไม่เจออะไรเลย → ปิด glow อันก่อน
            previousTarget?.SetHighlighted(false);
            currentTarget = null;
            previousTarget = null;
        }

    }
}