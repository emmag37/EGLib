using UnityEngine;
using UnityEngine.UI;
using System;


/// <summary>
/// Wraps a <see cref="Slider"/> as a two-state on/off toggle, constraining it to
/// whole number values 0 and 1, playing a sound on change, and invoking a callback
/// with the resulting boolean state.
/// </summary>
public class UIToggle
{
    // ==================================================
    // Private Fields
    // ==================================================
    private Slider slider;
    private Action<bool> action;
    private int sound;
    private IAudio audioService;

    // ==================================================
    // Constructor
    // ==================================================

    /// <summary>
	/// Configures the given slider as a binary toggle (0 or 1 only) and subscribes to
	/// its value changed event.
	/// </summary>
	/// <param name="slider">The slider to configure as a toggle.</param>
	/// <param name="action">Callback invoked with the new toggle state whenever the slider value changes.</param>
	/// <param name="sound">The sound effect played each time the toggle is moved.</param>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="slider"/> or <paramref name="action"/> is null.
	/// </exception>
    public UIToggle(Slider slider, Action<bool> action, int sound)
    {
        if (slider == null)
            throw new ArgumentNullException(nameof(slider));
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        this.slider = slider;
        this.action = action;
        this.sound = sound;

        audioService = ServiceLocator.Get<IAudio>();

        slider.wholeNumbers = true;
        slider.minValue = 0;
        slider.maxValue = 1;

        slider.onValueChanged.AddListener(Moved);
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Unsubscribes from the slider's value changed event. Should be called when
	/// this toggle is no longer neede to avoid a dangling listener.
	/// </summary>
    public void Dispose() => slider.onValueChanged.RemoveListener(Moved);


    // ==================================================
    // Private Methods
    // ==================================================

    /// <summary>
	/// Handles the slider's value changed event. Plays the configured sound and invokes
	/// the callback with the value interpreted as a boolean (0 is <c>false</c>).
	/// </summary>
	/// <param name="v">The slider's vew value (0 or 1).</param>
    private void Moved(float v)
    {
        audioService.PlaySoundEffect(sound);

        action.Invoke(v != 0);
    }
}

