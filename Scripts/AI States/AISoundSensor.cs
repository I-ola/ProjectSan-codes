using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISoundSensor : MonoBehaviour
{
    AiAgent agent;
    [HideInInspector] public Vector3 noticedSoundLocation;
    [HideInInspector] public bool checkedLocation = false;
    [HideInInspector] public float intensity;

    
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.RegisterSensor(this);
        agent = GetComponent<AiAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!checkedLocation)
        {           
            if (noticedSoundLocation != null && intensity > agent.config.soundIntensityMax)
            {
                
                if (agent.sensor.IsSoundClose(noticedSoundLocation) && agent.IsNotInAttackStates && !agent.GoingToCover)
                {
                     agent.stateMachine.ChangeState(AiStateId.Suspect);
                }
               

            }
            
        }
       

    }

    private void OnDestroy()
    {
        if(SoundManager.instance != null)
            SoundManager.instance.DeRegitster(this);
    }
    public void SoundHeard(Vector3 location, float _intensity)
    {
        //Debug.Log(_intensity);
        intensity = _intensity;
        noticedSoundLocation = location;
    }
}
