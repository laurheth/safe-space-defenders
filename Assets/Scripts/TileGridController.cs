using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGridController : MonoBehaviour
{

    public Tilemap tileMap;
    public Tilemap collisionMap;
    public Tilemap nowalkingMap;
    GridLayout grid;
    int xsize, ysize;
    int xmin, ymin;
    int xmax, ymax;
    bool[,] passable;
    bool[,] nowalking;
    float[,] pathcache;
    public int defeatedfoes;
    public int addedfoes;
    List<Transform> Entities;
    public GameObject[] Foes;
    public int[] FoeCost;
    //List<>
    // Use this for initialization
    void Start()
    {
        Entities = new List<Transform>();
        defeatedfoes = 0;
        addedfoes = 0;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Unit"))
        {
            Entities.Add(obj.transform);
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EnemyUnit"))
        {
            Entities.Add(obj.transform);
        }

        grid = GetComponent<GridLayout>();
        xsize = collisionMap.size.x;
        xmin = collisionMap.cellBounds.xMin;
        xmax = collisionMap.cellBounds.xMax;
        ysize = collisionMap.size.y;
        ymin = collisionMap.cellBounds.yMin;
        ymax = collisionMap.cellBounds.yMax;
        passable = new bool[xsize, ysize];
        nowalking = new bool[xsize, ysize];
        pathcache = new float[xsize, ysize];
        for (int i = 0; i < xsize; i++)
        {
            for (int j = 0; j < ysize; j++)
            {
                //passable[i+xmin,j+ymin]=valid
                if (collisionMap.HasTile(new Vector3Int(i + xmin, j + ymin, 0)))
                {
                    passable[i, j] = false;
                }
                else
                {
                    passable[i, j] = true;
                }
                if (nowalkingMap.HasTile(new Vector3Int(i + xmin, j + ymin, 0)))
                {
                    nowalking[i, j] = false;
                }
                else
                {
                    nowalking[i, j] = true;
                }
                //Debug.Log(i + " " + j + " "+passable[i,j]);
                //Debug.Log(xmin + " " + xmax);
            }
        }

    }

    public void AddPod(int difficulty) {
        //int postoadd;
        Vector3Int newpos=Vector3Int.zero;

        /*
        newpos[0] += Random.Range(xmin,xmax);
        newpos[1] += Random.Range(ymin,ymax);
        */
        int side = Random.Range(0, 4);
        int choice;
        GameObject newobj;
        int breaker=1;
        while (difficulty > 0)
        {
            breaker++;
            choice = Random.Range(0, Foes.Length);
            if (breaker % 100 == 0) { choice = 0; }
            while (FoeCost[choice] > difficulty)
            {
                choice--;
            }
            addedfoes++;
            difficulty -= FoeCost[choice];
            do
            {
                switch (side)
                {
                    default:
                    case 0:
                        newpos[0] = xmin + 1;
                        newpos[1] = Random.Range(ymin + 1, ymax - 2);
                        break;
                    case 1:
                        newpos[1] = ymin + 1;
                        newpos[0] = Random.Range(xmin + 1, xmax - 2);
                        break;
                    case 2:
                        newpos[0] = xmax - 2;
                        newpos[1] = Random.Range(ymin + 1, ymax - 2);
                        break;
                    case 3:
                        newpos[1] = ymax - 2;
                        newpos[0] = Random.Range(xmin + 1, xmax - 2);
                        break;
                }
            } while (!passable[newpos[0] - xmin, newpos[1] - ymin] || HasObj(newpos.x, newpos.y));
            /*newpos[0] = 1;
            newpos[1] = Random.Range(ymin + 1, ymax - 2);*/
            newobj = Instantiate(Foes[choice], newpos, Quaternion.identity, transform);
            Entities.Add(newobj.transform);

        }
    }

    public Vector3Int RandomValidPos(Unit.Action todo) {
        int breaker = 0;
        int x, y;
        Vector3 testpos=Vector3.zero;
        do
        {
            x = Random.Range(xmin, xmax);
            y = Random.Range(ymin, ymax);
            testpos[0] = x;
            testpos[1] = y;
            breaker++;
        } while (!validPos(testpos, todo) && breaker < 1000);
        return Vector3Int.FloorToInt(testpos);
    }

    public bool validPos(Vector3 pos_w, Unit.Action todo)
    {
        // treating nowalking map like entities i.e. blocks walking but not attacks
        // except for "only entities" which is targetting units, not map features
        bool skipentities = true;
        bool onlyentities = false;
        if (todo != null)
        {
            if (todo.GetActType() != Unit.ActType.Movement)
            {
                skipentities = false;
            }
            if (todo.GetActType() == Unit.ActType.Targetted || todo.GetActType() == Unit.ActType.Melee)
            {
                onlyentities = true;

            }
            //}
            //if (todo)
            //Debug.Log(todo.GetActType());
        }
        Vector3Int pos = grid.WorldToCell(pos_w); // NOTE TO SELF: USE THIS MORE
        if (pos.x > xmin && pos.x < xmax && pos.y > ymin && pos.y < ymax)
        {
            pos[2] = 0;
            if (!onlyentities)
            {
                return passable[pos.x - xmin, pos.y - ymin] && tileMap.HasTile(pos) && (!skipentities || (!HasObj(pos.x, pos.y) && nowalking[pos.x - xmin, pos.y - ymin]));
            }
            else
            {
                if (todo.GetTag() != "")
                {
                    GameObject obj = GetObjectPrecise(pos.x, pos.y);
                    if (obj == null || obj.tag != todo.GetTag())
                    {
                        return false;
                    }
                }
                return HasObj(pos.x, pos.y);
            }
            //return !collisionMap.HasTile(pos) && tileMap.HasTile(pos);
        }
        else
        {
            return false;
        }
    }

    public bool CheckLine(Vector3Int startPos_w, Vector3Int endPos_w, int maxdist = 20, bool checkentities = false)
    {
        Vector3Int offset = new Vector3Int(xmin, ymin, 0);
        Vector3Int startPos = startPos_w - offset;
        Vector3Int endPos = endPos_w - offset;
        Vector3 checkPos = startPos_w - offset;
        Vector3 step = (new Vector3(endPos_w.x - startPos_w.x,
                                   endPos_w.y - startPos_w.y,
                                    endPos_w.z - startPos_w.z)).normalized;
        if ((startPos - endPos).magnitude > maxdist) { return true; }
        int i, j;
        int breaker = 0;
        while (Vector3Int.RoundToInt(checkPos) != endPos && breaker < maxdist)
        {
            breaker++;
            checkPos += step;
            i = Mathf.RoundToInt(checkPos.x);
            j = Mathf.RoundToInt(checkPos.y);
            //Debug.Log(checkPos + " " + passable[i, j]);
            if (i == startPos.x && j == startPos.y) { continue; }
            if (!passable[i, j]) { return true; }
            if (checkentities && HasObj(i + xmin, j + ymin)) { return true; }
        }
        return false;
    }

    public void PathFromCache(Vector3Int startPos_w, Vector3Int endPos_w, List<Vector3Int> steps, int maxDist=50,bool ismelee=false) {

        Vector3Int offset = new Vector3Int(xmin, ymin, 0);
        Vector3Int startPos = startPos_w - offset;
        Vector3Int endPos = endPos_w - offset;
        startPos[2] = 0;
        endPos[2] = 0;
        int i, j;
        if (pathcache[endPos.x,endPos.y]>maxDist) {
            return;
        }

        Vector3Int currentPos = endPos;
        Vector3Int nextStep = endPos;
        float distval;
        int breaker = 0;
        steps.Clear();
        while (currentPos != startPos && breaker < maxDist)
        {
            //Debug.Log(currentPos + " " + endPos);
            steps.Add(currentPos + offset);
            if (ismelee && currentPos == endPos)
            {
                distval = maxDist;
            }
            else
            {
                distval = pathcache[currentPos.x, currentPos.y];
            }
            for (i = -1; i < 2; i++)
            {
                for (j = -1; j < 2; j++)
                {
                    if (pathcache[currentPos.x + i, currentPos.y + j] < distval)
                    {
                        distval = pathcache[currentPos.x + i, currentPos.y + j];
                        nextStep = currentPos + new Vector3Int(i, j, 0);
                    }
                }
            }
            currentPos = nextStep;
            breaker++;
        }
        steps.Add(currentPos + offset);
        steps.Reverse();
        //Remove duplicates
        if (steps.Count > 0)
        {
            i = 0;
            Vector3Int lastVect = Vector3Int.zero;
            while (i < steps.Count)
            {
                if (steps[i] == lastVect)
                {
                    steps.RemoveAt(i);
                }
                else
                {
                    lastVect = steps[i];
                    i++;
                }
            }
        }

    }

    public void FillPathCache(Vector3Int startPos_w,int maxDist=50) {
        startPos_w[2] = 0;
        int i, j, ii, jj;
        for (i = 0; i < xsize;i++) {
            for (j = 0; j < ysize;j++) {
                pathcache[i, j] = maxDist+1;
            }
        }
        Vector3Int offset = new Vector3Int(xmin, ymin, 0);
        Vector3Int startPos = startPos_w - offset;
        pathcache[startPos.x, startPos.y] = 0;
        //int currentdist = 0;
        bool nochange = false;
        int sxmin, sxmax, symin, symax;
        sxmin = startPos.x - 1;
        sxmax = startPos.x + 1;
        symin = startPos.y - 1;
        symax = startPos.y + 1;
        float steplength;
        int breaker=0;
        while (!nochange && breaker<100)
        {
            breaker++;
            if (sxmin > 1) { sxmin--; }
            if (sxmax < xsize - 1) { sxmax++; }
            if (symin > 1) { symin--; }
            if (symax < ysize - 1) { symax++; }
            nochange = true;
            for (i = sxmin; i < sxmax; i++)
            {
                for (j = symin; j < symax; j++)
                {
                    //if (Mathf.Approximately(pathcache[i,j],currentdist)) {

                    for (ii = -1; ii < 2; ii++)
                    {
                        for (jj = -1; jj < 2; jj++)
                        {
                            if (!passable[i, j] || (HasObj(i + xmin, j + ymin)) || !nowalking[i, j])
                            {
                                continue;
                            }
                            steplength = Mathf.Sqrt(Mathf.Pow(ii, 2) + Mathf.Pow(jj, 2));
                            if (pathcache[i + ii, j + jj] + steplength+0.0001 < pathcache[i,j])
                            {
                                pathcache[i, j] = pathcache[i+ii, j+jj] + steplength;
                                nochange = false;
                            }
                        }
                    }
                    //}
                }
            }
            //currentdist++;
        }
    }

    // Dijkstra's algorithm, easier to write imo
    public void getPath(Vector3Int startPos_w, Vector3Int endPos_w, List<Vector3Int> steps, int maxDist = 50, bool leavemap = false)
    {//, Unit.ActType todo=Unit.ActType.Movement) {
        startPos_w[2] = 0;
        endPos_w[2] = 0;
        //int maxDist = 100;
        int i, j, ii, jj;
        float[,] dists = new float[xsize, ysize];
        for (i = 0; i < xsize; i++)
        {
            for (j = 0; j < ysize; j++)
            {

                dists[i, j] = maxDist;

            }
        }
        Vector3Int offset = new Vector3Int(xmin, ymin, 0);
        Vector3Int startPos = startPos_w - offset;
        Vector3Int endPos = endPos_w - offset;

        //Debug.Log(startPos + " " + endPos + " " + offset);
        if (!leavemap)
        {
            dists[endPos[0], endPos[1]] = 0;
        }
        else {
            for (i = 1; i < xsize-1; i++)
            {
                for (j = 1; j < ysize-1; j++)
                {
                    if (i == 1 || i == xsize - 2 || j == 1 || j == ysize - 2)
                    {
                        dists[i, j] = 0;
                    }

                }
            } 
        }
        float currentDist = 0;
        int breaker = 0;
        float steplength = 0;
        while (Mathf.Approximately(dists[startPos[0], startPos[1]], maxDist) && breaker < maxDist)
        {
            breaker++;
            //currentDist++;
            for (i = 1; i < xsize - 1; i++)
            {
                for (j = 1; j < ysize - 1; j++)
                {
                    if (i != startPos.x || j != startPos.y)
                    {
                        if (!passable[i, j] || (!leavemap && HasObj(i + xmin, j + ymin)) || !nowalking[i,j])
                        {
                            continue;
                        }
                    }

                    currentDist = dists[i, j];
                    for (ii = -1; ii < 2; ii++)
                    {
                        for (jj = -1; jj < 2; jj++)
                        {
                            steplength = Mathf.Sqrt(Mathf.Pow(ii, 2) + Mathf.Pow(jj, 2));
                            if (dists[i + ii, j + jj] + steplength < currentDist)
                            {
                                dists[i, j] = dists[i + ii, j + jj] + steplength;
                                currentDist = dists[i, j];
                            }
                        }
                    }
                }
            }
        }
        if (!Mathf.Approximately(dists[startPos[0], startPos[1]], maxDist))
        {

            Vector3Int currentPos = startPos;
            Vector3Int nextStep = startPos;
            float distval;
            breaker = 0;
            while (currentPos != endPos && breaker < maxDist)
            {
                //Debug.Log(currentPos + " " + endPos);
                steps.Add(currentPos + offset);
                distval = dists[currentPos.x, currentPos.y];
                for (i = -1; i < 2; i++)
                {
                    for (j = -1; j < 2; j++)
                    {
                        if (dists[currentPos.x + i, currentPos.y + j] < distval)
                        {
                            distval = dists[currentPos.x + i, currentPos.y + j];
                            nextStep = currentPos + new Vector3Int(i, j, 0);
                        }
                    }
                }
                currentPos = nextStep;
                breaker++;
            }
            steps.Add(currentPos + offset);
        }
        //Remove duplicates
        if (steps.Count>0) {
            i = 0;
            Vector3Int lastVect = Vector3Int.zero;
            while (i<steps.Count) {
                if (steps[i]==lastVect) {
                    steps.RemoveAt(i);
                }
                else {
                    lastVect = steps[i];
                    i++;
                }
            }
        }
        //steps.
    }

    bool HasObj(int x, int y)
    {

        foreach (Transform trans in Entities)
        {
            if (Mathf.FloorToInt(trans.position.x) == x && Mathf.FloorToInt(trans.position.y) == y)
            {
                return true;
            }
        }
        return false;
    }

    public GameObject GetObjectPrecise(int x, int y, GameObject requester = null)
    {
        GameObject toReturn = null;
        foreach (Transform trans in Entities)
        {
            if (Mathf.FloorToInt(trans.position.x) == x && Mathf.FloorToInt(trans.position.y) == y)
            {
                if (trans.gameObject == requester)
                {
                    continue;
                }
                return trans.gameObject;
            }
        }
        return toReturn;
    }

    public GameObject GetObject(int x, int y, GameObject requester = null, string findtag="")
    {
        float mindist = 1000;
        GameObject toReturn = null;
        float currentdist = 1000;
        foreach (Transform trans in Entities)
        {
            currentdist = Mathf.Sqrt(Mathf.Pow(x - Mathf.FloorToInt(trans.position.x), 2f)
                                   + Mathf.Pow(y - Mathf.FloorToInt(trans.position.y), 2f));
            if (currentdist < mindist && trans.gameObject != requester)
            {
                //Debug.Log(trans.tag + " " + findtag);
                if (findtag == "" || trans.tag == findtag)
                {
                    mindist = currentdist;
                    toReturn = trans.gameObject;
                }
            }
        }
        return toReturn;
    }

    public List<Transform> GetInCone(Vector3Int startpos_w, Vector3Int targpos_w, int range, int range2 = -1) {
        if (range2==-1) {
            range2=range;
        }
        List<Transform> toReturn = new List<Transform>();
        Vector3 startpos = new Vector3(startpos_w[0], startpos_w[1], 0);
        Vector3 targpos = new Vector3(targpos_w[0], targpos_w[1], 0);
        Vector3 vect = targpos - startpos;
        Vector3 perpvect = new Vector3(-vect[1], vect[0], 0);
        Vector3 p1 = startpos;
        Vector3 p2=startpos+(vect.normalized * range) - perpvect.normalized * range2 / 2f;
        Vector3 p3=startpos + (vect.normalized * range) + perpvect.normalized * range2 / 2f;
        Vector3 v1 = p2 - p1;
        Vector3 v2 = p3 - p2;
        Vector3 v3 = p1 - p3;
        Vector3 v1p, v2p, v3p;
        foreach(Transform trans in Entities) {
            v1p = trans.position - p1;
            v2p = trans.position - p2;
            v3p = trans.position - p3;
            if (Vector3.Dot(v1,v1p)>=0 && Vector3.Dot(v2, v2p) >= 0 && Vector3.Dot(v3, v3p) >= 0) {
                if (!CheckLine(startpos_w,Vector3Int.FloorToInt(trans.position),range)) {
                    toReturn.Add(trans);
                }
            }
        }

        return toReturn;
    }

    public List<Transform> GetInCircle(Vector3Int startpos_w,int range) {
        List<Transform> toReturn = new List<Transform>();
        Vector3 startpos = new Vector3(startpos_w[0], startpos_w[1], 0);
        foreach (Transform trans in Entities)
        {
            if ((trans.position-startpos).magnitude<=range)
            {
                if (!CheckLine(startpos_w, Vector3Int.FloorToInt(trans.position), range))
                {
                    toReturn.Add(trans);
                }
            }
        }

        return toReturn;
    }

    public void RemoveEntity(GameObject obj) {
        Entities.Remove(obj.transform);
    }

    /*
    // Try to implement A*
    // err nevermind not quite worth the effort for game jam
    public void getPath(Vector3Int startPos, Vector3Int endPos, List<Vector3Int> steps) {

        List<PathNode> closedList = new List<PathNode>();
        List<PathNode> openList = new List<PathNode>();
        int breaker = 0;
        openList.Add(new PathNode(startPos, startPos, 0));

        // do until endpoint is reached, or giving up
        while (openList[0].pos != endPos && breaker<1000 ) {

            // Add lowest cost item in openList to closedList
            closedList.Add(openList[0]);


            for (int i = -1; i <= 1;i++) {
                for (int j = -1; j <= 1;j++) {

                    if (i<xmin || i>=xmax || j<ymin || j>=ymax) {
                        continue;
                    }
                    if (passable[i-xmin+openList[0].pos.x,j-ymin+ openList[0].pos.y]) {
                        PathNode newNode =
                            new PathNode(openList[0].pos + new Vector3Int(i, j, 0),
                                     openList[0].pos, 1);
                        if 
                    }

                }
            }
            openList.RemoveAt(0);

        }
    }

    struct PathNode {
        public Vector3Int pos;
        public Vector3Int cameFrom;
        public int cost;
        public PathNode(Vector3Int x, Vector3Int y, int z) {
            pos = x;
            cameFrom = y;
            cost = z;
        }
    }

	// Update is called once per frame
	//void Update () {
		
	//}
    */
}
