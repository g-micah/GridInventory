using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public GameObject myPrefab;
    public static GameObject itemBeingDragged;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform startParent;
    private Transform initialParent;
    private Vector3 initialPosition;
    private List<ItemSlot> startDockedSlots;
    private List<ItemSlot> collidedSlots;
    private List<ItemSlot> activeSlots;
    private List<int> activeSlotsIndex;
    private int sizeX;
    private int sizeY;

    private void Start()
    {
        canvas = GameObject.Find("InventoryCanvas").GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        startDockedSlots = new List<ItemSlot>();
        collidedSlots = new List<ItemSlot>();
        activeSlots = new List<ItemSlot>();
        activeSlotsIndex = new List<int>();

        initialParent = transform.parent;
        initialPosition = transform.position;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        itemBeingDragged = gameObject;
        startParent = transform.parent;
        
        sizeX = (int)Mathf.Round(rectTransform.rect.width / 70);
        sizeY = (int)Mathf.Round(rectTransform.rect.height / 70);

        canvasGroup.alpha = .5f;
        canvasGroup.blocksRaycasts = false;

        UndockCurrentSlots();

        transform.SetParent(canvas.transform);
        rectTransform.SetAsLastSibling();
    }
    public void OnDrag(PointerEventData eventData)
    {
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;     //This one does not center item on mouse
        transform.position = Input.mousePosition;                                     //This one centers item on mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // If item is not dropped on a slot
        if (!ItemSlot.Dropping(this))
        {
            activeSlots = new List<ItemSlot>(startDockedSlots);
            DockToActiveSlots(startParent);
        }
        // If item lands in spot and comes from initial parent
        else if (ComingFromGiver())
        {
            Instantiate(myPrefab, initialPosition, Quaternion.identity, initialParent);
        }
        itemBeingDragged = null;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!collidedSlots.Contains((ItemSlot)col.gameObject.GetComponent(typeof(ItemSlot))))
        {
            collidedSlots.Add((ItemSlot)col.gameObject.GetComponent(typeof(ItemSlot)));
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        activeSlots.Clear();
        activeSlotsIndex.Clear();

        ItemSlot closestObj = GetClosestSlot(Inventory.itemSlots, transform.position);

        activeSlots.Add((ItemSlot)closestObj.GetComponent(typeof(ItemSlot)));
        activeSlotsIndex.Add(collidedSlots.IndexOf(activeSlots[0]));

        if ((sizeX * sizeY) > 1)
        {
            // 0: not used
            // 1: left / top half
            // 2: right / bottom half
            ushort x = 0;
            ushort y = 0;
            if (sizeX % 2 == 0)
            {
                x = GetSlotHalf(true);
            }
            if (sizeY % 2 == 0)
            {
                y = GetSlotHalf(false);
            }
           //Debug.Log("X: " + x + "- Y: " + y);

            // Add enough activeSlots size of draggable to 
            AddExtraActiveSlots(x, y, Inventory.itemSlots.IndexOf(closestObj));
        }

        // Set correct item slots as selected and others as unselected
        for (int i = 0; i < collidedSlots.Count; i++)
        {
            collidedSlots[i].IsSelected = activeSlotsIndex.Contains(i);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (collidedSlots.Contains((ItemSlot)col.gameObject.GetComponent(typeof(ItemSlot))))
        {
            int index = collidedSlots.IndexOf((ItemSlot)col.gameObject.GetComponent(typeof(ItemSlot)));

            collidedSlots[index].IsSelected = false;
            collidedSlots.RemoveAt(index);

            if(collidedSlots.Count == 0)
            {
                activeSlots.Clear();
            }
        }
    }

    private void AddExtraActiveSlots(ushort x, ushort y, int starterSlotIndex)
    {
        int i;
        int j;
        int sizeLeft = (sizeX - 1) / 2;
        int sizeRight = sizeLeft;
        int sizeTop = (sizeY - 1) / 2;
        int sizeBottom = sizeTop;
        int menuX = starterSlotIndex % 8;
        int menuY = starterSlotIndex / 8;
        int newIndex;

        if (x == 1)
        {
            sizeLeft += (sizeX - 1) % 2;
        }
        else if (x == 2)
        {
            sizeRight += (sizeX - 1) % 2;
        }
        if (y == 1)
        {
            sizeTop += (sizeY - 1) % 2;
        }
        else if (y == 2)
        {
            sizeBottom += (sizeY - 1) % 2;
        }

        //Add slots to left
        for (i = 1; i <= sizeLeft; i++)
        {
            if (menuX - i >= 0)
            {
                newIndex = (menuX - i) + menuY * 8;

                activeSlots.Add((ItemSlot)Inventory.itemSlots[newIndex].GetComponent(typeof(ItemSlot)));
                activeSlotsIndex.Add(collidedSlots.IndexOf(activeSlots[activeSlots.Count - 1]));

                //Add slots above and below left
                for (j = 1; j <= sizeTop; j++)
                {
                    if (menuY - j >= 0)
                    {
                        newIndex = (menuX - i) + (menuY - j) * 8;

                        activeSlots.Add((ItemSlot)Inventory.itemSlots[newIndex].GetComponent(typeof(ItemSlot)));
                        activeSlotsIndex.Add(collidedSlots.IndexOf(activeSlots[activeSlots.Count - 1]));
                    }
                }

                for (j = 1; j <= sizeBottom; j++)
                {
                    if (menuY + j <= 5)
                    {
                        newIndex = (menuX - i) + (menuY + j) * 8;

                        activeSlots.Add((ItemSlot)Inventory.itemSlots[newIndex].GetComponent(typeof(ItemSlot)));
                        activeSlotsIndex.Add(collidedSlots.IndexOf(activeSlots[activeSlots.Count - 1]));
                    }
                }
            }
        }//END for loop

        //Add slots to right
        for (i = 1; i <= sizeRight; i++)
        {
            if (menuX + i <= 7)
            {
                newIndex = (menuX + i) + menuY * 8;

                activeSlots.Add((ItemSlot)Inventory.itemSlots[newIndex].GetComponent(typeof(ItemSlot)));
                activeSlotsIndex.Add(collidedSlots.IndexOf(activeSlots[activeSlots.Count - 1]));

                //Add slots above and below right
                for (j = 1; j <= sizeTop; j++)
                {
                    if (menuY - j >= 0)
                    {
                        newIndex = (menuX + i) + (menuY - j) * 8;

                        activeSlots.Add((ItemSlot)Inventory.itemSlots[newIndex].GetComponent(typeof(ItemSlot)));
                        activeSlotsIndex.Add(collidedSlots.IndexOf(activeSlots[activeSlots.Count - 1]));
                    }
                }

                for (j = 1; j <= sizeBottom; j++)
                {
                    if (menuY + j <= 5)
                    {
                        newIndex = (menuX + i) + (menuY + j) * 8;

                        activeSlots.Add((ItemSlot)Inventory.itemSlots[newIndex].GetComponent(typeof(ItemSlot)));
                        activeSlotsIndex.Add(collidedSlots.IndexOf(activeSlots[activeSlots.Count - 1]));
                    }
                }
            }
        }//END for loop

        //Add slots to top
        for (i = 1; i <= sizeTop; i++)
        {
            if (menuY - i >= 0)
            {
                newIndex = menuX + (menuY - i) * 8;

                activeSlots.Add((ItemSlot)Inventory.itemSlots[newIndex].GetComponent(typeof(ItemSlot)));
                activeSlotsIndex.Add(collidedSlots.IndexOf(activeSlots[activeSlots.Count - 1]));
            }
        }

        //Add slots to bottom
        for (i = 1; i <= sizeBottom; i++)
        {
            if (menuY + i <= 5)
            {
                newIndex = menuX + (menuY + i) * 8;

                activeSlots.Add((ItemSlot)Inventory.itemSlots[newIndex].GetComponent(typeof(ItemSlot)));
                activeSlotsIndex.Add(collidedSlots.IndexOf(activeSlots[activeSlots.Count - 1]));
            }
        }
    } //END  AddExtraActiveSlots


    private ushort GetSlotHalf(bool checkHorizontal)
    {
        if (checkHorizontal)
        {
            if (activeSlots[0].transform.position.x >= transform.position.x)
            {
                return 1; //Left Half
            }
            else
            {
                return 2; //Right Half
            }
        }
        else
        {
            if (activeSlots[0].transform.position.y <= transform.position.y)
            {
                return 1; //Top Half
            }
            else
            {
                return 2; //Bottom Half
            }
        }
    }

    private ItemSlot GetClosestSlot(List<ItemSlot> availableSlots, Vector3 position)
    {
        ItemSlot closestSlot = default;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (ItemSlot potentialTarget in availableSlots)
        {
            Vector3 directionToSlot = potentialTarget.transform.position - position;
            float dSqrToTarget = directionToSlot.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestSlot = potentialTarget;
            }
        }

        return closestSlot;
    }

    public ItemSlot GetBottomRightActiveSlot()
    {
        return GetClosestSlot(activeSlots, (itemBeingDragged.transform.position + new Vector3(5000, -5000, 0)));
    }

    public bool IsDroppable()
    {
        if (activeSlots.Count == sizeX * sizeY)
        {
            foreach (ItemSlot slot in activeSlots)
            {
                if (slot.IsDocked)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public void DockToActiveSlots(Transform newParent)
    {
        foreach (ItemSlot slot in activeSlots)
        {
            slot.IsDocked = true;
            slot.dockedDraggable = this;
        }
        transform.SetParent(newParent);
        rectTransform.anchoredPosition = new Vector3(((rectTransform.rect.width / -2) - 5), ((rectTransform.rect.height + 10) / 2), 0);
    }

    private void UndockCurrentSlots()
    {
        collidedSlots.Clear();
        startDockedSlots.Clear();

        // if item is already in a slot, undock the slots
        if (transform.parent != initialParent)
        {
            foreach (ItemSlot slot in Inventory.itemSlots)
            {
                if (slot.dockedDraggable == this)
                {
                    slot.IsDocked = false;
                    collidedSlots.Add(slot);
                    startDockedSlots.Add(slot);
                }
            }
        }
    }

    public bool ComingFromGiver()
    {
        return startParent == initialParent;
    }
}
