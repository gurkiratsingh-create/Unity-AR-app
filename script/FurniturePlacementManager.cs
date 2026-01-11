using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FurniturePlacementManager : MonoBehaviour
{
    public GameObject SpawnableFurniture;
    public ARSessionOrigin sessionOrigin;
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private GameObject currentFurniture;

    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (IsPointerOverUI())
                return;

            if (raycastManager.Raycast(Input.GetTouch(0).position, raycastHits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = raycastHits[0].pose;

                // Only spawn if not already placed
                if (currentFurniture == null)
                {
                    currentFurniture = Instantiate(SpawnableFurniture, hitPose.position, hitPose.rotation);

                    // Optional: add transform control script
                    currentFurniture.AddComponent<ARTransformController>();

                    // Optional: Add physics
                    var rb = currentFurniture.AddComponent<Rigidbody>();
                    rb.isKinematic = true;

                    currentFurniture.AddComponent<BoxCollider>();

                    // Disable planes
                    foreach (var plane in planeManager.trackables)
                    {
                        plane.gameObject.SetActive(false);
                    }
                    planeManager.enabled = false;
                }
            }
        }
    }

    // UI blocking check (more robust than just Button check)
    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.GetTouch(0).position
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    // Change furniture from UI
    public void SwitchFurniture(GameObject furniture)
    {
        SpawnableFurniture = furniture;
        currentFurniture = null; // Allow placing the new one
    }
}


