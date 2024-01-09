using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class UserSettings : MonoBehaviour
{
    public static UserSettings instance;

    public GameObject optionsMenu;
    public float aimSensitivity = 1f;
    public float normalSensitivity = 1f;
    public float musicVolume;
    public float sfxVolume;
    public Slider aimSlider;
    public Slider normalSlider;
    private const string PLAYER_AIM_SENSITIVITY = "AimSensitivity";
    private const string PLAYER_NORMAL_SENSITIVITY = "NormalSensitivity";
    private void Awake()
    {
        instance = this;
        //Debug.Log("On Awake Aim Snensitivity is "+ PlayerPrefs.GetFloat(PLAYER_AIM_SENSITIVITY, 1f));
        //Debug.Log("On Awake Normal Snensitivity is "+ PlayerPrefs.GetFloat(PLAYER_NORMAL_SENSITIVITY, 1f));
        aimSensitivity = PlayerPrefs.GetFloat(PLAYER_AIM_SENSITIVITY, 1f);
        normalSensitivity = PlayerPrefs.GetFloat(PLAYER_NORMAL_SENSITIVITY, 1f);
    }
    // Start is called before the first frame update
    void Start()
    {
        UpdateVisuals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateVisuals()
    {
        aimSlider.value = PlayerPrefs.GetFloat(PLAYER_AIM_SENSITIVITY, 1f);
        normalSlider.value = PlayerPrefs.GetFloat(PLAYER_NORMAL_SENSITIVITY, 1f);
    }

    public void SetAimSensitivity(float newValue)
    {
        aimSensitivity = newValue;


    }

    public void SetNormalSensitivity(float newValue)
    {
        normalSensitivity = newValue;


        
    }

    public void CloseMenu()
    {
        PlayerPrefs.SetFloat(PLAYER_AIM_SENSITIVITY, aimSensitivity);
        PlayerPrefs.Save();
        PlayerPrefs.SetFloat(PLAYER_NORMAL_SENSITIVITY, normalSensitivity);
        PlayerPrefs.Save();
        //Debug.Log("On Close Aim Snensitivity is " + PlayerPrefs.GetFloat(PLAYER_AIM_SENSITIVITY, 1f));
        //Debug.Log("On Close Normal Snensitivity is " + PlayerPrefs.GetFloat(PLAYER_NORMAL_SENSITIVITY, 1f));
        optionsMenu.SetActive(false);
    }

    public void OpenMenu()
    {
        optionsMenu.SetActive(true);
    }
}
