
/// <summary>
/// Represents a collection whose current position can be stepped forward or backward, 
/// returning the item at the new position each time.
/// </summary>
/// <typeparam name="T">The type of item stored in the collection.</typeparam>
public interface IIncrementList<T>
{
    /// <summary>
	/// Advances to the next item in the collection and returns it.
	/// </summary>
	/// <returns>The item at the new current position.</returns>
    T Next();

    /// <summary>
	/// Moves to the previous item in the collection and returns it.
	/// </summary>
	/// <returns>The item at the new current position.</returns>
    T Prev();
}
