using UnityEngine;
using System;


/// <summary>
/// Defines the operations a UI view has access to for navigating within its host
/// and requesting targeted data updates, without exposing the host's full internal
/// implementation.
/// </summary>
public interface IUIViewHost
{
    /// <summary>
	/// Navigates to and shows the view identified by <paramref name="type"/>.
	/// </summary>
	/// <typeparam name="TType">The enum type identifying views within this host.</typeparam>
	/// <param name="type">The enum value of the view to show.</param>
    void PushView<TType>(TType type) where TType : struct, Enum;

    /// <summary>
	/// Closes/removes the currently active view of type <typeparamref name="TType"/>,
	/// returning navigation to the previous view.
	/// </summary>
	/// <typeparam name="TType">The enum type identifying views within this host.</typeparam>
    void PopView<TType>() where TType : struct, Enum;

	/// <summary>
	/// Applies a targeted, incremental data update to the relevant active view(s) without
	/// reopening with the given patch.
	/// </summary>
	/// <param name="patch">The patch describing the data update to apply.</param>
    void PatchUpdate(IUIPatch patch);
}
