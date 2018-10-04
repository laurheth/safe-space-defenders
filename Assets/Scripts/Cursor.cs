using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
    TileGridController gridController;
    LineRenderer line;
    public GameObject unit;
    Unit currentUnit;
    EnemyUnit currentEnemyUnit;
    public Vector3 offset;
    public int maxdist;
    Vector3Int oldpos;
    Vector3Int newpos;
    bool validpath;
    bool playerturn;
    bool won;
    bool recalcresist;
    bool addnew;
    bool lost;
    //bool snaptoentity;
    SpriteRenderer srend;
    List<Vector3Int> linesteps;
    List<Unit> PlayerUnits;
    List<EnemyUnit> EnemyUnits;
    public GameObject actMenu;
    ActionMenu actionMenu;
    int enemyid;
    GameObject cam;
    public GameObject winMsg;
    public GameObject loseMsg;
    float cx, cy;
    int difficulty;
    //bool added
    Unit.Action currentAction;
	// Use this for initialization
	void Start () {
        difficulty = 1;

        addnew = true;
        recalcresist = true;
        won = false;
        lost = false;
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        enemyid = 0;
        playerturn = false;
        //snaptoentity = false;
        actionMenu = actMenu.GetComponent<ActionMenu>();
        validpath = false;
        linesteps = new List<Vector3Int>();
        line = GetComponent<LineRenderer>();
        gridController = transform.parent.gameObject.GetComponent<TileGridController>();

        srend = GetComponent<SpriteRenderer>();
        newpos = Vector3Int.zero;
        oldpos = Vector3Int.zero;
        PlayerUnits = new List<Unit>();
        EnemyUnits = new List<EnemyUnit>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Unit"))
        {
            PlayerUnits.Add(obj.GetComponent<Unit>());
        }
        EnemyUnits.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EnemyUnit"))
        {
            EnemyUnits.Add(obj.GetComponent<EnemyUnit>());
        }
	}

	// Update is called once per frame
	void Update () {
        
        cx = Input.GetAxis("Horizontal");
        cy = Input.GetAxis("Vertical");
        cam.transform.position += new Vector3(cx/2f, cy/2f, 0);
        //cam.transform.position[1] += cy;
        if (won || lost) {
            if (won) {
                winMsg.SetActive(true);
            }
            else {
                loseMsg.SetActive(true);
            }
            return;
        }
        if (currentUnit!=null && !currentUnit.readyToMove()) {
            return;
        }
        if (currentEnemyUnit != null && !currentEnemyUnit.readyToMove())
        {
            return;
        }
        if (playerturn==false) {
            if (addnew) {
                addnew = false;
                difficulty++;
                gridController.AddPod(difficulty);
            }
            if (recalcresist)
            {
                foreach (Unit unt in PlayerUnits)
                {
                    if (unt != null)
                    {
                        unt.CalcResistance();
                    }
                }
                foreach (EnemyUnit unt in EnemyUnits)
                {
                    if (unt != null)
                    {
                        unt.CalcResistance();
                    }
                }
                recalcresist = false;
            }

            //Debug.Log("Enemyturn?");
            // Enemy turn goes here
            if (unit==null && EnemyUnits.Count>0) {
                
                currentEnemyUnit = EnemyUnits[enemyid];
                if (currentEnemyUnit != null)
                {
                    unit = currentEnemyUnit.gameObject;
                }
            }
            if (currentEnemyUnit != null && currentEnemyUnit.MovesLeft()) {
                if (currentEnemyUnit.readyToMove()) {
                    currentEnemyUnit.RunAI();
                }
            }
            else {
                enemyid++;
                if (enemyid<EnemyUnits.Count) {
                    currentEnemyUnit = EnemyUnits[enemyid];
                    if (currentEnemyUnit==null) {
                        EnemyUnits.RemoveAt(enemyid);
                        enemyid--;
                        return;
                    }
                    unit = currentEnemyUnit.gameObject;
                }
                else {
                    unit = null;
                    currentUnit = null;
                    playerturn = true;
                    enemyid = 0;
                    PlayerUnits.Clear();
                    foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Unit"))
                    {
                        PlayerUnits.Add(obj.GetComponent<Unit>());
                    }
                    if (PlayerUnits.Count==0) {
                        lost = true;
                        return;
                    }
                    foreach (Unit punit in PlayerUnits) {
                        punit.RenewMoves();
                    }
                    foreach (EnemyUnit punit in EnemyUnits)
                    {
                        punit.RenewMoves();
                    }
                    recalcresist=true;
                }
            }
            return;
        }
        else {
            addnew = true;
        }
        if (currentUnit!=null && currentUnit.readyToMove() && !currentUnit.MovesLeft()) {
            currentUnit = null;
            unit = null;
            return;
        }
        newpos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset);
        transform.position=offset+newpos;
        if (oldpos != newpos)
        {
            //Debug.Log("old"+oldpos);
            //Debug.Log("new"+newpos);
            oldpos = newpos+Vector3Int.zero;;
            validpath = false;
            if (gridController.validPos(transform.position,currentAction))
            {
                if (unit != null && currentAction != null && currentUnit.readyToMove()) {
                    srend.color = Color.cyan;
                    linesteps.Clear();

                    // Define path. Getpath does pathfinding (movement + melee)
                    // Raycast otherwise
                    // public enum ActType { Movement, Melee, Targetted, Cone, LineOfSight, Grenade };
                    switch(currentAction.GetActType()) {
                        default:
                            
                            if (!gridController.CheckLine(Vector3Int.FloorToInt(unit.transform.position),
                                                   newpos,maxdist)) {
                                linesteps.Add(Vector3Int.FloorToInt(unit.transform.position));
                                linesteps.Add(newpos);
                            }
                            break;
                        case Unit.ActType.Melee:
                            gridController.getPath(Vector3Int.FloorToInt(unit.transform.position),
                                                   newpos, linesteps, maxdist);
                            break;
                        case Unit.ActType.Movement:
                            /**/
                            gridController.PathFromCache(Vector3Int.FloorToInt(unit.transform.position),
                                                   newpos, linesteps, maxdist);
                            break;
                    }

                    if (linesteps.Count > 0)
                    {
                        line.enabled = true;
                        line.positionCount = linesteps.Count;
                        for (int i = 0; i < linesteps.Count; i++)
                        {
                            line.SetPosition(i, linesteps[i]+offset);
                        }
                        validpath = true;
                    }
                    else {
                        srend.color = Color.red;
                        line.enabled = false;
                        //oldpos = Vector3Int.zero;
                    }
                }
                else {
                    srend.color = Color.clear;
                    line.enabled = false;
                    //oldpos = Vector3Int.zero;
                }
            }
            else
            {
                srend.color = Color.clear;
                line.enabled = false;
                //oldpos = Vector3Int.zero;
            }
        }/*
        if (Input.GetButtonDown("Cancel")) {
            unit = null;
            currentUnit = null;
            srend.color = Color.red;
            line.enabled = false;
            oldpos = Vector3Int.zero;
        }*/
        if (Input.GetButtonDown("Fire1"))
        {
            if (unit != null && currentAction != null && validpath)
            {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
                if (currentAction.GetActType() == Unit.ActType.Movement
                    || currentAction.GetActType() == Unit.ActType.Melee)
                {
                    gridController.getPath(Vector3Int.RoundToInt(transform.position - offset), Vector3Int.RoundToInt(worldPoint), linesteps);

                    if (currentAction.GetActType() == Unit.ActType.Melee)
                    {
                        // do damage after moving
                        StartCoroutine(currentUnit.QueueAction(currentAction, linesteps[linesteps.Count - 1]));

                        int i = 0;
                        while (i<linesteps.Count) {
                            if (linesteps[i]==linesteps[linesteps.Count-1]) {
                                linesteps.RemoveAt(i);
                            }
                            else {
                                i++;
                            }
                        }
                        //linesteps.RemoveAll(linesteps[linesteps.Count - 1]);
                    }
                    if (linesteps.Count > 0)
                    {
                        gridController.FillPathCache(linesteps[linesteps.Count - 1], currentUnit.MoveDistance);
                    }
                    currentUnit.GiveMoveOrder(linesteps);

                    /*foreach (Vector3Int thisone in linesteps)
                    {
                        Debug.Log("After: " + linesteps);
                    }*/
                }
                else {
                    //currentUnit.PerformAction(currentAction, Vector3Int.RoundToInt(worldPoint));
                    StartCoroutine(currentUnit.AttackAnimation(currentAction, Vector3Int.RoundToInt(worldPoint)));
                }
                srend.color = Color.red;
                line.enabled = false;
                oldpos = Vector3Int.zero;
                //currentAction = null;
                if (!currentUnit.MovesLeft())
                {
                    actionMenu.ClearOptions();
                    unit = null;
                    currentAction = null;
                    recalcresist = true;
                    //currentUnit = null;
                    CheckForEnemyTurn();
                }
            }
            else {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0);
                Debug.Log("do raycast");
                if (hit) {
                    Debug.Log("hit something "+hit.transform.name);
                    if (hit.transform.tag=="Unit") {
                        if (hit.transform.gameObject.GetComponent<Unit>().MovesLeft())
                        {
                            unit = hit.transform.gameObject;
                            currentUnit = unit.GetComponent<Unit>();
                            actionMenu.DefineOptions(unit);
                            gridController.FillPathCache(Vector3Int.FloorToInt(unit.transform.position), currentUnit.MoveDistance);
                        }
                    }
                }
            }
        }
        if (Input.GetButtonDown("Fire2")) {
            oldpos = Vector3Int.zero;
        }
        // Switch between units
        if (playerturn && (Input.GetButtonDown("Fire3") || (unit==null && (currentUnit==null || currentUnit.readyToMove())) )) {
            oldpos = Vector3Int.zero;
            if (currentUnit==null && PlayerUnits[0].MovesLeft()) {
                currentUnit = PlayerUnits[0];
            }
            else {
                int stepstaken = 0;
                int i = PlayerUnits.IndexOf(currentUnit);
                do
                {
                    i++;
                    stepstaken++;
                    if (i >= PlayerUnits.Count) { i = 0; }
                    if (stepstaken>PlayerUnits.Count) {
                        playerturn = false;
                        actionMenu.ClearOptions();
                        unit = null;
                        currentAction = null;
                        return;
                    }
                } while (!PlayerUnits[i].MovesLeft());

                currentUnit = PlayerUnits[i];
            }
            unit = currentUnit.gameObject;
            actionMenu.DefineOptions(unit);
            gridController.FillPathCache(Vector3Int.FloorToInt(unit.transform.position), currentUnit.MoveDistance);
            if (recalcresist) {
                foreach (Unit unt in PlayerUnits) {
                    if (unt != null)
                    {
                        unt.CalcResistance();
                    }
                }
                foreach (EnemyUnit unt in EnemyUnits)
                {
                    if (unt != null)
                    {
                        unt.CalcResistance();
                    }
                }
            }
        }
	}

    public void SetAction(Unit.Action act) {
        currentAction = act;
        maxdist = currentAction.GetRange();
    }

    void CheckForEnemyTurn() {
        
        foreach (Unit playerunit in PlayerUnits) {
            if (playerunit.MovesLeft()) {
                playerturn = true;
                return;
            }
        }
        EnemyUnits.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EnemyUnit"))
        {
            EnemyUnits.Add(obj.GetComponent<EnemyUnit>());
        }
        if (EnemyUnits.Count == 0) { won = true; }
        playerturn = false;
    }

}
