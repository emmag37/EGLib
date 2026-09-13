using UnityEngine;
using UnityEngine.UI;
using System;


/// <summary>
/// Wraps a <see cref="Button"/> to play a sound effect and ivoke a callback on click.
/// </summary>
public class UIButton
{
    // ==================================================
    // Private Fields
    // ==================================================
    private Button button;
    private Action action;
    private IAudio audioService;
    private int sound;

    // ==================================================
    // Constructor
    // ==================================================

    /// <summary>
	/// Constructor that subscries to the given button's click event to play a sound and invoke
	/// the callback.
	/// </summary>
	/// <param name="button">The button to wrap.</param>
	/// <param name="action">Callback invoked each time the button is clicked.</param>
	/// <param name="sound">The sound effect played each time the button is clicked.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="button"/> or <paramref name="action"/> is null.</exception>
    public UIButton(Button button, Action action, int sound)    // should require sound now
    {
        if (button == null)
            throw new ArgumentNullException(nameof(button));
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        this.button = button;
        this.action = action;
        this.sound = sound;

        audioService = ServiceLocator.Get<IAudio>();
        button.onClick.AddListener(Click);
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Unsubscribes from the button's click event. Should be called when this wrapper is no
	/// longer needed to avoid a dangling listener.
	/// </summary>
    public void Dispose() => button.onClick.RemoveListener(Click);


    // ==================================================
    // Private Methods
    // ==================================================

    /// <summary>
	/// Handles the button's click event. Plays the configured sound and invokes the callback.
	/// </summary>
    private void Click()
    {
        audioService.PlaySoundEffect(sound);
        action?.Invoke();
    }
}

