using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class SelectionManager : MonoBehaviour
{
    
    public static SelectionManager Instance { get; set; }
    public GameObject interaction_Info_UI;
    public bool onTarget;

    Text interaction_text;

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

            if (selectionTransform.GetComponent<InteractableObject>() && selectionTransform.GetComponent<InteractableObject>().playerInRange)
            {
                interaction_text.text = selectionTransform.GetComponent<InteractableObject>().GetItemName();
                interaction_Info_UI.SetActive(true);
                onTarget = true;
            }
            else // there is a hit object, but it does not have a thing.
            {
                interaction_Info_UI.SetActive(false);
                onTarget = false;
            }

        }
        else // if there is no hit object.
        {
            interaction_Info_UI.SetActive(false);
        }
    }
}