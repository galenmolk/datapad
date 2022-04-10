using System;
using UnityEngine;
using UnityEngine.Android;

[Serializable]
public class PermissionType
{
    private enum PermissionPrecision
    {
        Fine,
        Coarse
    }

    public string Type
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_type))
                _type = GetPermissionString();

            return _type;
        }
    }
    
    [SerializeField] private PermissionPrecision permissionType;

    private string _type;
    
    private string GetPermissionString()
    {
        return permissionType == PermissionPrecision.Fine ? Permission.FineLocation : Permission.CoarseLocation;
    }
}
