using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "GameDefinition", menuName = "Hex/GameDefinition", order = 2)]
#endif

public class GameDefinition : ScriptableObject
{
    [SerializeField] public string GameName = "Puzzle One";
    [SerializeField] public string BoardString = "5 5 B bB1 wC1 bC2 wC3 bD2 wE1 ";
}
