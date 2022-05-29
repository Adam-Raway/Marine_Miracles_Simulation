using System;
using UnityEngine;

public class AnimalStats : MonoBehaviour
{
    public enum sex
    {
        male,
        female
    }
    public sex gender;

    public float sightRange;
    public float speed;
    public float rotationSpeed;

    public int ageOfMaturity;
    public int ageOfDeath;

    [NonSerialized] public float hunger;
    [NonSerialized] public float reproductiveUrge;
    [NonSerialized] public float age;

    public GameObject deadMassPrefab;

    protected int endOfFertility;
    protected float peakOfFertility;
    protected string oppositeSex;

    // Start is called once the gameObject is created, right before the first frame update
    void Start()
    {
        hunger = 0;
        age = 0;
        reproductiveUrge = 40;

        endOfFertility = ageOfDeath - 1;
        peakOfFertility = (ageOfMaturity + ageOfDeath) * 0.5f;
        if (UnityEngine.Random.value < 0.5)
        {
            gender = sex.male;
            gameObject.tag = "Male";
            oppositeSex = "Female";
        }
        else
        {
            gender = sex.female;
            gameObject.tag = "Female";
            oppositeSex = "Male";
        }
    }

    // FixedUpdate is called 50 times per second, mainly used for physics calculations. Number of frames is independent of user's hardware.
    private void FixedUpdate()
    {
        if (hunger >= 45 || age >= ageOfDeath)
        {
            die();
        }

        // Hunger increases by 1 every second while age increases by 1 every 0.5 mins.
        hunger += Time.deltaTime;
        age += 0.022f * Time.deltaTime;

        if (age > ageOfMaturity && age < endOfFertility)
        {
            reproductiveUrge += (-0.65f * Time.deltaTime) * (age - ageOfMaturity) * (age - endOfFertility);
        }
    }

    /// <summary>
    /// Runs when the creature dies, spawns a skeleton and then destroys the fish game object.
    /// </summary>
    void die()
    {
        Instantiate(deadMassPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        Destroy(this.gameObject);
    }
}
