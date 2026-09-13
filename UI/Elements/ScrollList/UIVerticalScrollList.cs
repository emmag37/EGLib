using UnityEngine;
using System.Collections.Generic;
using System;


/// <summary>
/// Class for a vertical scroll list that activates, binds, and manages a fixed size pool of
/// pre-instantiated item views implementing <see cref="IScrollItem{T}"/>.
/// </summary>
/// <typeparam name="T">The data type bound to each item view.</typeparam>
public class UIVerticalScrollList<T>
{
    // ==================================================
    // Protected Fields
    // ==================================================
    private Transform content;
    private GameObject itemView;    // prefab - must have component that extends IScrollItem
    private int maxItems;

    private IScrollItem<T>[] scrollItems;

    private bool populated = false;

    // ==================================================
    // Constructors
    // ==================================================

    /// <summary>
	/// Sets up the list's configuration and allocates the fixed size item pool.
	/// Call <see cref="Populate"/> afterward to instantiate the pooled item views.
	/// </summary>
	/// <param name="content">The parent transform that instantiated item views will be placed under.</param>
	/// <param name="itemView">The prefab to instantiate for each pooled item. Must have a component implementing <see cref="IScrollItem{T}"/>.</param>
	/// <param name="maxItems">The fixed number of items this list can display, set once at construction.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="content"/> or <paramref name="itemView"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="itemView"/> has no <see cref="IScrollItem{T}"/> component.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxItems"/> is not greater than 0.</exception>
    public UIVerticalScrollList(Transform content, GameObject itemView, int maxItems)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));
        if (itemView == null)
            throw new ArgumentNullException(nameof(itemView));
        if (!itemView.TryGetComponent<IScrollItem<T>>(out _))
            throw new ArgumentException($"Prefab '{itemView.name}' has no {typeof(IScrollItem<T>)} component.", nameof(itemView));
        if (maxItems <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxItems), "Max items must be greater than 0");

        this.content = content;
        this.itemView = itemView;
        this.maxItems = maxItems;

        scrollItems = new IScrollItem<T>[maxItems];
    }

    /// <summary>
	/// Instantiates <see cref="maxItems"/> copies of <see cref="itemView"/> under
	/// <see cref="content"/>, populating <see cref="scrollItems"/> and hiding each one
	/// until data is set. Logs a warning and does nothing if
	/// the list has already been populated.
	/// </summary>
    public void Populate()
    {
        if (populated)
        {
            Debug.LogWarning("[UIVerticalScrollList] Attempted to repopulate list items.");
            return;
        }

        for (int i = 0; i < maxItems; i++)
        {
            scrollItems[i] = UnityEngine.Object.Instantiate(itemView, content).GetComponent<IScrollItem<T>>();
            scrollItems[i].GameObject.SetActive(false);
        }

        populated = true;
    }

    /// <summary>
	/// Alternative to <see cref="Populate"/> that instantiates item views by alternating between
	/// the class's default <c>itemView</c> prefab (even indices) and
	/// <paramref name="itemView2"/> (odd indices), populating <see cref="scrollItems"/> and
	/// hiding each one until data is set. Logs a warning and does nothing if the list has
	/// already been populated.
	/// </summary>
	/// <param name="itemView2">
	/// The prefab to instantiate at odd indices. Must have a component implementing <see cref="IScrollItem{T}"/>.
	/// </param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="itemView2"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="itemView2"/> has no <see cref="IScrollItem{T}"/> component.</exception>
    public void PopulateAlternating(GameObject itemView2)
    {
        if (populated)
        {
            Debug.LogWarning("[UIVerticalScrollList] Attempted to repopulate list items.");
            return;
        }

        if (itemView2 == null)
            throw new ArgumentNullException(nameof(itemView2));
        if (!itemView2.TryGetComponent<IScrollItem<T>>(out _))
            throw new ArgumentException($"Prefab '{itemView2.name}' has no {typeof(IScrollItem<T>)} component.", nameof(itemView2));

        for (int i = 0; i < maxItems; i++)
        {
            if (i % 2 == 0)
                scrollItems[i] = UnityEngine.Object.Instantiate(itemView, content).GetComponent<IScrollItem<T>>();
            else
                scrollItems[i] = UnityEngine.Object.Instantiate(itemView2, content).GetComponent<IScrollItem<T>>();

            scrollItems[i].GameObject.SetActive(false);
        }

        populated = true;
    }

    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Activates and binds the first <c>min(dataList.Count, maxItems)</c> pooled item views to
	/// the given data, in order. If <paramref name="dataList"/> contains more items than
	/// <see cref="maxItems"/>, the excess entries are truncated and a warning is logged.
	/// </summary>
	/// <param name="dataList">The data to display, in the order the pooled item views should reflect.</param>
    public void SetData(IReadOnlyList<T> dataList)
    {
        int numItems = Math.Min(dataList.Count, maxItems);
        if (numItems < dataList.Count)
            Debug.LogWarning($"[UIVerticalScrollList] Data list count ({dataList.Count}) exceeds max items {maxItems}");

        for (int i = 0; i < numItems; i++)
        {
            scrollItems[i].GameObject.SetActive(true);
            scrollItems[i].Set(dataList[i], i);
        }
    }
}


