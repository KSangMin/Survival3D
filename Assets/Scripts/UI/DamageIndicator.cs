using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    Image image;
    public float flashSpeed;

    Coroutine co;

    private void Start()
    {
        image = GetComponent<Image>();

        CharacterManager.Instance.Player.condition.onTakeDamage += Flash;
    }

    public void Flash()
    {
        if (co != null) StopCoroutine(co);
        image.enabled = true;
        image.color = new Color(1, 0, 0);
        co = StartCoroutine(FadeAway());
    }

    IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while (a > 0)
        {
            a -= (startAlpha / flashSpeed) * Time.deltaTime;
            image.color = new Color(1, 0, 0, a);
            yield return null;
        }

        image.enabled = false;
    }
}
