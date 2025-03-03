using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractableGO;
    private IInteractable curInteractable;

    public TextMeshProUGUI promptText;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;

        CharacterManager.Instance.Player.controller._interactAction.started -= OnInteractInput;

        CharacterManager.Instance.Player.controller._interactAction.started += OnInteractInput;
    }

    private void Update()
    {
        if (Time.time - lastCheckTime >= checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

            if (Physics.Raycast(ray, out RaycastHit hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractableGO)
                {
                    curInteractableGO = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }
            else
            {
                curInteractableGO = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractableGO = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}
