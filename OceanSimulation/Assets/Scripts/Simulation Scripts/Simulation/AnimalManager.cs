using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that spawns and manages all the animal objects in the simulation.
/// </summary>
public class AnimalManager : MonoBehaviour
{
    public List<GameObject> animalPrefabs;
    public int startingNumOfAnimals;

    public static AnimalManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject prefab in animalPrefabs)
        {
            for (int i = 0; i < startingNumOfAnimals / animalPrefabs.Count; i++)
            {
                Vector3 randomSpawnPos = Vector3.zero;
                if (prefab.layer == 11 || prefab.layer == 12)
                {
                    randomSpawnPos = new Vector3(Random.Range(3, 70), Random.Range(5, 16), Random.Range(3f, 44)).Round(1);
                    Instantiate(prefab, randomSpawnPos, Quaternion.identity);
                }
                else
                {
                    randomSpawnPos = new Vector3(Random.Range(3, 70), 5.5f, Random.Range(3f, 44)).Round(1);
                    Ray heightRay = new Ray(randomSpawnPos, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(heightRay, out hit, 5, 1 << 0))
                    {
                        randomSpawnPos.y = hit.point.y + 0.223f;
                    }
                    if (randomSpawnPos.y <= 4.7) Instantiate(prefab, randomSpawnPos, Quaternion.identity);
                }
            }
        }
    }
}