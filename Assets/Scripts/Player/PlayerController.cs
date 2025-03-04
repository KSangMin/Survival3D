using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public float minXLook;
    public float maxXLook;
    private float _camCurXRot;
    public float lookSensitivity;
    public Transform _camTransform;
    private Vector2 _mouseDelta;

    [Header("Jump")]
    public float jumpPower;

    private Rigidbody _rb;

    private PlayerInput _Input;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    public InputAction attackAction;
    public InputAction inventoryInputAction;
    public InputAction interactAction;

    public bool canLook = true;
    public Action inventoryAction;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _Input = GetComponent<PlayerInput>();

        _moveAction = _Input.actions["Move"];
        _lookAction = _Input.actions["Look"];
        _jumpAction = _Input.actions["Jump"];
        attackAction = _Input.actions["Attack"];
        inventoryInputAction = _Input.actions["Inventory"];
        interactAction = _Input.actions["Interact"];

        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;
        _lookAction.performed -= OnLook;
        _lookAction.canceled -= OnLook;
        _jumpAction.started -= OnJump;
        inventoryInputAction.started -= OnInventory;

        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
        _lookAction.performed += OnLook;
        _lookAction.canceled += OnLook;
        _jumpAction.started += OnJump;
        inventoryInputAction.started += OnInventory;
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
        if(canLook) CameraLook();
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

    void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed) curMovementInput = context.ReadValue<Vector2>();
        else if(context.canceled) curMovementInput = Vector2.zero;
    }

    void OnLook(InputAction.CallbackContext context)
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

    void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded())
        {
            _rb.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    bool isGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + transform.forward * 0.2f + transform.up * 0.01f, Vector3.down)
            ,new Ray(transform.position - transform.forward * 0.2f + transform.up * 0.01f, Vector3.down)
            ,new Ray(transform.position + transform.right * 0.2f + transform.up * 0.01f, Vector3.down)
            ,new Ray(transform.position - transform.right * 0.2f + transform.up * 0.01f, Vector3.down)
        };

        foreach (Ray ray in rays)
        {
            if (Physics.Raycast(ray, 0.1f, groundLayerMask)) return true;
        }

        return false;
    }

    void OnInventory(InputAction.CallbackContext context)
    {
        if (context.started) ToggleCursor();
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
