using System.Collections;
using UnityEngine;

/// <summary>
/// A class that dictates the behavior of the crab object that it's attached to.
/// It can perform actions such as eating, breeding, searching for various entities,
/// and moving/wandering around. It has an AnimalStats script.
/// </summary>

[RequireComponent(typeof(AnimalStats))]
public class CrabBehavior : MonoBehaviour
{
    public Transform tf;
    public Rigidbody rb;
    public AnimalStats stats;
    public GameObject confirmedMate;

    private Vector3 targetLocation = Vector3.zero;
    private IEnumerator currentAction;

    private bool currentlyDoingSomething = false;
    private bool shouldBeDoingNothing = false;
    private bool canEat = true;
    private int foodLayerMask = (1 << 13);
    private int numberOfFoodSearches;
    private float doingNothingTimer;

    delegate bool attemptToMate(GameObject potentialMate);

    private void Start()
    {
        UniversalManager.Instance.totalPopulationOfCrabs++;
    }

    private void Update()
    {
        if (Vector3.Dot(tf.up, Vector3.down) > 0)
        {
            rb.velocity = Vector3.zero;
            changeCurrentAction();
            Vector3 unitRight = tf.position + tf.right;
            Roll(-unitRight.y + transform.position.y);
        }

        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        if (shouldBeDoingNothing)
        {
            doingNothingTimer += Time.deltaTime;
            rb.velocity = Vector3.zero;
        }

        if (doingNothingTimer > 10)
        {
            doingNothingTimer = 0;
            shouldBeDoingNothing = false;
            confirmedMate.GetComponent<CrabBehavior>().confirmedMate = null;
            confirmedMate = null;
        }

        if (numberOfFoodSearches >= 5)
        {
            changeCurrentAction(moveToDestination(randomTargetLocation()));
            numberOfFoodSearches = 0;
        }
        else if (!currentlyDoingSomething && !shouldBeDoingNothing)
        {
            decideNextAction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Deactivating")
        {
            decideNextAction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Decomposable")
        {
            if (other.gameObject.layer == 13) other.GetComponent<DeadBiomassBehavior>().fishInContact--;
            changeCurrentAction(moveToDestination(randomTargetLocation()));
            deactivateEating(3);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        int layer = collision.gameObject.layer;
        if (collision.collider.tag == "Decomposable" && stats.hunger > 5 && canEat)
        {
            collision.gameObject.GetComponent<DeadBiomassBehavior>().fishInContact++;
            changeCurrentAction(eating(collision.transform.position));
        }
        else if(layer < 13 && layer > 9)
        {
            rb.velocity = Vector3.zero;

            if (collision.gameObject == confirmedMate)
            {
                confirmedMate = null;
                stats.reproductiveUrge = 0;

                if (gameObject.tag == "Female")
                {
                    Instantiate(gameObject, tf.position + Vector3.right * 2, Quaternion.identity);

                    shouldBeDoingNothing = false;
                }
                currentlyDoingSomething = false;
                doingNothingTimer = 0;
            }
            else changeCurrentAction(moveToDestination(tf.position + -tf.forward, -1));
        }
        else if (collision.collider.tag == "Wall" && collision.gameObject.layer == 2)
        {
            rb.velocity = Vector3.zero;
            changeCurrentAction(moveToDestination(tf.position + -tf.forward, -1));
        }
    }

    /// <summary>
    /// Selects a random location within +or- 9 units of the current position in the x & z axis,
    /// then determines the y level of the terrain at that position.
    /// </summary>
    /// <param name="horizontalBias"> A multiplier for all the X & Z values of the Random.Range() function. </param>
    /// <returns>
    /// A Vector3 representing the randomly selected location (including the terrain-specific height).
    /// </returns>
    Vector3 randomTargetLocation(float horizontalBias = 1)
    {
        targetLocation = tf.position + new Vector3(
            tf.forward.x + Random.Range(-9f * horizontalBias, 9f * horizontalBias) / horizontalBias, 5.5f,
            tf.forward.z + Random.Range(-9f * horizontalBias, 9f * horizontalBias) / horizontalBias);
        RaycastHit hit;
        if (Physics.Raycast(targetLocation, Vector3.down, out hit, 5, 1 << 0)) targetLocation.y = hit.point.y + 0.223f;
        return targetLocation;
    }

    /// <summary>
    /// Searches for a specified object or entity within a certain radius.
    /// </summary>
    /// <param name="layersToSearch"> The layers that will be searched for by the Physics.OverlapSphere() </param>
    /// <param name="colliderTag"> The tag of the object that is being searched for. </param>
    /// <param name="attemptToMate"> Passes in and runs a function that follows the 'attemptToMate' delegate. </param>
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
                return randomTargetLocation();
            }

            if (attemptToMate != null)
            {
                if (attemptToMate(closestEntity))
                {
                    return closestEntityPos;
                }
                else
                {
                    return randomTargetLocation();
                }
            }

            if (colliderTag == "Decomposable") numberOfFoodSearches++;
            return closestEntityPos;
        }
        else
        {
            return randomTargetLocation();
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
            CrabBehavior mateBehaviorScript = potentialMate.GetComponent<CrabBehavior>();
            if (mateBehaviorScript.stats.reproductiveUrge >= 30 && mateBehaviorScript.stats.hunger < 700)
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
        while (secondsPassed < 7)
        {
            if (tf.rotation != direction) tf.rotation = Quaternion.Slerp(transform.rotation, direction, stats.rotationSpeed * Time.deltaTime);
            stats.hunger = Mathf.Clamp(stats.hunger - 20f, 0, 30);
            secondsPassed += Time.deltaTime;
            yield return null;
        }
        currentlyDoingSomething = false;
    }

    /// <summary>
    /// Moves the crab towards a target location over the course of multiple frames.
    /// </summary>
    /// <param name="destination"> The destination that the creature is moving towards. </param>
    /// <param name="movementDirection">
    /// An optional parameter that if set to -1 will cause the crab to move straight backwards (in its -Z direction).
    /// </param>
    /// <returns>
    /// An IEnumerator that makes the function run over multiple frames.
    /// </returns>
    IEnumerator moveToDestination(Vector3 destination, int movementDirection = 1)
    {
        destination.y = tf.position.y;
        Quaternion direction = Quaternion.LookRotation((destination - tf.position).normalized);
        if (movementDirection == 1)
        {
            while (tf.rotation != direction && tf.position.Round() != destination.Round())
            {
                tf.rotation = Quaternion.Slerp(transform.rotation, direction, stats.rotationSpeed * Time.deltaTime);
                tf.position += tf.forward * stats.speed * Time.deltaTime;
                destination.y = tf.position.y;
                direction = Quaternion.LookRotation((destination - tf.position).normalized);
                yield return null;
            }
        }
        while (tf.position.Round(1) != destination.Round(1))
        {
            tf.position += tf.forward * stats.speed * Time.deltaTime * movementDirection;
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
    /// Decides what action the creature should perform next.
    /// These actions include searching for food, for a mate, or aimlessly wandering around.
    /// </summary>
    void decideNextAction()
    {
        rb.velocity = Vector3.zero;
        if (stats.hunger > 30)
        {
            changeCurrentAction(moveToDestination(searchForEntity(foodLayerMask, "Decomposable")));
        }
        else if (stats.reproductiveUrge >= 15 && gameObject.tag == "Male" && confirmedMate == null)
        {
            changeCurrentAction(moveToDestination(searchForEntity(1 << gameObject.layer, "Female", tryToMate)));
        }
        else if (stats.hunger > 5)
        {
            changeCurrentAction(moveToDestination(searchForEntity(foodLayerMask, "Decomposable")));
        }
        else
        {
            changeCurrentAction(moveToDestination(randomTargetLocation()));
        }
    }

    void Roll(float Amount)
    {
        rb.AddRelativeTorque(0f, 0f, Amount);
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