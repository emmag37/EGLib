using UnityEngine;
using System;
using System.Text.RegularExpressions;

/// <summary>
/// Static class to validate a username against length, special characters,
/// and profanity.
/// </summary>
public static class UsernameValidator
{
    // ==================================================
    // Constants
    // ==================================================
    private const int LowerBound = 3;
    private const int UpperBound = 16;

    private const string Chars = @"^\w+$";

    // ==================================================
    // Private Fields
    // ==================================================
    private static IProfanityFilter profanityFilter;


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Initializes the <see cref="UsernameValidator"/> with a required profanity filter dependency.
	/// </summary>
	/// <param name="profanityFilter"The profanity filter instance used to validate usernames.></param>
	/// <exception cref="ArgumentNullException">Thrown whten <paramref name="profanityFilter"/> is null.</exception>
    public static void Initialize(IProfanityFilter profanityFilter)
    {
        if (profanityFilter == null)
            throw new ArgumentNullException(nameof(profanityFilter));

        UsernameValidator.profanityFilter = profanityFilter;
    }

    /// <summary>
	/// Validates a username against length, special characters, and profanity.
	/// </summary>
	/// <param name="name">The username to validate.</param>
	/// <returns>
	/// <see cref="InvalidUsernameType.None"/> if the username passes all checks, otherwise the
	/// specific <see cref="InvalidUsernameType"/> indicating why it failed. Checks are evaluated
	/// in order (length, then characters, then profanity), so only the first failure is returned.
	/// </returns>
	/// <exception cref="InvalidOperationException">Thrown if this method is called before <see cref="Initialize(IProfanityFilter)"/>.</exception>
    public static InvalidUsernameType IsUsernameValid(string name)
    {
        if (profanityFilter == null)
            throw new InvalidOperationException("[UsernameValidator] Must initialize before calling IsUsernameValid");

        // correct length
        if (name.Length < LowerBound)
            return InvalidUsernameType.Short;

        // no special chars
        if (!Regex.IsMatch(name, Chars))
            return InvalidUsernameType.SpecialChars;

        // no profanity
        if (profanityFilter.ContainsProfanity(name))
            return InvalidUsernameType.Profanity;

        return InvalidUsernameType.None;
    }
}
