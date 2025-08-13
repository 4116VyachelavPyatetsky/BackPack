using UnityEngine;
using System.Collections.Generic;

public class FightItemCreator : MonoBehaviour
{
    private List<GameObject> fightItems = new List<GameObject>();

    public List<StringGameObjectPair> itemsFightPrefabs = new List<StringGameObjectPair>();

    public void CreateNewIco(ItemData itemData)
    {
        var pair = itemsFightPrefabs.Find(p => p.name == itemData.config.name);
        GameObject newIco = Instantiate(pair.prefab, transform);
        newIco.GetComponent<ItemInFight>().itemData = itemData;
        fightItems.Add(newIco);
    }

    public void RemoveIco(ItemData itemData)
    {
        var obj = FindIcoByItemData(itemData);
        if (obj != null)
        {
            fightItems.Remove(obj);
            Destroy(obj);
        }
    }


    public GameObject FindIcoByItemData(ItemData itemData)
    {
        return fightItems.Find(go =>
        {
            var comp = go.GetComponent<ItemInFight>();
            return comp != null && comp.itemData == itemData;
        });
    }

    public void StartFightingItems()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<ItemInFight>().StartReload();
        }
    }

    public void StopFightingItems()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<ItemInFight>().EndReload();
        }
    }

}

[System.Serializable]
public class StringGameObjectPair
{
    public string name;
    public GameObject prefab;
}
