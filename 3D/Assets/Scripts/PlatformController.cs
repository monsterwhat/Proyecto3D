using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] Rigidbody platformRB;
    [SerializeField] Transform[] platformPositions;
    [SerializeField] float platformSpeed = 20;
    private int actualPosition = 0;
    private int nextPosition = 1;
    private bool moveToNext = true;
    private float waitTime = 2;

    void Update()
    {
        movePlatform();
    }

    void movePlatform()
    {
        if (moveToNext)
        {
            StopCoroutine(waitForMove(0));
            platformRB.MovePosition(Vector3.MoveTowards(platformRB.position, platformPositions[nextPosition].position, platformSpeed * Time.deltaTime));
        }
        if (Vector3.Distance(platformRB.position, platformPositions[nextPosition].position) <= 0)
        {
            StartCoroutine(waitForMove(waitTime));
            actualPosition = nextPosition;
            nextPosition++;
            if (nextPosition > platformPositions.Length - 1)
            {
                nextPosition = 0;
            }
        }
    }

    IEnumerator waitForMove(float time)
    {
        moveToNext = false;
        yield return new WaitForSeconds(time);
        moveToNext = true;
    }

}
