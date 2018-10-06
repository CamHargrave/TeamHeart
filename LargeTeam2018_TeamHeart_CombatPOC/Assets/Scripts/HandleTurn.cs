using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string Type;
    public string Attacker; // name of attacker
    public GameObject AttackersGameObject;  // who attacks
    public GameObject AttackersTarget;      // who gets attacked
    public int Priority;    // the turn priority of the attacker

    // which attack is performed
}
