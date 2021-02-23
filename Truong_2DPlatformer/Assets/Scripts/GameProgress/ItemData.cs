using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// stores data about specific key, was it used, is it in itemSlot is it on map, is it placed in dropTarget
/// </summary>
[Serializable]
public struct ItemData
{
    public string uid;
    public string slotUid;
    public string dropTargetUid;
    public ItemState state;
    public SerializableVector3 position;

    public ItemData(string itemUid, string slotUid, string dropTargetUid, ItemState state, UnityEngine.Vector3 position)
    {
        this.uid = itemUid;
        this.slotUid = slotUid;
        this.dropTargetUid = dropTargetUid;
        this.state = state;
        this.position = new SerializableVector3(position);    }

    public void UpdateState()
    {
        if (slotUid.IsValidGuid())
        {
            state = ItemState.InInventory;
        }
        else if (dropTargetUid.IsValidGuid())
            state = ItemState.PlacedOnDropTarget;
        else
            state = ItemState.Unknown;
    }
}