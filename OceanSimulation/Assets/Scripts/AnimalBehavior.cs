using UnityEngine;

public class AnimalBehavior : MonoBehaviour
{
    public Transform tf;
    public Rigidbody rb;
    public AnimalStats stats;

    public enum behaviorState
    {
        movingToTarget,
        wandering,
        searchingFood,
        eating,
        searchingMate,
        mating,
    }
    public behaviorState currentBehavior = behaviorState.searchingFood;

    public Vector3 targetLocation;

    // Update is called once per frame. Number of frames is dependent on the user's hardware.
    void Update()
    {
        switch (currentBehavior)
        {
            case behaviorState.movingToTarget:
                tf.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((targetLocation - tf.position).normalized), stats.rotationSpeed);
                tf.position += (targetLocation - tf.position).normalized * stats.speed;
                if (tf.position.Round(0) == targetLocation.Round(0))
                {
                    currentBehavior = behaviorState.eating;
                }

                break;
            case behaviorState.wandering:
                break;
            case behaviorState.searchingFood:
                targetLocation = searchForEntity(1 << 8, "Food");
                currentBehavior = behaviorState.movingToTarget;

                break;
            case behaviorState.eating:
                stats.hunger = Mathf.Clamp(stats.hunger - 3 * Time.deltaTime, 0, 20);
                break;
            case behaviorState.searchingMate:
                break;
            case behaviorState.mating:
                break;
            default:
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Eating");
        if (collision.collider.tag == "Food")
        {
            currentBehavior = behaviorState.eating;
        }
        else if (collision.gameObject.layer == 0)
        {
            targetLocation = randomTargetLocation();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exited");
        if (currentBehavior == behaviorState.eating)
        {
            //Make a funciton that decides what state the fish should be in next. this statement below is a placeholder.
            currentBehavior = behaviorState.searchingFood;
        }
    }

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

    Vector3 randomTargetLocation()
    {
        return tf.position + new Vector3(
            tf.forward.x + Random.Range(-2f, 2f),
            tf.forward.y + Random.Range(-1f, 1f),
            tf.forward.z + Random.Range(-2f, 2f)
            ).normalized * stats.sightRange;
    }
}
