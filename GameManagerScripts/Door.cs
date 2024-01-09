using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public DoorType doorType;
    public Animator animator;
    DialogueTrigger dialogueTrigger;
    public enum DoorType
    {
        Door1 = 1,
        Door2 = 2,
        Door3 = 3,
        Door4 = 4
    }
    enum DoorState
    {
        Opened,
        Closed
    }
    DoorState presentState = DoorState.Closed;
    bool IsClosed
    {
        get
        {
            return presentState == DoorState.Closed;
        }
    }

    bool IsOpened
    {
        get
        {
            return presentState == DoorState.Opened;
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
       
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(canOpen);
        if(IsOpened)
        {
            StartCoroutine(CloseDoor(10));
        }
        //DoorAnimation();
    }

    public void CheckKey(Key key, Door door, bool pos)
    {
        if(key != null && door.IsClosed)
        {
            //Debug.Log((int)key.keytype == (int)door.doorType);
            if ((int)key.keytype == (int)door.doorType)
            {
                StartCoroutine(OpenDoor(pos));
            }
            else
            {
                dialogueTrigger.TriggerDialogue();
            }
        }
        else if(key !=null && door.IsOpened)
        {
            if ((int)key.keytype == (int)door.doorType)
            {
                StartCoroutine(CloseDoor(0));
            }
        }
        else
        {
            dialogueTrigger.TriggerDialogue();
        }
    }


    void DoorAnimation()
    {

    }

    IEnumerator OpenDoor(bool pos)
    {
        if (pos)
        {
            animator.SetBool("OpenDoorFront", true);
        }
        else
        {
            
            animator.SetBool("OpenDoorBack", true);
        }
        
        yield return new WaitForSeconds(0.5f);
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        presentState = DoorState.Opened;
    }

    IEnumerator CloseDoor(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("OpenDoorBack", false);
        animator.SetBool("OpenDoorFront", false);
        yield return new WaitForSeconds(0.5f);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        presentState = DoorState.Closed;
    }
}
