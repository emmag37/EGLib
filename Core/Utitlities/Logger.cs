using UnityEngine;

// for classes without UnityEngine (core)
public static class Logger
{
    public static void Log(string message) => Debug.Log(message);
    public static void Error(string message) => Debug.LogError(message);
    public static void Warning(string message) => Debug.LogWarning(message);
}
