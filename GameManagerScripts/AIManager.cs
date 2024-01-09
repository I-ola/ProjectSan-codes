using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager instance;
    AiAgent[] agents;
    public Transform player;
    public bool attackingPlayer = false;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("There is another instance of AIManager");
        }
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
       DoOnStart();
       
    }

    // Update is called once per frame
    void Update()
    {
        if(agents != null)
        {
            for(int i = 0; i < agents.Length; i++)
            {
                float distance = Vector3.Distance(agents[i].transform.position, player.position);

                if (distance <= 30f && !agents[i].IsDead)
                {

                    agents[i].gameObject.SetActive(true);
                }

                if (agents[i].targeting != null)
                {
                    if (agents[i].targeting.HasTarget)
                    {
                        attackingPlayer = true;
                        if (agents[i].IsDead)
                        {
                            attackingPlayer = false;
                        }
                    }
                    

                }
                /*if (StateMachineDead(agents[i]))
                {
                    agents[i].gameObject.SetActive(false);
                }*/


            }
        }
    }

    bool StateMachineDead(AiAgent agent)
    {
        if (agent.stateMachine != null)
        {
            
            if (agent.IsDead)
            {
                
                return true;
            }
            else
            {
                return false;
            }

        }
        return false;
    }
    void DoOnStart()
    {
        agents = GameObject.FindObjectsOfType<AiAgent>();
      

        for (int i = 0; i < agents.Length; i++)
        {
            float distance = Vector3.Distance(agents[i].transform.position, player.position);

            if (distance > 30f)
                agents[i].gameObject.SetActive(false);

        }
    }

}
