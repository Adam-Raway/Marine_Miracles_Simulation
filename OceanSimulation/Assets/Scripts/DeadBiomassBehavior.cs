using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBiomassBehavior : MonoBehaviour
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
            if (Random.value >= 0.5f) KelpManager.Instance.spawnKelp(transform.position.x, transform.position.z);
            Destroy(gameObject);
        }

        currentHealth -= fishInContact * Time.deltaTime;
        if (currentHealth <= 0 && !healthDepleted)
        {
            deactivationAlertCollider.SetActive(true);
            healthDepleted = true;
        }
    }
}
