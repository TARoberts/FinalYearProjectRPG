using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerHealthValue = 20;
    private int _startingHP;
    private bool _dead = false;
    [SerializeField] private Transform _respawnPoint;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Difficulty_Manager manager;
    [SerializeField] private float _damageTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (_respawnPoint == null)
        {
            _respawnPoint = this.transform;
        }

        if (_playerAnimator == null)
        {
            _playerAnimator = gameObject.GetComponent<Animator>();
        }

        if (manager == null)
        {
            manager = GameObject.FindGameObjectWithTag("Difficulty_Manager").GetComponent<Difficulty_Manager>();
        }
        _startingHP = playerHealthValue;

        GetComponent<CharacterController>().detectCollisions = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealthValue <= 0 && !_dead)
        {
            _dead = true;
            manager.playerDeaths += 1;
            _playerAnimator.SetBool("Dead", true);
            StartCoroutine(DieAndRespawn());
        }

        if (_damageTimer > 0)
        {
            _damageTimer -= Time.deltaTime;
        }
    }

    IEnumerator DieAndRespawn()
    {
        yield return new WaitForSeconds(1.0f);
        this.gameObject.GetComponent<CharacterController>().enabled = false;
        transform.SetPositionAndRotation(_respawnPoint.position, _respawnPoint.rotation);
        gameObject.GetComponent<CharacterController>().enabled = true;
        yield return new WaitForSeconds(2.0f);
        _dead = false;
        _playerAnimator.SetBool("Dead", false);
        playerHealthValue = _startingHP;
    }
}
