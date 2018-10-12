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
    
    private Vector3 startPosition;
    //private Vector3 stopPosition;

    private bool actionStarted = false;

    public GameObject Selector;

    public GameObject HeroToAttack;

    private float animationSpeed = 10f;

    // Use this for initialization
    void Start ()
    {
        startPosition = transform.position;
        CurrentState = TurnState.PROCESSING;
        bsm = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        Selector.SetActive(false);
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

    void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.Type = "Enemy";
        myAttack.AttackersName = Enemy.Name;
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = bsm.HeroesInBattle[Random.Range(0, bsm.HeroesInBattle.Count)];
        myAttack.TurnPriority = Enemy.TurnPriority;

        int randomAttack = Random.Range(0, Enemy.Attacks.Count);
        myAttack.ChosenAttack = Enemy.Attacks[randomAttack];
        Debug.Log(this.gameObject.name + " uses " + myAttack.ChosenAttack.AttackName + " for " + myAttack.ChosenAttack.AttackBaseDamage + " DMG!");

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
        doDamage();

        // back to start position
        Vector3 firstPosition = startPosition;
        while (moveTowards(firstPosition)) { yield return null; }

        // remove this performer from the list in the BattleStateMachine (BSM)
        bsm.ExecutePerformersList.RemoveAt(0);

        // reset bsm -> wait
        bsm.BattleState = BattleStateMachine.ActionState.WAIT;

        // end coroutine
        actionStarted = false;

        // reset this enemy state
        Debug.Log("Enemy Action Ended");
        CurrentState = TurnState.PROCESSING;
    }

    private bool moveTowards(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animationSpeed * Time.deltaTime));
    }

    private void doDamage()
    {
        float calculatedDamage = Enemy.CurrentATK + bsm.ExecutePerformersList[0].ChosenAttack.AttackBaseDamage;

        HeroToAttack.GetComponent<HeroStateMachine>().TakeDamage(calculatedDamage);
    }
}
