using UnityEngine;


/// <summary>
/// Defines the contract for an item view used within a <see cref="UIVerticalScrollList{T}"/>,
/// allowing the list to instantiate, activate, and bind data to items without knowing their
/// concrete type.
/// </summary>
/// <typeparam name="T">The data type this item view can be bound to.</typeparam>
public interface IScrollItem<T>
{
    /// <summary>
	/// The GameObject this item view is attached to, used by the owning list to control its
	/// active state.
	/// </summary>
    GameObject GameObject { get; }

    /// <summary>
	/// Binds this item view to the given data at the given position within the list.
	/// </summary>
	/// <param name="data">The data to display.</param>
	/// <param name="index">The item's index within the list's current data set.</param>
    void Set(T data, int index);
}
