using UnityEngine;

public class Activatable : DraggableUI, IHasItemData
{
    public ActivatableItemConfig config;
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
