using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaloriesBar : MonoBehaviour
{
    private Slider slider;
    public Text hungerCounter;

    public GameObject playerState;

    private float currentHunger, maxHunger;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        currentHunger = playerState.GetComponent<PlayerState>().currentHunger;
        maxHunger = playerState.GetComponent<PlayerState>().maxHunger;

        float fillValue = currentHunger / maxHunger;
        slider.value = fillValue;

        hungerCounter.text = currentHunger + "/" + maxHunger;
    }
}
