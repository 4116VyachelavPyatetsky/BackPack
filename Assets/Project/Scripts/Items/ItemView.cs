using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour
{
    private Image image;
    private IHasItemData itemHolder;

    private void Awake()
    {
        image = GetComponent<Image>();
        itemHolder = GetComponent<IHasItemData>();
    }

    public void Upgrade()
    {
        var data = itemHolder.ItemData;
        if (data.lvl < data.config.lvlMax)
        {
            data.lvl++;
            image.sprite = data.config.levelSprites[data.lvl];
        }
    }
}
