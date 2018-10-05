using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public int movesLeft;
    bool animating;
    Rigidbody2D rb;
    protected TileGridController gridController;
    List<Vector3Int> steps;
    bool dieAfterMove;
    bool doresistcalc;
    UnitCanvas unitCanvas;
    List<Vector3> damagesources;
    Vector3 actualpos;
    GameObject effectcanvas;
    Text effectcanvastext;
    public int damageresistance;
    public int adjacency;
    int bonus;
    // Use this for initialization
    public virtual void Awake()
    {
        bonus = 0;
        //adjacency = 1;
        doresistcalc = true;
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
        actions.Add(new Action("Move", MoveDistance, ActType.Movement, 0,-1,"","",Color.white));
        actions.Add(new Action("Blow vuvuzela", 6, ActType.Cone, 6,6,"EnemyUnit","DOOT!",Color.white));
        actions.Add(new Action("Finger guns", 9, ActType.Targetted, 6,0,"Unit","Finger guns",Color.white));
        actions.Add(new Action("Glitterbomb", 6, ActType.Grenade, 10,2,"","Glitterbomb",Color.white));
        //actions.Add(new Action("Strike a pose", 0, ActType.Grenade, 10, 20));
        actions.Add(new Action("Bear hug", MoveDistance, ActType.Melee, 6,-1,"","Bear hug",Color.cyan));
    }

    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        effectcanvas = GameObject.FindGameObjectWithTag("EffectCanvas");
        effectcanvastext = effectcanvas.transform.Find("Text").gameObject.GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        //Debug.Log(steps.Count);
        if (steps.Count > 0)
        {
            currentVect = steps[0] + offset - transform.position;
            currentVect[2] = currentVect[1];
            if ((currentVect).magnitude - speed * Time.fixedDeltaTime > pathcloseness)
            {
                rb.MovePosition(transform.position + currentVect.normalized * speed * Time.fixedDeltaTime);
            }
            else
            {
                if (steps.Count == 1)
                {
                    transform.position = new Vector3(steps[0].x, steps[0].y, steps[0].y) + offset;
                    //rb.MovePosition(new Vector2(steps[0].x,steps[0].y));
                }
                currentVect = Vector3.zero;
                steps.RemoveAt(0);
            }
        }
        else if (dieAfterMove) {
            Destroy(gameObject);
        }
        /*else if (movesLeft<=0 && doresistcalc) {
            doresistcalc = false;
            //CalcResistance(true);
        }*/
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
        doresistcalc = true;
        damagesources.Clear();
        //damageresistance = 0;
        movesLeft = movesPerTurn;
        /*if (bonus > 0) { bonus--; }
        else if (bonus < 0) { bonus++; }*/
    }

    public void CalcResistance(bool recursive=false) {
        int i = Mathf.FloorToInt(transform.position.x);
        int j = Mathf.FloorToInt(transform.position.y);
        int ii, jj;
        GameObject obj;
        damageresistance = bonus;
        for (ii = -1; ii < 2;ii++) {
            for (jj = -1; jj < 2;jj++) {
                obj = gridController.GetObjectPrecise(i + ii, j + jj, gameObject);
                if (obj!=null && obj.tag==this.tag) {
                    if (recursive) {
                        if (tag == "Unit")
                        {
                            obj.GetComponent<Unit>().CalcResistance();
                        }
                        else if (tag=="EnemyUnit") {
                            obj.GetComponent<EnemyUnit>().CalcResistance();
                        }
                    }
                    damageresistance+=obj.GetComponent<Unit>().adjacency;
                    //damageresistance++;
                    //damageresistance++;
                }
            }
        }
        // Damage resistance reduction for being surrounded
        if (damagesources.Count>1) {
            for (ii = 0; ii < damagesources.Count-1;ii++) {
                for (jj = ii + 1; jj < damagesources.Count;jj++) {
                    // small bonus for 60 degree diff
                    if (Vector3.Dot(damagesources[ii].normalized,damagesources[jj].normalized)<0.51) {
                        damageresistance--;
                    }
                    // larger for 90 degree & greater
                    if (Vector3.Dot(damagesources[ii].normalized, damagesources[jj].normalized) < 0)
                    {
                        damageresistance--;
                    }
                }
            }
        }
        unitCanvas.SetResist(damageresistance);
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

    public void SetBonus(int newbonus) {
        if (newbonus > 0)
        {
            bonus = Mathf.Min(newbonus,bonus+newbonus);
        }
        else if (newbonus<0) {
            bonus = Mathf.Max(newbonus, bonus + newbonus);
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
        int hexdamage;
        string tagspecific;
        Color actioncolor;
        string attacksound;
        public Action(string nom, int rng, ActType actType, int dmg, int rng2, string tg, string effsnd,Color clr,int hexdmg=0) {
            menuName = nom;
            range = rng;
            hexdamage = hexdmg;
            if (rng2<=0) {
                range2 = rng;
            }
            else {
                range2 = rng2;
            }
            attacksound = effsnd;

            actioncolor = clr;
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
        public Color GetSoundColor() {
            return actioncolor;
        }
        public string GetSound() {
            return attacksound;
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
        public int GetHexDamage() {
            return hexdamage;
        }
        public string GetTag() {
            return tagspecific;
        }
    }

    public void Damage(int dmg,Vector3 source,string tg="") {
        if (Morale <= 0) { return; }
        if (gameObject.tag == tg)
        {
            dmg *= -1;
            //Morale -= dmg;
        }
        if (dmg>0) {
            if (source.x > 1)
            {
                damagesources.Add(source - transform.position);
            }
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
            /*if (tag == "EnemyUnit")
            {
                gridController.defeatedfoes++;
            }*/
            //Destroy(gameObject);
        }
    }
    /*private void LateUpdate()
    {
        actualpos = transform.position;
        actualpos[2] = -actualpos[1];
        transform.position = actualpos;
    }*/
    public IEnumerator QueueAction(Action todo, Vector3Int target) {
        //Action todo_updated = new Action(todo.GetName)
        yield return null;
        movesLeft -= 2;
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
    public IEnumerator PerformAction(Action todo, Vector3Int target) {
        //movesLeft--;
        List<Transform> transes=null;
        switch (todo.GetActType()) {
            default:
            case ActType.Targetted:
                GameObject targetMe = gridController.GetObject(target.x,target.y, gameObject);
                transes = new List<Transform>();
                transes.Add(targetMe.transform);
                //targetMe.GetComponent<Unit>().Damage(todo.GetDamage(),transform.position,gameObject.tag);
                break;
            case ActType.Cone:
                transes = gridController.GetInCone(Vector3Int.FloorToInt(transform.position),
                                         target, todo.GetRange(),todo.GetRange2());
                break;
            case ActType.Grenade:
                transes = gridController.GetInCircle(target, todo.GetRange2());
                break;
        }
        Quaternion q;
        float effspeed = 5;
        Vector3 direction;
        Vector3 location;
        float distance;
        if (transes!=null && transes.Count>0) {
            foreach (Transform trans in transes)
            {
                if (todo.GetTag() != "") {
                    if (trans.tag!=todo.GetTag()) {
                        continue;
                    }
                }

                effectcanvastext.text = todo.GetSound();
                effectcanvastext.color = todo.GetSoundColor();
                /*
                direction = (trans.position - transform.position);
                effectcanvas.transform.position = transform.position;
                q = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg,Vector3.forward);
                effectcanvas.transform.rotation = q;
                distance = direction.magnitude;
                while (distance>0) {
                    distance -= effspeed*Time.deltaTime;
                    effectcanvas.transform.position += direction.normalized * effspeed*Time.deltaTime;
                    yield return null;
                }*/
                location = Vector3.up+(trans.position + transform.position) / 2f;
                direction = (trans.position - transform.position);
                distance = direction.magnitude;
                if (direction.x < 0) { direction *= -1; }
                q = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
                effectcanvas.transform.rotation = q;
                if (distance<4) {
                    effectcanvas.transform.position = location + Vector3.up;
                }
                else {
                    effectcanvas.transform.position = location;
                }

                distance = 0.5f;
                while (distance>0) {
                    distance -= Time.deltaTime;
                    effectcanvas.transform.position = location + 0.1f*Vector3.up*Mathf.Sin(2*Mathf.PI*effspeed*Time.time);
                    yield return null;
                }

                trans.gameObject.GetComponent<Unit>().Damage(Mathf.Max(1,bonus+todo.GetDamage()),transform.position,gameObject.tag);
                if (todo.GetHexDamage() != 0)
                {
                    trans.gameObject.GetComponent<Unit>().SetBonus(todo.GetHexDamage());
                }
            }
            effectcanvastext.text = "";
        }
    }

    public IEnumerator AttackAnimation(Action todo, Vector3Int target) {
        movesLeft-=2;
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
        yield return PerformAction(todo, target);
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

    public void SetDetails(string newname, string newpronouns, int newmorale, int newmove, Sprite newsprite, List<Action> newactions, int newadj,Sprite detailsprite, Color newcolor) {
        gameObject.name = newname;
        GetComponent<SpriteRenderer>().sprite = newsprite;
        GetComponent<SpriteRenderer>().color = newcolor;
        //Debug.Log(transform.Find("DetailsSprite").name);
        transform.Find("DetailsSprite").gameObject.GetComponent<SpriteRenderer>().sprite = detailsprite;
        actions.Clear();
        for (int i = 0; i < newactions.Count; i++)
        {
            actions.Add(newactions[i]);
        }
        MaxMorale = newmorale;
        Morale = newmorale;
        MoveDistance = newmove;
        unitCanvas.SetMaxMP(MaxMorale);
        unitCanvas.SetCurrentMP(MaxMorale);
        adjacency = newadj;
    }
}
