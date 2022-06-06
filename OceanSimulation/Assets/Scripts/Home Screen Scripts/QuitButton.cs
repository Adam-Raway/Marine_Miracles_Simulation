using UnityEngine;

/// <summary>
/// Provides functionality to the Quit button in the home screen.
/// </summary>
public class QuitButton : MonoBehaviour
{
    /// <summary>
    /// When called this function closes the application entirely.
    /// </summary>
    public void quitProgram()
    {
        Application.Quit();
    }
}
