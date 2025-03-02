using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UI_Condition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition stamina {  get { return uiCondition.stamina; } }

    public float noHungerHealthDamage;

    private void Update()
    {
        hunger.Subtract(hunger.passiveDeltaValue * Time.deltaTime);
        stamina.Add(stamina.passiveDeltaValue * Time.deltaTime);

        if (hunger.curValue <= 0f) health.Subtract(noHungerHealthDamage * Time.deltaTime);
        if (health.curValue <= 0f) Dead();
    }

    void Heal(float amount)
    {
        health.Add(amount);
    }

    void Eat(float amount)
    {
        hunger.Add(amount);
    }

    void Dead()
    {
        Debug.Log("ав╬З╢ы!");
    }
}
