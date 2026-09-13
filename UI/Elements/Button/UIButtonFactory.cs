using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


/// <summary>
/// Provides factory methods for building pre-configured <see cref="UIButton"/> instances for
/// common interaction patterns (navigation, popups, input activation, patch updates, and
/// incrementing/decrementing a list), so callers don't need to write the wrapping lambda by hand.
/// </summary>
public static class UIButtonFactory
{
    /// <summary>
	/// Creates a button that pushes the given view type onto the host's navigation stack when
	/// clicked.
	/// </summary>
	/// <typeparam name="TType">The enum type identifying views within the host.</typeparam>
	/// <param name="button">The button to wrap.</param>
	/// <param name="host">The view host to navigate through.</param>
	/// <param name="viewType">The view type to show on click.</param>
	/// <param name="sound">The sound to play on click.</param>
    public static UIButton Navigate<TType>(Button button, IUIViewHost host, TType viewType, int sound) where TType : struct, Enum
        => new UIButton(button, () => host.PushView(viewType), sound);

	/// <summary>
	/// Creates a button that pops the current view of type <typeparamref name="TType"/> from
	/// the host's navigation stack when clicked, closing a pop-up view.
	/// </summary>
	/// <typeparam name="TType">The enum type identifying views within the host.</typeparam>
	/// <param name="button">The button to wrap.</param>
	/// <param name="host">the view host to navigate through.</param>
	/// <param name="sound">The sound to play on click.</param>
	public static UIButton ClosePopUp<TType>(Button button, IUIViewHost host, int sound) where TType : struct, Enum
        => new UIButton(button, host.PopView<TType>, sound);

	/// <summary>
	/// Creates a button that activates the given input field for editing when clicked.
	/// </summary>
	/// <param name="button">The button to wrap.</param>
	/// <param name="inputField">The input field to activate on click.</param>
	/// <param name="sound">The sound to play on click.</param>
	public static UIButton EditInput(Button button, TMP_InputField inputField, int sound)
        => new UIButton(button, inputField.ActivateInputField, sound);

	/// <summary>
	/// Creates a button that sends a data patch to the host when clicked. The patch is
	/// constructed fresh from <paramref name="getPatch"/> at click time, so it can reflect the
	/// current state rather than being fixed at button creation.
	/// </summary>
	/// <param name="button">The button to wrap.</param>
	/// <param name="host">The view host to send the patch to.</param>
	/// <param name="getPatch">Factory invoked on click to produce the patch.</param>
	/// <param name="sound">The sound to play on click.</param>
	public static UIButton SendPatch(Button button, IUIViewHost host, Func<IUIPatch> getPatch, int sound)
        => new UIButton(button, () => host.PatchUpdate(getPatch()), sound);

	/// <summary>
	/// Creates a button that advances the given list to its next item when clicked, invoking
	/// the callback with the new value.
	/// </summary>
	/// <typeparam name="T">The type of item in the list.</typeparam>
	/// <param name="button">The button to wrap.</param>
	/// <param name="list">The list to advance.</param>
	/// <param name="onChanged">Callback invoked with the new current item on click.</param>
	/// <param name="sound">The sound to play on click.</param>
	public static UIButton Increment<T>(Button button, IIncrementList<T> list, Action<T> onChanged, int sound)
        => new UIButton(button, () => onChanged(list.Next()), sound);

	/// <summary>
	/// Creates a button that moves the given list back to its previous item when clicked,
	/// invoking the callback with the new value.
	/// </summary>
	/// <typeparam name="T">The type of item in the list.</typeparam>
	/// <param name="button">The button to wrap.</param>
	/// <param name="list">The list to advance.</param>
	/// <param name="onChanged">Callback invoked with new current item on click.</param>
	/// <param name="sound">The sound to play on click.</param>
	public static UIButton Decrement<T>(Button button, IIncrementList<T> list, Action<T> onChanged, int sound)
        => new UIButton(button, () => onChanged(list.Prev()), sound);
}
