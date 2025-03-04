using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerController controller;
    [HideInInspector] public PlayerCondition condition;
    [HideInInspector] public Equipment equipment;

    public ItemData itemData;
    public Action addItem;

    public Transform dropPosition;

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        equipment = GetComponent<Equipment>();
    }
}
