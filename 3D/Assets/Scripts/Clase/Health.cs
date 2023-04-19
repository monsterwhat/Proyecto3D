using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int lives = 3;
    public bool _protected = false;
    public float protectedTime = 1;
    public float waitTime = 0.2f;
    public bool dead = false;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Substract(int count)
    {
        if (!_protected && lives > 0)
        {
            lives -= count;
            animator.Play("Hit");
            StartCoroutine(Protect());
            StartCoroutine(StopVelocity());
        }
        if (!dead && lives == 0)
        {
            animator.Play("Dead");
        }
    }

    IEnumerator Protect()
    {
        _protected = true;
        yield return new WaitForSeconds(protectedTime);
        _protected = false;
    }

    IEnumerator StopVelocity()
    {
        var actualVelocity = GetComponent<Player>().playerSpeed;
        GetComponent<Player>().playerSpeed = 0;
        yield return new WaitForSeconds(waitTime);
        GetComponent<Player>().playerSpeed = actualVelocity;
    }

}
