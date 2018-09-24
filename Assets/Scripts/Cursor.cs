using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
    TileGridController gridController;
    LineRenderer line;
    public GameObject unit;
    public Vector3 offset;
    public int maxdist;
    Vector3Int oldpos;
    Vector3Int newpos;
    SpriteRenderer srend;
    List<Vector3Int> linesteps;
	// Use this for initialization
	void Start () {
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
            if (gridController.validPos(transform.position))
            {
                srend.color = Color.cyan;
                if (unit != null) {
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
                    }
                    else {
                        srend.color = Color.red;
                        line.enabled = false;
                        oldpos = Vector3Int.zero;
                    }
                }
            }
            else
            {
                srend.color = Color.red;
                line.enabled = false;
                oldpos = Vector3Int.zero;
            }
        }
	}
}
