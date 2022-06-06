using UnityEngine;

/// <summary>
/// A class that manages the behavior of kelp objects; i.e. losing health over time
/// when in contact with a fish, alerting said fish of when the kelp is about to be
/// destroyed, and then destroying the kelp object when its health is depleted.
/// </summary>
public class KelpBehavior : MonoBehaviour
{
    public GameObject deactivationAlertCollider;
    public float health;
    public int fishInContact;

    private bool healthDepleted = false;
    private float currentHealth;

    private void OnEnable()
    {
        currentHealth = health;
        healthDepleted = false;
        fishInContact = 0;
    }

    private void Update()
    {
        fishInContact = Mathf.Clamp(fishInContact, 0, 5);
        if (healthDepleted)
        {
            gameObject.SetActive(false);
            KelpManager.Instance.deactivatedKelp.Add(gameObject);
        }

        currentHealth -= fishInContact * Time.deltaTime;
        if (currentHealth <= 0 && !healthDepleted)
        {
            deactivationAlertCollider.SetActive(true);
            healthDepleted = true;
        }
    }
}
