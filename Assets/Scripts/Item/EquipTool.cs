using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    private bool isAttacking;
    public float attackDistance;

    [Header("Resource Gathering")]
    public bool canGatherResource;

    [Header("Combat")]
    public bool canDealDamage;
    public int damage;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnAttackInput()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
            Invoke("OnCanAttack", attackRate);
        }
    }

    void OnCanAttack()
    {
        isAttacking = false;
    }
}
