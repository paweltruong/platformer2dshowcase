using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// As struct, in case we will want to store additional information and data like item stack size,
/// types for items that are not static f.e bananas that spawn during play
/// </summary>
[Serializable]
public struct ItemSlotData
{
    public string uid;
    public string itemUid;

    public ItemSlotData(string slotUid, string itemUid)
    {
        this.uid = slotUid;
        this.itemUid = itemUid;
    }
}