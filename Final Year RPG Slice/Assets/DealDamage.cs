using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private PlayerStats _playerStats;
    [SerializeField] private GameObject _manager;
    private Difficulty_Manager _self;
    [SerializeField] private bool _isAOE;
    private float timer = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        if (_manager == null)
        {
            _manager = GameObject.FindGameObjectWithTag("Difficulty_Manager");
        }

        _self = _manager.GetComponent<Difficulty_Manager>();
        _playerStats = _player.GetComponent<PlayerStats>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player && !_isAOE)
        {
            if(_playerStats.playerHealthValue > 0 && !_player.GetComponent<InputSytem>().defending)
            {
                float damage = 5 * _self.attackDamageModifier;
                int roundDamage = (int)damage;
                _playerStats.playerHealthValue -= roundDamage;
                Debug.Log(roundDamage);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == _player && _isAOE && timer <= 0)
        {
            if (_playerStats.playerHealthValue > 0 && !_player.GetComponent<InputSytem>().defending)
            {
                float damage = 2 * _self.attackDamageModifier;
                int roundDamage = (int)damage;
                _playerStats.playerHealthValue -= roundDamage;
                Debug.Log(roundDamage);
            }
            timer = 0.1f;
        }
    }
}
