using UnityEngine;
using System;
using System.Collections.Generic;


/// <summary>
/// A fixed-capacity list that keeps items sorted in descending order (highest value at index 0).
/// When full, adding a new value that outranks the current lowest entry removes the current lowest entry
/// to make space.
/// </summary>
/// <typeparam name="T">
/// The item type, which must implement <see cref="IComparable{TermInfoBooleans}"/> to define the ranking order.
/// </typeparam>
public abstract class CappedRankedList<T> where T : IComparable<T>
{
    // ==================================================
    // Public Fields
    // ==================================================

    /// <summary>
	/// A read-only view of the ranked items, sorted in descending order.
	/// </summary>
    public IReadOnlyList<T> ROList => list;

    // ==================================================
    // Protected Fields
    // ==================================================
    [SerializeField] protected List<T> list = new List<T>();
    protected readonly int capacity;

    // ==================================================
    // Private Fields
    // ==================================================
    private static readonly IComparer<T> DescComparer = Comparer<T>.Create((a, b) => b.CompareTo(a));


    // ==================================================
    // Constructor
    // ==================================================

    /// <summary>
	/// Creates a new capped ranked list.
	/// </summary>
	/// <param name="capacity">The maximum number of items the list will hold. Defaults to 100.</param>
    protected CappedRankedList(int capacity = 100)
    {
        this.capacity = capacity;
    }

    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Attempts to insert a value into its correctly sorted (descending) position in the list.
	/// If the list is at capacity, the value is only inserted if it outranks the current lowest
	/// entry, in which case the lowest entry is removed to make space.
	/// </summary>
	/// <param name="value">The value to attempt to insert.</param>
	/// <returns><c>true</c>if the value was inserted, <c>false</c> otherwise.</returns>
    public virtual bool TryAddValue(T value)
    {
        if (list.Count == capacity && value.CompareTo(list[capacity - 1]) <= 0)   // less than or equal : -n means less than, 0 means equal
            return false;

        if (list.Count == capacity)
            list.RemoveAt(capacity - 1);

        InsertValue(value);

        return true;
    }

    // ==================================================
    // Private Methods
    // ==================================================

    private int InsertValue(T value)
    {
        // use built-in binary search with descending comparer
        int index = list.BinarySearch(value, DescComparer);

        // BinarySearch returns bitwise complement of insertion index to keep the list sorted
        if (index < 0)
            index = ~index;

        list.Insert(index, value);

        return index;
    }
}
