using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderAttackDialogue : MonoBehaviour
{
    public static UnderAttackDialogue instance;
    [HideInInspector] public DialogueTrigger dialogueTrigger;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
