using UnityEngine;
using static UnityEngine.UI.Image;

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

    Camera cam;

    private void Start()
    {
        animator = GetComponent<Animator>();
        cam = Camera.main;
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

    public void OnHit()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if(Physics.Raycast(ray, out RaycastHit hit, attackDistance))
        {
            if (canGatherResource && hit.collider.TryGetComponent(out Resource resource)) resource.Gather(hit.point, hit.normal);
        }
        Debug.Log(hit.collider?.name);
    }
}
