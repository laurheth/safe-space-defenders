using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit {
    public float clusterPercent;
    public float healPercent;
    //public float movePercent;
    //List<>
    //int actionint;
    // Use this for initialization
    public override void Awake()
    {
        base.Awake();
        actions.Clear();
        actions.Add(new Action("Move", MoveDistance, ActType.Movement, 0));
        //actions.Add(new Action("Blow vuvuzela", 6, ActType.Cone, 6, 6, "EnemyUnit"));
        actions.Add(new Action("Threatening gesture", 9, ActType.Targetted, 2, 0, "Unit"));
        //actions.Add(new Action("Glitterbomb", 6, ActType.Grenade, 10, 2));
        //actions.Add(new Action("Strike a pose", 0, ActType.Grenade, 10, 20));
        //actions.Add(new Action("Board with a nail in it", MoveDistance, ActType.Melee, 3,0,"Unit"));
    }
    /*
	// Update is called once per frame
	void Update () {
		
	}*/
    int i;
    GameObject target;
    Vector3Int pos;// = Vector3Int.FloorToInt(transform.position);
    List<Vector3Int> linesteps;// = new List<Vector3Int>();

    public void RunAI() {
        pos = Vector3Int.FloorToInt(transform.position);
        pos[2] = 0;
        //Debug.Log("AI?");
        int breaker = 0;
        bool success = false;
        //int i;
        //rnd;
        //GameObject target;
        linesteps = new List<Vector3Int>();
        /*while (linesteps.Count == 0 && breaker < 1000)
        {
            breaker++;
            gridController.getPath(Vector3Int.RoundToInt(transform.position - offset),
                                   gridController.RandomValidPos(actions[0]),
                                   linesteps, actions[0].GetRange());
        }
        GiveMoveOrder(linesteps);*/
        if (movesLeft>(movesPerTurn-1)) {

            DoMoveAction();

        }
        else {
            int chooseaction = 0;
            //string targettag = "Unit";
            GameObject skipself;
            while (breaker<100 && !success) {
                breaker++;
                skipself = gameObject;
                chooseaction = Random.Range(1, actions.Count);
                //Debug.Log(actions[chooseaction].GetTag());
                if (actions[chooseaction].GetRange() == 0) { skipself = null; }
                if ((actions[chooseaction].GetTag() == "EnemyUnit") ||
                    (actions[chooseaction].GetTag()=="" && Random.Range(0,100)<healPercent)) {
                    target = gridController.GetObject(pos.x, pos.y, skipself, "EnemyUnit");
                    if (!target.GetComponent<EnemyUnit>().isDamaged()) {

                        continue; // don't heal if not damaged!
                        //success = true;

                    }
                }
                else {
                    target = gridController.GetObject(pos.x, pos.y, skipself, "Unit");
                    //Debug.Log(target.name);
                }
                if (target == null) { continue; }
                switch (actions[chooseaction].GetActType())
                {
                    default:

                        if (!gridController.CheckLine(pos,
                                               Vector3Int.FloorToInt(target.transform.position),
                                                      actions[chooseaction].GetRange()))
                        {
                            linesteps.Add(Vector3Int.FloorToInt(target.transform.position));
                        }
                        break;
                    case Unit.ActType.Melee:
                    case Unit.ActType.Movement:
                        gridController.getPath(pos,
                                               Vector3Int.FloorToInt(target.transform.position), linesteps, actions[chooseaction].GetRange());
                        break;
                }
                if (linesteps.Count > 0) { success = true; }
            }
            if (!success) {
                DoMoveAction();
            }
            else {
                if (actions[chooseaction].GetActType()==Unit.ActType.Melee) {
                    StartCoroutine(QueueAction(actions[chooseaction], linesteps[linesteps.Count - 1]));

                    i = 0;
                    while (i < linesteps.Count)
                    {
                        if (linesteps[i] == Vector3Int.FloorToInt(target.transform.position))
                        {
                            linesteps.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }
                    GiveMoveOrder(linesteps);
                }
                else {
                    StartCoroutine(AttackAnimation(actions[chooseaction], Vector3Int.FloorToInt(target.transform.position)));
                }
            }
        }
        //gridController()
    }

    void DoMoveAction() {
        float rnd = Random.Range(0, 100);
        //GameObject target;
        //Vector3Int pos = Vector3Int.FloorToInt(transform.position);
        //List<Vector3Int> linesteps = new List<Vector3Int>();
        if (rnd > clusterPercent)
        {
            target = gridController.GetObject(pos.x, pos.y, gameObject, "Unit"); // Move to playerunit
        }
        else
        {
            target = gridController.GetObject(pos.x, pos.y, gameObject, "EnemyUnit"); // Cluster
        }
        if (target != null)
        {
            gridController.getPath(Vector3Int.RoundToInt(transform.position - offset),
                                   Vector3Int.FloorToInt(target.transform.position),
                                   linesteps);
        }
        else {
            int breaker=0;
            while (linesteps.Count == 0 && breaker < 1000)
            {
                breaker++;
                gridController.getPath(Vector3Int.RoundToInt(transform.position - offset),
                                       gridController.RandomValidPos(actions[0]),
                                       linesteps);
            }
        }
        i = 0;
        // min range
        while (i < linesteps.Count)
        {
            if (i < actions[0].GetRange())
            {
                i++;
            }
            else
            {
                linesteps.RemoveAt(i);
            }
        }
        i = 0;
        // remove last step
        if (target != null)
        {
            while (i < linesteps.Count)
            {
                if (linesteps[i] == Vector3Int.FloorToInt(target.transform.position))
                {
                    linesteps.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            GiveMoveOrder(linesteps);
        }
    }

    /*bool AttemptAttacks() {
        bool success = false;

        return success;
    }*/
}
