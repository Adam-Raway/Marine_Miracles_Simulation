using System.Linq;
using UnityEngine;

/// <summary>
/// Animates the wave created by the WaveManager object by manipulating the vertices,
/// mesh & collider of the attached water object. 
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WaterWaveAnimator : MonoBehaviour
{
    public bool flipped;

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    private void FixedUpdate()
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = WaveManager.Instance.getWaveHeight(vertices[i].x, vertices[i].z);
        }
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
        if (flipped)
        {
            meshFilter.mesh.triangles = meshFilter.mesh.triangles.Reverse().ToArray();
        }
    }

    private void LateUpdate()
    {
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = meshFilter.mesh;
    }
}
