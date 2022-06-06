using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys any plastic that the net collider (of the System 002 object) touches.
/// </summary>
public class NetCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            Destroy(other.gameObject);
            UniversalManager.Instance.totalNumOfPlastic--;
            UniversalManager.Instance.numOfPlasticRecycled++;
        }
    }
}
