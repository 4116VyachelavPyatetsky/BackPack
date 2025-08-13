using UnityEngine;

public class Weapon : DraggableUI, IHasItemData
{
    public ShootingItemConfig config;
    public ItemData ItemData 
    { 
        get => runtimeData; 
        set => runtimeData = value; 
    }

    private ItemData runtimeData;

    private void Awake()
    {
        base.Awake();
        runtimeData = new ItemData(config);
    }
}
