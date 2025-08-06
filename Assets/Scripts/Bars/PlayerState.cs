using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; set; }

    // Player Health
    public float currentHealth;
    public float maxHealth;

    // Player Hunger
    public float currentHunger;
    public float maxHunger;

    float distanceTravelled = 0;
    Vector3 lastPosition;

    public GameObject playerBody;


    // Player Hydration
    public float currentHydration;
    public float maxHydration;

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
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentHydration = maxHydration;
        StartCoroutine(decreaseHydration());
    }

    IEnumerator decreaseHydration()
    {
        while (true)
        {
            currentHydration -= 1;
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator decreaseHealth()
    {
        while (currentHunger == 0)
        {
            currentHealth -= 1;

            if (currentHunger != 0 || currentHealth == 0)
            {
                // reset stats for testing because we don't have a game over state yet lul
                currentHealth = maxHealth;
                currentHunger = maxHunger;
                yield break;
            }

            yield return new WaitForSeconds(2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        distanceTravelled += Vector3.Distance(playerBody.transform.position, lastPosition);
        lastPosition = playerBody.transform.position;

        if (distanceTravelled >= 5 && currentHunger != 0)
        {
            distanceTravelled = 0;
            currentHunger -= 1;

            if (currentHunger == 0)
            {
                StartCoroutine(decreaseHealth());
            }
        }
    }
}
