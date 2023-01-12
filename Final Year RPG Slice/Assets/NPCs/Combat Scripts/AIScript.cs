using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
    public enum state { idle, look, chase, attack};
    public enum type { minon, monster, boss, other};
    public state AIState = state.idle;
    public float distance;
    private GameObject player;
    private int HP = 5;
    private bool iFrame = false;
    //[SerializeField] player_combat combat;
    [SerializeField] Rigidbody body;
    private Animator animator;
    

    public float speed = 3;
    private bool canAttack = true;
    private bool patrolling = true;
    public float range;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        distance = 2000;
        body = GetComponent<Rigidbody>();
        Vector3 toOther = player.transform.position - transform.position;
        body.AddForce(toOther.normalized * -2.0f, ForceMode.Impulse);
    }
    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(player.transform.position, this.transform.position);
        if (HP == 0)
        {
            distance = 2000;
        }

        if (distance > 18.0f)
        {
            AIState = state.idle;
            animator.SetBool("Spotted_Enemy", false);
            animator.SetBool("Moving", false);
        }

        else if (distance > 15f && distance <= 18.0f)
        {
            AIState = state.look;
            animator.SetBool("Spotted_Enemy", true);
            animator.SetBool("Moving", false);
        }

        else if (distance > range && distance <= 15f)
        {
            AIState = state.chase;
            animator.SetBool("Moving", true);
        }

        else if (distance <= range)
        {
            AIState = state.attack;
            animator.SetBool("Moving", false);
        }

        if (AIState == state.idle)
        {
            //if (patrolling)
            //{
            //    patrolling = false;
            //    StartCoroutine(patrol());
            //} 
        }

        else if (AIState == state.look)
        {
            Vector3 relativePos = player.transform.position - transform.position;

            relativePos.y = 0;
            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.1f);
        }

        else if (AIState == state.chase)
        {
            float step = speed * Time.deltaTime;
            Vector3 relativePos = player.transform.position - transform.position;

            relativePos.y = 0;
            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.1f);

            Vector3 newPos = Vector3.MoveTowards(transform.position, player.transform.position, step);

            newPos.y = transform.position.y;

            transform.position = newPos;
        }

        else if (AIState == state.attack)
        {
            if (canAttack && HP > 0)
            {
                canAttack = false;
                animator.SetBool("Attack", true);
                StartCoroutine(attack());
            }
        }
    }

    private Vector3 RandomVector(float min, float max)
    {
        var x = Random.Range(min, max);
        var y = 0;
        var z = Random.Range(min, max);
        return new Vector3(x, y, z);
    }
    IEnumerator attack()
    {
        
        Vector3 toOther = player.transform.position - transform.position;
        if (iFrame == false)
        {
            //body.AddForce(toOther.normalized * -2.0f, ForceMode.Impulse);
            HP = HP - 1;
        }
        yield return new WaitForSeconds(0.25f);
        animator.SetBool("Attack", false);

        yield return new WaitForSeconds(2.0f);
        
        canAttack = true;
    }

    IEnumerator patrol()
    {
        body.velocity = RandomVector(-5f, 5f);
        yield return new WaitForSeconds(2.5f);
        patrolling = true;
        yield return null;
    }
}
