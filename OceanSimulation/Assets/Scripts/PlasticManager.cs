using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that spawns and manages all the plastic objects in the simulation.
/// </summary>
public class PlasticManager : MonoBehaviour
{
    public static PlasticManager Instance;

    public List<GameObject> plasticPrefabs;
    public float seaLevel;

    public float timeBeforeSpawningStarts;
    public float plasticAmountMultiplier = 1;
    public float delay = 1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnPlastic());
    }

    /// <summary>
    /// Spawns plastic at a specific rate based on the Log10 of the time passed.
    /// </summary>
    /// <returns>
    /// An IEnumerator that makes the function run over multiple frames.
    /// </returns>
    IEnumerator spawnPlastic()
    {
        int numOfPlastic = 0;
        float timePassed = 0;

        yield return new WaitForSeconds(timeBeforeSpawningStarts);
        while (timePassed < timeBeforeSpawningStarts)
        {
            numOfPlastic = Mathf.FloorToInt(Mathf.Log10(timePassed * plasticAmountMultiplier)) + 1;
            instantiatePlastic(numOfPlastic);
            timePassed += delay;
            yield return new WaitForSeconds(delay);
        }
        
        while (true)
        {
            numOfPlastic = Mathf.FloorToInt(Mathf.Log10(timePassed * 0.75f * plasticAmountMultiplier)) + 1;
            instantiatePlastic(numOfPlastic);
            timePassed += delay;
            yield return new WaitForSeconds(delay);
        }
    }

    /// <summary>
    /// Creates some plastic objects and spawns them within a random range of X & Z coords at sea level.
    /// </summary>
    /// <param name="amountToSpawn"> The number of plastic objects the function should spawn. </param>
    void instantiatePlastic(int amountToSpawn)
    {
        int prefabsIndex = Mathf.RoundToInt(Random.value);
        for (int i = 0; i < amountToSpawn; i++)
        {
            Vector3 randomSpawnPos = new Vector3(Random.Range(3, 70), seaLevel, Random.Range(3f, 44)).Round(1);
            Instantiate(plasticPrefabs[prefabsIndex], randomSpawnPos, Quaternion.identity);
            prefabsIndex += 1;
            if (prefabsIndex >= plasticPrefabs.Count) prefabsIndex = 0;
        }
        UniversalStats.Instance.totalNumOfPlastic++;
    }
}
