using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class CraftingManager : MonoBehaviour
{
    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI;

    public List<string> inventoryItemList = new List<string>();

    //category buttons 
    Button toolsBTN;

    //category buttons 
    Button craftAxeBTN;

    //Requirment Text
    Text AxeReq1, AxeReq2;

    public bool isOpen;//check if screen is open

    //All Blueprint 
    public CraftingBlueprint AxeBLP = new CraftingBlueprint("Axe", 2, "Stone", 3, "Stick", 3);


    public static CraftingManager Instance { get; set; }

    public void Awake()
    {
        if (Instance != null & Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;

        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });

        AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<Text>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<Text>();

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("AxeButton").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });


    }
    void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
    }

    void CraftAnyItem(CraftingBlueprint blueprintToCraft)
    {
        //add items into inventory 
        InventorySystem.Instance.AddInventory(blueprintToCraft.itemName);


        //remove resources from inventory 
        if (blueprintToCraft.numOfReq == 1) {
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
        }
        else if (blueprintToCraft.numOfReq == 2) {
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req2, blueprintToCraft.Req2amount);
       }
        // Show message once when crafting
        Debug.Log(blueprintToCraft.itemName + " created");

        //refresh list after removing/adding items
        //InventorySystem.Instance.ReCalculateList();
        StartCoroutine(calculate());
        
        RefreshNeededItem();
       
    }

    public IEnumerator calculate()
    {
        yield return new WaitForSeconds(0.1f);
        InventorySystem.Instance.ReCalculateList();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshNeededItem(); //want to refresh in every frame 
        if (Input.GetKeyDown(KeyCode.C) && !isOpen)
        {
            Debug.Log("C is pressed");
            craftingScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);
            if (!InventorySystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            isOpen = false;
        }
    }


    private void RefreshNeededItem() // a bit hard coded to check if an item can be crafted 
    {
        int stone_count = 0;
        int stick_count = 0;

        inventoryItemList = InventorySystem.Instance.itemList;

        foreach (string itemName in inventoryItemList)
        {
            switch (itemName)
            {
                case "Stone":
                    stone_count += 1;
                    break;

                case "Stick":
                    stick_count += 1;
                    break;

            }


        }

        ///-----AXE-----//
        AxeReq1.text = "3 Stone [" + stone_count + "]";
        AxeReq2.text = "3 Stick [" + stick_count + "]";

        //checks if there is enough materials
        if (stone_count >= 3 && stick_count >= 3)
        {

            craftAxeBTN.gameObject.SetActive(true); //enough materials
        }
        else
        {
            craftAxeBTN.gameObject.SetActive(false); //not enough materials
        }
    }
}
