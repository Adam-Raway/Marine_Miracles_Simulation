using System;
using UnityEngine;

/// <summary>
/// A class that stores all the stats to do with an animal such as its hunger, age,
/// and reproductive urge. It also increases these stats over time and destroys the
/// animal object it is attached to if the hunger or age get too large.
/// </summary>
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
    public int maxHunger;

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
        reproductiveUrge = 0;

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
        if (hunger >= maxHunger || age >= ageOfDeath)
        {
            die();
        }

        hunger += Time.deltaTime;
        age += 0.066f * Time.deltaTime;

        if (age > ageOfMaturity && age < endOfFertility)
        {
            reproductiveUrge += (-0.65f * Time.deltaTime) * (age - ageOfMaturity) * (age - endOfFertility);
        }
    }

    /// <summary>
    /// Runs when the creature dies, spawns a skeleton and then destroys the fish game object.
    /// </summary>
    public void die()
    {
        Instantiate(deadMassPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        Destroy(this.gameObject);
    }
}
