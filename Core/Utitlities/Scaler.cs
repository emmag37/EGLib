using UnityEngine;
using System;


/// <summary>
/// Static utility that computes and applies a scale factor derived from the
/// active orthographic camera's size relative to a reference resolution. Ensures
/// scaling stays consistent across UI and game objects by centralizing the calculation in
/// one place.
/// </summary>
public static class Scaler
{
    // ==================================================
    // Private Fields
    // ==================================================
    private static float scale;         // only access for the actual scale, keeps scaling consistent across the project
    private static Camera cam;          // camera that the scaling applies to

    // ==================================================
    // Initialization
    // ==================================================

    /// <summary>
	/// Calculates the current scale factor from the given orthographic camera and
	/// reference resolution, then adjusts the
	/// camera's orthographic size to match the current screen aspect ratio. Must be called
	/// before any other method on this class.
	/// </summary>
	/// <param name="c">The orthographic camera to calculate and apply scale for.</param>
	/// <param name="refHeight">The reference height used to calculate scale.</param>
	/// <param name="refWidth">The reference width used to calculate scale.</param>
	/// <param name="ppu">Pixels per unit used to calculate scale.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="c"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="c"/> is not orthograpic.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown if <paramref name="c"/>'s orthographic size, <paramref name="refHeight"/>,
	/// <paramref name="refWidth"/>, or <paramref name="ppu"/> is less than or equal to 0.
	/// </exception>
    public static void CalculateAndSetScale(Camera c, float refHeight, float refWidth, float ppu)
    {
        if (c == null)
            throw new ArgumentNullException(nameof(c));
        if (!c.orthographic)
            throw new ArgumentException("[Scalar] Camera must be orthographic to calculate scale");
        if (c.orthographicSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(c.orthographicSize), "[Scalar] Camera.orthographicSize must be greater than 0");

        if (refHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(refHeight), "[Scaler] refHeight must be greater than 0");
        if (refWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(refWidth), "[Scaler] refWidth must be greater than 0");
        if (ppu <= 0)
            throw new ArgumentOutOfRangeException(nameof(ppu), "[Scaler] ppu must be greater than 0");

        cam = c;

        float referenceAspect = refWidth / refHeight;
        float currentAspect = (float)Screen.width / Screen.height;
        float baseOrthoSize = (refHeight / ppu) / 2f;

        if (currentAspect < referenceAspect)
            cam.orthographicSize = baseOrthoSize * (referenceAspect / currentAspect);
        else
            cam.orthographicSize = baseOrthoSize;

        float refOrtho = refHeight / 2f / ppu;
        scale = cam.orthographicSize / refOrtho;
    }

    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Applies the calculated uniform scale to the given transforms's local scale
	/// (X and Y only, Z stays 1).
	/// </summary>
	/// <param name="transform">The transform to scale.</param>
	/// <exception cref="InvalidOperationException">Thrown if called before <see cref="CalculateAndSetScale"/>.</exception>
    public static void ApplyLocalScale(Transform transform)
    {
        if (scale == 0)
            throw new InvalidOperationException("[Scaler] Attempted to set value before initializing scale");

        transform.localScale = new Vector3(scale, scale, 1f);
    }

    /// <summary>
	/// Returns a copy of the given position with the Y component scale-adjusted, splitting
	/// the difference between the unscaled position and the scale factor.
	/// </summary>
	/// <param name="pos">The world position to adjust.</param>
	/// <returns>A new <see cref="Vector3"/> with the Y component scaled.</returns>
	/// <exception cref="InvalidOperationException">Thrown if called before <see cref="CalculateAndSetScale"/>.</exception>
    public static Vector3 CalculateScaledYPos(Vector3 pos)
    {
        if (scale == 0)
            throw new InvalidOperationException("[Scaler] Attempted to set value before initializing scale");

        pos.y *= (scale + 1) / 2;
        return pos;
    }

    /// <summary>
	/// Converts a world-space position to a scaled local position within the given UI rect,
	/// using the camera set by <see cref="CalculateAndSetScale"/>, and applies the result to
	/// the given transform's local position with an optional scaled Y offset.
	/// </summary>
	/// <param name="transform">The UI transform to position.</param>
	/// <param name="rect">The RectTransform whose local space the position is converted into.</param>
	/// <param name="worldPos">The source world position.</param>
	/// <param name="yOffset">An additional Y offset (unscaled) to apply (scaled internally before use).</param>
	/// /// <exception cref="InvalidOperationException">Thrown if called before <see cref="CalculateAndSetScale"/>.</exception>
    public static void UIApplyScaledPos(Transform transform, RectTransform rect, Vector3 worldPos, float yOffset = 0)
    {
        if (scale == 0)
            throw new InvalidOperationException("[Scaler] Attempted to set value before initializing scale");

        float scaledOffset = scale * yOffset;

        Vector2 screenPos = cam.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            screenPos,
            null, // if overlay
            out Vector2 uIPos
        );

        transform.localPosition = new Vector2(uIPos.x, uIPos.y + scaledOffset);
    }
}
