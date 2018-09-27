using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 moffset;
    public float pathcloseness;
    public int MoveDistance;
    //public string[] actions;
    public List<Action> actions;
    //Vector3Int position;
    Vector3 currentVect;
    public float speed;
    public int MaxMorale;
    public int Morale;
    public int movesPerTurn;
    protected int movesLeft;
    Rigidbody2D rb;
    protected TileGridController gridController;
    List<Vector3Int> steps;
    // Use this for initialization
    public virtual void Start()
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
        actions.Add(new Action("Move", MoveDistance, ActType.Movement, 0));
        actions.Add(new Action("Blow vuvuzela", 6, ActType.Cone, 6,6,"EnemyUnit"));
        actions.Add(new Action("Finger guns", 9, ActType.Targetted, 6,0,"Unit"));
        actions.Add(new Action("Glitterbomb", 6, ActType.Grenade, 10,2));
        //actions.Add(new Action("Strike a pose", 0, ActType.Grenade, 10, 20));
        actions.Add(new Action("Bear hug", MoveDistance, ActType.Melee, 6));
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

    public void RenewMoves() {
        movesLeft = movesPerTurn;
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

    public enum ActType { Movement, Melee, Targetted, Cone, Grenade };

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
            return Random.Range(damage/2,(damage*3)/2);
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

    public IEnumerator QueueAction(Action todo, Vector3Int target) {
        //Action todo_updated = new Action(todo.GetName)
        yield return null;
        Debug.Log("Action Queued");
        while (!readyToMove()) {
            //Debug.Log("Waiting...");
            yield return null;
        }
        //Debug.Log("Doit!");
        Debug.Log("Attempting...");
        PerformAction(todo, target);
        Debug.Log("Done!");
    }
    public bool isDamaged() {
        return Morale < MaxMorale;
    }
    public void PerformAction(Action todo, Vector3Int target) {
        movesLeft--;
        List<Transform> transes=null;
        switch (todo.GetActType()) {
            default:
            case ActType.Targetted:
                GameObject targetMe = gridController.GetObject(target.x,target.y, gameObject);
                targetMe.GetComponent<Unit>().Damage(todo.GetDamage(),gameObject.tag);
                break;
            case ActType.Cone:
                transes = gridController.GetInCone(Vector3Int.FloorToInt(transform.position),
                                         target, todo.GetRange(),todo.GetRange2());
                break;
            case ActType.Grenade:
                transes = gridController.GetInCircle(target, todo.GetRange2());
                break;
        }
        if (transes!=null && transes.Count>0) {
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
