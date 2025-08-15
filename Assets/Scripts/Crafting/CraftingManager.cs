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
    public GameObject constructionScreenUI;

    public List<string> inventoryItemList = new List<string>();

    //category buttons 
    Button toolsBTN, constuctionBTN;


    //category buttons 
    Button craftAxeBTN, craftFoundationBTN, craftWallBTN;

    //Requirment Text
    Text AxeReq1, AxeReq2;

    Text ConReq1, WallReq1;

    public bool isOpen;//check if screen is open

    //All Blueprint 
    public CraftingBlueprint AxeBLP = new CraftingBlueprint("Axe",1, 2, "Stone", 3, "Stick", 3);
    public CraftingBlueprint FoundationBLP = new CraftingBlueprint("Foundation", 1, 1, "Stick", 3, "", 0);
    public CraftingBlueprint WallBLP = new CraftingBlueprint("Wall", 1, 1, "Stick", 3, "", 0);
    //internal object constructionScreenUI;

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

        constuctionBTN = craftingScreenUI.transform.Find("ConstructionButton").GetComponent<Button>();
        constuctionBTN.onClick.AddListener(delegate { OpenConstructionCategory(); });


        //AXE
        AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<Text>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<Text>();

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("AxeButton").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });


        //Foundation
        ConReq1 = constructionScreenUI.transform.Find("Foundation").transform.Find("req1").GetComponent<Text>();

        craftFoundationBTN = constructionScreenUI.transform.Find("Foundation").transform.Find("FoundationButton").GetComponent<Button>();
        craftFoundationBTN.onClick.AddListener(delegate { CraftAnyItem(FoundationBLP); });
        
        //Wall
        WallReq1 = constructionScreenUI.transform.Find("Wall").transform.Find("req1").GetComponent<Text>();
        
        craftWallBTN = constructionScreenUI.transform.Find("Wall").transform.Find("WallButton").GetComponent<Button>();
        craftWallBTN.onClick.AddListener(delegate { CraftAnyItem(WallBLP); });

    }
    void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
        constructionScreenUI.SetActive(false);

    }
    void OpenConstructionCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        constructionScreenUI.SetActive(true);

    }


    void CraftAnyItem(CraftingBlueprint blueprintToCraft)
    {
        //produce the numebr of items based on the blue print 
        for (var i = 0; i < blueprintToCraft.numOfItemsToProduce; i++)
        {
            //add items into inventory
            InventorySystem.Instance.AddInventory(blueprintToCraft.itemName);
        }
       
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
       
    }

    public IEnumerator calculate()
    {
        yield return 0;//
        InventorySystem.Instance.ReCalculateList();
        RefreshNeededItem();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isOpen && !ConstructionManager.Instance.inConstructionMode)
        {
            Debug.Log("C is pressed");
            craftingScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false; ;

            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);
            constructionScreenUI.SetActive(false);
            if (!InventorySystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                SelectionManager.Instance.EnabledSelection();
                SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;


            }

            isOpen = false;
        }
    }


    public void RefreshNeededItem() // a bit hard coded to check if an item can be crafted 
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
        if (stone_count >= 3 && stick_count >= 3 && InventorySystem.Instance.CheckSlotsAvailable(1))
        {

            craftAxeBTN.gameObject.SetActive(true); //enough materials


        }
        else
        {
            craftAxeBTN.gameObject.SetActive(false); //not enough materials

        }

        ///-----Foundation-----//
        ConReq1.text = "3 Stick [" + stick_count + "]";

        //checks if there is enough materials
        if (stick_count >= 3 && InventorySystem.Instance.CheckSlotsAvailable(1))
        {
            craftFoundationBTN.gameObject.SetActive(true); //enough materials
        }
        else
        {
            craftFoundationBTN.gameObject.SetActive(false); //not enough materials

        }

        ///-----Wall-----//
        WallReq1.text = "3 Stick [" + stick_count + "]";

        //checks if there is enough materials
        if (stick_count >= 3 && InventorySystem.Instance.CheckSlotsAvailable(1))
        {
            craftWallBTN.gameObject.SetActive(true); //enough materials
        }
        else
        {
            craftWallBTN.gameObject.SetActive(false); //not enough materials

        }

    }
}
