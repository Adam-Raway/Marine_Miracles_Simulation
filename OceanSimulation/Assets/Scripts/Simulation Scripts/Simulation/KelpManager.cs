using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that spawns, respawns, and manages all the kelp objects in the simulation.
/// </summary>
public class KelpManager : MonoBehaviour
{
    public GameObject kelpPrefab;
    public int startingNumOfKelp;
    public List<GameObject> deactivatedKelp;

    public static KelpManager Instance;

    float chanceOfKelpToSpawn;
    float totalNumOfKelp;

    private void Awake()
    { 
        totalNumOfKelp = UniversalManager.Instance.totalNumOfKelp;
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < startingNumOfKelp; i++)
        {
            spawnKelp(Random.Range(3, 70), Random.Range(3f, 44));
        }
    }

    private void Update()
    {
        chanceOfKelpToSpawn = UniversalManager.Instance.chanceOfKelpToSpawn;
        if (deactivatedKelp.Count > 0 && Random.Range(0, chanceOfKelpToSpawn * 1.1f) < chanceOfKelpToSpawn * Time.deltaTime)
        {
            int numOfKelpToSpawn = Mathf.Clamp( Mathf.RoundToInt(Random.Range(chanceOfKelpToSpawn * 0.005f,
                chanceOfKelpToSpawn * 0.001f) * (totalNumOfKelp - deactivatedKelp.Count)), 1, 100);
            for (int i = 0; i < numOfKelpToSpawn; i++)
            {
                int index = Random.Range(0, deactivatedKelp.Count - 1);
                deactivatedKelp[index].SetActive(true);
                deactivatedKelp.RemoveAt(index);
                if (deactivatedKelp.Count == 0) break;
            }
        }
    }

    /// <summary>
    /// Finds the height of the terrain at a given x & z coordinate, then spawns a kelp at that position.
    /// </summary>
    /// <param name="_x"> The X coordinate of the kelp's spawn position. </param>
    /// <param name="_z"> The Z coordinate of the kelp's spawn position. </param>
    public void spawnKelp(float _x, float _z)
    {
        Vector3 position = new Vector3(_x, 5.5f, _z);
        Ray heightRay = new Ray(position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(heightRay, out hit, 5, 1 << 0))
        {
            position.y = hit.point.y + 0.4f;
        }
        if (position.y <= 4.7) Instantiate(kelpPrefab, position, Quaternion.identity);
        UniversalManager.Instance.totalNumOfKelp++;
    }
}
