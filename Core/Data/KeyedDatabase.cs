using UnityEngine;
using System.Collections.Generic;
using System;


/// <summary>
/// Base class for a ScriptableObject backed database that maps keys to values/
/// Serialized as an array with key-value pairs in the inspector.
/// </summary>
/// <typeparam name="TKey">Type key to lookup entries. Must be non-null for reference types.</typeparam>
/// <typeparam name="TValue">Type value associated with each key.</typeparam>
public abstract class KeyedDatabase<TKey, TValue> : ScriptableObject
{
    // ==================================================
    // Inspector Fields
    // ==================================================

    /// <summary>
	/// Array of entries to be added in inspector.
	/// </summary>
    [SerializeField] private Entry[] entries;

    /// <summary>
	/// Single entry struct to be determined in inspector.
	/// </summary>
    [Serializable]
    protected struct Entry
    {
        /// <summary>Lookup key for this entry.</summary>
        public TKey key;

        /// <summary>Value associated with <see cref="key"/>.</summary>
        public TValue value;
    }

    // ==================================================
    // Private Fields
    // ==================================================
    private Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();


    // ==================================================
    // Initialization
    // ==================================================

    /// <summary>
	/// Builds the lookup dictionary at runtime from <see cref="entries"/> array.
	/// Must be called before attempting <see cref="TryGetValue"/>. Null keys or duplicate entries
	/// are logged as errors then skipped.
	/// </summary>
	/// <exception cref="InvalidCastException">Thrown if <see cref="entries" is null.</exception>
    public void Initialize()
    {
        if (entries == null)
            throw new InvalidOperationException("[KeyedDatabase] Requires non-null entries array for initialization");

        foreach (Entry entry in entries)
        {
            if (entry.key == null)
            {
                Debug.LogError("[KeyedDatabase] Found null key in entries, skipping entry");
                continue;
            }
            if (dict.ContainsKey(entry.key))
            {
                Debug.LogError($"[KeyedDatabase] Duplicate key: {entry.key}");
                continue;
            }
            dict[entry.key] = entry.value;
        }
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Attempts to get the value associated with the given key.
	/// </summary>
	/// <param name="key">Key to lookup.</param>
	/// <param name="value">Holds the found value if this returns true, otherwise holds original reference.</param>
	/// <returns><c>true</c> if the key was found, <c>false</c> otherwise.</returns>
    public bool TryGetValue(TKey key, out TValue value) => dict.TryGetValue(key, out value);
}