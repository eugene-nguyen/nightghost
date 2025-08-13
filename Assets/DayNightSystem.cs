using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public Light directionalLight;
    public float dayDurationInSeconds = 24.0f; //adjusts the duration of a full fay in seconds
    public int currentHour;
    float currrentTimeOfDay = 0.35f; //eqals 8 in the morning

    public List<SkyboxTimeMapping> timeMappings;

    float blendedValue = 0.0f;
    bool lockNextDayTrigger = false;


    public TextMeshProUGUI timeUI;
    // Update is called once per frame
    void Update()
    {
        //calculate the current time od day based on the game time
        currrentTimeOfDay += Time.deltaTime / dayDurationInSeconds;
        currrentTimeOfDay %= 1; //ensures it stays between 0 and 1

        currentHour = Mathf.FloorToInt(currrentTimeOfDay * 24);


        timeUI.text = $"{currentHour}:00";
        //update the directional light's rotation
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3((currrentTimeOfDay * 365) - 90, 170, 0));

        //update the skybox mateiral based on the time of day 
        UpdateSkybox();

    }

    private void UpdateSkybox()
    {
        Material currentSkybox = null;
        foreach (SkyboxTimeMapping mapping in timeMappings)
        {
            if (currentHour == mapping.hour)
            {
                currentSkybox = mapping.skyboxMaterial;

                if (currentSkybox.shader != null)
                {
                    if (currentSkybox.shader.name == "Custom/SkyboxTransition") //checks shader being used

                    {
                        blendedValue += Time.deltaTime; //increase the blended value 
                        blendedValue = Mathf.Clamp01(blendedValue); // want the value to be between 0 and 1
                        currentSkybox.SetFloat("_TransitionFactor", blendedValue); //changes the sliders value 
                    }
                   
                    else
                    {
                        blendedValue = 0.0f;
                    }
                }
                break;
            }
        }

        if (currentHour == 0 && lockNextDayTrigger == false) //if midnight and the lock next day is false, trigger the next day
        {
            TimeManager.Instance.TriggerNextDay();
            lockNextDayTrigger = true;
        }
        if (currentHour != 0) {
            lockNextDayTrigger = false;
        }

        if (currentSkybox != null)
        {
            RenderSettings.skybox = currentSkybox;
        }
    }

    [System.Serializable]
    public class SkyboxTimeMapping
    {
        public string phaseName;
        public int hour; // the hour of the day (0-23)
        public Material skyboxMaterial; //the corresponding skybox material for the current hour

    }

}

