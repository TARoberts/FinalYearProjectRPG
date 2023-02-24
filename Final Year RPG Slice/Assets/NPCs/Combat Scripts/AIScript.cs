using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
    public enum state { idle, look, chase, attack, dead};
    public enum type { minion, monster, boss, other};
    public state AIState = state.idle;
    public type monsterType = type.minion;
    public float distance;

    public Transform spawnPoint;
    [SerializeField] private float _respawnTime = 5f;

    private GameObject _player;
    private int _playerHP = 200;
    private bool _invunerablePlayer = false;
    //[SerializeField] player_combat combat;
    [SerializeField] Rigidbody body;
    private Animator animator;
    [SerializeField] private GameObject _wing1Collider, _wing2Collider;
    [SerializeField] private GameObject _JawCollider;

    public float detectRange, chaseRange;
    

    public float speed;
    public float turnSpeed;
    private bool _canAttack = true;
    private bool _patrolling = true;
    public float range;
    public float attackDelay;

    private bool _animationLocked = false;

    public float specialTimer1, specialTimer2;

    public int monsterHP;
    private int _monHP;

    private bool _respawning = false;

    [SerializeField] private PlayFire _specialAttackScript;
    [SerializeField] private Difficulty_Manager manager;



    private void Start()
    {
        animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player");
        distance = 2000;
        body = GetComponent<Rigidbody>();
        Vector3 toOther = _player.transform.position - transform.position;
        

        if (detectRange == 0 || chaseRange == 0 || specialTimer1 == 0 || specialTimer2 == 0 || monsterHP == 0)
        {
            switch (monsterType)
            {
                case type.minion:
                    detectRange = 18f;
                    chaseRange = 15f;
                    specialTimer1 = 0f;
                    specialTimer2 = 0f;
                    monsterHP = 10;
                    Debug.Log("Undeclared variables, using Minion Defaults");
                    break;

                case type.monster:
                    detectRange = 15f;
                    chaseRange = 10f;
                    specialTimer1 = 20f;
                    specialTimer2 = 0f;
                    monsterHP = 30;
                    Debug.Log("Undeclared variables, using Monster Defaults");
                    break;

                case type.boss:
                    detectRange = 20f;
                    chaseRange = 18f;
                    specialTimer1 = 20f;
                    specialTimer2 = 30f;
                    monsterHP = 50;
                    Debug.Log("Undeclared variables, using Boss Defaults");
                    break;

                default:

                    Debug.Log("Monster type not assigned, cannot use defaults");
                    break;

            }
        }
        
        if (manager == null)
        {
            manager = GameObject.FindGameObjectWithTag("Difficulty_Manager").GetComponent<Difficulty_Manager>();
        }

        _monHP = monsterHP;
        
        GetStats();
        
    }

    public void GetStats()
    {
        monsterHP =  (int)(monsterHP * manager.hitPointsModifier);
        turnSpeed = turnSpeed * manager.turnRateModifier;
        attackDelay = attackDelay * manager.recoveryTime;
        detectRange = detectRange * manager.aggressionRange;
        Debug.Log(detectRange);
        chaseRange = chaseRange * manager.aggressionRange;
        Debug.Log(chaseRange);
        specialTimer1 = manager.specialAttackCooldown;
        specialTimer2 = manager.specialAttackCooldown + 10f;

        Debug.Log("Updated Stats");
    }
    // Update is called once per frame
    void Update()
    {
        _playerHP = _player.GetComponent<PlayerStats>().playerHealthValue;

        if (monsterHP <= 0)
        {
            
            _animationLocked = true;
            AIState = state.dead;
            if (!_respawning)
            {
                OnDeath();
            }
            
            _respawning = true;
        }

        distance = Vector3.Distance(_player.transform.position, this.transform.position);
        if (_playerHP <= 0)
        {
             distance = 2000;
        }

        if (manager.updateReady)
        {
            manager.updateReady = false;
            GetStats();
        }


        if (!_animationLocked)
        {
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
        }

        
        #region OldStateMachine
            //if (AIState == state.idle)
            //{
            //    //if (patrolling)
            //    //{
            //    //    patrolling = false;
            //    //    StartCoroutine(patrol());
            //    //} 
            //    animator.SetBool("Moving", false);
            //    animator.SetFloat("Speed", 0);
            //}

            //else if (AIState == state.look)
            //{
            //    Vector3 relativePos = _player.transform.position - transform.position;

            //    relativePos.y = 0;
            //    // the second argument, upwards, defaults to Vector3.up
            //    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            //    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.1f);
            //    animator.SetFloat("Speed", 0);
            //}

            //else if (AIState == state.chase)
            //{
            //    float step = speed * Time.deltaTime;
            //    Vector3 relativePos = _player.transform.position - transform.position;

            //    relativePos.y = 0;
            //    // the second argument, upwards, defaults to Vector3.up
            //    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            //    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.1f);

            //    if (transform.rotation == rotation)
            //    {
            //        Vector3 newPos = Vector3.MoveTowards(transform.position, _player.transform.position, step);

            //        newPos.y = transform.position.y;

            //        transform.position = newPos;
            //        animator.SetFloat("Speed", speed);
            //    }         
            //}

            //else if (AIState == state.attack)
            //{

            //    animator.SetBool("Moving", false);
            //    animator.SetFloat("Speed", 0);


            //    float step = speed * Time.deltaTime;
            //    Vector3 relativePos = _player.transform.position - transform.position;

            //    relativePos.y = 0;
            //    // the second argument, upwards, defaults to Vector3.up
            //    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            //    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 0.1f);

            //    if (transform.rotation == rotation && !_animationLocked)
            //    {
            //        if (_canAttack && _playerHP > 0)
            //        {
            //            if (monsterType == type.minion)
            //            {
            //                _canAttack = false;
            //                animator.SetBool("Attack", true);
            //                StartCoroutine(MinionAttack());
            //            }

            //            else if (monsterType == type.monster)
            //            {
            //                MonsterAttacks();
            //            }

            //            else if (monsterType == type.boss)
            //            {
            //                BossAttacks();
            //            }
            //        }
            //    }

            //}
            #endregion
        
        Vector3 relativePos = _player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        float step = speed * Time.deltaTime;

        switch (AIState)
        {
            case state.idle:
                animator.SetBool("Moving", false);
                animator.SetFloat("Speed", 0);
                break;

            case state.look:
                relativePos.y = 0;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turnSpeed);
                animator.SetFloat("Speed", 0);
                break;

            case state.chase:
                relativePos.y = 0;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turnSpeed);
                if (transform.rotation == rotation)
                {
                    Vector3 newPos = Vector3.MoveTowards(transform.position, _player.transform.position, step);

                    newPos.y = transform.position.y;

                    transform.position = newPos;
                    animator.SetFloat("Speed", speed);
                }
                break;

            case state.attack:

                animator.SetBool("Moving", false);
                animator.SetBool("Idle", false);
                animator.SetFloat("Speed", 0);
                relativePos.y = 0;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turnSpeed);

                if (transform.rotation == rotation && !_animationLocked)
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
                break;

            case state.dead:

                animator.SetBool("Dead", true);
                animator.SetBool("Moving", false);
                animator.SetFloat("Speed", 0f);
                break;

        }

        if (AIState != state.idle && AIState != state.dead)
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

    #region MonsterFunctions

    void MonsterAttacks()
    {
        _animationLocked = true;
        _canAttack = false;
        if (specialTimer1 <= 0)
        {
            //do special attack flurry
            specialTimer1 = 20f;
            StartCoroutine(MonsterSpecialAttackChain());
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
        animator.SetBool("Attack1", true);
        
        yield return new WaitForSeconds(2f);
        animator.SetBool("Attack2", true);
        animator.SetBool("Attack1", false);
        yield return new WaitForSeconds(2f);
        animator.SetBool("Attack1", true);
        animator.SetBool("Attack2", false);
        yield return new WaitForSeconds(2f);
        animator.SetBool("Attack2", true);
        animator.SetBool("Attack1", false);
        yield return new WaitForSeconds(2f);
        animator.SetBool("Attack2", false);
        _canAttack = true;
        _animationLocked = false;
    }
    #endregion

    void BossAttacks()
    {
        _animationLocked = true;
        _canAttack = false;
        if (specialTimer1 <= 0)
        {
            specialTimer1 = 20f;
            StartCoroutine(BossSpecialAttackChain());

        }

        else if (specialTimer2 <= 0)
        {
            if (specialTimer1 < 5)
            {
                specialTimer1 = 5f;
            }
            specialTimer2 = 30f;
            StartCoroutine(BossSpecialAttackFlame());

        }
        else
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Attack1", true);
            
            StartCoroutine(BossNormalAttack());
        }
    }

    IEnumerator BossNormalAttack()
    {

        _JawCollider.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Attack1", false);
        
        yield return new WaitForSeconds(2.0f);
        _JawCollider.SetActive(false);
        yield return new WaitForSeconds(attackDelay);
        _animationLocked = false;
        _canAttack = true;
    }

    IEnumerator BossSpecialAttackChain()
    {
        animator.SetBool("Attack2", true);
        animator.SetBool("Attack1", false);
        _wing1Collider.SetActive(true);
        _wing2Collider.SetActive(true);
        yield return new WaitForSeconds(0.25f);

        animator.SetBool("Attack2", false);

        yield return new WaitForSeconds(2.0f);
        _wing1Collider.SetActive(false);
        _wing2Collider.SetActive(false);

        yield return new WaitForSeconds(attackDelay);
        _animationLocked = false;
        _canAttack = true;
    }

    IEnumerator BossSpecialAttackFlame()
    {
        animator.SetBool("FlameAttack", true);
        _specialAttackScript.enabled = true;
        yield return new WaitForSeconds(2f);
        animator.SetBool("FlameAttack", false);
        _specialAttackScript.enabled = false;
        yield return new WaitForSeconds(attackDelay);
        _canAttack = true;
        _animationLocked = false;
    }

    private void OnDeath()
    {
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(_respawnTime);

        if (monsterType == type.boss || monsterType == type.monster)
        {
            manager.significantEnemyDeaths++;
        }
        monsterHP = _monHP;
        transform.position = spawnPoint.position;
        _respawning = false;
        GetStats();
    }
}
