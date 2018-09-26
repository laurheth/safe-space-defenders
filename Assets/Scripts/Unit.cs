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
    public int MaxMorale;
    int Morale;
    public int movesPerTurn;
    int movesLeft;
    Rigidbody2D rb;
    TileGridController gridController;
    List<Vector3Int> steps;
    // Use this for initialization
    void Start()
    {
        Morale = MaxMorale;
        actions = new List<Action>();
        movesLeft = movesPerTurn;
        currentVect = Vector3.zero;
        rb = GetComponent<Rigidbody2D>();
        steps = new List<Vector3Int>();
        //position = Vector3Int.RoundToInt(transform.position - offset);
        gridController = transform.parent.gameObject.GetComponent<TileGridController>();
        //gridController.blockPosition()
        actions.Add(new Action("Move", 9, ActType.Movement, 0));
        actions.Add(new Action("Blow vuvuzela", 6, ActType.Cone, 10,6,"EnemyUnit"));
        actions.Add(new Action("Finger guns", 9, ActType.Targetted, -6,0,"Unit"));
        actions.Add(new Action("Glitterbomb", 6, ActType.Grenade, 10,2));
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
        int range2;
        ActType type;
        int damage;
        string tagspecific;
        public Action(string nom, int rng, ActType actType, int dmg, int rng2=-1, string tg="") {
            menuName = nom;
            range = rng;
            if (rng2<=0) {
                range2 = rng;
            }
            else {
                range2 = rng2;
            }
            type = actType;
            damage = dmg;
            tagspecific = tg;
        }
        public string GetName() {
            return menuName;
        }
        public int GetRange() {
            return range;
        }
        public int GetRange2() {
            return range2;
        }
        public ActType GetActType() {
            return type;
        }
        public int GetDamage() {
            return damage;
        }
        public string GetTag() {
            return tagspecific;
        }
    }

    public void Damage(int dmg,string tg="") {
        if (gameObject.tag != tg)
        {
            Morale -= dmg;
        }
        else {
            Morale += dmg;
        }
        if (Morale <=0) {
            gridController.RemoveEntity(gameObject);
            Destroy(gameObject);
        }
    }

    public void PerformAction(Action todo, Vector3Int target) {
        movesLeft--;
        List<Transform> transes=null;
        switch (todo.GetActType()) {
            default:
            case ActType.Targetted:
                GameObject targetMe = gridController.GetObject(target.x,target.y, gameObject);
                targetMe.GetComponent<Unit>().Damage(todo.GetDamage());
                break;
            case ActType.Cone:
                transes = gridController.GetInCone(Vector3Int.FloorToInt(transform.position),
                                         target, todo.GetRange());
                break;
            case ActType.Grenade:
                transes = gridController.GetInCircle(target, todo.GetRange2());
                break;
        }
        if (transes==null || transes.Count>0) {
            foreach (Transform trans in transes)
            {
                if (todo.GetTag() != "") {
                    if (trans.tag!=todo.GetTag()) {
                        continue;
                    }
                }
                trans.gameObject.GetComponent<Unit>().Damage(todo.GetDamage(),gameObject.tag);
            }
        }
    }
}
