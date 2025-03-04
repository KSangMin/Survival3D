using System;
using UnityEngine;

public interface IDamageable
{
    void TakePhysicalDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamageable
{
    public UI_Condition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition stamina {  get { return uiCondition.stamina; } }

    public float noHungerHealthDamage;

    public event Action onTakeDamage;

    private void Update()
    {
        hunger.Subtract(hunger.passiveDeltaValue * Time.deltaTime);
        stamina.Add(stamina.passiveDeltaValue * Time.deltaTime);

        if (hunger.curValue <= 0f) health.Subtract(noHungerHealthDamage * Time.deltaTime);
        if (health.curValue <= 0f) Dead();
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    void Dead()
    {
        Debug.Log("ав╬З╢ы!");
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        onTakeDamage?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if(stamina.curValue < amount) return false;

        stamina.Subtract(amount);
        return true;
    }
}
