using UnityEngine;
using System;


/// <summary>
/// Generic base class for a UI view identified by an enum value of type
/// <typeparamref name="TType"/>, managed by an <see cref="IUIViewHost"/>. Handles
/// show/hide with GameObject activation and delegates data binding to derived classes
/// throw <see cref="SetInfo"/>.
/// </summary>
/// <typeparam name="TType">
/// The enum type used to identify this view within its host's navigation system.
/// </typeparam>
public abstract class UIView<TType> : MonoBehaviour where TType : struct, Enum
{
    // ==================================================
    // Public Fields
    // ==================================================

    /// <summary>
	/// The enum value identifying this view. Implemented by derived classes to declare
	/// which <typeparamref name="TType"/> value they represent.
	/// </summary>
    public abstract TType Type { get; }

    // ==================================================
    // Protected Fields
    // ==================================================

    /// <summary>
	/// The host managing this view's navigation and lifecycle. Set with <see cref="Initialize"/>.
	/// </summary>
    protected IUIViewHost Host { get; private set; }

    // ==================================================
    // Initialization
    // ==================================================

    /// <summary>
	/// Assigns the host responsible for this view. Must be called before the view is shown.
	/// </summary>
	/// <param name="host"></param>
    public virtual void Initialize(IUIViewHost host) => Host = host;

    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Activates this view's GameObject and binds the given data with <see cref="SetInfo"/>.
	/// </summary>
	/// <param name="data">Optional data to populate the view with.</param>
    public virtual void Show(IUIData data = null)
    {
        gameObject.SetActive(true);
        SetInfo(data);
    }

    /// <summary>
	/// Deactivates this view's GameObject.
	/// </summary>
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
	/// Refreshes this view's contents with new data without changing its active state.
	/// Logs an error and does nothing if <paramref name="data"/> is null.
	/// </summary>
	/// <param name="data">The data to update the view with. Must not be null.</param>
    public virtual void UpdateView(IUIData data)
    {
        if (data == null)
        {
            Debug.LogError("[UIView] Attempted to update view with null data");
            return;
        }
        SetInfo(data);
    }

    // ==================================================
    // Protected Methods
    // ==================================================

    /// <summary>
	/// Applies the given data to this view's UI elements. Implement by derived
	/// classes to bind view specific fields (text, images, etc.).
	/// </summary>
	/// <param name="data">
	/// The data to bind, or <c>null</c> if the view has no data driven content.
	/// </param>
    protected abstract void SetInfo(IUIData data = null);
}
