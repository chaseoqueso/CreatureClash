using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [System.Serializable]
    public struct characterLine {
        // Speaker
        public PlayerObject speaker;

        // Dialogue line
        [TextArea(3, 10)]
        public string sentence;
    }
    
    // List of lines
    public List<characterLine> characterLines;
}