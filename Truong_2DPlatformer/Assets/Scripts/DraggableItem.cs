using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]//Needed for overriding Sorting Order in worldSpaceCanvas
[RequireComponent(typeof(CanvasGroup))]//Needed for allowing to drop on ItemSlot(UIRaycasting)
public class DraggableItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] string uid = System.Guid.NewGuid().ToString();
    [Tooltip("Layer to make sure that dragged item is not below map elements. This is also layer when placed inside itemSlot")]
    [SerializeField] string sortingLayerWhenDragged = "UI";
    [SerializeField] int orderInLayerWhenDragged;
    [Tooltip("Layers of objects that this item can be dropped onto")]
    [SerializeField] LayerMask dropTargetLayer;
    [Tooltip("Collider that ensures that player is inside this collider when trying to pick this object up (should be trigger)")]
    [SerializeField] Collider2D pickupRangeCollider;
    [Tooltip("Player's layer")]//TODO: if npc's can pickup objects they can be added here too
    [SerializeField] LayerMask whoCanPickup;

    string initialSortingLayer;
    int initialOrderInLayer;
    public Vector3 initialPosition;
    Transform initialParent;

    bool isAttachedToItemSlot;
    Vector3 itemSlotPosition;
    /// <summary>
    /// when item was used or placed in dropTarget the this field is true
    /// </summary>
    bool alreadyUsed;

    Camera mainCamera;
    Canvas canvas;
    CanvasGroup canvasGroup;
    /// <summary>
    /// will be populated at start with supportet item slots handles
    /// </summary>
    ItemSlot[] itemSlots;
    bool isDragged;

    public string Uid => uid;
    public string SlotUid { get; private set; }
    /// <summary>
    /// uid of object in the level that item was dropped on
    /// </summary>
    public string DropTargetId { get; private set; }

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        SetInitialValues();
    }

    void Start()
    {
        mainCamera = Camera.main;
        itemSlots = FindObjectsOfType<ItemSlot>();
    }

    private void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 && isDragged)
        {
            if (isAttachedToItemSlot)
                ResetPositionInSlot();
            else
                RestoreInitialValues();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!alreadyUsed)
        {
            //allow dragging from item slot or from level if in pickup range
            bool isInRange = false;
            if (isAttachedToItemSlot
                || (isInRange = IsInPickupRange(eventData)))
            {
                //allow to detect drop (fe. on item slots), this wont catch UIraycasts when dragging
                canvasGroup.blocksRaycasts = false;
                SetHigherSortingLayer();
                ToggleItemSlotHighlights(true);
                isDragged = true;

            }
            if (!isAttachedToItemSlot && !isInRange)
                HelperText.DisplayText("Out of range", 1f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragged)
        {
            var position = mainCamera.ScreenToWorldPoint(eventData.position);
            position.z = 0;//mouse Z sometimes messes up displaying object in camera
            this.transform.position = position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragged)
        {
            if (!isAttachedToItemSlot)
            {
                //if we allow to drop item directly from level not from inventory
                if (!TryPlaceItem(eventData.position))
                {
                    //restore item to its original position on the map
                    RestoreInitialValues();
                }
            }
            else
            {
                //draging out from item slot
                if (!TryPlaceItem(eventData.position))
                {
                    //restore item to its item slot(dropped on invalid place but is part of player's inventory,then return to assigned item slot)
                    ResetPositionInSlot();
                }
            }

            //restore drag for this object
            canvasGroup.blocksRaycasts = true;
            ToggleItemSlotHighlights(false);
            isDragged = false;
        }
    }

    /// <summary>
    /// check if player is in item's pickup range
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    bool IsInPickupRange(PointerEventData eventData)
    {
        Collider2D[] results = new Collider2D[1];
        pickupRangeCollider.OverlapCollider(new ContactFilter2D { layerMask = whoCanPickup, useLayerMask = true }, results);
        if (results != null && results[0] != null)
            return true;
        return false;
    }

    /// <summary>
    /// check if item is being dropped on valid itemDropTarget and if operation was successful
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns>true if successfuly placed object in itemDropTarget</returns>
    bool TryPlaceItem(Vector3 mousePosition)
    {
        //get collision ray with object we are dropping item on
        var ray = mainCamera.ScreenPointToRay(mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
        RaycastHit2D[] results = new RaycastHit2D[1];
        var ra = Physics2D.GetRayIntersection(ray, 100f, dropTargetLayer);
        if (ra.collider != null)
        {
            //found object we are dropping on
            var itemDropTarget = ra.collider.GetComponent<ItemDropTarget>();
            return PlaceOnDropTarget(itemDropTarget);
        }
        return false;
    }

    public bool PlaceOnDropTarget(ItemDropTarget dropTarget)
    {
        if (dropTarget == null)
            Debug.LogError($"Cannot find {nameof(dropTarget)} component");

        //is drop target valid?can we drop this item here?(f.e this key's uid matches the keypillar's allowed uids)
        if (dropTarget.IsValidForItem(this.Uid))
        {
            //empty item slot of current item
            var slot = GetAttachedItemSlot();
            if (slot == null)
            {
                //slot might be null when loading already placed object
                SlotUid = null;
            }
            else
            {
                //placing item in dropTarget so need to empty the inventory slot it occupies
                slot.SetSlotState(null);
                SlotUid = null;
            }

            //restore parent to worldPointCanvas and then perform placement positioning
            RestoreInitialValues();
            dropTarget.PlaceItem(this);
            DropTargetId = dropTarget.Uid;
            alreadyUsed = true;
            return true;
        }
        else
        {
            HelperText.DisplayText(StringResources.HelpMessages.Item_InvalidPlacement, Constants.HelpMessage_ShortDuration);
            Debug.LogWarning(string.Format(StringResources.System_Messages.DropTarget_ValidationFailedFormat,
                dropTarget.name, this.name, this.Uid));
            return false;
        }
    }

    void SetInitialValues()
    {
        initialPosition = transform.position;
        initialSortingLayer = canvas.sortingLayerName;
        initialOrderInLayer = canvas.sortingOrder;
        initialParent = this.transform.parent;
    }

    void RestoreInitialValues()
    {
        transform.SetParent(initialParent);
        transform.position = initialPosition;
        canvas.sortingLayerName = initialSortingLayer;
        canvas.sortingOrder = initialOrderInLayer;
    }


    void SetHigherSortingLayer()
    {
        //to avoid dragged object hiding behind props & decals, and to draw on top of item slot
        canvas.sortingLayerName = sortingLayerWhenDragged;
        canvas.sortingOrder = orderInLayerWhenDragged;
    }

    /// <summary>
    /// highlight item slots and show availability colors
    /// </summary>
    /// <param name="enable"></param>
    void ToggleItemSlotHighlights(bool enable)
    {
        foreach (var itemSlot in itemSlots)
            itemSlot.ToggleAvailabilityColors(enable);
    }

    public void AttachToItemSlot(ItemSlot slot)
    {
        //check if moving between slots
        if (isAttachedToItemSlot)
        {
            //empty old slot
            var oldSlot = GetAttachedItemSlot();
            oldSlot.SetSlotState(null);
        }
        slot.SetSlotState(slot.Uid);

        this.transform.SetParent(slot.transform);

        //items have rect anchors to center, so zero will align in the middle of item slot
        ResetPositionInSlot();
        SetHigherSortingLayer();
        isAttachedToItemSlot = true;
        SlotUid = slot.Uid;
    }

    /// <summary>
    /// reset anchored position to 0,0
    /// </summary>
    void ResetPositionInSlot()
    {
        var myRect = GetComponent<RectTransform>();
        myRect.anchoredPosition = Vector3.zero;
    }

    ItemSlot GetAttachedItemSlot()
    {
        var found = itemSlots.FirstOrDefault(itemSlot => itemSlot.gameObject == transform.parent.gameObject);
        if (found != null)
            return found.GetComponent<ItemSlot>();
        return null;
    }
}
