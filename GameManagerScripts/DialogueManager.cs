using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogObject;
    [HideInInspector] public bool canStart;
    public Queue<string> sentences;
    [HideInInspector] public Animator dialogueAnimator;

    private void Awake()
    {
        if(instance !=  null)
        {
            Debug.LogError("There is another instance of DialogueManager");
        }
        instance = this;

        sentences = new Queue<string>();
        dialogueAnimator = dialogObject.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
       
    
    }

    private void Update()
    {
        if (dialogueAnimator.GetBool("isOpen"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void StartDialog(Dialogues dialogues)
    {
        //dialogObject.SetActive(true);
        dialogueAnimator.SetBool("isOpen", true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        nameText.text = dialogues.name;

        sentences.Clear();

        foreach(string dialogue in dialogues.sentences)
        {
            sentences.Enqueue(dialogue);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

           
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }
    void EndDialogue()
    {
        canStart = true;
        dialogueAnimator.SetBool("isOpen", false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
}
