using UnityEngine;

/// <summary>
/// An object that calculates waves by determining the wave's height at any given
/// time (by offsetting the sine function) and X & Z coordinates.
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public float amplitude = 1f;
    public float length = 2f;
    public float speed = 1f;
    public float offset = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    private void Update()
    {
        offset += Time.deltaTime * speed;
    }

    /// <summary>
    /// Determines the height of the wave at any given offset and X & Z coordinates.
    /// </summary>
    /// <param name="_x"> The X coordinate of the object calling the function. </param>
    /// <param name="_z"> The Z coordinate of the object calling the function.. </param>
    /// <returns>
    /// The calculated wave height as a float. 
    /// </returns>
    public float getWaveHeight(float _x, float _z)
    {
        return amplitude * Mathf.Sin(_x / length + offset) * Mathf.Sin(_z / length + offset);
    }
}
