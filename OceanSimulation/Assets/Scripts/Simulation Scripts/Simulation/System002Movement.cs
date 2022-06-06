using System.Collections;
using UnityEngine;

/// <summary>
/// A class that manages the movement of the System 002 object.
/// </summary>
public class System002Movement : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float actionDelay;
    public int xIncrement;
    public int lastMoveX;
    public int minZ;
    public int maxZ;
    public Vector3 finalPos;
    public AudioSource _boatHorn;
    public InfoMenu infoMenu;

    int zDifference;

    private void OnEnable()
    {
        zDifference = maxZ - minZ;
        StartCoroutine(movementPattern());
        _boatHorn.Play();
        infoMenu.toggleSystem002State();
    }

    /// <summary>
    /// Creates the back and forth sweeping pattern of the system002 by using,
    /// for loops with the move() function inside them.
    /// </summary>
    /// <returns> An IEnumerator that makes the function run over multiple frames. </returns>
    IEnumerator movementPattern()
    {
        for (int _x = -4; _x < lastMoveX; _x += xIncrement)
        {
            for (int _z = minZ; _z <= maxZ; _z += zDifference)
            {
                yield return move(new Vector3(_x, transform.position.y, _z));
            }

            _x += xIncrement;

            for (int _z = maxZ; _z >= minZ; _z -= zDifference)
            {
                yield return move(new Vector3(_x, transform.position.y, _z));
            }
        }

        yield return move(finalPos);
        infoMenu.toggleSystem002State();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Rotates the boat to face towards its destination, then moves it towards that destination.
    /// </summary>
    /// <param name="destination"> The destination that the boat is moving towards. </param>
    /// <returns> An IEnumerator that makes the function run over multiple frames. </returns>
    IEnumerator move(Vector3 destination)
    {
        Quaternion direction = Quaternion.LookRotation((destination - transform.position));
        if (transform.rotation != direction)
        {
            yield return new WaitForSeconds(actionDelay);

            while (transform.rotation != direction)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, direction, rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        yield return new WaitForSeconds(actionDelay);

        while (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }
    }
}
