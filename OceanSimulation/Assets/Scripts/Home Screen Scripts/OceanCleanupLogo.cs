using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script does 2 things. First, if a turtle graphic is provided in the
/// public Animator slot, then this script will toggle it. Second, it allows
/// the button to open the Ocean Cleanup Foundation's website when pressed.
/// </summary>
public class OceanCleanupLogo : MonoBehaviour
{
    public Animator turtleGraphic;
    public Button logoButton;

    /// <summary>
    /// This function toggles the turtle graphic's sliding animation.
    /// </summary>
    public void logoHoverToggled()
    {
        turtleGraphic.SetTrigger("Toggle");
    }

    /// <summary>
    /// Opens the Ocean Cleanup Foundation's website in the computer's.
    /// default browser. Note: If this program is running on a WebGL build,
    /// then the website will open in the same tab and close the game.
    /// </summary>
    public void openOceanCleanupSite()
    {
        Application.OpenURL("https://theoceancleanup.com/");
    }
}
