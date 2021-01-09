using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject invMenu;

    [SerializeField] private int invMenuRows;
    private int slotAmount;
    public static bool[] slotFilled;
    public static List<ItemSlot> itemSlots;

    // Start is called before the first frame update
    void Start()
    {
        itemSlots = new List<ItemSlot>();

        if (invMenuRows == 0)
        {
            invMenuRows = 8;
        }

        slotAmount = invMenu.transform.childCount;
        slotFilled = new bool[slotAmount];
        
        for (int i = 0; i < slotAmount; i++)
        {
            itemSlots.Add((ItemSlot)invMenu.transform.GetChild(i).GetComponent(typeof(ItemSlot)));
        }
    }

}
