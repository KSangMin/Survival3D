using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    public UI_ItemSlot[] slots;
    public GameObject uiInventory;
    public Transform slotPanel;
    public Transform dropPosition;
    
    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;

    public Button useButton;
    public Button equipButton;
    public Button unequipButton;
    public Button dropButton;

    public GameObject useButtonGo;
    public GameObject equipButtonGo;
    public GameObject unequipButtonGo;
    public GameObject dropButtonGo;

    private PlayerController controller;
    private PlayerCondition condition;

    ItemData selectedItem;
    int selectedItemIndex;

    int curEquipIndex;

    private void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        uiInventory.SetActive(false);
        slots = new UI_ItemSlot[slotPanel.childCount];

        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<UI_ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }

        ClearSelectedItem();

        CharacterManager.Instance.Player.addItem -= AddItem;
        CharacterManager.Instance.Player.controller.inventoryInputAction.started -= Toggle;

        CharacterManager.Instance.Player.addItem += AddItem;
        CharacterManager.Instance.Player.controller.inventoryInputAction.started += Toggle;

        useButton.onClick.AddListener(OnUseButton);
        equipButton.onClick.AddListener(OnEquipbutton);
        unequipButton.onClick.AddListener(OnUnequipbutton);
        dropButton.onClick.AddListener(OnDropButton);
    }

    void ClearSelectedItem()
    {
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        useButtonGo.SetActive(false);
        equipButtonGo.SetActive(false);
        unequipButtonGo.SetActive(false);
        dropButtonGo.SetActive(false);
    }

    void Toggle(InputAction.CallbackContext context)
    {
        if (context.started) uiInventory.SetActive(!uiInventory.activeInHierarchy);
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;
        if (data.isStackable)
        {
            UI_ItemSlot slot = GetItemStack(data);
            if(slot != null)
            {
                slot.quantity++;
                CharacterManager.Instance.Player.itemData = null;
                UpdateUI();
                return;
            }
        }

        UI_ItemSlot emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            CharacterManager.Instance.Player.itemData = null;
            UpdateUI();
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
        return;
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null) slots[i].Set();
            else slots[i].Clear();
        }
    }

    UI_ItemSlot GetItemStack(ItemData data)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount) return slots[i];
        }
        return null;
    }

    UI_ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null) return slots[i];
        }
        return null;
    }

    void ThrowItem(ItemData data)
    {
        Instantiate(data.prefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.description;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        for(int i = 0; i < selectedItem.consumables.Length ;i++)
        {
            selectedItemStatName.text += selectedItem.consumables[i].type.ToString() + "\n";
            selectedItemStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        useButtonGo.SetActive(selectedItem.type == ItemType.Consumable);
        equipButtonGo.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].isEquipped);
        unequipButtonGo.SetActive(selectedItem.type == ItemType.Equipable && slots[index].isEquipped);
        dropButtonGo.SetActive(true);
    }

    void OnUseButton()
    {
        if(selectedItem.type == ItemType.Consumable)
        {
            for(int i = 0; i < selectedItem.consumables.Length ; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.consumables[i].value);
                        break;
                }
            }

            RemoveSelectedItem();
        }
    }

    void OnDropButton()
    {
        UnEquip(selectedItemIndex);
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--;
        if(slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null;
            slots[selectedItemIndex].item = null;
            selectedItemIndex = -1;
            ClearSelectedItem();
        }

        UpdateUI();
    }

    void OnEquipbutton()
    {
        if (slots[curEquipIndex].isEquipped) UnEquip(curEquipIndex);

        slots[selectedItemIndex].isEquipped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.equipment.EquipNew(selectedItem);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    void UnEquip(int index)
    {
        slots[index].isEquipped = false;
        CharacterManager.Instance.Player.equipment.UnEquip();
        UpdateUI();

        if(selectedItemIndex == index) SelectItem(selectedItemIndex);
    }

    void OnUnequipbutton()
    {
        UnEquip(selectedItemIndex);
    }
}
