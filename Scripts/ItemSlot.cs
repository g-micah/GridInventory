using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour//, IDropHandler 
{
    [System.NonSerialized]
    public Draggable dockedDraggable;

    private Image image;
    private bool isSelected;
    private bool isDocked;

    private void Awake()
    {
        image = GetComponent<Image>();
        dockedDraggable = null;
    }

    // When dropped on a slot
    public static bool Dropping(Draggable itemDropping)
    {
        if (itemDropping.IsDroppable())
        {
            itemDropping.DockToActiveSlots(itemDropping.GetBottomRightActiveSlot().transform);

            return true;
        }
        return false;
    }

    public bool IsSelected
    {
        get 
        {
            return isSelected;
        }
        set
        {
            if(!isDocked)
            {
                if (value)
                {
                    if (Draggable.itemBeingDragged.GetComponent<Draggable>().IsDroppable())
                    {
                        image.color = new Color32(0, 255, 0, 50);
                    }
                    else
                    {
                        image.color = new Color32(255, 0, 0, 50);
                    }
                }
                else
                {
                    image.color = new Color32(70, 71, 84, 163);
                }

                isSelected = value;
            }
        } 
    } // END IsSelected
    public bool IsDocked
    {
        get
        {
            return isDocked;
        }
        set
        {
            if (value)
            {
                image.color = new Color32(84, 85, 99, 255);
                isDocked = true;
            }
            else
            {
                dockedDraggable = null;
                isDocked = false;
            }
        }
    }// END IsOccupied


    /*
    public void OnDrop(PointerEventData eventData)
    {
        itemDropping = (Draggable)Draggable.itemBeingDragged.GetComponent(typeof(Draggable));

        if (itemDropping.IsDroppable())
        {
            itemDropping.DockToActiveSlots(itemDropping.GetBottomRightActiveSlot().transform);
        }
    }*/
}
