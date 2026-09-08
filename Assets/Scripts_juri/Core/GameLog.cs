using UnityEngine;

/// <summary>
/// Editor-only logging.
///
/// Calls marked [Conditional] are removed by the compiler in player builds,
/// including the evaluation of their arguments. That matters here for two
/// reasons: the puzzle scripts were logging their own correct answers straight
/// into the shipped Player.log, and the string concatenation in the hot paths
/// (flag changes, room changes) was allocating on every call.
///
/// Debug.LogError / Debug.LogWarning are intentionally left alone - real
/// problems should still be reported in a build.
/// </summary>
public static class GameLog
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object message, Object context)
    {
        Debug.Log(message, context);
    }
}
