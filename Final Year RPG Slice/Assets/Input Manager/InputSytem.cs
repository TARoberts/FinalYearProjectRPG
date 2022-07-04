using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSytem : MonoBehaviour
{
    public float jumpHeight, moveSpeed;
    public bool inEditor;

    private bool onFloor, attackLocked;

    [SerializeField] private Transform checkSource, playerCameraPosition;
    public LayerMask groundMask;
    public float floorDistance;
    private float gravityValue = -9.81f;
    private Vector3 playerVelocity;

    private DogKnightControls dogKnightControls;
    public CharacterController dogController;

    private void Awake()
    {
        
        dogController = GetComponent<CharacterController>();

        dogKnightControls = new DogKnightControls();
        dogKnightControls.Player.Jump.Enable();
        dogKnightControls.Player.Jump.performed += Jump;

        dogKnightControls.Player.Attack.Enable();
        dogKnightControls.Player.Attack.performed += Attack;

        dogKnightControls.Player.Movement.Enable();
        

        if (!inEditor)
        {
            jumpHeight = jumpHeight * 2;
        }
    }

    private void FixedUpdate()
    {
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

        move.Normalize();

        move = Quaternion.AngleAxis(playerCameraPosition.rotation.eulerAngles.y, Vector3.up) * move;

        dogController.Move(move * Time.deltaTime * localSpeed);

        if (move.magnitude > 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            dogController.transform.forward = move;
        }


        

        playerVelocity.y += gravityValue * Time.deltaTime;
        dogController.Move(playerVelocity * Time.deltaTime);
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
            attackLocked = true;
            if (!attackLocked)
            {
                Debug.Log("Attackingway");
                StartCoroutine(AttackLock());
            }
    }

    IEnumerator AttackLock()
    {
        yield return new WaitForSeconds(1.0f);
        attackLocked = false;
    }
}

