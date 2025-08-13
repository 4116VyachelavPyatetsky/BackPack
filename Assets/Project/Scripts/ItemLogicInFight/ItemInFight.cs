using UnityEngine;
using UnityEngine.UI;

public class ItemInFight : MonoBehaviour
{
    //public Image imageLoad;
    public ItemData itemData;

    public Animator loadAnimator;

    protected void Start()
    {
        loadAnimator.speed = 1f/itemData.config.cooldown;
    }


    public void StartReload()
    {
        loadAnimator.SetTrigger("startReload");
    }

    public void EndReload()
    {
        loadAnimator.SetTrigger("endReload");
    }

    public virtual void Loaded()
    {

    }
}
