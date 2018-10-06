using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateMachine : MonoBehaviour
{
    public BaseEnemy Enemy;
    private BattleStateMachine bsm;
    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState CurrentState;

    private float cur_cooldown = 0f;
    private float max_cooldown = 1f;
    private Vector3 startPosition;
    //private Vector3 stopPosition;
    private bool actionStarted = false;
    public GameObject HeroToAttack;
    private float animSpeed = 10f;

    // Use this for initialization
    void Start ()
    {
        startPosition = transform.position;
        CurrentState = TurnState.PROCESSING;
        bsm = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        switch (CurrentState)
        {
            case (TurnState.PROCESSING):
                {
                    //UpgradeProgressBar();
                    CurrentState = TurnState.CHOOSEACTION;
                    break;
                }
            case (TurnState.CHOOSEACTION):
                {
                    ChooseAction();
                    CurrentState = TurnState.WAITING;
                    break;
                }
            case (TurnState.WAITING):
                {
                    // idle state
                    break;
                }
            case (TurnState.ACTION):
                {
                    StartCoroutine(timeForAction());
                    break;
                }
            case (TurnState.DEAD):
                {
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    void UpgradeProgressBar()
    {
        cur_cooldown = cur_cooldown + Time.deltaTime;
        float calc_cooldown = cur_cooldown / max_cooldown;
        if (cur_cooldown >= max_cooldown)
        {
            CurrentState = TurnState.CHOOSEACTION;
        }

    }   // VOID UPGRADEPROGRESSBAR

    void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.Type = "Enemy";
        myAttack.Attacker = Enemy.Name;
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = bsm.HerosInBattle[Random.Range(0, bsm.HerosInBattle.Count)];
        myAttack.Priority = Enemy.Priority;
        bsm.CollectActions(myAttack);
    }

    private IEnumerator timeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;
        Debug.Log("Enemy Action Started");

        // animate the enemy near the hero to attack
        Vector3 heroPosition = new Vector3(HeroToAttack.transform.position.x, HeroToAttack.transform.position.y, HeroToAttack.transform.position.z + 1.0f);
        while (moveTowards(heroPosition)) { yield return null; }

        // wait
        yield return new WaitForSeconds(0.5f);

        // do damage

        // back to start position
        Vector3 firstPosition = startPosition;
        while (moveTowards(firstPosition)) { yield return null; }

        // remove this performer from the list in the BattleStateMachine (BSM)
        bsm.ExecutePerformList.RemoveAt(0);

        // reset bsm -> wait
        bsm.BattleStates = BattleStateMachine.PerformAction.WAIT;

        // end coroutine
        actionStarted = false;

        // reset this enemy state
        Debug.Log("Enemy Action Ended");
        CurrentState = TurnState.PROCESSING;
    }

    private bool moveTowards( Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
}
