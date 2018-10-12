using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter
{
    public string Name = "";
    // Character's name

    public int TurnPriority = -1;                              
    // Placeholder variable to determine order of Character actions

    public bool ActionSet = false;                  
    // Checks if Character has decided on an action

    public bool PerformedAction = false;
    // Variable to check if Character has already performed an action that turn

    public GameObject Selector;

    public List<BaseAttack> Attacks = new List<BaseAttack>();

    public float BaseHP = 0.0f;
    public float CurrentHP = 0.0f;

    public float BaseMP = 0.0f;
    public float CurrentMP = 0.0f;

    public float BaseATK = 0.0f;
    public float CurrentATK = 0.0f;

    public float BaseDEF = 0.0f;
    public float CurrentDEF = 0.0f;

    public int Stamina = 0;
    public int Intellect = 0;
    public int Dexterity = 0;
    public int Agility = 0;
}
