
/// <summary>
/// Defines a generic file based storage contract for saving and loading serializable data by
/// file name, independent of the underlying storage format.
/// </summary>
public interface IFileStorage
{
    /// <summary>
	/// Serializes and saves <paramref name="data"/> to the file named <paramref name="fileName"/>
	/// </summary>
	/// <typeparam name="T">The type of data to save.</typeparam>
	/// <param name="fileName">The name of the file to write.</param>
	/// <param name="data">The data to save.</param>
    void Save<T>(string fileName, T data);

	/// <summary>
	/// Loads and deserializes the file named <paramref name="fileName"/>.
	/// </summary>
	/// <typeparam name="T">The type to deserialize into. Must have a parameterless constructor.</typeparam>
	/// <param name="fileName">The name of the file to load.</param>
	/// <returns>The deserialized data.</returns>
    T Load<T>(string fileName) where T : new();
}
