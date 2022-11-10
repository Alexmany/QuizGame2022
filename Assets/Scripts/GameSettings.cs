using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Game Setting", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    public int tries;
    public int characterLimit;    
}
