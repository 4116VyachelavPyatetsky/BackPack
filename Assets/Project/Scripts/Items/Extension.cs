using UnityEngine;

public class Extension : DraggableExtension, IHasItemData
{
    public ExtensionItemConfig config;

    public ItemData ItemData
    {
        get => runtimeData;
        set => runtimeData = value;
    }

    private ItemData runtimeData;

    protected void Awake()
    {
        base.Awake();
        runtimeData = config.CreateRuntimeData();
    }
}
