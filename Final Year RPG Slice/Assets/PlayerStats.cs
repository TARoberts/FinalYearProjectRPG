using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerHealthValue = 20;
    private bool _dead = false;
    [SerializeField] private Transform _respawnPoint;
    [SerializeField] private Animator _playerAnimator;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealthValue <= 0 && !_dead)
        {
            _dead = true;
            _playerAnimator.SetBool("Dead", true);
            StartCoroutine(DieAndRespawn());
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
        playerHealthValue = 20;
    }
}
