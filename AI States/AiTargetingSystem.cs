using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class AiTargetingSystem : MonoBehaviour
{
    float ageLimit = 3.0f;
    AiSensoryMemory memory = new AiSensoryMemory(10);
    AiSensor sensor;
    AiMemory bestMemory;

    public bool HasTarget
    {
        get
        {
            return bestMemory != null;
        }
    }

    public GameObject Target
    {
        get
        {
            return bestMemory.gameObject;
        }
    }

    public Vector3 TargetPosition
    {
        get
        {
            return bestMemory.gameObject.transform.position;
        }
    }


    public bool TargetInSight
    {
        get
        {
            if(bestMemory != null)
            {
                return bestMemory.Age < 0.5f; //seconds
            }
            return false;
            
        }
    }

    public float TargetDistance
    {
        get
        {
            return bestMemory.distance;
        }
    }

    public float distWeight = 1.0f;
    public float angleWeight = 1.0f;
    public float ageWeight = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        sensor = GetComponent<AiSensor>();
    }

    // Update is called once per frame
    void Update()
    {
        memory.UpdateSenses(sensor);
        memory.ForgetMemory(ageLimit);
        EvaluateScores();
     
       
    }

    void EvaluateScores()
    {
        bestMemory = null;
         
        foreach(AiMemory memory in memory.memories)
        {
            memory.score = CalculateScore(memory);
            if(bestMemory == null || memory.score > bestMemory.score)
            {
                bestMemory = memory;
            }
        }
    }

    float NormalizeValues(float value, float maxValue)
    {
        return 1.0f - (value / maxValue);
    }
    float CalculateScore(AiMemory memory)
    {
        float distanceScore = NormalizeValues(memory.distance ,sensor.detectDist) * distWeight;
        float angleScore = NormalizeValues(memory.angle, sensor.detectAngle) * angleWeight;
        float ageScore = NormalizeValues(memory.Age, ageLimit) * ageWeight;

       // Debug.Log(distanceScore);
        return distanceScore + angleScore + ageScore;
    }

    private void OnDrawGizmos()
    {
        float maxScore = float.MinValue;
        foreach( var memory in memory.memories)
        {
            maxScore = Mathf.Max(maxScore, memory.score);
        }
        foreach(var memory in memory.memories)
        {
            Color color = Color.green;
            if(memory == bestMemory)
            {
                color = Color.yellow;
            }
            color.a = memory.score / maxScore ;
          
            Gizmos.color = color;
            Gizmos.DrawSphere(memory.position, 0.5f);
        }
    }
}
