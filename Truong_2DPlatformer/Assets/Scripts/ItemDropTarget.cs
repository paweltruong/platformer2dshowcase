using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// should me added to the object that items or keys can be dropped into (unlocked)
/// </summary>
public class ItemDropTarget : MonoBehaviour
{
    [SerializeField] string uid;
    [Tooltip("Uid of items that can be placed here")]//TODO: for pickups like regen potions etc should be added new propety with ObjectType(because their uids can be generated at runtime)
    [SerializeField] string[] validItemUids;

    [Tooltip("For positioning placed items")]
    [SerializeField] float placementOffsetX;
    [Tooltip("For positioning placed items")]
    [SerializeField] float placementOffsetY;

    public UnityEvent onItemPlaced;

    public string Uid=>uid;

    /// <summary>
    /// Place item in itemDropTarget (reparent, reposition)
    /// </summary>
    /// <param name="item"></param>
    public void PlaceItem(DraggableItem item)
    {
        var newItemPosition = this.transform.position;
        newItemPosition.x += placementOffsetX;
        newItemPosition.y += placementOffsetY;
        item.transform.position = newItemPosition;
        onItemPlaced.Invoke();
    }

    /// <summary>
    /// check if dropTarget is valid placement for item with Uid
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool IsValidForItem(string itemUid)
    {
        if (validItemUids != null && validItemUids.Any(uid => uid == itemUid))
            return true;
        return false;
    }
}
