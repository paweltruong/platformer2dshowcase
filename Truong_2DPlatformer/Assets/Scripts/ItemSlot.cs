using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] string uid;
    [Tooltip("Highlight color when item can be placed in this slot")]
    [SerializeField] Color availableColor;
    [Tooltip("Highlight color when slot is occupied")]
    [SerializeField] Color unavailableColor;
    [SerializeField] Image availabilityImage;


    [Header("Status & EditorDebugging")]
    public bool showAvailabilityStatus;
    public bool isEmpty;

    Image backgroundImage;

    public string ItemUid { get; private set; }
    public string Uid=>uid;
    
    void Awake()
    {
        backgroundImage = GetComponent<Image>();
    }

    void LateUpdate()
    {
        SetAvailabilityColor();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isEmpty)
        {
            if (eventData.pointerDrag != null)
            {
                var draggable = eventData.pointerDrag.GetComponent<DraggableItem>();
                PutItemIntoSlot(draggable);
            }
        }
    }
   
    void SetAvailabilityColor()
    {
        if (showAvailabilityStatus)
            availabilityImage.color = this.isEmpty ? availableColor : unavailableColor;
        else
        {
            var transparentColor = new Color32(255, 255, 255, 0);
            availabilityImage.color = transparentColor;
        }
    }

    void PutItemIntoSlot(DraggableItem item)
    {
        if (item != null)
        {
            //Debug.Log($"{item.name} dropped on {name}");
            //logic is inside Item class (in order to simplify loading)
            item.AttachToItemSlot(this);
        }
    }

    /// <summary>
    /// show or hide availability highlight colors (used for displaying drag drop hint)
    /// </summary>
    /// <param name="enabled"></param>
    public void ToggleAvailabilityColors(bool enabled)
    {
        showAvailabilityStatus = enabled;
    }

    /// <summary>
    /// mark slot as empty
    /// </summary>
    void Empty()
    {
        ItemUid = string.Empty;
        isEmpty = true;
        backgroundImage.raycastTarget = true;//to reallow dropping on itemSlot
    }

    /// <summary>
    /// Set slot state according to provided data
    /// </summary>
    /// <param name="itemUid">null for setting slot as empty</param>
    public void SetSlotState(string itemUid = null)
    {
        if (itemUid.IsValidGuid())
        {
            //to prevent interruption draging item from slot disable raycast on this when not empty
            backgroundImage.raycastTarget = false;

            isEmpty = false;
            this.ItemUid = itemUid;
        }
        else
            Empty();
    }
}
