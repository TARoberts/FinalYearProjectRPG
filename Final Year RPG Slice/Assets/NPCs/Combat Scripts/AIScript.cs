using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
    public enum state { idle, look, chase, attack};
    public enum type { minion, monster, boss, other};
    public state AIState = state.idle;
    public type monsterType = type.minion;
    public float distance;

    private GameObject _player;
    private int _playerHP = 5;
    private bool _invunerablePlayer = false;
    //[SerializeField] player_combat combat;
    [SerializeField] Rigidbody body;
    private Animator animator;

    public float detectRange, chaseRange;
    

    public float speed = 3;
    private bool _canAttack = true;
    private bool _patrolling = true;
    public float range;

    private bool _animationLocked = false;

    public float specialTimer1 = 20, specialTimer2 = 30;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player");
        distance = 2000;
        body = GetComponent<Rigidbody>();
        Vector3 toOther = _player.transform.position - transform.position;
        

        if (detectRange == 0 || chaseRange == 0 || specialTimer1 == 0 || specialTimer2 == 0)
        {
            switch (monsterType)
            {
                case type.minion:
                    detectRange = 18f;
                    chaseRange = 15f;
                    specialTimer1 = 0f;
                    specialTimer2 = 0f;
                    Debug.Log("Undeclared variables, using Minion Defaults");
                    break;

                case type.monster:
                    detectRange = 15f;
                    chaseRange = 10f;
                    specialTimer1 = 20f;
                    specialTimer2 = 0f;
                    Debug.Log("Undeclared variables, using Monster Defaults");
                    break;

                case type.boss:
                    detectRange = 20f;
                    chaseRange = 18f;
                    specialTimer1 = 20f;
                    specialTimer2 = 30f;
                    Debug.Log("Undeclared variables, using Boss Defaults");
                    break;

                default:

                    Debug.Log("Monster type not assigned, cannot use defaults");
                    break;

            }
        }
        

    }
    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(_player.transform.position, this.transform.position);
        if (_playerHP <= 0 || _animationLocked)
        {
            distance = 2000;
        }

        if (distance > detectRange)
        {
            AIState = state.idle;
            animator.SetBool("Idle", false);
            animator.SetBool("Moving", false);
            
        }

        else if (distance > chaseRange && distance <= detectRange)
        {
            AIState = state.look;
            animator.SetBool("Moving", false);
            animator.SetBool("Idle", true);
        }

        else if (distance > range && distance <= chaseRange)
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
            animator.SetBool("Moving", false);
            animator.SetFloat("Speed", 0);
        }

        else if (AIState == state.look)
        {
            Vector3 relativePos = _player.transform.position - transform.position;

            relativePos.y = 0;
            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.1f);
            animator.SetFloat("Speed", 0);
        }

        else if (AIState == state.chase)
        {
            float step = speed * Time.deltaTime;
            Vector3 relativePos = _player.transform.position - transform.position;

            relativePos.y = 0;
            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.1f);

            if (transform.rotation == rotation)
            {
                Vector3 newPos = Vector3.MoveTowards(transform.position, _player.transform.position, step);

                newPos.y = transform.position.y;

                transform.position = newPos;
                animator.SetFloat("Speed", speed);
            }         
        }

        else if (AIState == state.attack)
        {

            animator.SetBool("Moving", false);
            animator.SetFloat("Speed", 0);
            

            float step = speed * Time.deltaTime;
            Vector3 relativePos = _player.transform.position - transform.position;

            relativePos.y = 0;
            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.1f);

            if (transform.rotation == rotation)
            {
                if (_canAttack && _playerHP > 0)
                {
                    if (monsterType == type.minion)
                    {
                        _canAttack = false;
                        animator.SetBool("Attack", true);
                        StartCoroutine(MinionAttack());
                    }

                    else if (monsterType == type.monster)
                    {
                        MonsterAttacks();
                    }

                    else if (monsterType == type.boss)
                    {
                        BossAttacks();
                    }
                }
            }

        }

        if (AIState != state.idle)
        {
            if (monsterType == type.monster && specialTimer1 > 0)
            {
                specialTimer1 -= Time.deltaTime;
            }

            else if (monsterType == type.boss)
            {
                if (specialTimer1 > 0)
                {
                    specialTimer1 -= Time.deltaTime;
                }

                if (specialTimer2 > 0)
                {
                    specialTimer2 -= Time.deltaTime;
                }
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
    IEnumerator MinionAttack()
    {
        
        Vector3 toOther = _player.transform.position - transform.position;
        if (_invunerablePlayer == false)
        {
            //body.AddForce(toOther.normalized * -2.0f, ForceMode.Impulse);
            _playerHP = _playerHP - 1;
        }
        yield return new WaitForSeconds(0.25f);
        animator.SetBool("Attack", false);

        yield return new WaitForSeconds(2.0f);
        
        _canAttack = true;
    }

    IEnumerator patrol()
    {
        body.velocity = RandomVector(-5f, 5f);
        yield return new WaitForSeconds(2.5f);
        _patrolling = true;
        yield return null;
    }

    void MonsterAttacks()
    {
        _animationLocked = true;
        _canAttack = false;
        if (specialTimer1 <= 0)
        {
            //do special attack flurry
            specialTimer1 = 20f;
            _canAttack = true;
            _animationLocked = false;
        }
        else
        {
            
            animator.SetBool("Attack1", true);
            animator.SetBool("Attack2", false);
            Debug.Log("Bonk " + (animator.GetBool("Attack1")));
            StartCoroutine(MonsterNormalAttack());
        }
    }

    IEnumerator MonsterNormalAttack()
    {
        Vector3 toOther = _player.transform.position - transform.position;
        if (_invunerablePlayer == false)
        {
            //body.AddForce(toOther.normalized * -2.0f, ForceMode.Impulse);
            _playerHP = _playerHP - 2;
        }
        yield return new WaitForSeconds(0.25f);
        animator.SetBool("Attack1", false);

        yield return new WaitForSeconds(2.0f);

        _animationLocked = false;
        _canAttack = true;
    }

    IEnumerator MonsterSpecialAttackChain()
    {
        yield return null;
    }

    void BossAttacks()
    {
        _animationLocked = true;
        _canAttack = false;
        if (specialTimer1 <= 0)
        {
            specialTimer1 = 20f;
            //do special attack flurry
            _canAttack = true;
            _animationLocked = false;
        }
        else
        {
            animator.SetBool("Attack1", true);
            StartCoroutine(BossNormalAttack());
        }
    }

    IEnumerator BossNormalAttack()
    {
        Vector3 toOther = _player.transform.position - transform.position;
        if (_invunerablePlayer == false)
        {
            //body.AddForce(toOther.normalized * -2.0f, ForceMode.Impulse);
            _playerHP = _playerHP - 3;
        }
        yield return new WaitForSeconds(0.25f);
        animator.SetBool("Attack1", false);

        yield return new WaitForSeconds(2.0f);

        _animationLocked = false;
        _canAttack = true;
    }

    IEnumerator BossSpecialAttackChain()
    {
        yield return null;
    }

    IEnumerator BossSpecialAttackFlame()
    {
        yield return null;
    }
}
