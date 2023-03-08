using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSytem : MonoBehaviour
{
    public float jumpHeight, moveSpeed;
    public bool inEditor, inMenu;

    private bool onFloor, attackLocked;

    public bool defending = false;
    [SerializeField] private Transform checkSource, playerCameraPosition;
    public LayerMask groundMask;
    public float floorDistance;
    private float gravityValue = -9.81f;
    private Vector3 playerVelocity;

    private DogKnightControls dogKnightControls;
    public CharacterController dogController;
    private Animator dogAnimator;

    [SerializeField] private GameObject _attackCollider;

    private float attackTimer = 0;

    private bool _canPlaySound = true;

    private Audio_Player sound;

    private bool _canMove = true;

    private void Awake()
    {
        
        dogController = GetComponent<CharacterController>();
        dogAnimator = GetComponent<Animator>();
        if (_attackCollider == null)
        {
            Debug.Log("No colldier for attacks, attack wont work");
        }
        else
        {
            _attackCollider.SetActive(false);
        }
        dogKnightControls = new DogKnightControls();
        dogKnightControls.Player.Jump.Enable();
        dogKnightControls.Player.Jump.performed += Jump;

        dogKnightControls.Player.Attack.Enable();
        dogKnightControls.Player.Attack.performed += Attack;

        dogKnightControls.Player.Defend.Enable();
        dogKnightControls.Player.Defend.performed += Defend;
        dogKnightControls.Player.Defend.canceled += Defend_cancelled;

        dogKnightControls.Player.Movement.Enable();

        sound = GetComponent<Audio_Player>();

        if (!inEditor)
        {
            jumpHeight = jumpHeight * 2;
        }
    }

    private void FixedUpdate()
    {
        if (dogAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack01") ||
            dogAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack02"))
        {
            _canMove = false;
        }

        else if (dogAnimator.GetCurrentAnimatorStateInfo(0).IsName("Defend"))
        {
            _canMove = false;
            defending = true;
        }

        else
        {
            defending = false;
            _canMove = true;
        }
        if (inMenu && dogKnightControls.Player.enabled)
        {
            dogKnightControls.Player.Disable();
        }
        else
        {
            dogKnightControls.Player.Enable();
        }

        if ((dogAnimator.GetInteger("AttackState") != 0))
        {
            _attackCollider.SetActive(true);
        }
        else
        {
            _attackCollider.SetActive(false);
        }

        float localSpeed = moveSpeed;
        onFloor = Physics.CheckSphere(checkSource.position, floorDistance, groundMask);
        if (!onFloor)
        {
            localSpeed = localSpeed / 2;
        }
        
        if (onFloor && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        Vector2 inputVector = dogKnightControls.Player.Movement.ReadValue<Vector2>();

        Vector3 move = (new Vector3(inputVector.x, 0, inputVector.y));

        if(_canMove)
        {
            float mag = move.magnitude;
            //move.Normalize();

            move = Quaternion.AngleAxis(playerCameraPosition.rotation.eulerAngles.y, Vector3.up) * move;

            dogController.Move(move * Time.deltaTime * localSpeed);

            dogAnimator.SetFloat("Movement", mag);

            if (move.magnitude > 0)
            {
                Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
                dogController.transform.forward = move;
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            dogController.Move(playerVelocity * Time.deltaTime);
        }
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
        if (attackTimer <= 0f)
        {
            dogAnimator.SetInteger("AttackState", 0);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        onFloor = Physics.CheckSphere(checkSource.position, floorDistance, groundMask);
        Debug.Log(onFloor);
        if (onFloor)
            {
            Debug.Log("Jumpingway" + context.phase);
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -1.0f * gravityValue);
            }
        else
            {
                Debug.Log("Not grounded");
            }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        attackTimer = .8f;
        if ((((dogAnimator.GetInteger("AttackState") == 0)) || (attackLocked && (dogAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack02")))) && !inMenu)
        {
            AttackSound();
            attackLocked = true;
            Debug.Log("Attackingway");
            dogAnimator.SetInteger("AttackState", 1);
        }
        else if (dogAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack01") && !inMenu)
        {
            AttackSound();
            Debug.Log("Attackingway");
            dogAnimator.SetInteger("AttackState", 2);
        }
    }

    public void Defend(InputAction.CallbackContext context)
    {
        dogAnimator.SetBool("Defending", true);
        defending = true;
    }

    public void Defend_cancelled(InputAction.CallbackContext context)
    {
        dogAnimator.SetBool("Defending", false);
    }

    private void AttackSound()
    {
        if (sound != null && _canPlaySound)
        {
            sound.PlaySound();
            _canPlaySound = false;
            StartCoroutine(WaitToPlaySound());
        }
    }

    IEnumerator WaitToPlaySound()
    {
        yield return new WaitForSeconds(.8f);
        _canPlaySound = true;
    }
}

