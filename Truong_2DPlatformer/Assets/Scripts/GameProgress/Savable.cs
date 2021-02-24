using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// base class for savable objects, might be useful as separete class when migrating to ecs (pillar keys are not savable but have uid)
/// </summary>
public class Savable : MonoBehaviour
{
    [SerializeField] string uid;
    [SerializeField] ObjectType gameObjectType;


    public void SaveProgress(IGameProgress gameProgress)
    {
        if (gameProgress.Current != null)
        {
            switch (gameObjectType)
            {
                case ObjectType.PlayerCharacter:
                    SavePlayerData(gameProgress);
                    break;
                case ObjectType.ItemSlot:
                    SaveItemSlotData(gameProgress);
                    break;
                case ObjectType.Key:
                    SaveItemData(gameProgress);
                    break;
                default:
                    break;
            }
        }
    }

    public void LoadProgress(IGameProgress gameProgress)
    {
        if (gameProgress.Current != null)
        {
            switch (gameObjectType)
            {
                case ObjectType.PlayerCharacter:
                    LoadPlayerData(gameProgress);
                    break;
                case ObjectType.ItemSlot:
                    LoadItemSlotData(gameProgress);
                    break;
                case ObjectType.Key:
                    LoadItemData(gameProgress);
                    break;
                default:
                    break;
            }
        }
    }


    void SavePlayerData(IGameProgress gameProgress)
    {
        gameProgress.Current.playerPosition = new SerializableVector3(transform.position);
        gameProgress.Current.playerScaleX = transform.localScale.x;
    }
    void LoadPlayerData(IGameProgress gameProgress)
    {
        var characterMove = GetComponent<Character>();
        if (characterMove == null)
            Debug.LogError($"{name} does not have {nameof(Character)} component, check {nameof(Savable)}.{nameof(gameObjectType)}");

        transform.position = gameProgress.Current.playerPosition.ToVector3();
        characterMove.SetDirection(gameProgress.Current.playerScaleX);
        Debug.Log($"PlayerScaleX loaded: {gameProgress.Current.playerScaleX}");
    }



    private void SaveItemSlotData(IGameProgress gameProgress)
    {
        var itemSlot = GetComponent<ItemSlot>();
        if (itemSlot == null)
            Debug.LogError($"{name} does not have {nameof(ItemSlot)} component, check {nameof(Savable)}.{nameof(gameObjectType)}");
        gameProgress.Current.SaveItemSlot(uid, itemSlot.ItemUid);
    }
    void LoadItemSlotData(IGameProgress gameProgress)
    {
        if (gameProgress.Current.ItemSlots == null)
            Debug.LogError($"Save data corrupted (no item slots)");

        var found = gameProgress.Current.ItemSlots.FirstOrDefault(its => its.uid == uid);
        if (found.uid == uid)
        {
            var itemSlot = GetComponent<ItemSlot>();
            if (itemSlot == null)
                Debug.LogError($"{name} does not have {nameof(ItemSlot)} component, check {nameof(Savable)}.{nameof(gameObjectType)}");

            itemSlot.SetSlotState(found.itemUid);
        }
        else
        {
            Debug.LogWarning($"No data found for slot {uid}");
        }
    }

    private void SaveItemData(IGameProgress gameProgress)
    {
        var item = GetComponent<DraggableItem>();
        if (item == null)
            Debug.LogError($"{name} does not have {nameof(DraggableItem)} component, check {nameof(Savable)}.{nameof(gameObjectType)}");
        gameProgress.Current.SaveItem(uid, item.SlotUid, item.DropTargetId);
    }
    void LoadItemData(IGameProgress gameProgress)
    {
        if (gameProgress.Current.Items == null)
            Debug.LogError($"Save data corrupted (no items)");

        var found = gameProgress.Current.Items.FirstOrDefault(its => its.uid == uid);
        if (found.uid == uid)
        {
            var item = GetComponent<DraggableItem>();
            if (item == null)
                Debug.LogError($"{name} does not have {nameof(DraggableItem)} component, check {nameof(Savable)}.{nameof(gameObjectType)}");

            switch (found.state)
            {
                case ItemState.InInventory:
                    var foundSlots = FindObjectsOfType<ItemSlot>();
                    var slot = foundSlots.FirstOrDefault(s => s.Uid == found.slotUid);
                    if (slot != null)
                        item.AttachToItemSlot(slot);
                    else
                        Debug.LogError($"Slot {found.slotUid} not found for item {found.uid}");
                    break;
                case ItemState.PlacedOnDropTarget:
                    var dropTargets = FindObjectsOfType<ItemDropTarget>();
                    var dropTarget = dropTargets.FirstOrDefault(dt => dt.Uid == found.dropTargetUid);
                    if (dropTarget != null)
                        item.PlaceOnDropTarget(dropTarget);
                    else
                        Debug.LogError($"DropTarget {found.dropTargetUid} not found for item {found.uid}");
                    break;
                default:
                    //place inside world
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"No data found for item {uid}");
        }
    }

}
