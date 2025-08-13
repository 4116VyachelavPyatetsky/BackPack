using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // Основной список предметов с позицией
    public List<PositionedItemData> Items { get; private set; } = new List<PositionedItemData>();

    public FightItemCreator creator;

    [Header("Prefabs")]
    public List<StringGameObjectPair> itemsPrefabs = new List<StringGameObjectPair>();
    public Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

    private const string SaveKey = "InventoryData";

    private void Awake()
    {
        Instance = this;
        foreach (var entry in itemsPrefabs)
        {
            if (!prefabs.ContainsKey(entry.name))
                prefabs.Add(entry.name, entry.prefab);
        }
        LoadInventory();
    }

    public void AddItem(ItemData item, Vector2 position)
    {
        Items.Add(new PositionedItemData(item, position));
        creator.CreateNewIco(item);
        SaveInventory();
    }

    public void RemoveItem(ItemData item)
    {
        var found = Items.Find(i => i.Item == item);
        if (found != null)
        {
            Items.Remove(found);
            creator.RemoveIco(item);
            SaveInventory();
        }
    }

    public void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData();
        foreach (var positioned in Items)
        {
            saveData.items.Add(new PositionedItemSave(positioned.Item, positioned.Position));
        }
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
            return;

        string json = PlayerPrefs.GetString(SaveKey);
        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        Items.Clear();

        foreach (var saved in saveData.items)
        {
            if (prefabs.TryGetValue(saved.itemConfigName, out GameObject prefab))
            {
                GameObject obj = Instantiate(prefab,transform.parent);
                obj.GetComponent<RectTransform>().localPosition = saved.position;
                var itemHolder = obj.GetComponent<IHasItemData>();
                if (itemHolder != null)
                {
                    var config = obj.GetComponent<IHasItemData>().ItemData.config;
                    ItemData newItem = config.CreateRuntimeData();
                    newItem.lvl = saved.level;
                    itemHolder.ItemData = newItem;
                    DraggableUI drag = obj.GetComponent<DraggableUI>();
                    drag.blockSystem.StartHighlight(obj.GetComponent<RectTransform>());
                    drag.blockSystem.TrySetObject();
                    drag.blockSystem.StopHighlight();
                    Items.Add(new PositionedItemData(newItem, saved.position));
                }
            }
            else
            {
                Debug.LogWarning($"Prefab for '{saved.itemConfigName}' not found in dictionary.");
            }
        }
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        Debug.Log("Сохранённые данные инвентаря удалены.");
    }
}

[System.Serializable]
public class PositionedItemData
{
    public ItemData Item;
    public Vector2 Position;

    public PositionedItemData(ItemData item, Vector2 position)
    {
        Item = item;
        Position = position;
    }
}

[System.Serializable]
public class PositionedItemSave
{
    public string itemConfigName;
    public int level;
    public Vector2 position;

    public PositionedItemSave(ItemData itemData, Vector2 pos)
    {
        itemConfigName = itemData.config.itemName;
        level = itemData.lvl;
        position = pos;
    }
}

[System.Serializable]
public class InventorySaveData
{
    public List<PositionedItemSave> items = new List<PositionedItemSave>();
}
