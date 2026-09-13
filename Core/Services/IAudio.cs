using System;

[Serializable]
public class AudioSettings
{
	public bool MusicOn = true;
	public bool SFXOn = true;
}

/// <summary>
/// Provides a playback control for music and sound effects, identified by audio ID.
/// </summary>
public interface IAudio
{
	/// <summary>
	/// Plays the music track associated with the given audio ID. Stops any currently playing
	/// music first.
	/// </summary>
	/// <param name="audioID">Identifier of the music track to play.</param>
	void PlayMusic(int audioID);

	/// <summary>
	/// Stops the currently playing music, if any.
	/// </summary>
	void StopMusic();

	/// <summary>
	/// Enables or disables music playback.
	/// </summary>
	/// <param name="on">True to enable music, false to disable it.</param>
	void SetMusicOn(bool on);

	/// <summary>
	/// Plays the one shot sound effect associated with the given audio ID.
	/// </summary>
	/// <param name="audioID">Identifier of the sound effect to play.</param>
	void PlaySoundEffect(int audioID);

	/// <summary>
	/// Enables or disables sound effect playback.
	/// </summary>
	/// <param name="on">True to enable sound effects, false to disable them.</param>
	void SetEffectsOn(bool on);

	/// <summary>
	/// Gets the current audio settings.
	/// </summary>
	/// <returns>The current audio settings implemented as <typeparamref name="TSettings"/>.</returns>
	AudioSettings GetSettings();
}
