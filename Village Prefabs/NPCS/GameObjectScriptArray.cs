using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameObjectScriptArray : ScriptableObject
{
    public List<GameObject> Characters = new List<GameObject>();
}
