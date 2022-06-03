using System;
using UnityEngine;

/// <summary>
/// Stores and manipulates stats that need to be accessed universally.
/// </summary>
public class UniversalStats : MonoBehaviour
{
    public static UniversalStats Instance;

    public float startingChanceOfKelpToSpawn;
    public float chanceOfKelpToSpawn;
    public float totalNumOfPlastic;
    public float totalNumOfKelp;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    private void Update()
    {
        chanceOfKelpToSpawn = Mathf.Clamp(startingChanceOfKelpToSpawn - Mathf.Log(totalNumOfPlastic, 20), 1, 20);
    }
}
