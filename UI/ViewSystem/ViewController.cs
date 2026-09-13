using UnityEngine;
using System;
using System.Collections.Generic;


/// <summary>
/// Manages a fixed capacity stack of <typeparamref name="TView"/> instances, looked
/// up by their <typeparamref name="TType"/> identifier, providing a push/pop
/// navigation and in-place data updates. When the stack is at capacity, pushing a new
/// view relaces (hides) the current top view rather than growing the stack.
/// </summary>
/// <typeparam name="TView">The view type managed by this controller.</typeparam>
/// <typeparam name="TType">The enum type identifying each view.</typeparam>
/// <typeparam name="TData">The data type passed to views on show/update.</typeparam>
public class ViewController<TView, TType, TData>
    where TView : UIView<TType>
    where TType : struct, Enum
    where TData : IUIData
{
    // ==================================================
    // Public Fields
    // ==================================================

    /// <summary>
	/// The number of views currently on the stack.
	/// </summary>
    public int Count => viewStack.Count;

    // ==================================================
    // Private Fields
    // ==================================================
    private Dictionary<TType, TView> viewDictionary;
    private Stack<TView> viewStack;

    private int stackCapacity;

    // ==================================================
    // Constructor
    // ==================================================

    /// <summary>
	/// Builds a lookup of all provided views by their <see cref="UIView{TType}.Type"/>,
	/// initializes each with the given host, and prepares an empty navigation stack
	/// with the given capacity.
	/// </summary>
	/// <param name="viewList">
	/// All views this controller can navigate to. Null entries are skipped and logged
	/// as errors. Entries with a duplicate <see cref="UIView{TType}.Type"/> are
	/// skipped and logged as errors, keeping the first occurence.
	/// </param>
	/// <param name="stackCapacity">The maximum number of views the navigation stack can hold at once.</param>
	/// <param name="host">The host passed to each view's <see cref="UIView{TType}.Initialize"/> call.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewList"/> or <paramref name="host"/> is null.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="stackCapacity"/> is not greater than 0.</exception>
    public ViewController(TView[] viewList, int stackCapacity, IUIViewHost host)
    {
        if (viewList == null)
            throw new ArgumentNullException(nameof(viewList));
        if (stackCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(stackCapacity), "[ViewController] Argument must be greater than 0");
        if (host == null)
            throw new ArgumentNullException(nameof(host));

        this.stackCapacity = stackCapacity;

        viewStack = new Stack<TView>(capacity: stackCapacity);
        viewDictionary = new Dictionary<TType, TView>();

        // initialize the dictionary
        foreach (TView view in viewList)
        {
            if (view == null)
            {
                Debug.LogError($"[ViewController] Null view in view list");
                continue;
            }
            if (viewDictionary.ContainsKey(view.Type))
            {
                Debug.LogError($"Duplicate view type: {view.Type}");
                continue;
            }

            view.Initialize(host);
            viewDictionary.Add(view.Type, view);
        }
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Shows the view for <paramref name="type"/> and pushes it onto the navigation
	/// stack. If the stack is already at capacity, the current top view is popped
	/// and hidden first, so the stack size never exceeds its configured capacity.
	/// Logs an error and does nothing if no view is registered for <paramref name="type"/>.
	/// </summary>
	/// <param name="type">The type of view to show and push.</param>
	/// <param name="data">The data to pass to the view's <see cref="UIView{TType}.Show"/>.</param>
    public void PushView(TType type, TData data)
    {
        if (!viewDictionary.TryGetValue(type, out TView newView))
        {
            Debug.LogError($"[ViewController] Could not access view for type {type}");
            return;
        }

        if (viewStack.Count == stackCapacity)   // remove the top most view
        {
            TView topView = viewStack.Pop();
            topView.Hide();
        }

        viewStack.Push(newView);
        newView.Show(data);
    }

    /// <summary>
	/// Returns the <typeparamref name="TType"/> of the view currently on top
	/// of the navigation stack without removing it.
	/// </summary>
	/// <returns>
	/// The top view's type, or <c>default</c> with a logged warning if the stack is empty.
	/// </returns>
    public TType PeekViewType()
    {
        if (viewStack.Count == 0)
        {
            Debug.LogWarning("[ViewController] Peek called on empty stack");
            return default;
        }

        return viewStack.Peek().Type;
    }

    /// <summary>
	/// Pops and hides the view currently on top of the navigation stack.
	/// Logs an error and does nothing if the stack is empty.
	/// </summary>
    public void PopView()
    {
        if (viewStack.Count == 0)
        {
            Debug.LogError("[ViewController] Pop called on empty stack");
            return;
        }

        TView topView = viewStack.Pop();
        topView.Hide();
    }

    /// <summary>
	/// Pops and hides every view on the navigation stack until it is empty.
	/// </summary>
    public void ClearViews()
    {
        while (viewStack.Count > 0) PopView();
    }

    /// <summary>
	/// Refreshes the view for <paramref name="type"/> with new data through its
	/// <see cref="UIView{TType}.UpdateView"/> call, without changing the navigation stack.
	/// Logs an error and does nothing if no view is registered for <paramref name="type"/>.
	/// </summary>
	/// <param name="type">The type of view to update.</param>
	/// <param name="data">The data to update the view with.</param>
    public void UpdateView(TType type, TData data)
    {
        if (!viewDictionary.TryGetValue(type, out TView view))
        {
            Debug.LogError($"[ViewController] Could not access view for type {type}");
            return;
        }

        view.UpdateView(data);
    }
}
