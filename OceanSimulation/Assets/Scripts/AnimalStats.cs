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

    protected int endOfFertility;
    protected float peakOfFertility;

    // Start is called once the gameObject is created, right before the first frame update
    void Start()
    {
        endOfFertility = ageOfDeath - 1;
        peakOfFertility = (ageOfMaturity + ageOfDeath) * 0.5f;
    }

    // FixedUpdate is called 50 times per second, mainly used for physics calculations. Number of frames is independent of user's hardware.
    private void FixedUpdate()
    {
        if (hunger >= 20 || age >= ageOfDeath)
        {
            Destroy(gameObject);
        }

        // Hunger increases by 1 every second while age increases by 1 every 0.5 mins.
        hunger += Time.deltaTime;
        age += 0.033f * Time.deltaTime;

        if (age > ageOfMaturity && age < endOfFertility)
        {
            reproductiveUrge += (-0.65f * Time.deltaTime) * (age - ageOfMaturity) * (age - endOfFertility);
        }
    }
}
