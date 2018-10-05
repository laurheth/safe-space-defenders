using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit {
    public float clusterPercent;
    public float healPercent;
    //public enum EnemyType { Bigot, Fash, Fedora, Rich };
    public int typenum;
    //public int minaidist;
    //public float movePercent;
    //List<>
    //int actionint;
    // Use this for initialization
    public override void Awake()
    {
        base.Awake();
        actions.Clear();
        actions.Add(new Action("Move", MoveDistance, ActType.Movement, 0, -1, "", "", Color.white));
        switch (typenum) {
            default:
            case 0: // Bigot
                actions.Add(new Action("Threatening gesture", 9, ActType.Targetted, 2, 0, "Unit", "Threatening gesture >:(", Color.white));
                break;
            case 1: // Fash
                actions.Add(new Action("Board with a nail in it", MoveDistance, ActType.Melee, 4, 0, "Unit","Board with a nail in it /- /-",Color.white));
                actions.Add(new Action("Sharing fascist literature", MoveDistance, ActType.Melee, 4, 0, "EnemyUnit", "Check out this FashTube video!", Color.green,1));
                break;
            case 2: // Fedorabro
                actions.Add(new Action("Well actually", 6, ActType.Cone, 3, 6, "Unit","Well ACTUALLY...",Color.white,-1));
                actions.Add(new Action("Ethics", 6, ActType.Cone, 3, 6, "EnemyUnit", "#FedoraGate", Color.cyan,1));
                break;
            case 3: // RichNimbyMan
                actions.Add(new Action("Money", 0, ActType.Grenade, 10, 20,"EnemyUnit","Seed funds!",Color.green));
                actions.Add(new Action("Gentrification", 6, ActType.Grenade, 6, 4,"Unit","GENTRIFICATION",Color.red,-1));
                break;
        }
        //actions.Add(new Action("Move", MoveDistance, ActType.Movement, 0,-1,"","",Color.white));
        //actions.Add(new Action("Blow vuvuzela", 6, ActType.Cone, 6, 6, "EnemyUnit"));
        //actions.Add(new Action("Threatening gesture", 9, ActType.Targetted, 2, 0, "Unit","Threatening gesture >:(",Color.white));
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
        if (Morale <= 0) 
        { 
            movesLeft = 0;
            return;
        }
        pos = Vector3Int.FloorToInt(transform.position);
        pos[2] = 0;
        Debug.Log("AI?");
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
            //movesLeft = 0;
            //return;

        }
        else {
            int chooseaction = 0;
            //string targettag = "Unit";
            GameObject skipself;

            while (breaker<10 && !success) {
                breaker++;
                skipself = gameObject;
                chooseaction = Random.Range(1, actions.Count);
                //Debug.Log(actions[chooseaction].GetTag());
                if (actions[chooseaction].GetRange() == 0) { skipself = null; }
                if ((actions[chooseaction].GetTag() == "EnemyUnit") ||
                    (actions[chooseaction].GetTag()=="" && Random.Range(0,100)<healPercent)) {
                    target = gridController.GetObject(pos.x, pos.y, skipself, "EnemyUnit");
                    if (target != null && !target.GetComponent<EnemyUnit>().isDamaged()) {

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
                        if (linesteps[i] == linesteps[linesteps.Count - 1])
                        {
                            linesteps.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }
                    /*while (i < linesteps.Count)
                    {
                        if (linesteps[i] == Vector3Int.FloorToInt(target.transform.position))
                        {
                            linesteps.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }*/
                    GiveMoveOrder(linesteps);
                }
                else {
                    StartCoroutine(AttackAnimation(actions[chooseaction], Vector3Int.FloorToInt(target.transform.position)));
                }
            }
        }
        //gridController()
        Debug.Log("AI done");
    }

    void DoMoveAction()
    {
        float rnd = Random.Range(0, 100);

        List<Vector3> options =
                gridController.availablesquares(Vector3Int.FloorToInt(transform.position),
                                               MoveDistance);
        //options.Sort()
        float minz;
        int minind;
        int breaker = 0;
        linesteps.Clear();
        while (linesteps.Count == 0 && options.Count > 0 && breaker < 100)
        {
            Debug.Log("Weird here?" + options.Count + name);
            breaker++;
            minind = -1;
            minz = 10000;
            //breaker++;
            for (int i = 0; i < options.Count; i++)
            {
                //Debug.Log(options[i].z);
                if (minz > options[i].z)
                {
                    minz = options[i].z;
                    minind = i;
                }
            }
            if (gridController.validPos(options[minind], actions[0]))
            {
                gridController.getPath(Vector3Int.FloorToInt(transform.position),
                                       Vector3Int.RoundToInt(options[minind]),
                                       linesteps);
            }
            if (linesteps.Count == 0)
            {
                options.RemoveAt(minind);
            }
        }
        Debug.Log("Foundone?" + linesteps.Count);
        //}
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
        /*while (i < linesteps.Count)
        {
            if (linesteps[i] == Vector3Int.FloorToInt(target.transform.position))
            {
                linesteps.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }*/
        //Debug.Log(linesteps[linesteps.Count - 1]);
        //gridController.GenAIMap(linesteps[linesteps.Count - 1].x, linesteps[linesteps.Count - 1].y, -adjacency);
        GiveMoveOrder(linesteps);
        //gridController.GenAIMap(linesteps[linesteps.Count - 1].x, linesteps[linesteps.Count - 1].y, -adjacency);

    }
}
