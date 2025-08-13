using UnityEngine;
using UnityEngine.UI;

// ====== Конфигурация предмета (редактируется в инспекторе) ======
public abstract class ItemConfig : ScriptableObject
{
    [Header("Base Settings")]
    public string itemName;

    public int width;

    public float cooldown;
    public int height;

    public int centerX;
    public int centerY;

    public bool[] shape;

    [Header("Level Settings")]
    public int lvlMax;
    public Sprite[] levelSprites;

    public bool GetCell(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return false;
        return shape[y * width + x];
    }

    public int[] GetRelativePosition(int blockId)
    {
        int posX = blockId % width - centerX;
        int posY = blockId / width - centerY;
        return new int[] { posX, posY };
    }

    public virtual ItemData CreateRuntimeData()
    {
        return new ItemData(this);
    }
}

[CreateAssetMenu(menuName = "Items/ShootingItemConfig")]
public class ShootingItemConfig : ItemConfig
{
    [Header("Shooting Settings")]
    public int[] damage;

    public override ItemData CreateRuntimeData()
    {
        return new ShootingItemData(this);
    }
}

[CreateAssetMenu(menuName = "Items/ActivatableItemConfig")]
public class ActivatableItemConfig : ItemConfig
{
    [Header("Activation Settings")]
    public bool isToggle;

    public int[] healing;

    public override ItemData CreateRuntimeData()
    {
        return new ActivatableItemData(this);
    }
}

[CreateAssetMenu(menuName = "Items/ExtensionItemConfig")]
public class ExtensionItemConfig : ItemConfig
{
    public override ItemData CreateRuntimeData()
    {
        return new ItemData(this);
    }
}


// ====== Runtime-данные ======
public class ItemData
{
    public ItemConfig config;
    public int lvl;

    public bool СompareItems(ItemData anotherItemData)
    {
        return lvl == anotherItemData.lvl && config.itemName == anotherItemData.config.itemName;
    }

    public ItemData(ItemConfig config)
    {
        this.config = config;
        this.lvl = 0;
    }
}

public class ShootingItemData : ItemData
{
    public ShootingItemConfig ShootingConfig => (ShootingItemConfig)config;

    public ShootingItemData(ShootingItemConfig config) : base(config) { }
}

public class ActivatableItemData : ItemData
{
    public ActivatableItemConfig ActivatableConfig => (ActivatableItemConfig)config;

    public ActivatableItemData(ActivatableItemConfig config) : base(config) { }

    public int GetHealing()
    {
        if (lvl >= 0 && lvl < ActivatableConfig.healing.Length)
            return ActivatableConfig.healing[lvl];
        return 0;
    }
}


public class ExtensionItemData : ItemData
{
    public ExtensionItemConfig ExtensionConfig => (ExtensionItemConfig)config;

    public ExtensionItemData(ExtensionItemConfig config) : base(config) { }
}

public static class ImageExtensions
{
    public static void SetAlpha(this Image image, float alpha)
    {
        var c = image.color;
        c.a = alpha;
        image.color = c;
    }
}

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Game Data/Player Stats")]
public class PlayerStats : ScriptableObject
{
    public int money;
    public int health;

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void RemoveMoney(int amount)
    {
        money = Mathf.Max(0, money - amount);
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
    }

    public void Heal(int amount)
    {
        health += amount;
    }
}

