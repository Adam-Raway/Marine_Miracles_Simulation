using UnityEngine;

static public class ExtensionMethods
{
    /// <summary>
    /// Rounds a vector3 to a specific number of decimal places.
    /// </summary>
    /// <param name="vector"> The Vector3 to round. </param>
    /// <param name="decimalPlaces"> The number of decimal places to round the Vector3 to. </param>
    /// <returns> A vector3 rounded to the specified decimal places.</returns>
    public static Vector3 Round(this Vector3 vector, int decimalPlaces = 0)
    {
        float multiplier = 1 * Mathf.Pow(10, decimalPlaces);

        return new Vector3(
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier,
            Mathf.Round(vector.z * multiplier) / multiplier);
    }

    /// <summary>
    /// Determines the direction (positive or negative) of a float.
    /// </summary>
    /// <param name="number"> The number that will have its direction determined. </param>
    /// <returns> Returns either -1 or 1 depending on whether the number is positive or negative. </returns>
    public static float Direction(float number)
    {
        return number / Mathf.Abs(number);
    }
}
