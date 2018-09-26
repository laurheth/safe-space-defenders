using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
    TileGridController gridController;
    LineRenderer line;
    public GameObject unit;
    Unit currentUnit;
    public Vector3 offset;
    public int maxdist;
    Vector3Int oldpos;
    Vector3Int newpos;
    bool validpath;
    //bool snaptoentity;
    SpriteRenderer srend;
    List<Vector3Int> linesteps;
    public GameObject actMenu;
    ActionMenu actionMenu;
    Unit.Action currentAction;
	// Use this for initialization
	void Start () {
        //snaptoentity = false;
        actionMenu = actMenu.GetComponent<ActionMenu>();
        validpath = false;
        linesteps = new List<Vector3Int>();
        line = GetComponent<LineRenderer>();
        gridController = transform.parent.gameObject.GetComponent<TileGridController>();
        srend = GetComponent<SpriteRenderer>();
        newpos = Vector3Int.zero;
        oldpos = Vector3Int.zero;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        newpos = Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset);
        transform.position=offset+newpos;
        if (oldpos != newpos)
        {
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
                            /*if(!Physics2D.Linecast(
                                new Vector2(unit.transform.position.x, unit.transform.position.y),
                                new Vector2(newpos.x, newpos.y),
                                LayerMask.GetMask("Default"))) {*/
                            /*Debug.Log(Physics2D.Linecast(
                                new Vector2(unit.transform.position.x, unit.transform.position.y),
                                new Vector2(newpos.x+offset[0], newpos.y+offset[1]),*/
                            if (!gridController.CheckLine(Vector3Int.FloorToInt(unit.transform.position),
                                                   newpos,maxdist)) {
                                //LayerMask.GetMask("Default")).transform.name);
                                linesteps.Add(Vector3Int.FloorToInt(unit.transform.position));
                                linesteps.Add(newpos);
                            }
                            break;
                        case Unit.ActType.Melee:
                        case Unit.ActType.Movement:
                            gridController.getPath(Vector3Int.FloorToInt(unit.transform.position),
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
                        oldpos = Vector3Int.zero;
                    }
                }
                else {
                    srend.color = Color.clear;
                    line.enabled = false;
                    oldpos = Vector3Int.zero;
                }
            }
            else
            {
                srend.color = Color.clear;
                line.enabled = false;
                oldpos = Vector3Int.zero;
            }
        }
        if (Input.GetButtonDown("Cancel")) {
            unit = null;
            currentUnit = null;
            srend.color = Color.red;
            line.enabled = false;
            oldpos = Vector3Int.zero;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (unit != null && currentAction != null && validpath)
            {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
                if (currentAction.GetActType() == Unit.ActType.Movement
                    || currentAction.GetActType() == Unit.ActType.Melee)
                {
                    gridController.getPath(Vector3Int.RoundToInt(transform.position - offset), Vector3Int.RoundToInt(worldPoint), linesteps);
                    currentUnit.GiveMoveOrder(linesteps);
                }
                else {
                    currentUnit.PerformAction(currentAction, Vector3Int.RoundToInt(worldPoint));
                }
                srend.color = Color.red;
                line.enabled = false;
                oldpos = Vector3Int.zero;
                if (!currentUnit.MovesLeft())
                {
                    unit = null;
                    currentUnit = null;
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
                        }
                    }
                }
            }
        }
	}

    public void SetAction(Unit.Action act) {
        currentAction = act;
        maxdist = currentAction.GetRange();
    }
}
