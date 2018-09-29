using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 moffset;
    public Vector3 handlocation;
    public GameObject whiteFlagPrefab;
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
    bool animating;
    Rigidbody2D rb;
    protected TileGridController gridController;
    List<Vector3Int> steps;
    bool dieAfterMove;
    UnitCanvas unitCanvas;
    List<Vector3> damagesources;
    public int damageresistance;
    // Use this for initialization
    public virtual void Start()
    {
        dieAfterMove = false;
        damagesources = new List<Vector3>();
        damageresistance = 0;
        unitCanvas = GetComponentInChildren<UnitCanvas>();
        animating = false;
        Morale = MaxMorale;

        unitCanvas.SetMaxMP(MaxMorale);
        unitCanvas.SetCurrentMP(MaxMorale);

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
        else if (dieAfterMove) {
            Destroy(gameObject);
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
        damagesources.Clear();
        //damageresistance = 0;
        movesLeft = movesPerTurn;
    }

    void CalcResistance() {
        int i = Mathf.FloorToInt(transform.position.x);
        int j = Mathf.FloorToInt(transform.position.y);
        int ii, jj;
        GameObject obj;
        damageresistance = 0;
        for (ii = -1; ii < 2;ii++) {
            for (jj = -1; jj < 2;jj++) {
                obj = gridController.GetObjectPrecise(i + ii, j + jj, gameObject);
                if (obj!=null && obj.tag==this.tag) {
                    damageresistance++;
                    damageresistance++;
                }
            }
        }
        if (damagesources.Count>1) {
            for (ii = 0; ii < damagesources.Count-1;ii++) {
                for (jj = ii + 1; jj < damagesources.Count;jj++) {
                    if (Vector3.Dot(damagesources[ii].normalized,damagesources[jj].normalized)<0.5) {
                        damageresistance--;
                    }
                }
            }
        }
    }

    public bool readyToMove()
    {
        return (steps.Count == 0) && !animating;
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

    public void Damage(int dmg,Vector3 source,string tg="") {
        if (gameObject.tag == tg)
        {
            dmg *= -1;
            //Morale -= dmg;
        }
        if (dmg>0) {
            damagesources.Add(source-transform.position);
            CalcResistance();
            dmg = Mathf.Max(1, dmg - damageresistance);
        }
        Morale -= dmg;
        if (Morale > MaxMorale) { Morale = MaxMorale; }
        unitCanvas.SetCurrentMP(Morale);
        StartCoroutine(unitCanvas.DamageAnimation(-dmg));
        if (Morale <=0) {
            movesPerTurn=0;
            movesLeft = 0;
            gridController.RemoveEntity(gameObject);
            GameObject newflag=Instantiate(whiteFlagPrefab, transform, false);
            newflag.transform.localPosition = handlocation;
            gridController.getPath(Vector3Int.FloorToInt(transform.position),
                                   Vector3Int.one, steps, 200, true);
            dieAfterMove = true;
            //Destroy(gameObject);
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
        StartCoroutine(AttackAnimation(todo, target));
        Debug.Log("Done!");
    }
    public bool isDamaged() {
        return Morale < MaxMorale;
    }
    public void PerformAction(Action todo, Vector3Int target) {
        //movesLeft--;
        List<Transform> transes=null;
        switch (todo.GetActType()) {
            default:
            case ActType.Targetted:
                GameObject targetMe = gridController.GetObject(target.x,target.y, gameObject);
                targetMe.GetComponent<Unit>().Damage(todo.GetDamage(),transform.position,gameObject.tag);
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
                trans.gameObject.GetComponent<Unit>().Damage(todo.GetDamage(),transform.position,gameObject.tag);
            }
        }
    }

    public IEnumerator AttackAnimation(Action todo, Vector3Int target) {
        movesLeft--;
        animating = true;
        Vector3 startpos = transform.position + Vector3.zero;
        Vector3 vec = (new Vector3(target.x, target.y, 0) - startpos).normalized;
        float maxmoved = 2 * pathcloseness;
        float moved = 0;
        while (moved<maxmoved) {
            moved += speed*Time.deltaTime;
            transform.position = startpos + moved * vec;
            yield return null;
        }
        PerformAction(todo, target);
        yield return null;
        while (moved >0)
        {
            moved -= speed*Time.deltaTime;
            transform.position = startpos + moved * vec;
            yield return null;
        }
        transform.position = startpos;
        animating = false;
    }
}
