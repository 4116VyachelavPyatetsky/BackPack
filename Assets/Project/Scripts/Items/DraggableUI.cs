using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private bool inInventory = false;


    public BlockSystem blockSystem;  // назначай из инспектора или ищи в Awake


    private Vector3 startPosition;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        blockSystem = FindFirstObjectByType<BlockSystem>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        blockSystem.StartHighlight(rectTransform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint
        );
        if (inInventory) blockSystem.ClearSlotsByObject(rectTransform);
        AlignToCenterOfObject(localPoint);
        blockSystem.ShowBlocksStates(rectTransform);
    }

    public void AlignToCenterOfObject(Vector2 localPoint)
    {
        var thisData = GetComponent<IHasItemData>().ItemData;
        float offsetX = (thisData.config.width);
        float offsetY = (thisData.config.height);
        localPoint.x += (offsetX * 55f);
        localPoint.y += (offsetY * 55f);
        rectTransform.localPosition = localPoint;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if (blockSystem.canMerge)
        {
            CombineWith(blockSystem.mergeItem.GetComponent<DraggableUI>());
            InventoryManager.Instance.RemoveItem(GetComponent<IHasItemData>().ItemData);
            Destroy(gameObject);
        }
        else
        {
            if (!blockSystem.canPlace)
            {
                rectTransform.anchoredPosition = startPosition;
                if(inInventory)blockSystem.TrySetObject(true);
            }
            else 
            {
                blockSystem.TrySetObject();
                if (!inInventory) 
                {
                    inInventory = true;
                    InventoryManager.Instance.AddItem(GetComponent<IHasItemData>().ItemData, rectTransform.anchoredPosition);
                }
                ItemsCreator creator = FindFirstObjectByType<ItemsCreator>();
                if (creator != null)
                {
                    creator.RemoveFromList(gameObject);
                }
            }
        }
        blockSystem.StopHighlight();
        blockSystem.ResetAllBlocksColor();
    }

    private void CombineWith(DraggableUI other)
    {
        other.GetComponent<ItemView>().Upgrade();
    }
}
