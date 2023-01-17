using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty_Manager : MonoBehaviour
{
    private bool _managerActive = true;

    //records the number of times a player is hit by enemies across a given time period in seconds
    public float hitsTaken = 0;
    private float _timeAlive = 0f;
    public float playerDeaths = 0f;

    //the variables controlled by the system

    public float turnRateModifier = 1f;
    private float _maxTurnRateModifier = 5f;
    private float _minTurnRateModifier = 0.2f;

    public float attackDamageModifier = 1.0f;
    private float _maxAttackDamageModifier = 3.0f;
    private float _minAttackDamageModifier = 0.5f;

    public float incomingDamageModifer = 1.0f;
    private float _maxIncomingDamageModifier = 2.0f;
    private float _minIncomingDamageModifier = 0.75f;

    public float aggressionRange = 1.0f;
    private float _maxAggressionRange = 1.5f;
    private float _minAggressionRange = 0.5f;

    public float specialAttackCooldown = 20.0f;
    private float _maxSpecialAttackCooldown = 40.0f;
    private float _minSpecialAttackCooldown = 10f;

    public float recoveryTime = 2.0f;
    private float _maxRecoveryTime = 4.0f;
    private float _minRecoveryTime = 0.5f;

    public float hitPointsModifier = 1.0f;
    private float _maxHitPointsModifier = 1.5f;
    private float _minHitPointsModifier = 0.5f;

    public int movesetIDNum = 1;


    //a multiplier applied to all values based on current expected difficulty
    public float intensityMultiplier = 1f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer();
    }

    private void Timer()
    {

        _timeAlive += Time.deltaTime;
        
    }
}
