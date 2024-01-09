using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMemory
{
    public float Age
    {
        get
        {
            return Time.time - lastSeen;
        }
    }
    public GameObject gameObject;
    public Vector3 position;
    public Vector3 direction;
    public float distance;
    public float angle;
    public float lastSeen;
    public float score;
}
public class AiSensoryMemory 
{
    public List<AiMemory> memories = new List<AiMemory>();
    GameObject[] characters;

    public AiSensoryMemory(int maxPlayers)
    {
        characters = new GameObject[maxPlayers];
    }

    public AiMemory FetchMemory(GameObject gameObject)
    {
        AiMemory memory = memories.Find(x => x.gameObject == gameObject);
        if(memory == null)
        {
            memory =new AiMemory();
            memories.Add(memory);
        }
        return memory;
    }

    public void RefreshMemory(GameObject agent, GameObject target)
    {
        AiMemory memory = FetchMemory(target);
        memory.gameObject = target;
        memory.position = target.transform.position;
        memory.direction = target.transform.position - agent.transform.position;
        memory.distance = memory.direction.magnitude;
        memory.angle = Vector3.Angle(agent.transform.forward, memory.direction);
        memory.lastSeen = Time.time;
    }

    public void UpdateSenses(AiSensor sensor)
    {
        int target = sensor.Filter(characters, "Player");
        for (int i = 0; i < target; i++)
        {
            GameObject targets = characters[i];
            RefreshMemory(sensor.gameObject, targets);
        }
    }

    public void ForgetMemory(float maxAge)
    {
        memories.RemoveAll(n => n.Age > maxAge);
        memories.RemoveAll(n => !n.gameObject);
        //memories.RemoveAll(n => n.gameObject.GetComponent<Health>().isDead());
    }
}
