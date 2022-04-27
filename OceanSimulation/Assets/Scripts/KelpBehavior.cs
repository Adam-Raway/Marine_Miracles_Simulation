using System.Collections;
using UnityEngine;

public class KelpBehavior : MonoBehaviour
{
    private bool destroyKelpActive = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Eaten");
        if (!destroyKelpActive && other.gameObject.layer > 9 && other.gameObject.layer < 13)
        {
            StartCoroutine(destroyKelp());
            destroyKelpActive = true;
        }
    }

    IEnumerator destroyKelp()
    {
        yield return new WaitForSeconds(5f);
        transform.position = new Vector3(-1000, -1000, -1000);

        yield return null;
        Destroy(gameObject);
    }
}
