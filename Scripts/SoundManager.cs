using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    List<AISoundSensor> aiSoundSensors = new List<AISoundSensor>();

    public enum SoundsCategory 
    {
        Walking,
        Running,
        CrouchWalking,
        Shooting
    }

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {

    }
    private void Update()
    {
    
    }

    public void RegisterSensor(AISoundSensor sensor)
    {
        aiSoundSensors.Add(sensor);
    }

    public void DeRegitster(AISoundSensor sensor)
    {
        aiSoundSensors.Remove(sensor);
    }
    public void SoundEmitted( Vector3 location, SoundsCategory sound, float _intensity)
    {
        //Debug.Log(intensity);
        foreach (var sensor in aiSoundSensors)
        {
            sensor.SoundHeard(location, _intensity);
        }

    }




}



