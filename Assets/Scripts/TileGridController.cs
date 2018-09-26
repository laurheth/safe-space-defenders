using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGridController : MonoBehaviour {
    
    public Tilemap tileMap;
    public Tilemap collisionMap;
    GridLayout grid;
    int xsize, ysize;
    int xmin, ymin;
    int xmax, ymax;
    bool[,] passable;
    List<Transform> Entities;
    //List<>
	// Use this for initialization
	void Start () {
        Entities = new List<Transform>();

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Unit")) {
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
        passable = new bool[xsize,ysize];
        for (int i = 0; i < xsize;i++) {
            for (int j = 0; j < ysize;j++) {
                //passable[i+xmin,j+ymin]=valid
                if (collisionMap.HasTile(new Vector3Int(i+xmin,j+ymin,0))) {
                    passable[i, j] = false;
                }
                else {
                    passable[i, j] = true;
                }
                //Debug.Log(i + " " + j + " "+passable[i,j]);
                //Debug.Log(xmin + " " + xmax);
            }
        }

	}
	
    public bool validPos(Vector3 pos_w, Unit.Action todo) {
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
        Vector3Int pos = grid.WorldToCell(pos_w);
        if (pos.x > xmin && pos.x < xmax && pos.y > ymin && pos.y < ymax)
            {
            pos[2] = 0;
            if (!onlyentities)
            {
                return passable[pos.x - xmin, pos.y - ymin] && tileMap.HasTile(pos) && (!skipentities || !HasObj(pos.x, pos.y));
            }
            else {
                return HasObj(pos.x, pos.y);
            }
        //return !collisionMap.HasTile(pos) && tileMap.HasTile(pos);
        }
        else {
            return false;
        }
    }

    public bool CheckLine(Vector3Int startPos_w, Vector3Int endPos_w, int maxdist=20, bool checkentities=false) {
        Vector3Int offset = new Vector3Int(xmin, ymin, 0);
        Vector3Int startPos = startPos_w - offset;
        Vector3Int endPos = endPos_w - offset;
        Vector3 checkPos = startPos_w - offset;
        Vector3 step = (new Vector3(endPos_w.x - startPos_w.x,
                                   endPos_w.y - startPos_w.y,
                                    endPos_w.z - startPos_w.z)).normalized;
        if ((startPos - endPos).magnitude > maxdist) { return true; }
        int i, j;
        int breaker=0;
        while (Vector3Int.RoundToInt(checkPos) != endPos && breaker<maxdist) {
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

    // Djriska or whomever's algorithm, easier to write imo
    public void getPath(Vector3Int startPos_w, Vector3Int endPos_w, List<Vector3Int> steps, int maxDist=50) {
        startPos_w[2] = 0;
        endPos_w[2] = 0;
        //int maxDist = 100;
        int i, j,ii,jj;
        float[,] dists = new float[xsize, ysize];
        for (i = 0; i < xsize;i++) {
            for (j = 0; j < ysize;j++) {

                dists[i, j] = maxDist;

            }
        }
        Vector3Int offset = new Vector3Int(xmin, ymin, 0);
        Vector3Int startPos = startPos_w - offset;
        Vector3Int endPos = endPos_w - offset;

        //Debug.Log(startPos + " " + endPos + " " + offset);

        dists[endPos[0], endPos[1]] = 0;
        float currentDist = 0;
        int breaker = 0;
        float steplength = 0;
        while (Mathf.Approximately(dists[startPos[0], startPos[1]], maxDist) && breaker < maxDist)
        {
            breaker++;
            //currentDist++;
            for (i = 1; i < xsize-1; i++)
            {
                for (j = 1; j < ysize-1; j++)
                {
                    if (i != startPos.x || j != startPos.y)
                    {
                        if (!passable[i, j] || HasObj(i + xmin, j + ymin))
                        {
                            continue;
                        }
                    }

                    currentDist = dists[i, j];
                    for (ii = -1; ii < 2;ii++) {
                        for (jj = -1; jj < 2; jj++)
                        {
                            steplength = Mathf.Sqrt(Mathf.Pow(ii, 2) + Mathf.Pow(jj, 2));
                            if (dists[i+ii, j+jj]+steplength < currentDist)
                            {
                                dists[i, j] = dists[i + ii, j + jj] + steplength;
                                currentDist = dists[i, j];
                            }
                        }
                    }
                }
            }
        }
        if (!Mathf.Approximately(dists[startPos[0], startPos[1]],maxDist))
        {

            Vector3Int currentPos = startPos;
            Vector3Int nextStep = startPos;
            float distval;
            breaker = 0;
            while (currentPos != endPos && breaker < maxDist)
            {
                //Debug.Log(currentPos + " " + endPos);
                steps.Add(currentPos+offset);
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
    }

    bool HasObj(int x, int y) {

        foreach (Transform trans in Entities) {
            if (Mathf.FloorToInt(trans.position.x)==x && Mathf.FloorToInt(trans.position.y) == y) {
                return true;
            }
        }
        return false;
    }

    public GameObject GetObject(int x, int y, GameObject requester=null) {
        float mindist = 1000;
        GameObject toReturn=null;
        float currentdist = 1000;
        foreach (Transform trans in Entities)
        {
            currentdist = Mathf.Sqrt(Mathf.Pow(x - Mathf.FloorToInt(trans.position.x), 2f)
                                   + Mathf.Pow(y - Mathf.FloorToInt(trans.position.y), 2f));
            if (currentdist < mindist && trans.gameObject != requester) {
                mindist = currentdist;
                toReturn = trans.gameObject;
            }
        }
        return toReturn;
    }

    public List<Transform> GetInCone(Vector3Int startpos_w, Vector3Int targpos_w,int range) {
        List<Transform> toReturn = new List<Transform>();
        Vector3 startpos = new Vector3(startpos_w[0], startpos_w[1], 0);
        Vector3 targpos = new Vector3(targpos_w[0], targpos_w[1], 0);
        Vector3 vect = targpos - startpos;
        Vector3 perpvect = new Vector3(-vect[1], vect[0], 0);
        Vector3 p1 = startpos;
        Vector3 p2=startpos+(vect.normalized * range) - perpvect.normalized * range / 2f;
        Vector3 p3=startpos + (vect.normalized * range) + perpvect.normalized * range / 2f;
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
