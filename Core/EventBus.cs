using System;
using System.Collections.Generic;


/// <summary>
/// Static type-keyed publist/subscribe event bus. Listeners subscribe with a strongly-typed
/// callback for a given event type <c>T</c>, and publishing an instance of <c>T</c> invokes all
/// subscribed callbacks for that type.
/// </summary>
public static class EventBus
{
    // ==================================================
    // Private Fields
    // ==================================================
    private static Dictionary<Type, Action<object>> events = new();
    private static Dictionary<Delegate, Action<object>> lookup = new();

    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Subscribes the given callback to events of type <typeparamref name="T"/>. Internally
	/// wraps the callback so multiple event types can share a single untyped delegate map. The
	/// wrapper is tracked so the same callback instance can later be removed with
	/// <see cref="Unsubscribe{T}"/>.
	/// </summary>
	/// <typeparam name="T">The event type to listen for. Must be a struct.</typeparam>
	/// <param name="callback">The callback to invoke when an event of type <typeparamref name="T"/> is published. Must not be null.</param>
    public static void Subscribe<T>(Action<T> callback) where T : struct
    {
        if (callback == null)
        {
            Logger.Error("[EventBus] Attempted subscribe with null action");
            return;
        }

        var type = typeof(T);
        Action<object> wrapper = (obj) => callback((T)obj);
        lookup[callback] = wrapper; // store the wrapper so we can remove it later

        if (events.TryGetValue(type, out var action))
        {
            events[type] = action + wrapper;    // add this action to the event listener
        }
        else
        {
            events[type] = wrapper; // initialize event listener with this action
        }
    }

    /// <summary>
	/// Unsubscribes the given callback from events of type <typeparamref name="T"/>. Does
	/// nothing if the callback was never subscribed/already unsubscribed.
	/// </summary>
	/// <typeparam name="T">The event type to stop listening for. Must be a struct.</typeparam>
	/// <param name="callback">The callback to remove. Must not be null.</param>
    public static void Unsubscribe<T>(Action<T> callback) where T : struct
    {
        if (callback == null)
        {
            Logger.Error("[EventBus] Attempted unsubscribe with null action");
            return;
        }

        var type = typeof(T);
        if (!lookup.TryGetValue(callback, out var wrapper)) // check if this action was saved earlier
        {
            return;
        }

        if (events.TryGetValue(type, out var action))
        {
            action -= wrapper;  // remove this action from the event listener

            if (action == null)
            {
                events.Remove(type);
            }
            else
            {
                events[type] = action;
            }
        }

        lookup.Remove(callback);    // remove this action from saved earlier
    }

    /// <summary>
	/// Publishes an event, invoking every callback currently subscribed to type <typeparamref name="T"/>.
	/// Does nothing if there are no subscribers.
	/// </summary>
	/// <typeparam name="T">The event type being published. Must be a struct.</typeparam>
	/// <param name="evt">The event instance to pass to each subscriber.</param>
    public static void Publish<T>(T evt) where T : struct
    {
        var type = typeof(T);

        if (events.TryGetValue(type, out var action))
        {
            action.Invoke(evt);   // send event to all listeners
        }
    }
}
