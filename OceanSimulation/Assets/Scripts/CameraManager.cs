using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that stores a set of predefined positions and rotations then applies
/// one of them to the attached camera object depending on the user's arrowkey inputs.
/// </summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    /// <summary>
    /// A class that stores both a position and an Euler rotation to be used to
    /// set the tranform of the attached camera object.
    /// </summary>
    [System.Serializable]
    public class CameraPositions
    {
        public Vector3 position;
        public Vector3 eulerRotation;
    }
    public List<CameraPositions> bottomCameraPos = new List<CameraPositions>();
    public List<CameraPositions> middleCameraPos = new List<CameraPositions>();
    public List<CameraPositions> topCameraPos = new List<CameraPositions>();

    public List<List<CameraPositions>> allCameraPos = new List<List<CameraPositions>>();

    private int horizontalIndex;
    private int verticalIndex;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
        allCameraPos = new List<List<CameraPositions>>() { bottomCameraPos, middleCameraPos, topCameraPos };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            horizontalIndex += 1;
            if (horizontalIndex >= allCameraPos[verticalIndex].Count) horizontalIndex = 0;
            moveCamera();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            horizontalIndex -= 1;
            if (horizontalIndex < 0) horizontalIndex = allCameraPos[verticalIndex].Count - 1;
            moveCamera();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            verticalIndex += 1;
            if (verticalIndex >= allCameraPos.Count) verticalIndex = 0;
            moveCamera();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            verticalIndex -= 1;
            if (verticalIndex < 0) verticalIndex = allCameraPos.Count - 1;
            moveCamera();
        }
    }

    /// <summary>
    /// Sets the camera to the position and rotation obtained from indexing the allCameraPos array. 
    /// </summary>
    void moveCamera()
    {
        transform.position = allCameraPos[verticalIndex][horizontalIndex].position;
        transform.rotation = Quaternion.Euler(allCameraPos[verticalIndex][horizontalIndex].eulerRotation);
    }
}
