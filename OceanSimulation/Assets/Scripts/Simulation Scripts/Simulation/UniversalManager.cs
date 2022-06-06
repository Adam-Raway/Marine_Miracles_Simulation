using System.Collections;
using UnityEngine;

/// <summary>
/// Stores and manipulates stats that need to be accessed universally, and is
/// responsible for spawning the System 002 boat when the user presses the spacebar.
/// </summary>
public class UniversalManager : MonoBehaviour
{
    public static UniversalManager Instance;

    public float startingChanceOfKelpToSpawn;
    public float chanceOfKelpToSpawn;

    public int totalNumOfPlastic;
    public int totalNumOfKelp;
    public int numOfPlasticRecycled;
    public int numOfFishPoisoned;
    public int totalPopulationOfSalmon;
    public int totalPopulationOfTurtles;
    public int totalPopulationOfCrabs;

    public GameObject system002;
    public InfoMenu infoMenu;

    public GameObject endScreen;
    public TMPro.TextMeshProUGUI endScreenDeadFishMsg;
    public TMPro.TextMeshProUGUI endScreenRecycledMsg;

    bool simulationEnded = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    private void Update()
    {
        chanceOfKelpToSpawn = Mathf.Clamp(startingChanceOfKelpToSpawn - totalNumOfPlastic * 0.2f, 1, startingChanceOfKelpToSpawn);
        if (Input.GetKeyDown(KeyCode.Space) && !system002.activeInHierarchy && !simulationEnded)
        {
            system002.transform.position = new Vector3(-4, 20.06f, -27);
            system002.transform.rotation = Quaternion.identity;
            system002.SetActive(true);
        }

        if (totalPopulationOfSalmon == 0 && totalPopulationOfTurtles == 0 && totalPopulationOfCrabs == 0 && !simulationEnded)
        {
            StartCoroutine(endSimulation());
        }
    }

    /// <summary>
    /// Opens the end screen of the simulation scene and updates the messages to contain
    /// the number of fish that died to plastic poisoning and the number of plastics recycled.
    /// </summary>
    /// <returns>
    /// An IEnumerator that makes the function run over multiple frames. 
    /// </returns>
    IEnumerator endSimulation()
    {
        simulationEnded = true;
        yield return new WaitForSeconds(5);

        endScreenDeadFishMsg.text = $"In total, {numOfFishPoisoned} sea creatures died due to eating plastic. This number " +
                $"doesn't include the fish that died indirectly because the plastic pollution decreased the amount of kelp.";
        endScreenRecycledMsg.text = $"On the bright side, the System 002 cleaned up {numOfPlasticRecycled} pieces of plastic, " +
            $"saving many fish. And you could save even more fish in real life by donating or volunteering with its creators: " +
            $"The Ocean Cleanup Foundation.";
        endScreen.SetActive(true);
    }
}
