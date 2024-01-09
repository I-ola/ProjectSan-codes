using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogues
{
    public string name;

    [TextArea(3, 4)]
    public string[] sentences;
}
