using UnityEngine;
using System;


/// <summary>
/// Thin wrapper around <see cref="PlayerPrefs"/> providing a null checked key/value access,
/// a proper bool type (stored internally as an int), and an immediate save after every write.
/// </summary>
public static class PlayerPrefsStorage
{
    // ==================================================
    // Get Methods
    // ==================================================

    /// <summary>
	/// Retrieves a bool value stored under the given key, backed by an underlying int (1 = true,
	/// 0 = false).
	/// </summary>
	/// <param name="key">The key to look up. Must not be null.</param>
	/// <param name="defaultValue">The value to return if the key doesn't exist.</param>
	/// <returns>The stored value, or <paramref name="defaultValue"/> if the key doesn't exist.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
    public static bool GetBool(string key, bool defaultValue)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int value = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
        return value == 1;
    }

    /// <summary>
	/// Retrieves an int value stored under the given key.
	/// </summary>
	/// <param name="key">The key to look up. Must not be null.</param>
	/// <param name="defaultValue">The value to return if the key doesn't exist.</param>
	/// <returns>The stored value, or <paramref name="defaultValue"/> if the key doesn't exist.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
    public static int GetInt(string key, int defaultValue)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        return PlayerPrefs.GetInt(key, defaultValue);
    }

    /// <summary>
	/// Retrieves an string value stored under the given key.
	/// </summary>
	/// <param name="key">The key to look up. Must not be null.</param>
	/// <param name="defaultValue">The value to return if the key doesn't exist.</param>
	/// <returns>The stored value, or <paramref name="defaultValue"/> if the key doesn't exist. Must not be null</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="defaultValue"/> is null.</exception>
    public static string GetString(string key, string defaultValue)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (defaultValue == null)
            throw new ArgumentNullException(nameof(defaultValue));

        return PlayerPrefs.GetString(key, defaultValue);
    }


    // ==================================================
    // Set Methods
    // ==================================================

    /// <summary>
	/// Stores a bool value under the given key, backed by an underlying int (1 = true, 0 = false)
	/// and immediately saves. Logs an error and does nothing if the key is null.
	/// </summary>
	/// <param name="key">The key to store the value under.</param>
	/// <param name="value">The value to store.</param>
    public static void SetBool(string key, bool value)
    {
        if (key == null)
        {
            Debug.LogError("[PlayerPrefsStorage] Attempted to set bool with null key");
            return;
        }

        PlayerPrefs.SetInt(key, value ? 1 : 0);
        Save();
    }

    /// <summary>
	/// Stores an int value under the given key and immediately saves. Logs an error and does
	/// nothing if the key is null.
	/// </summary>
	/// <param name="key">The key to store the value under.</param>
	/// <param name="value">The value to store.</param>
    public static void SetInt(string key, int value)
    {
        if (key == null)
        {
            Debug.LogError("[PlayerPrefsStorage] Attempted to set int with null key");
            return;
        }

        PlayerPrefs.SetInt(key, value);
        Save();
    }

    /// <summary>
	/// Stores an string value under the given key and immediately saves. Logs an error and does
	/// nothing if the key or the value is null.
	/// </summary>
	/// <param name="key">The key to store the value under.</param>
	/// <param name="value">The value to store.</param>
    public static void SetString(string key, string value)
    {
        if (key == null)
        {
            Debug.LogError("[PlayerPrefsStorage] Attempted to set string with null key");
            return;
        }
        if (value == null)
        {
            Debug.LogError("[PlayerPrefsStorage] Attempted to set string with null value");
            return;
        }

        PlayerPrefs.SetString(key, value);
        Save();
    }


    // ==================================================
    // Private Methods
    // ==================================================

    /// <summary>
	/// Immediately writes all pending <see cref="PlayerPrefs"/> changes to disk.
	/// </summary>
	/// <remarks>
	/// Setting several values back to back will incure one disk write per call rather than a single
	/// batched write.
	/// </remarks>
    private static void Save()
    {
        PlayerPrefs.Save();
    }
}
