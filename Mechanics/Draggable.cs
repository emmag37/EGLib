using UnityEngine;
using UnityEngine.InputSystem;
using System;


/// <summary>
/// Performs user input dragging for a game object. Releases when and where the touch ends.
/// </summary>
/// <remarks>
/// Draggable within specified boundaries adjusted for the objects size.
/// ie, keeps the object inside the box outlined by boundaries.
/// Only one touch at a time.
/// </remarks>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Draggable : MonoBehaviour
{
    // ==================================================
    // Events
    // ==================================================
    /// <summary>
	/// Invoked when the user touches the player.
	/// </summary>
    public event Action StartDrag;

    /// <summary>
	/// Invoked when the user releases the player.
	/// </summary>
    public event Action<Vector3> Released;

    // ==================================================
    // Private Fields
    // ==================================================
    private Camera cam;

    private bool isDragging = false;
    private Vector3 dragOffset;

    private float minX, maxX, minY, maxY;


    // ==================================================
    // Unity Lifecycle Methods
    // ==================================================

    void Update()
    {
        Drag();
    }


    // ==================================================
    // Initialization
    // ==================================================

    /// <summary>
	/// Initializes the drag movement with the specified boundaries and camera.
	/// </summary>
	/// <param name="boundaries">Bounds of movement area, exclusive to the object.</param>
	/// <param name="cam">Camera to track movement.</param>
    public void Initialize(Bounds boundaries, Camera cam)
    {
        this.cam = cam;
        if (cam == null)
            Debug.LogError("[Draggable] Camera not found");

        // adjust the boundaries to the player size
        Vector3 objExtents = GetComponent<SpriteRenderer>().bounds.extents;
        float xRadius = objExtents.x;
        float yRadius = objExtents.y;

        minX = boundaries.min.x + xRadius;
        maxX = boundaries.max.x - xRadius;
        minY = boundaries.min.y + yRadius;
        maxY = boundaries.max.y - yRadius;

        if (minX >= maxX || minY >= maxY)
            Debug.LogError($"[Draggable] Invalid boundaries: x ({minX}, {maxX}), y ({minY}, {maxY})");
    }


    // ==================================================
    // Private Methods
    // ==================================================

    /// <summary>
	/// Drags and drops the player from user input. Relies on Update().
	/// </summary>
    private void Drag()
    {
        if (cam == null) return;

        Vector2 screenPos;
        bool pressedThisFrame, isPressed, releasedThisFrame;

        if (Touchscreen.current != null)                                
        {
            var t = Touchscreen.current.primaryTouch;                   // enforce one touch only
            screenPos = t.position.ReadValue();
            pressedThisFrame = t.press.wasPressedThisFrame;
            isPressed = t.press.isPressed;
            releasedThisFrame = t.press.wasReleasedThisFrame;
        }
        else if (Mouse.current != null)
        {
            var m = Mouse.current;                                      // for use in the editor
            screenPos = m.position.ReadValue();
            pressedThisFrame = m.leftButton.wasPressedThisFrame;
            isPressed = m.leftButton.isPressed;
            releasedThisFrame = m.leftButton.wasReleasedThisFrame;
        }
        else
            return;

        Vector3 pointerWorldPos = cam.ScreenToWorldPoint(screenPos);
        pointerWorldPos.z = 0;
        
        if (!isDragging && pressedThisFrame)                            // start moving
        {
            Collider2D hit = Physics2D.OverlapPoint(pointerWorldPos);   // check if mouse is on the collider
            if (hit && hit.gameObject == gameObject)
            {
                isDragging = true;
                dragOffset = transform.position - pointerWorldPos;

                StartDrag?.Invoke();
            }
        }
        else if (isDragging && isPressed)                               // continue moving
        {
            Vector3 newPos = pointerWorldPos + dragOffset;              // calculate new position

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);               // clamp position to boundaries
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            transform.position = newPos;
        }
        else if (isDragging && releasedThisFrame)                       // release
        {
            isDragging = false;
            Released?.Invoke(transform.position);      
        }
    }
}
