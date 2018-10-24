using UnityEngine;

public static class Vector3Extensions
{
    /// <summary>
    /// Keeps the original values and replaces with the supplied ones.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        float newX = x ?? original.x;
        float newY = y ?? original.y;
        float newZ = z ?? original.z;

        return new Vector3(newX, newY, newZ);
    }

    public static bool IsBehind(this Vector3 queried, Vector3 forward)
    {
        return Vector3.Dot(queried, forward) < 0f;
    }
}
