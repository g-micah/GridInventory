// TODO: Make trash button change color when hovering while dragging an item

using UnityEngine;
using UnityEngine.EventSystems;

public class TrashSlot : MonoBehaviour, IDropHandler
{
    private Draggable itemDropping;
    


    public void OnDrop(PointerEventData eventData)
    {
        itemDropping = Draggable.itemBeingDragged.GetComponent<Draggable>();

        if (!itemDropping.ComingFromGiver())
        {
            Destroy(itemDropping.gameObject);
        }
    }

}
