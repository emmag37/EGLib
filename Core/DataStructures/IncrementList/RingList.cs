using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// A list that wraps around at its ends, so calling <see cref="Next"/> past the last
/// returns the first, and calling <see cref="Prev"/> before the first item returns the last.
/// Maintains a current index that moves with each call.
/// </summary>
/// <typeparam name="T">The type of item stored in the list.</typeparam>
public class RingList<T> : IIncrementList<T>
{
    // ==================================================
    // Private Fields
    // ==================================================
    private List<T> list;
    private int currIdx;

    // ==================================================
    // Constructor
    // ==================================================

    /// <summary>
	/// Creates a new ring list wrapping the given list. Start index initializes to 0 for an empty list.
	/// </summary>
	/// <param name="list">The backing list to wrap. Must be non-null.</param>
	/// <param name="startIndex">The initial current index. Defaults to 0.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="list"/> is null.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="startIndex"/>is greater than or equal to <paramref name="list"'s count.</exception>
    public RingList(List<T> list, int startIndex = 0)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
        if (list.Count > 0 && startIndex >= list.Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        this.list = list;
        if (list.Count == 0)
            currIdx = 0;
        else
            currIdx = startIndex;
    }

    // ==================================================
    // Interface Methods
    // ==================================================

    /// <summary>
	/// Advances to the next item in the list, wrapping to the first item if the current position 
	/// is the last item.
	/// </summary>
	/// <returns>The item at the new current position, or <c>default(T)</c> if the list is empty.</returns>
    public T Next()
    {
        if (list.Count == 0)
            return default(T);

        currIdx = (currIdx + 1) % list.Count;
        return list[currIdx];
    }

    /// <summary>
	/// Moves the the previous item in the list, wrapping to the last item if the current position
	/// is the first item.
	/// </summary>
	/// <returns>The item at the new current position, or <c>default(T)</c> if the list is empty.</returns>
    public T Prev()
    {
        if (list.Count == 0)
            return default(T);

        currIdx = (currIdx - 1 + list.Count) % list.Count;
        return list[currIdx];
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Sets the current index to the position of the given value, if found.
	/// </summary>
	/// <param name="value">The value to search for.</param>
	/// <returns><c>true</c> if the value was found and the current index was updated, <c>false</c> otherwise.</returns>
    public bool SetCurrentIndexAtValue(T value)
    {
        int idx = list.IndexOf(value);
        if (idx == -1)
            return false;

        currIdx = idx;
        return true;
    }


    /// <summary>
	/// Appends a value to the end of the list.
	/// </summary>
	/// <param name="value">The value to add.</param>
    public void Add(T value) => list.Add(value);

    /// <summary>
	/// Inserts a value at the specified index.
	/// </summary>
	/// <param name="index">The zero-based index at which the value should be inserted.</param>
	/// <param name="value">The value to insert.</param>
    public void Insert(int index, T value)
    {
        list.Insert(index, value);

        if (index == currIdx)
            currIdx++;
    }

    /// <summary>
	/// Removes the item at the specified index.
	/// </summary>
	/// <param name="index">The zero-based index of the item to remove.</param>
    public void RemoveAt(int index)
    {
        list.RemoveAt(index);

        if (list.Count == 0)
            currIdx = 0;
        if (index == currIdx)
            currIdx = (currIdx + 1) % list.Count;
    }
}
