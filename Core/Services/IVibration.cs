
/// <summary>
/// Provides contrl over haptic feedback (vibration).
/// </summary>
public interface IVibration
{
    /// <summary>
	/// Triggers a short vibration pulse.
	/// </summary>
    void ShortVibration();

    /// <summary>
	/// Enables or disables vibration.
	/// </summary>
	/// <param name="on">True to enable vibration, false to disable it.</param>
    void SetVibrationOn(bool on);

    /// <summary>
	/// Gets whether vibration is currently enabled.
	/// </summary>
	/// <returns><c>true</c> if vibration is enabled, <c>false</c> otherwise.</returns>
    bool GetSettings();
}
