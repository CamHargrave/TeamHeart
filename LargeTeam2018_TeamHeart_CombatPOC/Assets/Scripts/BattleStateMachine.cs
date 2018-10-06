using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


// Public actions mean that they are accessed by another class object
// Private actions are used only within the class itself

public class BattleStateMachine : MonoBehaviour
{
    // The three stages of the State Machine
    public enum PerformAction
    {
        WAIT,           // Waiting for input form the character objects

        TAKEACTION,     // Retrieving data from current character
                        // (Target Data, Action Type, etc.)
                        // and performing action

        PERFORMACTION   // Currently a Placholder State
    }
    public PerformAction BattleStates;

    // HandleTurn is a class object holding the information of
    // the current actor and their target
    //
    // This list manages which character is
    // next in line to perform an action
    //
    public List<HandleTurn> PerformList = new List<HandleTurn>();

    public List<HandleTurn> ExecutePerformList = new List<HandleTurn>();

    // A list of all the player characters currently on the field
    public List<GameObject> HerosInBattle = new List<GameObject>();

    // A list of all the enemy characters currently on the field
    public List<GameObject> EnemysInBattle = new List<GameObject>();

    // A combined list of all characters on the field
    public List<GameObject> CharactersInBattle = new List<GameObject>();

    // The combined count of all characters on the field
    [SerializeField]
    private int charactersCount = 0;

    // Actionable player characters (I.E. not dead) on the field
    public List<GameObject> HerosToManage = new List<GameObject>();

    // State machine for 
    // Character Action GUI
    public enum HeroGUI
    {
        // GUI activates and conforms to
        // next actionable player character
        // on the list
        ACTIVATE,

        // 
        WAITING,

        // not used yet
        INPUT1,

        // not used yet
        INPUT2,

        // 
        DONE
    }
    public HeroGUI HeroInput;

    // Turn data for player character to be added to turn list
    private HandleTurn heroChoice;

    // GUI Objects in Unity
    // - - -
    // Specifies which enemy to target
    public GameObject EnemyButton;

    // The panel that contains the enemy buttons
    public Transform Spacer;

    // The panel containing possible character actions
    public GameObject AttackPanel;

    // Contains Enemy info
    public GameObject EnemySelectPanel;

