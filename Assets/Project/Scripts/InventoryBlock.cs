using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryBlock : MonoBehaviour
{
    public int positionX;
    public int positionY;

    BlockSystem blockSystem;

    void Awake()
    {
        blockSystem = transform.parent.GetComponent<BlockSystem>();
    }
    public void Setup(BlockSystem blockSystem, int x, int y)
    {
        this.blockSystem = blockSystem;
        positionX = x;
        positionY = y;
    }
}
