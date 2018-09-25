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
    SpriteRenderer srend;
    List<Vector3Int> linesteps;
	// Use this for initialization
	void Start () {
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
            if (gridController.validPos(transform.position))
            {
                if (unit != null && currentUnit.readyToMove()) {
                    srend.color = Color.cyan;
                    linesteps.Clear();
                    gridController.getPath(Vector3Int.FloorToInt(unit.transform.position),newpos,linesteps,maxdist);
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
                    srend.color = Color.red;
                    line.enabled = false;
                    oldpos = Vector3Int.zero;
                }
            }
            else
            {
                srend.color = Color.red;
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
            if (unit != null && validpath)
            {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
                gridController.getPath(Vector3Int.RoundToInt(transform.position - offset), Vector3Int.RoundToInt(worldPoint), linesteps);
                currentUnit.GiveMoveOrder(linesteps);
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
                        }
                    }
                }
            }
        }
	}
}
