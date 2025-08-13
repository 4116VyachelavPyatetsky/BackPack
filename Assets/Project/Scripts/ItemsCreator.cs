using System.Collections.Generic;
using UnityEngine;

public class ItemsCreator : MonoBehaviour
{
    public GameObject[] Items;

    public StageManager manager;
    public int amountOfItemsToSpawn = 3;

    const int reafreshCost = 15;
    private Vector2 startPosition = new Vector2(-285, -520);
    private Vector2 offset = new Vector2(285, 0);

    private List<GameObject> nowObjects = new List<GameObject>();

    void Start()
    {
        SpawnItems();
    }
    public void SpawnItems()
    {
        foreach (var obj in nowObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        nowObjects.Clear();

        for (int i = 0; i < amountOfItemsToSpawn; i++)
        {
            GameObject prefab = Items[Random.Range(0, Items.Length)];
            GameObject newObj = Instantiate(prefab, transform.parent);

            RectTransform rect = newObj.GetComponent<RectTransform>();

            rect.anchoredPosition = startPosition + offset * i;

            nowObjects.Add(newObj);
        }
    }

    public void ChangeAmmountOfItems(int ammount = 1)
    {
        amountOfItemsToSpawn += ammount;
        offset = new Vector2(startPosition.x * -2 / (amountOfItemsToSpawn - 1) , 0);

    }

    public void RefreshItems()
    {
        if (manager.SpendMoney(reafreshCost))
        {
            SpawnItems();
        }
    }

    public void RemoveFromList(GameObject obj)
    {
        if (nowObjects.Contains(obj))
        {
            nowObjects.Remove(obj);
        }
    }

}
