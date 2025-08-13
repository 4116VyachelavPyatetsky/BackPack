using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public class DraggableExtension :  MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        blockSystem.ShowExtensions(rectTransform);
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
        AlignToCenterOfObject(localPoint);
        blockSystem.ShowExtensionsStates(rectTransform);
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
        blockSystem.TrySetExtension();
        blockSystem.HideExtensions();
        if (blockSystem.extension) Destroy(gameObject);
        else rectTransform.localPosition = startPosition;
    }
}
