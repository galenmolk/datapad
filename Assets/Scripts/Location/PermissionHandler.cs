using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

public class PermissionHandler : MonoBehaviour
{
    public static readonly UnityEvent OnPermissionGranted = new();

    [SerializeField] private PermissionType permissionType;
    
    private void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(permissionType.Type))
        {
            DebugText.Instance.Print("Does not have permission");
            Permission.RequestUserPermission(permissionType.Type, GetCallbacks());
        }
    }

    private PermissionCallbacks GetCallbacks()
    {
        PermissionCallbacks callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionDenied;
        callbacks.PermissionGranted += PermissionGranted;
        callbacks.PermissionDeniedAndDontAskAgain += PermissionDeniedAndDontAskAgain;
        return callbacks;
    }
    
    private void PermissionGranted(string obj)
    {
        DebugText.Instance.Print("PermissionGranted");
        OnPermissionGranted.Invoke();
    }
    
    private void PermissionDenied(string obj)
    {
        DebugText.Instance.Print("PermissionDenied: Quitting");
    }
    
    private void PermissionDeniedAndDontAskAgain(string obj)
    {
        DebugText.Instance.Print("PermissionDeniedAndDontAskAgain: Quitting");
    }
}
