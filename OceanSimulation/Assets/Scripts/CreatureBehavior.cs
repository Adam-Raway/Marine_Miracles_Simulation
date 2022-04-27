using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBehavior : MonoBehaviour
{
    IEnumerator currentAction;

    public Transform tf;
    public Rigidbody rb;
    public AnimalStats stats;

    public Vector3 targetLocation;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        while (tf.rotation != direction)
        {
            tf.rotation = Quaternion.Slerp(transform.rotation, direction, stats.rotationSpeed);
            tf.position = Vector3.MoveTowards(tf.position, destination, stats.speed * Time.deltaTime);
            yield return null;
        }

        while (tf.position.Round() != destination.Round())
        {
            tf.position = Vector3.MoveTowards(tf.position, destination, stats.speed * Time.deltaTime);
            yield return null;
        }
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

            return closestEntity;
        }
        else
        {
            return randomTargetLocation();
        }

    }

    /// <summary>
    /// Selects a random location within +or- 2 units of the current position in the x & z axis, and +or- 1 unit in the y axis.
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
}
