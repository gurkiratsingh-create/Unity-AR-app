using UnityEngine;
using UnityEngine.EventSystems;

public class ARTransformController : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private float initialDistance;
    private Vector3 initialScale;

    void Update()
    {
        // One finger to move
        if (Input.touchCount == 1 && !IsPointerOverUI())
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 move = new Vector3(touch.deltaPosition.x, 0, touch.deltaPosition.y);
                transform.position += move * Time.deltaTime * 0.01f;
            }
        }

        // Two fingers to scale
        if (Input.touchCount == 2 && !IsPointerOverUI())
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = transform.localScale;
            }
            else if (touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touchZero.position, touchOne.position);
                if (Mathf.Approximately(initialDistance, 0)) return;

                float scaleFactor = currentDistance / initialDistance;
                transform.localScale = initialScale * scaleFactor;
            }
        }
    }

    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.GetTouch(0).position
        };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
