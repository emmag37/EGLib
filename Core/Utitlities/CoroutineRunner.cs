using UnityEngine;


/// <summary>
/// A static singleton MonoBehaviour that provides a persistent GameObject
/// for running coroutines from non-MonoBehaviour classes or static contexts.
/// </summary>
public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner instance;

    /// <summary>
	/// Gets the singleton instance of the <see cref="CoroutineRunner"/>.
	/// Lazily creates a hidden, persistent GameObject to host the instance
	/// the first time it is accessed.
	/// </summary>
    public static CoroutineRunner Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Coroutine Runner");     // create a hidden game object for this property to live on
                Object.DontDestroyOnLoad(go);                           // persistence across all scenes
                instance = go.AddComponent<CoroutineRunner>();
            }

            return instance;
        }
    }
}
