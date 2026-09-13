using UnityEngine;
using System;


/// <summary>
/// Fixed-size object pool for pooled <see cref="Component"/>s of type <typeparamref name="TObj"/>,
/// using an intrusive doubly-linked free list (via <see cref="ILinkable{TSelf, TData}"/>'s
/// Next/Prev links) to track available instances without additional allocations.
/// </summary>
/// <typeparam name="TObj">
/// The pooled component type. Must implement <see cref="ILinkable{TSelf, TData}"/> to provide
/// the Next/Prev links used by the internal free list.
/// </typeparam>
/// <typeparam name="TData">The data type used to initialize each pooled instance.</typeparam>
public class GameObjectPool<TObj, TData> where TObj : Component, ILinkable<TObj, TData>
{
    // ==================================================
    // Private Fields
    // ==================================================
    private TObj[] objects;

    private TObj head = null;
    private TObj tail = null;


    // ==================================================
    // Constructors
    // ==================================================

    /// <summary>
	/// Instantiates <paramref name="size"/> copies of <paramref name="prefab"/>, initializes
	/// each with <paramref name="data"/>, and adds them all to the free list, ready to be
	/// checked out via <see cref="TryGetObject"/>.
	/// </summary>
	/// <param name="size">The fixed number of instances to pre-allocate.</param>
	/// <param name="prefab">The prefab to instantiate for each pooled instance. Must have a component of type <typeparamref name="TObj"/>.</param>
	/// <param name="data">The data passed to each instance's <c>Initialize</c> call.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="prefab"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="prefab"/> has no component of type <typeparamref name="TObj"/>.</exception>
    public GameObjectPool(int size, GameObject prefab, TData data)
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab), "Object pool requires a valid prefab.");
        if (!prefab.TryGetComponent<TObj>(out _))
            throw new ArgumentException($"Prefab '{prefab.name}' has no {typeof(TObj)} component.", nameof(prefab));

        objects = new TObj[size];
        for (int i = 0; i < size; i++)
        {
            objects[i] = UnityEngine.Object.Instantiate(prefab).GetComponent<TObj>();
            objects[i].Initialize(data);
            AppendToFreeList(objects[i]);
        }
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Attempts to check out a free object from the pool, activating it and removing it from
	/// the free list.
	/// </summary>
	/// <param name="obj">The checked-out object, or null if none were available.</param>
	/// <returns><c>true</c> if an object was available and returned, <c>false</c> otherwise.</returns>
    public bool TryGetObject(out TObj obj)
    {
        obj = GetFreeObject();
        return obj != null;
    }

    /// <summary>
	/// Returns a checked-out object to the pool, deactivating it and adding it back to the free
	/// list.
	/// </summary>
	/// <param name="obj">The object to return to the pool.</param>
	/// <returns><c>true</c> if the object was successfully returned, <c>false</c> otherwise.</returns>
    public bool RemoveObject(TObj obj) => AppendToFreeList(obj);


    // ==================================================
    // Private Methods
    // ==================================================

    /// <summary>
	/// Appends the given object to the tail of the free list and deactivates its GameObject.
	/// Guards against double frees.
	/// </summary>
	/// <param name="obj">The object to add to the free list. Must not be null.</param>
	/// <returns><c>true</c> if the object was added, <c>false</c> if already in the free list (logged as an error).</returns>
    private bool AppendToFreeList(TObj obj)
    {
        Debug.Assert(obj != null, "[GameObjectPool] Attempted to add a null object to free list");

        if (obj == head || obj.Next != null || obj == tail)
        {
            Debug.LogError("[GameObjectPool] Attempted a double free");
            return false;
        }

        if (head == null)
        {
            head = obj;
            obj.Prev = null;
        }
        else
        {
            tail.Next = obj;
            obj.Prev = tail;
        }

        obj.Next = null;
        tail = obj;
        obj.gameObject.SetActive(false);

        return true;
    }

    /// <summary>
	/// Removes and returns the head of the free list, activating its GameObject.
	/// </summary>
	/// <returns>The former head object, or null (logged as a warning) if the free list is empty.</returns>
    private TObj GetFreeObject()
    {
        if (head == null)
        {
            Debug.LogWarning($"[GameObjectPool] No available objects - all {objects.Length} objects in use");
            return null;
        }

        // removes and returns the head of the free list
        TObj obj = head;
        head = head.Next;

        if (head == null)  // no more pieces in the list
            tail = null;
        else
            head.Prev = null;

        obj.Next = null;
        obj.gameObject.SetActive(true);

        return obj;
    }
}
