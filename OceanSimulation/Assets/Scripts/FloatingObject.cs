using UnityEngine;

/// <summary>
/// A class that stores some miscellaneous methods that can be used anywhere throughout the project. 
/// </summary>
public class FloatingObject : MonoBehaviour
{
    public Rigidbody rb;

    private float maxWaveHeight;
    public float verticalOffset;

    private void Start()
    {
        maxWaveHeight = WaveManager.Instance.transform.position.y + WaveManager.Instance.amplitude + 1;
    }

    private void FixedUpdate()
    {
        Vector3 raycastPos = new Vector3(transform.position.x, maxWaveHeight, transform.position.z);
        Ray waveHeightRay = new Ray(raycastPos, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(waveHeightRay, out hit, Mathf.Infinity, 1 << 4))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y + verticalOffset, hit.point.z); 
        }
    }
}
