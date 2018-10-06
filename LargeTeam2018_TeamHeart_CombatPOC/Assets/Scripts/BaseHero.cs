using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero
{
    public int Priority;    // Placeholder variable to determine order of character actions
    public bool ActionSet = false;  // Checks if Player has decided on an action for every player character

    public string Name;

    public bool PerformedAction = false;    // Variable to check if actor has already performed an action that turn

    public float BaseHP;
    public float CurrentHP;

    public float BaseMP;
    public float CurrentMP;

    public int Stamina;
    public int Intellect;
    public int Dexterity;
    public int Agility;
}

