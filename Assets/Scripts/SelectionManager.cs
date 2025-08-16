using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }

    public GameObject selectedObject;
    public GameObject interaction_Info_UI;
    public bool onTarget;
    Text interaction_text;

    public Image defaultDotImage;
    public Image pickupImage;

    public GameObject selectedStorageBox;
    public GameObject selectedCampfire;
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

    private void Start()
    {
        interaction_text = interaction_Info_UI.GetComponent<Text>();
        onTarget = false;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;
            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();
            if (interactable && interactable.playerInRange)
            {

                selectedObject = interactable.gameObject;
                interaction_text.text = interactable.GetItemName();
                interaction_Info_UI.SetActive(true);
                onTarget = true;

                if (interactable.CompareTag("pickable"))
                {
                    defaultDotImage.gameObject.SetActive(false);
                    pickupImage.gameObject.SetActive(true);
                }
                else
                {
                    defaultDotImage.gameObject.SetActive(true);
                    pickupImage.gameObject.SetActive(false);
                }

            }
            else //if there is a hit, but w/o a interactable script
            {
                interaction_Info_UI.SetActive(false);
                onTarget = false;
                defaultDotImage.gameObject.SetActive(true);
                pickupImage.gameObject.SetActive(false);
            }

            StorageBox storageBox = selectionTransform.GetComponent<StorageBox>();
            if (storageBox && storageBox.playerInRange && PlacementSystem.Instance.inPlacementMode == false)
            {
                interaction_text.text = "Open";
                interaction_Info_UI.SetActive(true);
                selectedStorageBox = storageBox.gameObject;

                if (Input.GetMouseButtonDown(0))
                {
                    StorageManager.Instance.OpenBox(storageBox);
                }
            }
            else
            {
                if (selectedStorageBox != null)
                {
                    selectedStorageBox = null;
                }
            }

        }

        else // if there is no hit object.
        {

            interaction_Info_UI.SetActive(false);
            onTarget = false;
            defaultDotImage.gameObject.SetActive(true);
            pickupImage.gameObject.SetActive(false);
        }


    }

    public void DisableSelection()
    {
        pickupImage.enabled = false;
        defaultDotImage.enabled = false;
        interaction_Info_UI.SetActive(false);

        selectedObject = null;

    }
      public void EnabledSelection()
    {
        pickupImage.enabled = true;
        defaultDotImage.enabled = true;
        interaction_Info_UI.SetActive(true);

    }
}