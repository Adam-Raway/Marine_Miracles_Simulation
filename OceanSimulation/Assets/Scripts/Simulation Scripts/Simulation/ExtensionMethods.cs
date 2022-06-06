using UnityEngine;

/// <summary>
/// A class that stores some miscellaneous methods that can be used anywhere throughout the project. 
/// </summary>
static public class ExtensionMethods
{
    /// <summary>
    /// Rounds a float to a specific number of decimal places.
    /// </summary>
    /// <param name="num"> The floating point number to round. </param>
    /// <param name="decimalPlaces"> The number of decimal places to round the number to. </param>
    /// <returns> A floating point number rounded to the specified decimal places.</returns>
    public static float Round(float num, int decimalPlaces = 0)
    {
        float multiplier = 1 * Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(num * multiplier) / multiplier;
    }

    /// <summary>
    /// Rounds a vector3 to a specific number of decimal places.
    /// </summary>
    /// <param name="vector"> The Vector3 to round. </param>
    /// <param name="decimalPlaces"> The number of decimal places to round the Vector3 to. </param>
    /// <returns> A vector3 rounded to the specified decimal places.</returns>
    public static Vector3 Round(this Vector3 vector, int decimalPlaces = 0)
    {
        return new Vector3(Round(vector.x, decimalPlaces), Round(vector.y, decimalPlaces), Round(vector.z, decimalPlaces));
    }

    /// <summary>
    /// Maps an input within a starting range to a value in the output range and clamps it.
    /// </summary>
    /// <param name="value"> The starting value that fits in the initial range. </param>
    /// <param name="inputMin"> The minimum of the initial range (inclusive). </param>
    /// <param name="inputMax"> The maximum of the initial range (inclusive). </param>
    /// <param name="outputMin"> The minimum of the output range (inclusive). </param>
    /// <param name="outputMax"> The maximum of the output range (inclusive). </param>
    /// <returns> Returns the clamped value of the input value mapped to the output range. </returns>
    public static float Map(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(outputMin + ((outputMax - outputMin) / (inputMax - inputMin)) * (value - inputMin), outputMin, outputMax);
    }
}
