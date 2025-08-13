using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{

    public static InventorySystem Instance { get; set; }

    public GameObject inventoryScreenUI;

    public List<GameObject> slotList = new List<GameObject>();

    public List<string> itemList = new List<string>();

    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;

    public bool isOpen;

    // public bool isFull;

    // pickup notifs!!
    public GameObject pickupAlert;
    public Text pickupName;
    public Image pickupImage;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    void Start()
    {
        isOpen = false;

        PopulateSlotList();

    }

    private void PopulateSlotList()
    {
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {
            Debug.Log("i is pressed");
            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            inventoryScreenUI.SetActive(false);
            if (!CraftingManager.Instance.isOpen)
            { //if closed lock the screen 
                Cursor.lockState = CursorLockMode.Locked;
            }

            isOpen = false;
        }
    }

    public void AddInventory(string itemName)
    {
        whatSlotToEquip = FindNextEmptySlot();
        GameObject prefab = Resources.Load<GameObject>(itemName);
        if (prefab == null)
        {
            Debug.LogError("Failed to load item prefab from Resources with name: " + itemName);
            return;
        }
        itemToAdd = Instantiate(Resources.Load<GameObject>(itemName), whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
        itemToAdd.transform.SetParent(whatSlotToEquip.transform);

        itemList.Add(itemName);
        TriggerPickupNotif(itemName, itemToAdd.GetComponent<Image>().sprite);

        ReCalculateList();
        CraftingManager.Instance.RefreshNeededItem();
    }

    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }

    public bool CheckIfFull()
    {
        int counter = 0;
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }
        if (counter == 21)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void RemoveItem(string nameToRemove, int numToRemove)
    {
        int counter = numToRemove;
        for (var i = slotList.Count - 1; i >= 0; i--)
        { // start backwards
            if (slotList[i].transform.childCount > 0)
            {//checks if slot has a child
                if (slotList[i].transform.GetChild(0).name == nameToRemove + "(Clone)" && counter != 0)
                {
                    Destroy(slotList[i].transform.GetChild(0).gameObject);
                    counter -= 1;
                }
            }
        }

        ReCalculateList();
        CraftingManager.Instance.RefreshNeededItem();
    }

    public void ReCalculateList()
    {
        itemList.Clear();
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                string name = slot.transform.GetChild(0).name; //item (Clone)
                //string str1 = name;
                //string str2 = "(Clone)";

                string result = name.Replace("(Clone)", ""); //remove the 2nd string

                itemList.Add(result);
            }
        }
    }
    void TriggerPickupNotif(string itemName, Sprite itemSprite)
    {
        pickupAlert.SetActive(true);
        pickupName.text = itemName + " GET!!!";
        pickupImage.sprite = itemSprite;
        StartCoroutine(notifFade());
    }

    public IEnumerator notifFade()
    {
        yield return new WaitForSeconds(3);
        pickupAlert.SetActive(false);
        yield break;
    }
}