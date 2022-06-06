using UnityEngine;

/// <summary>
/// Manages the information UI and all it's components by updating numbers, toggling specific images, etc.
/// </summary>
public class InfoMenu : MonoBehaviour
{
    public GameObject tabKeyPrompt;
    public GameObject infoPanels;

    public GameObject spacebarGraphic;
    public GameObject activeText;

    public TMPro.TextMeshProUGUI numOfKelpUI;
    public TMPro.TextMeshProUGUI numOfPlaticUI;
    public TMPro.TextMeshProUGUI numOfPlaticRecycledUI;
    public TMPro.TextMeshProUGUI numOfFishPoisoned;
    public TMPro.TextMeshProUGUI numOfSalmon;
    public TMPro.TextMeshProUGUI numOfTurtles;
    public TMPro.TextMeshProUGUI numOfCrabs;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tabKeyPrompt.SetActive(false);
            infoPanels.SetActive(!infoPanels.activeInHierarchy);
        }

        numOfKelpUI.text = (UniversalManager.Instance.totalNumOfKelp - KelpManager.Instance.deactivatedKelp.Count).ToString("0");
        numOfPlaticUI.text = UniversalManager.Instance.totalNumOfPlastic.ToString("0");
        numOfPlaticRecycledUI.text = UniversalManager.Instance.numOfPlasticRecycled.ToString("0");
        numOfFishPoisoned.text = UniversalManager.Instance.numOfFishPoisoned.ToString("0");
        numOfSalmon.text = UniversalManager.Instance.totalPopulationOfSalmon.ToString("0");
        numOfTurtles.text = UniversalManager.Instance.totalPopulationOfTurtles.ToString("0");
        numOfCrabs.text = UniversalManager.Instance.totalPopulationOfCrabs.ToString("0");
    }

    /// <summary>
    /// Toggles the state of the System002 in the "System 002 Panel". Either the spacebar
    /// graphic is displayed (showing the user that they can activate teh System 002 by
    /// pressing spacebar) or some text shows that the System 002 is currently active.
    /// </summary>
    public void toggleSystem002State()
    {
        spacebarGraphic.SetActive(!spacebarGraphic.activeInHierarchy);
        activeText.SetActive(!activeText.activeInHierarchy);
    }
}
