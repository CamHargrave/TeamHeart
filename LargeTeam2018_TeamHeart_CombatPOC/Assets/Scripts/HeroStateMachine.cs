using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    private BattleStateMachine bsm;
    public BaseHero Hero;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }

    public TurnState CurrentState;

    //private float cur_cooldown = 0f;
    //private float max_cooldown = 2f;

    //public Image ProgressBar;

    public GameObject Selector;

    public GameObject EnemyToAttack;

    private Vector3 startPosition;

    private bool actionStarted = false;

    private float animSpeed = 10f;

    // Use this for initialization
    void Start ()
    {
        startPosition = transform.position;
        //cur_cooldown = Random.Range(0, 2.5f);

        bsm = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        CurrentState = TurnState.PROCESSING;
        Selector.SetActive(false);

    }   // VOID START

    // Update is called once per frame
    void Update ()
    {
        //Debug.Log(CurrentState);
        switch (CurrentState)
        {
            case (TurnState.PROCESSING):
                {
                    //UpgradeProgressBar();
                    CurrentState = TurnState.ADDTOLIST;
                    break;
                }
            case (TurnState.ADDTOLIST):
                {
                    bsm.HerosToManage.Add(this.gameObject);
                    CurrentState = TurnState.WAITING;
                    break;
                }
            case (TurnState.WAITING):
                {
                    // idle state
                    break;
                }
            /*case (TurnState.SELECTING):
                {
                    break;
                }//*/
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
	}   // VOID UPDATE

    /*void UpgradeProgressBar()
    {
        cur_cooldown = cur_cooldown + Time.deltaTime;
        float calc_cooldown = cur_cooldown / max_cooldown;
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
        if(cur_cooldown >= max_cooldown)
        {
            CurrentState = TurnState.ADDTOLIST;
        }

    }   // VOID UPGRADEPROGRESSBAR
    */

    private IEnumerator timeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;
        Debug.Log("Hero Action Started");

        // animate the enemy near the hero to attack
        Vector3 heroPosition = new Vector3(EnemyToAttack.transform.position.x, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z - 1.0f);
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
        Debug.Log("Hero Action Ended");
        CurrentState = TurnState.PROCESSING;
    }

    private bool moveTowards(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
}   // PUBLIC CLASS
