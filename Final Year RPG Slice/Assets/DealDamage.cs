using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private PlayerStats _playerStats;
    [SerializeField] private GameObject _manager;
    private Difficulty_Manager _self;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player)
        {
            if(_playerStats.playerHealthValue > 0)
            {
                float damage = 5 * _self.attackDamageModifier;
                int roundDamage = (int)damage;
                _playerStats.playerHealthValue -= roundDamage;
                Debug.Log(roundDamage);
            }
        }
    }
}
