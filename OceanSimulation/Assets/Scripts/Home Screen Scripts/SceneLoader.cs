using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is responsible for transitioning between different scenes in the program.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1;

    /// <summary>
    /// Loads the next scene in the program by calling the loadScene() coroutine. If the
    /// last scene is currently open, then it will load the first scene (index 0).
    /// </summary>
    public void loadNextScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            StartCoroutine(loadScene(0));
        }
        else StartCoroutine(loadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    /// <summary>
    /// Does a fade transition before opening the next scene in the program's queue.
    /// </summary>
    /// <param name="sceneIndex"> The index of the scene to transition to. </param>
    /// <returns>
    /// An IEnumerator that makes the function run over multiple frames. 
    /// </returns>
    IEnumerator loadScene(int sceneIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }
}
