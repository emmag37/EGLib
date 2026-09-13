using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Wraps a <see cref="TMP_InputField"/> with username validation, an inline error message
/// display, and a shake animaiton on invalid submission. Validated usernames are reported
/// to the caller with a supplied callback.
/// </summary>
public class UIUsernameInputField
{
    // ==================================================
    // Private Fields
    // ==================================================

    private TMP_InputField inputField;
    private Text invalidInput;

    private Action<string> action;

    private string username;
    private Dictionary<InvalidUsernameType, string> errorMessages = new Dictionary<InvalidUsernameType, string>
    {
        { InvalidUsernameType.Short, "Username must be at least 3 characters" },
        { InvalidUsernameType.SpecialChars, "Only letters, numbers, and underscores allowed" },
        { InvalidUsernameType.Profanity, "That username isn’t allowed" }
    };


    // ==================================================
    // Constructor
    // ==================================================

    /// <summary>
	/// Wires up the given input field to validate on submit and clear the error display
	/// on any edit.
	/// </summary>
	/// <param name="inputField">The input field to attach validation and listeners to.</param>
	/// <param name="invalidInput">The text element used to display validation error messages.</param>
	/// <param name="action">Callback invoked with the new username whenever a submitted value passes validation.</param>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="inputField"/>, <paramref name="invalidInput"/>, or <paramref name="action"/> is null.
	/// </exception>
    public UIUsernameInputField(TMP_InputField inputField, Text invalidInput, Action<string> action)
    {
        if (inputField == null)
            throw new ArgumentNullException(nameof(inputField));
        if (invalidInput == null)
            throw new ArgumentNullException(nameof(invalidInput));
        if (action == null)
            throw new ArgumentNullException(nameof(action));


        this.inputField = inputField;
        this.invalidInput = invalidInput;
        this.action = action;

        inputField.onSubmit.AddListener(value =>
        {
            UpdateUsername(value);
        });
        inputField.onValueChanged.AddListener(_ =>
        {
            invalidInput.gameObject.SetActive(false);
        });
        inputField.onSelect.AddListener(_ =>
        {
            if (invalidInput.enabled)
                inputField.text = String.Empty;
        });
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Sets the current username and reflects it in the input field's text without
	/// running validation or invoking the callback.
	/// </summary>
	/// <param name="name">The username to set. Must not be null.</param>
    public void SetUsername(string name)
    {
        if (name == null)
        {
            Debug.LogError("[UIUsernameInputField] Null string passed to SetUsername");
            return;
        }

        username = name;
        inputField.text = name;
    }

    // ==================================================
    // Private Methods
    // ==================================================

    /// <summary>
	/// Validates a submitted username. If valid, stores it, hides the error display,
	/// and invokes the callback with the new value. If invalid, displays the corresponding error
	/// message and plays a shake animation on the input field's text.
	/// </summary>
	/// <param name="name">The submitted username to validate. Must not be null.</param>
    private void UpdateUsername(string name)
    {
        if (name == null)
        {
            Debug.LogError("[UIUsernameInputField] Null string passed to UpdateUsername");
            return;
        }

        InvalidUsernameType error = UsernameValidator.IsUsernameValid(name);
        if (error == InvalidUsernameType.None)
        {
            username = name;
            invalidInput.gameObject.SetActive(false);

            action?.Invoke(name);
        }
        else
        {
            invalidInput.text = errorMessages[error];
            invalidInput.gameObject.SetActive(true);

            CoroutineRunner.Instance.StartCoroutine(ShakeTextRoutine());


        }
    }

    // ==================================================
    // Coroutines
    // ==================================================

    /// <summary>
	/// Horizontally shakes the input field's text for the given duration to signal
	/// invalid input, then restores its original position, then clears the test.
	/// </summary>
	/// <param name="duration">Length of the shake in seconds, defaults to 0.3f.</param>
	/// <param name="magnitude">The maximum horizontal offset applied each frame in local units, defaults to 8f.</param>
	/// <returns></returns>
    IEnumerator ShakeTextRoutine(float duration = 0.3f, float magnitude = 8f)
    {
        Vector3 originalPos = inputField.textComponent.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = originalPos.x + UnityEngine.Random.Range(-1f, 1f) * magnitude;
            inputField.textComponent.transform.localPosition = new Vector3(x, originalPos.y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        inputField.textComponent.transform.localPosition = originalPos;
    }

}
