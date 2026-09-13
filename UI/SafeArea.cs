using UnityEngine;


/// <summary>
/// Adjusts this RectTransform's anchors to fit within the device's safe area
/// (excluding nothces, cutouts, and rounded corners), applying extra top/bottom
/// insets on notched devices to keep content clear of the cutout.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaPanel : MonoBehaviour
{
    // ==================================================
    // Private Fields
    // ==================================================
    private RectTransform rectTransform;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }


    // ==================================================
    // Private Methods
    // ==================================================

    /// <summary>
	/// Recalculates and applies the RectTransform's anchors from the current
	/// <see cref="Screen.safeArea"/>. No-ops if the safe area hasn't changed since the
	/// last call. Applies additional top/bottom (and optionally side) insets on devices
	/// with a notch or cutout before converting it to normalized anchor coordinates.
	/// </summary>
    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        // only apply reduction if device actually has a notch/cutout
        bool hasNotch = safeArea.yMin > 0 || safeArea.yMax < Screen.height
                        || safeArea.xMin > 0 || safeArea.xMax < Screen.width;

        if (hasNotch)
        {
            float topExpansion = 20f;      // increase top inset
            float bottomExpansion = 20f;   // increase bottom inset
            float sideExpansion = 0f;      // increase side insets if needed

            safeArea.y -= bottomExpansion;
            safeArea.height += topExpansion + bottomExpansion;

            safeArea.x -= sideExpansion;
            safeArea.width += sideExpansion * 2;
        }

        // calculate new safe area/anchors
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 anchorMin = safeArea.position / screenSize;
        Vector2 anchorMax = (safeArea.position + safeArea.size) / screenSize;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
    }
}