	// Use this for initialization
	private void Start ()
    {
        BattleStates = PerformAction.WAIT;
        HerosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));

        EnemysInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        EnemysInBattle.Sort(SortEnemiesByName);

        HeroInput = HeroGUI.ACTIVATE;

        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);

        EnemyButtons();

        charactersCount = HerosInBattle.Count + EnemysInBattle.Count;
    }
	
	// Update is called once per frame
	private void Update ()
    {
        if (PerformList.Count == charactersCount)
        {
            ExecutePerformList = new List<HandleTurn>(PerformList);
            SortTurnOrder(ExecutePerformList);
            PerformList.Clear();
        }//*/

		switch (BattleStates)
        {
            case (PerformAction.WAIT):
                {
                    // if at least one character has 
                    // pushed action data to turn list
                    if (ExecutePerformList.Count > 0)
                    {
                        BattleStates = PerformAction.TAKEACTION;
                        Debug.Log("TAKEACTION started");
                    }
                    break;
                }
            case (PerformAction.TAKEACTION):
                {
                    GameObject performer = GameObject.Find(ExecutePerformList[0].Attacker);

                    if (ExecutePerformList[0].Type == "Enemy")
                    {
                        EnemyStateMachine esm = performer.GetComponent<EnemyStateMachine>();
                        esm.HeroToAttack = ExecutePerformList[0].AttackersTarget;
                        esm.CurrentState = EnemyStateMachine.TurnState.ACTION;
                    }
                    if (ExecutePerformList[0].Type == "Hero")
                    {
                        HeroStateMachine hsm = performer.GetComponent<HeroStateMachine>();
                        hsm.EnemyToAttack = ExecutePerformList[0].AttackersTarget;
                        hsm.CurrentState = HeroStateMachine.TurnState.ACTION;
                    }

                    BattleStates = PerformAction.PERFORMACTION;

                    break;
                }
            case (PerformAction.PERFORMACTION):
                {
                    // placeholder state while
                    // characters perform action
                    // Resets to WAIT
                    // when action is completed
                    break;
                }
        }   // switch (BattleStates)

        switch (HeroInput)
        {
            case (HeroGUI.ACTIVATE):
                {
                    if (HerosToManage.Count > 0 && ExecutePerformList.Count == 0)
                    {
                        HerosToManage.Sort(SortHeroesByPriority);
                        HerosToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                        heroChoice = new HandleTurn();
                        AttackPanel.SetActive(true);
                        HeroInput = HeroGUI.WAITING;
                    }
                    break;
                }
            case (HeroGUI.WAITING):
                {
                    // idle state

                    break;
                }
            case (HeroGUI.INPUT1):
                {
                    // not used yet
                    break;
                }
            case (HeroGUI.INPUT2):
                {
                    // not used yet
                    break;
                }
            case (HeroGUI.DONE):
                {
                    HeroInputDone();
                    break;
                }
        }   // switch (HeroInput)

    }   // void Update()


    // Called by character objects so that
    // they can be added to turn list
    public void CollectActions(HandleTurn input)
    {
        PerformList.Add(input);
    }

    // Retrieves information about every enemy character
    // on the field and adds them to enemy selction GUI
    private void EnemyButtons()
    {
        foreach(GameObject enemy in EnemysInBattle)
        {
            GameObject newButton = Instantiate(EnemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();
            Debug.Log("Enemy Found");

            Debug.Log(cur_enemy.Enemy.Name);

            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
            Debug.Log("Button Text Found");

            button.EnemyPrefab = enemy;
            Debug.Log("Button Prefab Set");

            buttonText.text = cur_enemy.Enemy.Name;
            Debug.Log("Button Text Set");
            
            newButton.transform.SetParent(Spacer, false);
        }
    }

    // Attack Button
    // Called by player button to specify
    // specify which action they want a
    // character to take
    public void Input1()
    {
        heroChoice.Attacker = HerosToManage[0].name;
        heroChoice.AttackersGameObject = HerosToManage[0];
        heroChoice.Type = "Hero";
        heroChoice.Priority = HerosToManage[0].GetComponent<HeroStateMachine>().Hero.Priority;

        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
        Debug.Log("Input1");
    }

    // Target selection
    // Called by player button to specify
    // what they want their action to affect
    public void Input2(GameObject ChosenEnemy)
    {
        heroChoice.AttackersTarget = ChosenEnemy;
        HeroInput = HeroGUI.DONE;
        Debug.Log("Input2");
    }

    // Adds player characters to turn list,
    // deactivates character selection,
    // moves on to next PC, 
    // and resets action GUI
    private void HeroInputDone()
    {
        PerformList.Add(heroChoice);
        EnemySelectPanel.SetActive(false);
        HerosToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        HerosToManage.RemoveAt(0);
        HeroInput = HeroGUI.ACTIVATE;
        Debug.Log("HeroInputDone");
    }

    
    // Possible function to establish turn order
    // Creates combined list of character objects
    // and sorts them by their priority
    private List<HandleTurn> SortTurnOrder(List<HandleTurn> Actors)
    {
        Actors.Sort(SortCharactersByPriority);

        return Actors;
    }

    // Sorts character objects by their priority (int)
    // PC: 0
    // Party characters: 2-5
    // Boss characters : +10
    // Standard enemies: +50
    private static int SortCharactersByPriority(HandleTurn h1, HandleTurn h2)
    {
        return h1.Priority.CompareTo(h2.Priority);

        //return 0;
    }//*/

    // Sort enemy characters by name (for GUI)
    private static int SortEnemiesByName(GameObject e1, GameObject e2)
    {
        return e1.GetComponent<EnemyStateMachine>().Enemy.Name.CompareTo(e2.GetComponent<EnemyStateMachine>().Enemy.Name);
    }

    // Sort player characters by priority (for GUI)
    private static int SortHeroesByPriority(GameObject h1, GameObject h2)
    {
        return h1.GetComponent<HeroStateMachine>().Hero.Priority.CompareTo(h2.GetComponent<HeroStateMachine>().Hero.Priority);
    }
}
