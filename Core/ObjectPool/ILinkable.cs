using UnityEngine;


/// <summary>
/// Defines the contract for a component usable in an intrusive doubly-linked list. Implementers
/// provide the link fields themselves.
/// </summary>
/// <typeparam name="TSelf">The implementing type itself, used so <see cref="Next"/> and <see cref="Prev"/> are strongly typed.</typeparam>
/// <typeparam name="TData">The data type used to initialize an instance.</typeparam>
public interface ILinkable<TSelf, TData> where TSelf : ILinkable<TSelf, TData>
{
    /// <summary>
	/// The next instance in the list, or null if this is the tail. Must be null on initialization.
	/// </summary>
    TSelf Next { get; set; }

    /// <summary>
	/// The previous instance in the list, or null if this is the head. Must be null on
	/// initialization.
	/// </summary>
    TSelf Prev { get; set; }

    /// <summary>
	/// Initializes this instance with the given data. Called once when the instance is first
	/// created.
	/// </summary>
	/// <param name="data">The data to initialize this instance with.</param>
    void Initialize(TData data);
}
