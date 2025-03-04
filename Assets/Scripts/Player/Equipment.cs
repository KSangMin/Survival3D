using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipParent;

    private PlayerController controller;
    private PlayerCondition condition;

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();

        controller.attackAction.performed -= OnAttackInput;

        controller.attackAction.performed += OnAttackInput;
    }

    public void EquipNew(ItemData data)
    {
        UnEquip();
        curEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if(curEquip != null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

    void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed && curEquip != null && controller.canLook)
        {
            curEquip.OnAttackInput();
        }
    }
}
