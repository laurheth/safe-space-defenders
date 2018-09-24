using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGridController : MonoBehaviour {
    
    public Tilemap tileMap;
    public Tilemap collisionMap;
    int xsize, ysize;
    int xmin, ymin;
    int xmax, ymax;
    bool[,] passable;
	// Use this for initialization
	void Start () {
        xsize = collisionMap.size.x;
        xmin = collisionMap.cellBounds.xMin;
        xmax = collisionMap.cellBounds.xMax;
        ysize = collisionMap.size.y;
        ymin = collisionMap.cellBounds.yMin;
        ymax = collisionMap.cellBounds.yMax;
        passable = new bool[xsize,ysize];
        for (int i = 0; i < xsize;i++) {
            for (int j = 0; j < ysize;j++) {

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
	
    // Djriska or whomever's algorithm, easier to write imo
    public void getPath(Vector3Int startPos_w, Vector3Int endPos_w, List<Vector3Int> steps) {
        startPos_w[2] = 0;
        endPos_w[2] = 0;
        int maxDist = 100;
        int i, j,ii,jj;
        int[,] dists = new int[xsize, ysize];
        for (i = 0; i < xsize;i++) {
            for (j = 0; j < ysize;j++) {

                dists[i, j] = maxDist;

            }
        }
        Vector3Int offset = new Vector3Int(xmin, ymin, 0);
        Vector3Int startPos = startPos_w - offset;
        Vector3Int endPos = endPos_w - offset;

        Debug.Log(startPos + " " + endPos + " " + offset);

        dists[endPos[0], endPos[1]] = 0;
        int currentDist = 0;
        int breaker = 0;
        while (dists[startPos[0], startPos[1]] == maxDist && breaker < maxDist)
        {
            breaker++;
            //currentDist++;
            for (i = 1; i < xsize-1; i++)
            {
                for (j = 1; j < ysize-1; j++)
                {
                    if (!passable[i,j]) {
                        continue;
                    }
                    currentDist = dists[i, j];
                    for (ii = -1; ii < 2;ii++) {
                        for (jj = -1; jj < 2; jj++)
                        {
                            if (dists[i+ii, j+jj]+Mathf.Abs(ii)+Mathf.Abs(jj) < currentDist)
                            {
                                dists[i, j] = dists[i + ii, j + jj] + Mathf.Abs(ii) + Mathf.Abs(jj);
                                currentDist = dists[i, j];
                            }
                        }
                    }
                }
            }
        }
        if (dists[startPos[0], startPos[1]] != maxDist)
        {

            Vector3Int currentPos = startPos;
            Vector3Int nextStep = startPos;
            int distval;
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
        }
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
