
/// <summary>
/// Defines a contract for filtering and detecting offensive language or profanity within text.
/// </summary>
public interface IProfanityFilter
{
    /// <summary>
	/// Determines whether the specified input string contains any profanities.
	/// </summary>
	/// <param name="input">The text string to analyze for profanity.</param>
	/// <returns><c>true</c> if input contains a profane substring, <c>false</c> otherwise.</returns>
    public bool ContainsProfanity(string input);
}
