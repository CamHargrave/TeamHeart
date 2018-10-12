using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string Type;

    public string AttackersName; 
    // name of attacker

    public GameObject AttackersGameObject;  
    // who attacks

    public GameObject AttackersTarget;      
    // who gets attacked

    public int TurnPriority;
    // the turn priority of the attacker

    public BaseAttack ChosenAttack;
    // which attack is performed
}
