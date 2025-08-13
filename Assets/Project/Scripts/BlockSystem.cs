using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BlockSystem : MonoBehaviour
{
    const int totalWidth = 7;
    const int totalHeight = 7;
    private SlotData[,] slots;
    public bool canPlace = true;
    public bool canMerge = false;
    public bool extension = false;

    private RectTransform DraggableItem;
    public RectTransform mergeItem;
    private Vector2 snapPosition;

    public GameObject slotPrefab;

    private const string SaveKey = "BlockSystem_Slots";

    void Awake()
    {
        slots = new SlotData[totalWidth, totalHeight];
        LoadSlots();
        GenerateUI();
    }
    void LoadSlots()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            Debug.Log("Data loaded");
            string json = PlayerPrefs.GetString(SaveKey);
            SlotSaveData saveData = JsonUtility.FromJson<SlotSaveData>(json);
            int index = 0;
            for (int y = 0; y < totalHeight; y++)
            {
                for (int x = 0; x < totalWidth; x++)
                {
                    slots[x, y] = new SlotData
                    {
                        isOpen = saveData.openStates[index],
                        objectInInventory = null,
                        item = null
                    };
                    index++;
                }
            }
        }
        else
        {
            int startX = (totalWidth - 3) / 2;
            int startY = (totalHeight - 3) / 2;
            for (int x = 0; x < totalWidth; x++)
            {
                for (int y = 0; y < totalHeight; y++)
                {
                    slots[x, y] = new SlotData { isOpen = false };
                }
            }
            for (int x = startX; x < startX + 3; x++)
            {
                for (int y = startY; y < startY + 3; y++)
                {
                    slots[x, y].isOpen = true;
                }
            }
        }
    }
    void GenerateUI()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        Vector2 startPos = new Vector2(-330, 330);
        float offset = 110f;

        for (int y = 0; y < totalHeight; y++)
        {
            for (int x = 0; x < totalWidth; x++)
            {
                var slotObj = Instantiate(slotPrefab, transform);
                slotObj.transform.localPosition = new Vector3(
                    startPos.x + (x * offset),
                    startPos.y - (y * offset),
                    0
                );
                var block = slotObj.GetComponent<InventoryBlock>();
                block.Setup(this, x, y);
                slotObj.SetActive(slots[x, y].isOpen);
            }
        }
    }

    public void SaveSlots()
    {
        if (slots != null)
        {
            bool[] openStates = new bool[totalWidth * totalHeight];
            int index = 0;
            for (int y = 0; y < totalHeight; y++)
            {
                for (int x = 0; x < totalWidth; x++)
                {
                    openStates[index] = slots[x, y].isOpen;
                    index++;
                }
            }

            string json = JsonUtility.ToJson(new SlotSaveData(openStates));
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }
    }

    public void ClearSlotSaves()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        slots = null;
        PlayerPrefs.Save();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void OnApplicationQuit()
    {
        SaveSlots();
    }
    public void StartHighlight(RectTransform item)
    {
        DraggableItem = item;
    }

    public void StopHighlight()
    {
        DraggableItem = null;
    }
    
    public void ShowBlocksStates(RectTransform item)
    {
        ResetAllBlocksColor();
        List<Vector2Int> targetSlots = GetTargetSlots(item, out canPlace);

        foreach (var slot in targetSlots)
        {
            if(slot.y >= 0 && slot.y <7 && slot.x >= 0 && slot.x <7)
            {
                int blockId = slot.y * 7 + slot.x;
                if (blockId >= 0 && blockId < 49)
                {
                    if (canMerge) 
                    {
                        transform.GetChild(blockId).GetComponent<Image>().color = Color.yellow;
                    }
                    else 
                    {
                        transform.GetChild(blockId).GetComponent<Image>().color = 
                        canPlace ? Color.green : Color.red;
                    }
                }
            }
        }
    }

    public void ShowExtensionsStates(RectTransform item)
    {
        ResetAllBlocksColor();
        List<Vector2Int> targetSlots = GetTargetSlotsForExtension(item, out extension);

        foreach (var slot in targetSlots)
        {
            if(slot.y >= 0 && slot.y <7 && slot.x >= 0 && slot.x <7)
            {
                int blockId = slot.y * 7 + slot.x;
                if (blockId >= 0 && blockId < 49)
                {
                    var image = transform.GetChild(blockId).GetComponent<Image>();
                    Color currentColor = image.color;
                    Color targetColor = extension ? Color.green : Color.red;
                    targetColor.a = currentColor.a;
                    image.color = targetColor;
                }
            }
        }

    }

    public void ResetAllBlocksColor()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var image = transform.GetChild(i).GetComponent<Image>();
            Color currentColor = image.color;
            Color newColor = Color.white;
            newColor.a = currentColor.a;
            image.color = newColor;
        }
    }

    public void TrySetObject(bool pass = false, bool extension = false)
    {
        if (canPlace || pass) 
        {
            List<Vector2Int> targetSlots = GetTargetSlots(DraggableItem, out _);
            ItemData data = DraggableItem.GetComponent<IHasItemData>().ItemData;
            foreach (var slot in targetSlots)
            {
                slots[slot.x, slot.y].item = data;
                slots[slot.x, slot.y].objectInInventory = DraggableItem;
            }
            DraggableItem.localPosition = snapPosition;
        }
    }

    public void TrySetExtension()
    {
        if (extension) 
        {
            List<Vector2Int> targetSlots = GetTargetSlots(DraggableItem, out _);
            foreach (var slot in targetSlots)
            {
                slots[slot.x, slot.y].isOpen = true;
                transform.GetChild(slot.y * 7 + slot.x).GetComponent<Image>().color = Color.white;
            }
        }

    }
    public void ClearSlotsByObject(RectTransform item)
    {
        if (item == null) return;

        ItemData itemData = item.GetComponent<IHasItemData>().ItemData;

        for (int x = 0; x < totalWidth; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                if (slots[x, y].item == itemData)
                {
                    slots[x, y].item = null;
                }
            }
        }
    }

    private List<Vector2Int> GetTargetSlots(RectTransform item, out bool canPlace)
    {
        List<Vector2Int> targetSlots = new List<Vector2Int>();
        ItemData itemData = item.GetComponent<IHasItemData>().ItemData;

        Vector2 offset = new Vector2(itemData.config.width / 2f - 0.5f, itemData.config.height / 2f - 0.5f);
        int itemPosX = (int)((item.localPosition.x + 55f) / 110f + 3f - offset.x);
        int itemPosY = -(int)((item.localPosition.y - 28f) / 110f - 3f + offset.y);
        snapPosition = new Vector2((itemPosX - 3 + offset.x) * 110f, ((-itemPosY) - offset.y + 3f) * 110f - 28f);

        canPlace = true;
        canMerge = true;

        for (int i = 0; i < itemData.config.shape.Length; i++)
        {
            if (!itemData.config.shape[i]) continue;

            int[] poses = itemData.config.GetRelativePosition(i);
            int slotX = itemPosX + poses[0];
            int slotY = itemPosY + poses[1];

            targetSlots.Add(new Vector2Int(slotX, slotY));

            if (slotX < 0 || slotX >= totalWidth || slotY < 0 || slotY >= totalHeight || !slots[slotX, slotY].isOpen)
            {
                canPlace = false;
                canMerge = false;
                continue;
            }

            // Слот занят
            if (slots[slotX, slotY].item != null)
            {
                canPlace = false;
                if (!slots[slotX, slotY].item.СompareItems(itemData))
                {
                    canMerge = false;
                }
                else mergeItem = slots[slotX, slotY].objectInInventory;
            }
            else
            {
                canMerge = false;
            }
        }

        return targetSlots;
    }

    private List<Vector2Int> GetTargetSlotsForExtension(RectTransform item, out bool canPlace)
    {
        List<Vector2Int> targetSlots = new List<Vector2Int>();
        ItemData itemData = item.GetComponent<IHasItemData>().ItemData;

        Vector2 offset = new Vector2(itemData.config.width / 2f - 0.5f, itemData.config.height / 2f - 0.5f);
        int itemPosX = (int)((item.localPosition.x + 55f) / 110f + 3f - offset.x);
        int itemPosY = -(int)((item.localPosition.y - 28f) / 110f - 3f + offset.y);
        snapPosition = new Vector2((itemPosX - 3 + offset.x) * 110f, ((-itemPosY) - offset.y + 3f) * 110f - 28f);

        canPlace = true;
        bool hasNeighborOpen = false; // <-- проверка соседей

        for (int i = 0; i < itemData.config.shape.Length; i++)
        {
            if (!itemData.config.shape[i]) continue;

            int[] poses = itemData.config.GetRelativePosition(i);
            int slotX = itemPosX + poses[0];
            int slotY = itemPosY + poses[1];

            targetSlots.Add(new Vector2Int(slotX, slotY));

            // Проверка границ
            if (slotX < 0 || slotX >= totalWidth || slotY < 0 || slotY >= totalHeight)
            {
                canPlace = false;
                continue;
            }

            // Если ячейка уже занята — тоже нельзя ставить
            if (slots[slotX, slotY].isOpen)
            {
                canPlace = false;
                continue;
            }

            // Проверка на соседей (по вертикали и горизонтали)
            Vector2Int[] neighbors =
            {
                new Vector2Int(slotX + 1, slotY),
                new Vector2Int(slotX - 1, slotY),
                new Vector2Int(slotX, slotY + 1),
                new Vector2Int(slotX, slotY - 1)
            };

            foreach (var n in neighbors)
            {
                if (n.x >= 0 && n.x < totalWidth && n.y >= 0 && n.y < totalHeight)
                {
                    if (slots[n.x, n.y].isOpen)
                    {
                        hasNeighborOpen = true;
                        break;
                    }
                }
            }
        }

        // Теперь canPlace только если и условие расположения, и наличие соседа выполнено
        if (!hasNeighborOpen)
            canPlace = false;

        return targetSlots;
    }

    public void SetItemObject(RectTransform item){
        DraggableItem = item;
    }

    public void ShowExtensions(RectTransform extensionObject)
    {
        DraggableItem = extensionObject;
        for (int y = 0 ; y < totalHeight; y++)
        {
            for (int x = 0; x < totalWidth; x++)
            {
                if(!slots[x,y].isOpen)
                {
                    GameObject blockHided = transform.GetChild(y*7+x).gameObject;
                    blockHided.SetActive(true);
                    blockHided.GetComponent<Image>().SetAlpha(0.5f);
                }
            }
        }
    }

    public void HideExtensions()
    {
        DraggableItem = null;
        for (int y = 0 ; y < totalHeight; y++)
        {
            for (int x = 0; x < totalWidth; x++)
            {
                if(!slots[x,y].isOpen)
                {
                    GameObject blockHided = transform.GetChild(y*7+x).gameObject;
                    blockHided.GetComponent<Image>().SetAlpha(1f);
                    blockHided.SetActive(false);
                }
            }
        }

    }

}


[System.Serializable]
public struct SlotData
{
    public bool isOpen;

    public RectTransform objectInInventory;
    public ItemData item;
}




[System.Serializable]
public class SlotSaveData
{
    public bool[] openStates;
    public SlotSaveData(bool[] states) { openStates = states; }
}