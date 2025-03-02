using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;

    private Rigidbody _rb;

    private PlayerInput _Input;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private InputAction _inventoryAction;
    private InputAction _interactAction;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _Input = GetComponent<PlayerInput>();

        _moveAction = _Input.actions["Move"];
        _lookAction = _Input.actions["Look"];
        _jumpAction = _Input.actions["Jump"];
        _attackAction = _Input.actions["Attack"];
        _inventoryAction = _Input.actions["Inventory"];
        _interactAction = _Input.actions["Interact"];

        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 dir = 
            transform.forward * curMovementInput.y
            + transform.right *curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rb.velocity.y;
        _rb.velocity = dir;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed) curMovementInput = context.ReadValue<Vector2>();
        else if(context.canceled) curMovementInput = Vector2.zero;
    }
}
