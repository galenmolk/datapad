using TMPro;
using UnityEngine;

public class DebugText : Singleton<DebugText>
{
    [SerializeField] private TMP_Text debugText;
    
    public void Print(string message)
    {
        print(message);
        debugText.text += $"{message}\n";
    }  
}