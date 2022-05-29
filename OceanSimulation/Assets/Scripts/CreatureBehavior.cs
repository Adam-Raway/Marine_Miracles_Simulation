using System.Collections;
using UnityEngine;

public class CreatureBehavior : MonoBehaviour
{
    IEnumerator currentAction;
    bool currentlyDoingSomething = false;

    public Transform tf;
    public Rigidbody rb;
    public AnimalStats stats;

    private Vector3 targetLocation;
    private int numberOfFoodSearches;

    private void Update()
    {
        if (numberOfFoodSearches >= 5)
        {
            changeCurrentAction(moveToDestination(randomTargetLocation()));
            numberOfFoodSearches = 0;
        }
        else if (!currentlyDoingSomething)
        {
            decideNextAction();
        }

        /*
        float rotationX = Mathf.Clamp(tf.rotation.eulerAngles.x, 0, 360);
        float rotationY = Mathf.Clamp(tf.rotation.eulerAngles.y, 0, 360);
        float rotationZ = Mathf.Clamp(tf.rotation.eulerAngles.z, 0, 360);
        tf.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
        {
            changeCurrentAction(moveToDestination((other.transform.position - tf.position).normalized * 2));
        }

        if (other.tag == "Food" && stats.hunger > 5) changeCurrentAction(eating(other.transform.position));
        else if (other.tag == "Deactivating") decideNextAction();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Food") decideNextAction();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            ContactPoint cp = collision.contacts[0];
            changeCurrentAction(moveToDestination(tf.position + cp.normal));
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
        while (true)
        {
            rb.velocity = Vector3.zero;
            tf.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((foodLocation - tf.position).normalized), stats.rotationSpeed * Time.deltaTime);
            stats.hunger = Mathf.Clamp(stats.hunger - 0.066f, 0, 30);
            yield return null;
        }
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

        yield return null;
        currentlyDoingSomething = false;
    }

    /// <summary>
    /// Searches for a specified object or entity within a certain radius.
    /// </summary>
    /// <param name="layersToSearch"> The layers that will be searched for by the Physics.OverlapSphere() </param>
    /// <param name="colliderTag"> The tag of the object that is being searched for. </param>
    /// <returns>
    /// A Vector3 representing the location of a target object, or a random location if no objects are found.
    /// </returns>
    Vector3 searchForEntity(LayerMask layersToSearch, string colliderTag)
    {
        Vector3 closestEntity = new Vector3(0, 0, 0);
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
                            closestEntity = currentEntity;
                        }
                    }
                }
            }

            if (closestEntityDistance == 0)
            {
                return randomTargetLocation();
            }

            numberOfFoodSearches++;
            return closestEntity;
        }
        else
        {
            return randomTargetLocation();
        }

    }

    /// <summary>
    /// Selects a random location within +or- 2 units of the current position
    /// in the x & z axis, and +or- 1 unit in the y axis.
    /// </summary>
    /// <returns>
    /// A Vector3 representing the randomly selected location.
    /// </returns>
    Vector3 randomTargetLocation()
    {
        return tf.position + new Vector3(
            tf.forward.x + Random.Range(-2f, 2f),
            tf.forward.y + Random.Range(-1f, 1f),
            tf.forward.z + Random.Range(-2f, 2f)
            ).normalized * stats.sightRange;
    }

    /// <summary>
    /// Decides what action the creature should perform next.
    /// These actions include searching for food, for a mate, or aimlessly wandering around.
    /// </summary>
    void decideNextAction()
    {
        if (stats.hunger > 15)
        {
            changeCurrentAction(moveToDestination(searchForEntity(1 << 8, "Food")));
        }
        else if (stats.reproductiveUrge > 40)
        {
            // Figure out mating system
        }
        else if (stats.hunger > 5)
        {
            changeCurrentAction(moveToDestination(searchForEntity(1 << 8, "Food")));
        }
        else
        {
            changeCurrentAction(moveToDestination(randomTargetLocation()));
        }
    }

    /// <summary>
    /// Stops whatever action the creature is currently performing,then starts a new action if one is provided.
    /// </summary>
    /// <param name="actionToBegin"> The new action to perform once the old one is terminated. </param>
    void changeCurrentAction(IEnumerator actionToBegin = null)
    {
        currentlyDoingSomething = true;

        if (currentAction != null)
        {
            StopCoroutine(currentAction);
        }

        if (actionToBegin != null)
        {
            currentAction = actionToBegin;
            StartCoroutine(currentAction);
        }
    }
}
