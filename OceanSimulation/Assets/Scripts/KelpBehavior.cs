using System.Collections;
using UnityEngine;

public class KelpBehavior : MonoBehaviour
{
    public GameObject deactivationAlertCollider;
    public float health;
    public int fishInContact;

    private bool healthDepleted = false;

    private void Update()
    {
        fishInContact = Mathf.Clamp(fishInContact, 0, 5);
        if (healthDepleted) Destroy(gameObject);
        health -= fishInContact * Time.deltaTime;
        if (health <= 0 && !healthDepleted)
        {
            deactivationAlertCollider.SetActive(true);
            healthDepleted = true;
        }
    }
}
