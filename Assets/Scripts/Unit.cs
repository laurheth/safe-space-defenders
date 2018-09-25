using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 moffset;
    public float pathcloseness;
    //public string[] actions;
    public List<Action> actions;
    //Vector3Int position;
    Vector3 currentVect;
    public float speed;
    public int movesPerTurn;
    int movesLeft;
    Rigidbody2D rb;
    TileGridController gridController;
    List<Vector3Int> steps;
    // Use this for initialization
    void Start()
    {
        actions = new List<Action>();
        movesLeft = movesPerTurn;
        currentVect = Vector3.zero;
        rb = GetComponent<Rigidbody2D>();
        steps = new List<Vector3Int>();
        //position = Vector3Int.RoundToInt(transform.position - offset);
        gridController = transform.parent.gameObject.GetComponent<TileGridController>();
        //gridController.blockPosition()
        actions.Add(new Action("Move", 6, ActType.Movement, 0));
        actions.Add(new Action("Blow vuvuzela", 6, ActType.Cone, 2));
        actions.Add(new Action("Finger guns", 9, ActType.Targetted, 6));
    }

    private void FixedUpdate()
    {
        //Debug.Log(steps.Count);
        if (steps.Count > 0)
        {
            currentVect = steps[0] + offset - transform.position;
            if ((currentVect).magnitude - speed * Time.fixedDeltaTime > pathcloseness)
            {
                rb.MovePosition(transform.position + currentVect.normalized * speed * Time.fixedDeltaTime);
            }
            else
            {
                if (steps.Count == 1)
                {
                    rb.MovePosition(new Vector3(steps[0].x, steps[0].y, 0) + offset);
                }
                currentVect = Vector3.zero;
                steps.RemoveAt(0);
            }
        }
    }

    // Update is called once per frame
    /*void Update () {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition)-moffset;
            gridController.getPath(Vector3Int.RoundToInt(transform.position-offset), Vector3Int.RoundToInt(worldPoint), steps);

        }
	}*/

    public bool MovesLeft()
    {
        return movesLeft > 0;
    }

    public bool readyToMove()
    {
        return steps.Count == 0;
    }

    public void GiveMoveOrder(List<Vector3Int> newsteps)
    {
        steps.Clear();
        movesLeft--;
        for (int i = 0; i < newsteps.Count; i++)
        {
            steps.Add(newsteps[i]);
        }
    }

    public enum ActType { Movement, Melee, Targetted, Cone, LineOfSight, Grenade };

    //[System.Serializable]
    public class Action {
        string menuName;
        int range;
        ActType type;
        int damage;
        public Action(string nom, int rng, ActType actType, int dmg) {
            menuName = nom;
            range = rng;
            type = actType;
            damage = dmg;
        }
        public string GetName() {
            return menuName;
        }
        public int GetRange() {
            return range;
        }
        public ActType GetActType() {
            return type;
        }
        public int GetDamage() {
            return damage;
        }
    }
}
