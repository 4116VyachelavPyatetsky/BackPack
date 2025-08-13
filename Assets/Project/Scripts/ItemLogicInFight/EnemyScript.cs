using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    Animation animationHit;
    public Image hpBar;

    void Awake()
    {
        animationHit = GetComponent<Animation>();
    }

    public void OnHit(float hpBarPos)
    {
        animationHit.Play();
        hpBar.fillAmount = hpBarPos;
    }
}
