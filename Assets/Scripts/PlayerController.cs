using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;

    [Header("Look")]
    private Transform _camTransform;
    public float minXLook;
    public float maxXLook;
    private float _camCurXRot;
    public float lookSensitivity;
    private Vector2 _mouseDelta;

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
        _camTransform = GetComponentInChildren<Camera>().transform;
        _rb = GetComponent<Rigidbody>();
        _Input = GetComponent<PlayerInput>();

        _moveAction = _Input.actions["Move"];
        _lookAction = _Input.actions["Look"];
        _jumpAction = _Input.actions["Jump"];
        _attackAction = _Input.actions["Attack"];
        _inventoryAction = _Input.actions["Inventory"];
        _interactAction = _Input.actions["Interact"];

        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;
        _lookAction.performed -= OnLook;
        _lookAction.canceled -= OnLook;

        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
        _lookAction.performed += OnLook;
        _lookAction.canceled += OnLook;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        CameraLook();
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

    public void OnLook(InputAction.CallbackContext context)
    {
        if(context.performed) _mouseDelta = context.ReadValue<Vector2>();
        else if(context.canceled) _mouseDelta = Vector2.zero;
    }

    void CameraLook()
    {
        _camCurXRot += _mouseDelta.y * lookSensitivity;
        _camCurXRot = Mathf.Clamp(_camCurXRot, minXLook, maxXLook);
        _camTransform.localEulerAngles = new Vector3(-_camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, _mouseDelta.x * lookSensitivity);
    }
}
