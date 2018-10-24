using UnityEngine;

public static class TransformExtensions
{
    public static Vector3 DirectionTo(this Transform transform, Vector3 destination)
    {
        return Vector3.Normalize(destination - transform.position);
    }
}
