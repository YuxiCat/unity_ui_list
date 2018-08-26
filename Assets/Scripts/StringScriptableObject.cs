using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
[System.Serializable] 
public class StringScriptableObject : ScriptableObject {
    public List<string> Strs = new List<string>(new string[]{});
}
