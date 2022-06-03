using System.Collections;
using UnityEngine;

/// <summary>
/// A class that dictates the behavior of the animal object that it's attached to.
/// It can perform actions such as eating, breeding, searching for various entities,
/// and moving/wandering around. It has an AnimalStats script.
/// </summary>

[RequireComponent(typeof(AnimalStats))]
public class AnimalBehavior : MonoBehaviour
{
    public Transform tf;
    public Rigidbody rb;
    public AnimalStats stats;
    public GameObject confirmedMate;

    private Vector3 targetLocation;
    private int numberOfFoodSearches;

    IEnumerator currentAction;
    bool currentlyDoingSomething = false;
    bool shouldBeDoingNothing = false;
    float doingNothingTimer;
    bool canEat = true;
    float downBias;

    int foodLayerMask = (1 << 8) | (1 << 9);

    delegate bool attemptToMate(GameObject potentialMate);

    private void Update()
    {
        if (shouldBeDoingNothing)
        {
            doingNothingTimer += Time.deltaTime;
        }

        if (doingNothingTimer > 7)
        {
            doingNothingTimer = 0;
            shouldBeDoingNothing = false;
            confirmedMate.GetComponent<AnimalBehavior>().confirmedMate = null;
            confirmedMate = null;
        }

        if (numberOfFoodSearches >= 5)
        {
            changeCurrentAction(moveToDestination(randomTargetLocation(downBias: downBias)));
            numberOfFoodSearches = 0;
        }
        else if (!currentlyDoingSomething && !shouldBeDoingNothing)
        {
            decideNextAction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
        {
            changeCurrentAction(moveToDestination((other.transform.position - tf.position).normalized * 2));
        }

        if (other.tag == "Food" && stats.hunger > 5 && canEat)
        {
            if (other.gameObject.layer == 8)
            {
                other.GetComponent<KelpBehavior>().fishInContact++;
                changeCurrentAction(eating(other.transform.position));
            } 
            else if (other.gameObject.layer == 9)
            {
                if (Random.value >= 0.75)
                {
                    Debug.Log("this fish died to plastic poisoning");
                    Destroy(other.gameObject);
                    UniversalStats.Instance.totalNumOfPlastic--;
                    stats.die();
                }
                else
                {
                    changeCurrentAction(eating(other.transform.position));
                }
            }
        }
        else if (other.tag == "Deactivating")
        {
           decideNextAction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Food")
        {
            if (other.gameObject.layer == 8) other.GetComponent<KelpBehavior>().fishInContact--;    
            changeCurrentAction(moveToDestination(randomTargetLocation(downBias: -0.25f, upBias: 2, horizontalBias: 2)));
            deactivateEating(3);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        int layer = collision.gameObject.layer;
        if (layer < 13 && layer > 9)
        {
            rb.velocity = Vector3.zero;

            if (collision.gameObject == confirmedMate)
            {
                confirmedMate = null;
                stats.reproductiveUrge = 0;

                if (gameObject.tag == "Female")
                {
                    Instantiate(gameObject, tf.position + Vector3.right, new Quaternion(0, 0, 0, 0));

                    shouldBeDoingNothing = false;
                }
                currentlyDoingSomething = false;
                doingNothingTimer = 0;
            }
            else
            {
                ContactPoint cp = collision.contacts[0];
                changeCurrentAction(moveToDestination(tf.position + cp.normal));
            }
        }
        else if (collision.collider.tag == "Wall")
        {
            Vector3 cp = collision.contacts[0].normal;
            changeCurrentAction(moveToDestination(tf.position + new Vector3(cp.x, 0.5f, cp.z)));
        }
        else if (collision.collider.tag == "Water")
        {
            Vector3 cp = collision.contacts[0].normal;
            downBias = 12;
            changeCurrentAction(moveToDestination(searchForEntity(foodLayerMask, "Food")));
        }
    }

    /// <summary>
    /// Reduces the creature's hunger every frame until it reaches 0 - the minimum value.
    /// </summary>
    /// <returns>
    /// An IEnumerator that makes the function run over multiple frames.
    /// </returns>
    IEnumerator eating(Vector3 foodLocation)
    {
        rb.velocity = Vector3.zero;
        Quaternion direction = Quaternion.LookRotation((foodLocation - tf.position).normalized);
        float secondsPassed = 0;
        while (secondsPassed < 3)
        {
            if (tf.rotation != direction) tf.rotation = Quaternion.Slerp(transform.rotation, direction, stats.rotationSpeed * Time.deltaTime);
            stats.hunger = Mathf.Clamp(stats.hunger - 0.066f, 0, 30);
            secondsPassed += Time.deltaTime;
            yield return null;
        }
        currentlyDoingSomething = false;
    }
    
    /// <summary>
    /// Moves the creature towards a target location over the course of multiple frames.
    /// </summary>
    /// <param name="destination"> The destination that the creature is moving towards. </param>
    /// <returns>
    /// An IEnumerator that makes the function run over multiple frames.
    /// </returns>
    IEnumerator moveToDestination(Vector3 destination)
    {
        Quaternion direction = Quaternion.LookRotation((destination - tf.position).normalized);
        while (tf.rotation != direction && tf.position.Round() != destination.Round())
        {
            tf.rotation = Quaternion.Slerp(transform.rotation, direction, stats.rotationSpeed * Time.deltaTime);
            tf.position += tf.forward * stats.speed * Time.deltaTime;
            direction = Quaternion.LookRotation((destination - tf.position).normalized);
            yield return null;
        }
        while (tf.position.Round(1) != destination.Round(1))
        {
            tf.position += tf.forward * stats.speed * Time.deltaTime;
            yield return null;
        }

        yield return null;
        currentlyDoingSomething = false;
    }

    /// <summary>
    /// Stops the creature's ability to eat then reactivates it after the specified amount of seconds.
    /// </summary>
    /// <param name="duration"> The duration that the deactivation should last for. </param>
    /// <returns>
    /// An IEnumerator that makes the function run over multiple frames.
    /// </returns>
    IEnumerator deactivateEating(float duration)
    {
        canEat = false;
        yield return new WaitForSeconds(duration);
        canEat = true;
    }

    /// <summary>
    /// Searches for a specified object or entity within a certain radius.
    /// </summary>
    /// <param name="layersToSearch"> The layers that will be searched for by the Physics.OverlapSphere() </param>
    /// <param name="colliderTag"> The tag of the object that is being searched for. </param>
    /// /// <param name="attemptToMate"> Passes in and runs a function that follows the 'attemptToMate' delegate. </param>
    /// <returns>
    /// A Vector3 representing the location of a target object, or a random location if no objects are found.
    /// </returns>
    Vector3 searchForEntity(LayerMask layersToSearch, string colliderTag, attemptToMate attemptToMate = null)
    {
        GameObject closestEntity = null;
        Vector3 closestEntityPos = new Vector3(0, 0, 0);
        float closestEntityDistance = 0;
        RaycastHit hit;
        Collider[] entityColliders = Physics.OverlapSphere(tf.position, stats.sightRange, layersToSearch);

        if (entityColliders.Length > 0)
        {
            foreach (Collider entity in entityColliders)
            {
                Vector3 currentEntity = entity.gameObject.transform.position;
                float entityDistance = (currentEntity - tf.position).sqrMagnitude;
                Ray rayToEntity = new Ray(tf.position, (currentEntity - tf.position).normalized);

                if (entityDistance < closestEntityDistance || closestEntityDistance == 0)
                {
                    Debug.DrawRay(tf.position, rayToEntity.direction);
                    if (Physics.Raycast(rayToEntity, out hit, stats.sightRange * 5))
                    {
                        if (hit.collider.tag == colliderTag)
                        {
                            closestEntityDistance = entityDistance;
                            closestEntityPos = currentEntity;
                            closestEntity = entity.gameObject;
                        }
                    }
                }
            }

            if (closestEntityDistance == 0)
            {
                return randomTargetLocation(downBias: downBias);
            }

            if (attemptToMate != null)
            {
                if(attemptToMate(closestEntity))
                {
                    return closestEntityPos;
                }
                else
                {
                    return randomTargetLocation(downBias: downBias);
                }
            }

            if (colliderTag == "Food") numberOfFoodSearches++;
            return closestEntityPos;
        }
        else
        {
            return randomTargetLocation(downBias: downBias);
        }

    }

    /// <summary>
    /// Tries to mate with another creature. If the random chance is great enough, then the other creature
    /// stops moving and this functions sets both of them as each other's confirmedMate.
    /// </summary>
    /// /// <param name="potentialMate"> The creature gameObject that this creature is trying to mate with. </param>
    /// <returns>
    /// True if the random values is greater than 0.3 and the mating is confirmed, false otherwise.
    /// </returns>
    bool tryToMate(GameObject potentialMate)
    {
        if (Random.value < 0.3) return false;
        else
        {
            AnimalBehavior mateBehaviorScript = potentialMate.GetComponent<AnimalBehavior>();
            if (mateBehaviorScript.stats.reproductiveUrge >= 30 && mateBehaviorScript.stats.hunger < 30)
            {
                mateBehaviorScript.changeCurrentAction();
                mateBehaviorScript.shouldBeDoingNothing = true;
                mateBehaviorScript.confirmedMate = gameObject;
                confirmedMate = potentialMate;
                return true;
            }
            else return false;
        }
    }

    /// <summary>
    /// Selects a random location within +or- 2 units of the current position
    /// in the x & z axis, and +or- 1 unit in the y axis.
    /// </summary>
    /// /// <param name="downBias"> A multiplier for the minimum value of the Random.Range() function. </param>
    /// /// <param name="upBias"> A multiplier for the maximum value of the Random.Range() function. </param>
    /// /// <param name="horizontalBias"> A multiplier for all the X & Z values of the Random.Range() function. </param>
    /// <returns>
    /// A Vector3 representing the randomly selected location.
    /// </returns>
    Vector3 randomTargetLocation(float downBias = 1, float upBias = 1, float horizontalBias = 1)
    {
        return tf.position + new Vector3(
            tf.forward.x + Random.Range(-2f * horizontalBias, 2f * horizontalBias) / horizontalBias,
            tf.forward.y + Random.Range(-0.25f * downBias, 0.25f * upBias) / ((downBias + upBias) * 0.5f),
            tf.forward.z + Random.Range(-2f * horizontalBias, 2f * horizontalBias) / horizontalBias
            ).normalized * stats.sightRange;
    }

    /// <summary>
    /// Decides what action the creature should perform next.
    /// These actions include searching for food, for a mate, or aimlessly wandering around.
    /// </summary>
    void decideNextAction()
    {
        downBias = ExtensionMethods.Map(stats.hunger, 0, 30, 1, 25);

        if (stats.hunger > 30)
        {
            changeCurrentAction(moveToDestination(searchForEntity(foodLayerMask, "Food")));
        }
        else if (stats.reproductiveUrge >= 15 && gameObject.tag == "Male" && confirmedMate == null)
        {
            changeCurrentAction(moveToDestination(searchForEntity(1 << gameObject.layer, "Female", tryToMate)));
        }
        else if (stats.hunger > 5)
        {
            changeCurrentAction(moveToDestination(searchForEntity(foodLayerMask, "Food")));
        }
        else
        {
            changeCurrentAction(moveToDestination(randomTargetLocation(downBias: downBias)));
        }
    }

    /// <summary>
    /// Stops whatever action the creature is currently performing,then starts a new action if one is provided.
    /// </summary>
    /// <param name="actionToBegin"> The new action to perform once the old one is terminated. </param>
    public void changeCurrentAction(IEnumerator actionToBegin = null)
    {
        currentlyDoingSomething = false;
        if (currentAction != null)
        {
            StopCoroutine(currentAction);
        }

        if (actionToBegin != null)
        {
            currentlyDoingSomething = true;
            currentAction = actionToBegin;
            StartCoroutine(currentAction);
        }
    }
}
