using UnityEngine;
using System.IO;
using System;
using System.Threading.Tasks;


/// <summary>
/// JSON-backed implementation of <see cref="IFileStorage"/> using <see cref="JsonUtility"/> to
/// serialize/deserialize files under <see cref="Application.persistentDataPath"/>. Distinguishes
/// a missing file (silent fresh start) from a corrupt file (backed up with a timestamped rename,
/// logged, and replaced with a fresh instance).
/// </summary>
public class JsonFileStorage : IFileStorage
{
    // ==================================================
    // Interface Methods
    // ==================================================

    /// <summary>
	/// Serializes <paramref name="data"/> to JSON and writes it to the file named
	/// <paramref name="fileName"/> under the persistent data path. Should always be called
	/// within a try/catch, since the underlying file write can throw.
	/// </summary>
	/// <typeparam name="T">The type of data to serialize.</typeparam>
	/// <param name="fileName">The name of the file to write.</param>
	/// <param name="data">The data to serialize and save. Must not be null.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="fileName"/> is null or empty.</exception>
    public void Save<T>(string fileName, T data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        string path = GetPath(fileName);
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);
    }

    /// <summary>
	/// Loads and deserializes the file name <paramref name="fileName"/> from the persistent data
	/// path. Returns a fresh <typeparamref name="T"/> if the file doesn't exist (silent fresh
	/// start). If the file exists but is empty, unparseable, or deserializes to null, the
	/// corrupt file is backed up via <see cref="BackupCorruptData"/>, an error is logged and a
	/// fresh <typeparamref name="T"/> is returned.
	/// </summary>
	/// <typeparam name="T">The type to deserialize into. Must have a parameterless constructor.</typeparam>
	/// <param name="fileName">The name of the file to load.</param>
	/// <returns>THe deserialized data, or a fresh <typeparamref name="T"/> if the file is missing or corrupt.</returns>
	/// <exception cref="IOException">Thrown if reading the file fails at the I/O level (recoverable by the caller).</exception>
	/// <exception cref="UnauthorizedAccessException">Thrown if the file cannot be accessed (recoverable by the caller).</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="fileName"/> is null or empty.</exception>
    public T Load<T>(string fileName) where T : new()
    {
        string path = GetPath(fileName);

        if (!File.Exists(path))
            return new T();

        string json = File.ReadAllText(path);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning($"[JsonFileStorage] Null or empty read from file {fileName}");
            return new T();
        }

        try
        {
            T result = JsonUtility.FromJson<T>(json);
            if (result == null)
            {
                Debug.LogError("[JsonFileStorage] From Json returned null result");
                BackupCorruptData(path);

                return new T();
            }

            return result;
        }
        catch (Exception e)
        {
            Debug.LogError($"[JsonFileStorage] From Json failed with exception {e}");
            BackupCorruptData(path);

            return new T();
        }
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Attempts <see cref="Save{T}"/>, retrying with a short delay if it fails due to a
	/// transient <see cref="IOException"/> or <see cref="UnauthorizedAccessException"/>.
	/// Rethrows the last exception if all retries are exhausted. Other exception types are not
	/// caught here and propagate immediately.
	/// </summary>
	/// <typeparam name="T">The type of data to serialize.</typeparam>
	/// <param name="fileName">The name of the file to write to.</param>
	/// <param name="data">The data to serialize and save.</param>
	/// <param name="retries">The maximum number of attempts before exiting, defaults to 3.</param>
	/// <exception cref="IOException">Thrown if all retries are exhausted while the failure is an I/O error.</exception>
	/// <exception cref="UnauthorizedAccessException">Thrown if all retries are exhausted while the failure is an access error.</exception>
    public async Task SaveWithRetries<T>(string fileName, T data, int retries = 3)
    {
        for (int i = 1; i <= retries; i++)
        {
            try
            {
                Save<T>(fileName, data);
                return;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Debug.LogError($"[JsonFileStorage] Save {fileName} failed attempt {i} with {e}");
                if (i < retries)
                    await Task.Delay(100);
                else
                    throw e;
            }
        }
    }

    /// <summary>
	/// Attempts <see cref="Load{T}"/>, retrying with a short delay if it fails due to a
	/// transient <see cref="IOException"/> or <see cref="UnauthorizedAccessException"/>.
	/// Rethrows the last exception if all retries are exhausted.
	/// </summary>
	/// <typeparam name="T">The type to deserialize into. Must have a parameterless constructor.</typeparam>
	/// <param name="fileName">The name of the file to load.</param>
	/// <param name="retries">The maximum number of attempts before giving up.</param>
	/// <returns>The loaded data.</returns>
	/// <exception cref="IOException">Thrown if all retries are exhausted while the failure is an I/O error.</exception>
	/// <exception cref="UnauthorizedAccessException">Thrown if all retries are exhausted while the failure is an access error.</exception>
    public async Task<T> LoadWithRetries<T>(string fileName, int retries = 3) where T : new()
    {
        if (retries < 1)
        {
            Debug.LogWarning("[JsonFileStorage] Called LoadWithRetries with no tries, returning defualt");
            return default(T);
        }

        for (int i = 1; i <= retries; i++)
        {
            try
            {
                return Load<T>(fileName);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Debug.LogError($"[JsonFileStorage] Load {fileName} failed attempt {i} with {e}");
                if (i < retries)
                    await Task.Delay(100);
                else
                    throw e;
            }
        }

        throw new FileLoadException(); // unreachable, for compiler
    }

    /// <summary>
	/// Renames the file at <paramref name="path"/> with a <c>.corrupt_&lt;timestamp&gt;.bak</c>
	/// suffix, preserving the corrupt data for inspection while freeing up the original path for
	/// a fresh save. Failures are caught and logged rather than thrown, since this is itself a
	/// best effort recovery step.
	/// </summary>
	/// <param name="path">The full path of the corrupt file to bac up.</param>
    public void BackupCorruptData(string path)
    {
        try
        {
            string backupPath = $"{path}.corrupt_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak";
            File.Move(path, backupPath);
            Debug.LogWarning($"[JsonFileStorage] Moved corrupt data file from {path} to {backupPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[JsonFileStorage] Failed to backup data at {path} with exception {e}");
        }
    }


    // ==================================================
    // Private Methods
    // ==================================================

    /// <summary>
	/// Builds the full path for a given file name under
	/// <see cref="Application.persistentDataPath"/>.
	/// </summary>
	/// <param name="fileName">The file name to resolve. Must not be null or empty.</param>
	/// <returns>The full file path.</returns>
	/// <exception cref="ArgumentException">Thrown if <paramref name="fileName"/> is null or empty.</exception>
    private string GetPath(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("[JsonFileStorage] File name is null or empty", nameof(fileName));

        return Path.Combine(Application.persistentDataPath, fileName);
    }
}
