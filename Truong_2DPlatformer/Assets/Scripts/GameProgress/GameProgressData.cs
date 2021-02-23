using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Plain Old CLR Object (POCO)
/// </summary>
[Serializable]
public class GameProgressData
{
    public string uid;
    /// <summary>
    /// ticks
    /// </summary>
    public long createdDate;
    public DateTime CreatedDate { get { return new DateTime(createdDate); } set { createdDate = value.Ticks; } }

    /// <summary>
    /// game was reset, initial positions etc. are not set up so data should not be applied to scene when game starts, it will be saved on exit
    /// </summary>
    public bool isFreshSave;


    public SerializableVector3 playerPosition;
    public float playerScaleX = 1;

    public List<ItemSlotData> ItemSlots;
    public List<ItemData> Items;

    public bool IsIdentical(GameProgressData dataToTest)
    {
        var fields = this.GetType().GetFields(System.Reflection.BindingFlags.Public);
        foreach (var field in fields)
        {
            //this will work if we dont use reference types
            if (field.GetValue(this) != field.GetValue(dataToTest))
                return false;
        }
        return true;
    }

    public void SaveItemSlot(string slotUid, string itemUid)
    {
        if (ItemSlots == null)
            ItemSlots = new List<ItemSlotData>();

        var slotItemDataToSave = ItemSlots.FirstOrDefault(i => i.uid == slotUid);
        if (slotItemDataToSave.uid != slotUid)
        {
            //for new save fill slot uid, if exists we will only update itemUid 
            slotItemDataToSave.uid = slotUid;
            slotItemDataToSave.itemUid = itemUid;
            ItemSlots.Add(slotItemDataToSave);
        }
        else
        {
            int index = ItemSlots.IndexOf(slotItemDataToSave);
            //update and replace
            slotItemDataToSave.itemUid = itemUid;
            ItemSlots[index] = slotItemDataToSave;
        }
    }
    public void SaveItem(string itemUid, string slotUid, string dropTargetUid)
    {
        if (Items == null)
            Items = new List<ItemData>();

        var itemDataToSave = Items.FirstOrDefault(i => i.uid == itemUid);
        if (itemDataToSave.uid != itemUid)
        {
            //if item data not present add new entry)
            itemDataToSave.uid = itemUid;
            Items.Add(itemDataToSave);
        }

        int index = Items.IndexOf(itemDataToSave);
        //fill new data
        itemDataToSave.slotUid = slotUid;
        itemDataToSave.dropTargetUid = dropTargetUid;
        itemDataToSave.UpdateState();
        Items[index] = itemDataToSave;
    }
}
