using UnityEngine;

public class TestMono : MonoBehaviour
{
    private int i;
    
    private void Start()
    {
        Debug.Log("Starting Value: " + i);
        
        this.ExecuteAfterDelay(2f, () =>
        {
            i += 100;
            Debug.Log("Value after delay: " + i);
        });
    }
}
