using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Pacman : MonoBehaviour {

    public int currentRow;
    public int currentCol;

    public List<string> finalMap;
    public int maxCol;

    private int score = 0;
    public Text text;
    public List<List<GameObject>> thePellets;

    public bool begin = false;

    private string direction = "up";
    private string cameFrom = "";
    private bool wasBlocked = false;

    List<List<GameObject>> pelletGroups;
    public GameObject targetPellet;

    int currentPelletGroup;

    bool usingAStarMovement = false;

    public List<GameObject> tilePath;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        

    }

    void FixedUpdate() {
        if (begin)
        {
            //iterate();
            //Normal Pellet following
            if (!usingAStarMovement)
            {
                safeGroupMovement();
            }

            //Following the Astar Path
            else if(usingAStarMovement)
            {
                aStarMovement();
            }
            
            if (string.Compare(direction, "up") == 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, 90f);
            }
            else if (string.Compare(direction, "left") == 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, 180);
            }
            else if (string.Compare(direction, "down") == 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, 270);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    //Called when Pacman has no threats to him. Eats pellets by the group
    void safeGroupMovement()
    {
        //Return a string for the direction Pacman moves to eat pellets in group

        //TODO
        //Need to think of more sophisticated way for him to go through group

        //Go through list of pellets in group and eat in order.
        
        bool upObstacle = false;
        bool downObstacle = false;
        bool leftObstacle = false;
        bool rightObstacle = false;
        //If Pellet is above
        if (transform.position.y < targetPellet.transform.position.y && (!wasBlocked || !(string.Compare(cameFrom, "up") == 0)))
        {

            //Move up and avoid obstacles
            if (thePellets[currentRow - 1][currentCol].tag == "Pellet")
            {
                transform.position = new Vector3(currentCol, -currentRow + 1, transform.position.z);
                currentRow--;
                direction = "up";
                removePellet();
                wasBlocked = false;

            }
            //Obstacle
            else 
            {
                upObstacle = true;
            }
        }

        //If Pellet is below
        else if (transform.position.y > targetPellet.transform.position.y && (!wasBlocked || !(string.Compare(cameFrom, "down") == 0)))
        {

            //Move down and avoid obstacles
            if (thePellets[currentRow + 1][currentCol].tag == "Pellet")
            {
                transform.position = new Vector3(currentCol, -currentRow - 1, transform.position.z);
                currentRow++;
                direction = "down";
                removePellet();
                wasBlocked = false;
            }

            //obstacle
            else
            {
                downObstacle = true;
            }
        }


        //If Pellet is left
        if (transform.position.x > targetPellet.transform.position.x && (!wasBlocked || !(string.Compare(cameFrom, "left") == 0)))
        {

            //Move left and avoid obstacles
            if (thePellets[currentRow][currentCol - 1].tag == "Pellet")
            {
                transform.position = new Vector3(currentCol - 1, -currentRow, transform.position.z);
                currentCol--;
                direction = "left";
                removePellet();
                wasBlocked = false;
            }

            //Obstacle
            else
            {
                leftObstacle = true;
            }
        }

        //If Pellet is right
        else if (transform.position.x < targetPellet.transform.position.x && (!wasBlocked || !(string.Compare(cameFrom, "right") == 0)))
        {

            //Move right and avoid obstacles
            if (thePellets[currentRow][currentCol + 1].tag == "Pellet")
            {
                transform.position = new Vector3(currentCol + 1, -currentRow, transform.position.z);
                currentCol++;
                direction = "right";
                removePellet();
                wasBlocked = false;
            }

            //Obstacle
            else
            {
                rightObstacle = true;
            }
        }

        //If theres an obstacle run a*
        if (rightObstacle || leftObstacle || upObstacle || downObstacle)
        {
            Debug.Log("Looking for a* path" + currentRow);
            aStarTile();

        }

        /*
        //Deal with obstacle avoidance here
        if (rightObstacle || leftObstacle || upObstacle || downObstacle)
        {
            if (rightObstacle || leftObstacle)
            {        //if going left or right and blocked
                if (thePellets[currentRow - 1][currentCol].tag == "Pellet")     //try up
                {
                    cameFrom = "down";
                    transform.position = new Vector3(currentCol, -currentRow + 1, transform.position.z);
                    direction = "up";
                    currentRow--;
                    removePellet();
                    wasBlocked = true;
                }
                else if (thePellets[currentRow + 1][currentCol].tag == "Pellet")      //try down
                {
                    cameFrom = "up";
                    transform.position = new Vector3(currentCol, -currentRow - 1, transform.position.z);
                    direction = "down";
                    currentRow++;
                    removePellet();
                    wasBlocked = true;
                }
                else
                {              //move back
                    if (leftObstacle)
                    {
                        transform.position = new Vector3(currentCol + 1, -currentRow, transform.position.z);
                        currentCol++;
                        direction = "right";
                        cameFrom = "left";
                        removePellet();
                        wasBlocked = true;
                    }
                    else
                    {
                        transform.position = new Vector3(currentCol - 1, -currentRow, transform.position.z);
                        currentCol--;
                        direction = "left";
                        cameFrom = "right";
                        removePellet();
                        wasBlocked = true;
                    }
                }
            }
            else {
                if (thePellets[currentRow][currentCol-1].tag == "Pellet")     //try left
                {

                    cameFrom = "right";
                    transform.position = new Vector3(currentCol - 1, -currentRow, transform.position.z);
                    direction = "left";
                    currentCol--;
                    removePellet();
                    wasBlocked = true;
                }
                else if (thePellets[currentRow][currentCol+1].tag == "Pellet")      //try right
                {
                    cameFrom = "left";
                    transform.position = new Vector3(currentCol + 1, -currentRow, transform.position.z);
                    direction = "right";
                    currentCol++;
                    removePellet();
                    wasBlocked = true;
                }
                else
                {              //move back
                    if (downObstacle)
                    {
                        transform.position = new Vector3(currentCol, -currentRow + 1, transform.position.z);
                        currentRow--;
                        direction = "up";
                        cameFrom = "down";
                        removePellet();
                        wasBlocked = true;
                    }
                    else
                    {
                        transform.position = new Vector3(currentCol, -currentRow - 1, transform.position.z);
                        currentRow++;
                        direction = "down";
                        cameFrom = "up";
                        removePellet();
                        wasBlocked = true;
                    }
                }
            }
        }*/
        

    }

    void aStarMovement()
    {
        Debug.Log("about to set position" + score);
        //Move Pacman to next spot in path
        transform.position = tilePath[0].transform.position;
        Debug.Log("pos set to " + tilePath[0].transform.position);
        //Set his new row and col correctly
        currentCol = tilePath[0].GetComponent<PelletInfo>().col;
        currentRow = tilePath[0].GetComponent<PelletInfo>().row;

        Debug.Log("moving along path");

        //Remove pellet if there is one
        removePellet();

        //Delete from array
        tilePath.RemoveAt(0);

        Debug.Log("Removed pellet from path");
        //Found goal
        if(tilePath.Count == 0)
        {
            Debug.Log("finished a* movement" + currentCol);
            usingAStarMovement = false;

            //Select new target
            findClosestGroup();
        }
    }

    void aStarTile()
    {

        tilePath = new List<GameObject>();

        bool goalReached = false;
        

        //Open list contains start
        List<GameObject> openList = new List<GameObject>();
        List<GameObject> closedList = new List<GameObject>();
        openList.Add(thePellets[currentRow][currentCol]);

        //Loop until we find the goal
        while (!goalReached)
        {
            //Sort list by total cost. Lowest cost in front of list
            openList.Sort(SortByCost);

            //Current tile is lowest cost
            GameObject currentTile = openList[0];
            PelletInfo currentTileInfo = currentTile.GetComponent<PelletInfo>();

            //Is current tile goal
            if (currentTile.gameObject == targetPellet)
            {
                goalReached = true;
                Debug.Log("Found it" + targetPellet.transform.position);

                //Add tile path to array
                GameObject tileForArray = currentTile;
                while (tileForArray != null)
                {
                    tilePath.Insert(0, tileForArray);
                    tileForArray = tileForArray.GetComponent<PelletInfo>().parent;
                }

                //Switch to astar path movement
                usingAStarMovement = true;
                Debug.Log("Starting a* movement" + currentRow);
                //Set targetpellet to first element of path
                targetPellet = tilePath[0];

                //Reset Pellet info
                Debug.Log("resetting pellets");
                currentTile.GetComponent<PelletInfo>().reset();
                for(int i = 0; i < closedList.Count; i++)
                {
                    closedList[i].GetComponent<PelletInfo>().reset();
                }
                for (int i = 0; i < openList.Count; i++)
                {
                    openList[i].GetComponent<PelletInfo>().reset();
                }
                Debug.Log("done with reset");
            }

            //Keep looking
            else
            {
                //Move current node to closed list and look at neighbors
                closedList.Add(currentTile);
                openList.RemoveAt(0);

                //Find neighbors
                List<GameObject> neighbors = findNeighbors(currentTile, currentTileInfo);


                for (int i = 0; i < neighbors.Count; i++)
                {
                    PelletInfo neighborInfo = neighbors[i].GetComponent<PelletInfo>();

                    //Neighbor in closed list & current start cost < neighbor start cost
                    if (closedList.Contains(neighbors[i]) && currentTileInfo.costFromStart < neighborInfo.costFromStart)
                    {
                        neighborInfo.costFromStart = currentTileInfo.costFromStart;
                        neighborInfo.parent = currentTile;
                    }

                    //neighbor in open list & current start cost < neighbor start cost
                    else if (openList.Contains(neighbors[i]) && currentTileInfo.costFromStart < neighborInfo.costFromStart)
                    {
                        neighborInfo.costFromStart = currentTileInfo.costFromStart;
                        neighborInfo.parent = currentTile;
                    }

                    //Neighbor not in either list. Set Distance from start and add to open
                    else if (!openList.Contains(neighbors[i]) && !closedList.Contains(neighbors[i]))
                    {
                        openList.Add(neighbors[i]);
                        neighborInfo.costFromStart = currentTileInfo.costFromStart + 1;
                        neighborInfo.parent = currentTile;

                    }
                }
            }
        }

    }

    //Used as overloaded operation to find the lower cost of two tiles. FOR TILES
    int SortByCost(GameObject alowest, GameObject acurrent)
    {
        PelletInfo lowest = alowest.GetComponent<PelletInfo>();
        PelletInfo current = acurrent.GetComponent<PelletInfo>();

        return lowest.totalCost(targetPellet).CompareTo(current.totalCost(targetPellet));
    }

    //Called when Pacman moves to check if he is eating a pellet
    void removePellet()
    {
        //Remove Pellet
        GameObject aPellet = thePellets[currentRow][currentCol].gameObject;
        if (aPellet.GetComponent<PelletInfo>().eaten == false)
        {
            score += 1;
            aPellet.GetComponent<SpriteRenderer>().enabled = false;
            aPellet.GetComponent<PelletInfo>().eaten = true;
            text.text = "SCORE: " + score;

            //is this the targetpellet?
            if (aPellet == targetPellet)
            {
                bool noPelletsLeftInGroup = true;
                for (int i = 0; i < pelletGroups[currentPelletGroup].Count; i++)
                {
                    //Find uneaten pellet and set as target
                    if (pelletGroups[currentPelletGroup][i].GetComponent<PelletInfo>().eaten == false)
                    {
                        targetPellet = pelletGroups[currentPelletGroup][i];
                        noPelletsLeftInGroup = false;
                        break;
                    }
                }
                //Group empty. Switch groups
                if (noPelletsLeftInGroup)
                {
                    findClosestGroup();
                }
            }
        }
    }

    List<GameObject> findNeighbors(GameObject currentTile, PelletInfo currentTileInfo)
    {
        List<GameObject> neighbors = new List<GameObject>();
        int rowIndex = currentTileInfo.row;
        int colIndex = currentTileInfo.col;

        //Directly below
        //Check for out of bounds
        if (finalMap.Count >= rowIndex + 1)
        {
            if (finalMap[rowIndex + 1][colIndex] == '.')
            {
                //Increment distance from start and add to list of neighbors
                GameObject aneighbor = thePellets[rowIndex + 1][colIndex];
                neighbors.Add(aneighbor);
            }
        }
        //Directly Above
        //Check for out of bounds
        if (rowIndex - 1 >= 0)
        {
            if (finalMap[rowIndex - 1][colIndex] == '.')
            {
                //Increment distance from start and add to list of neighbors
                GameObject aneighbor = thePellets[rowIndex - 1][colIndex];
                neighbors.Add(aneighbor);
            }
        }
        //Directly right
        //Check for out of bounds
        if (colIndex + 1 <= maxCol)
        {
            if (finalMap[rowIndex][colIndex + 1] == '.')
            {
                //Increment distance from start and add to list of neighbors
                GameObject aneighbor = thePellets[rowIndex][colIndex + 1];
                neighbors.Add(aneighbor);
            }
        }

        //Directly left
        //Check for out of bounds
        if (colIndex - 1 >= 0)
        {
            if (finalMap[rowIndex][colIndex - 1] == '.')
            {
                //Increment distance from start and add to list of neighbors
                GameObject aneighbor = thePellets[rowIndex][colIndex - 1];
                neighbors.Add(aneighbor);
            }
        }

        return neighbors;
    }

    //Called when Pacman needs to find the closest group of pellets to begin eating
    void findClosestGroup()
    {
        Debug.Log("finding new group" + score);
        //Iterate until we get the closest pellet
        bool foundClosest = false;
        int iter = 1;
        while (!foundClosest)
        {
            //Look Up
            //Check for out of bounds
            if (currentRow - iter >= 0)
            {
                if (thePellets[currentRow - iter][currentCol].tag == "Pellet")
                {
                    if (thePellets[currentRow - iter][currentCol].GetComponent<PelletInfo>().eaten == false)
                    {
                        foundClosest = true;
                        currentPelletGroup = thePellets[currentRow - iter][currentCol].GetComponent<PelletInfo>().groupNum;
                        break;
                    }
                }
            }

            //Look Down
            //Check for out of bounds
            if (currentRow + iter <= thePellets.Count)
            {
                if (thePellets[currentRow + iter][currentCol].tag == "Pellet")
                {
                    if (thePellets[currentRow + iter][currentCol].GetComponent<PelletInfo>().eaten == false)
                    {
                        foundClosest = true;
                        currentPelletGroup = thePellets[currentRow + iter][currentCol].GetComponent<PelletInfo>().groupNum;
                        break;
                    }
                }
            }

            //Look Left
            //Check for out of bounds
            if (currentCol - iter >= 0)
            {
                if (thePellets[currentRow][currentCol - iter].tag == "Pellet")
                {
                    if (thePellets[currentRow][currentCol - iter].GetComponent<PelletInfo>().eaten == false)
                    {
                        foundClosest = true;
                        currentPelletGroup = thePellets[currentRow][currentCol - iter].GetComponent<PelletInfo>().groupNum;
                        break;
                    }
                }
            }

            //Look Right
            //Check for out of bounds
            if (currentCol + iter <= maxCol)
            {
                if (thePellets[currentRow][currentCol + iter].tag == "Pellet")
                {
                    if (thePellets[currentRow][currentCol + iter].GetComponent<PelletInfo>().eaten == false)
                    {
                        foundClosest = true;
                        currentPelletGroup = thePellets[currentRow][currentCol + iter].GetComponent<PelletInfo>().groupNum;
                        break;
                    }
                }
            }

            //Look Up/Left
            //Check for out of bounds
            if (currentRow - iter >= 0 && currentCol - iter >= 0)
            {
                if (thePellets[currentRow - iter][currentCol - iter].tag == "Pellet")
                {
                    if (thePellets[currentRow - iter][currentCol - iter].GetComponent<PelletInfo>().eaten == false)
                    {
                        foundClosest = true;
                        currentPelletGroup = thePellets[currentRow - iter][currentCol - iter].GetComponent<PelletInfo>().groupNum;
                        break;
                    }
                }
            }

            //Look Up/Right
            //Check for out of bounds
            if (currentRow - iter >= 0 && currentCol + iter <= maxCol)
            {
                if (thePellets[currentRow - iter][currentCol + iter].tag == "Pellet")
                {
                    if (thePellets[currentRow - iter][currentCol + iter].GetComponent<PelletInfo>().eaten == false)
                    {
                        foundClosest = true;
                        currentPelletGroup = thePellets[currentRow - iter][currentCol + iter].GetComponent<PelletInfo>().groupNum;
                        break;
                    }
                }
            }

            //Look Down/Left
            //Check for out of bounds
            if (currentRow + iter <= thePellets.Count && currentCol - iter >= 0)
            {
                if (thePellets[currentRow + iter][currentCol - iter].tag == "Pellet")
                {
                    if (thePellets[currentRow + iter][currentCol - iter].GetComponent<PelletInfo>().eaten == false)
                    {
                        foundClosest = true;
                        currentPelletGroup = thePellets[currentRow + iter][currentCol - iter].GetComponent<PelletInfo>().groupNum;
                        break;
                    }
                }
            }

            //Look Down/Right
            //Check for out of bounds
            if (currentRow + iter <= thePellets.Count && currentCol + iter <= maxCol)
            {
                if (thePellets[currentRow + iter][currentCol + iter].tag == "Pellet")
                {
                    if (thePellets[currentRow + iter][currentCol + iter].GetComponent<PelletInfo>().eaten == false)
                    {
                        foundClosest = true;
                        currentPelletGroup = thePellets[currentRow + iter][currentCol + iter].GetComponent<PelletInfo>().groupNum;
                        break;
                    }
                }
            }

            //None this distance away. Increase distance to look
            iter++;
            
            //failsafe
            if(iter > 20)
            {
                Debug.Log("ERROR ITER BECAME TOO LARGE");
                break;
               
            }

        }
        Debug.Log("found group " + currentPelletGroup);

        //TODO make a better way to select this pellet
        //iterate through the group and select the first pellet
        for(int i = 0; i< pelletGroups[currentPelletGroup].Count; i++)
        {
            if(pelletGroups[currentPelletGroup][i].GetComponent<PelletInfo>().eaten == false)
            {
                targetPellet = pelletGroups[currentPelletGroup][i];
                break;
            }
        }
    }


    void iterate() {

        if (begin)
        {
            //Move Pacman TESTING
            if (string.Compare(direction, "up") == 0)
            {
                //No obstacle
                if (finalMap[currentRow - 1][currentCol] == '.')
                {
                    //Move Pacman Up
                    transform.position = new Vector3(currentCol, -currentRow + 1, transform.position.z);
                    currentRow--;

                    //Remove Pellet
                    GameObject aPellet = thePellets[currentRow][currentCol].gameObject;
                    if (aPellet.GetComponent<PelletInfo>().eaten == false)
                    {
                        score += 1;
                        aPellet.GetComponent<SpriteRenderer>().enabled = false;
                        aPellet.GetComponent<PelletInfo>().eaten = true;
                        text.text = "SCORE: " + score;
                    }

                }
                //Obstacle
                else
                {
                    chooseRandomDirection();
                }
            }

            else if (string.Compare(direction, "left") == 0)
            {
                if (finalMap[currentRow][currentCol - 1] == '.')
                {
                    //Move Pacman Left
                    transform.position = new Vector3(currentCol - 1, -currentRow, transform.position.z);
                    currentCol--;

                    //Remove Pellet
                    GameObject aPellet = thePellets[currentRow][currentCol].gameObject;
                    if (aPellet.GetComponent<PelletInfo>().eaten == false)
                    {
                        score += 1;
                        aPellet.GetComponent<SpriteRenderer>().enabled = false;
                        aPellet.GetComponent<PelletInfo>().eaten = true;
                        text.text = "SCORE: " + score;
                    }

                }
                else
                {
                    chooseRandomDirection();
                }
            }

            else if (string.Compare(direction, "down") == 0)
            {
                if (finalMap[currentRow + 1][currentCol] == '.')
                {
                    //Move Pacman Down
                    transform.position = new Vector3(currentCol, -currentRow - 1, transform.position.z);
                    currentRow++;

                    //Remove Pellet
                    GameObject aPellet = thePellets[currentRow][currentCol].gameObject;
                    if (aPellet.GetComponent<PelletInfo>().eaten == false)
                    {
                        score += 1;
                        aPellet.GetComponent<SpriteRenderer>().enabled = false;
                        aPellet.GetComponent<PelletInfo>().eaten = true;
                        text.text = "SCORE: " + score;
                    }

                }
                else
                {
                    chooseRandomDirection();
                }
            }

            else if (string.Compare(direction, "right") == 0)
            {
                if (finalMap[currentRow][currentCol + 1] == '.')
                {
                    //Move Pacman Right
                    transform.position = new Vector3(currentCol + 1, -currentRow, transform.position.z);
                    currentCol++;

                    //Remove Pellet
                    GameObject aPellet = thePellets[currentRow][currentCol].gameObject;
                    if (aPellet.GetComponent<PelletInfo>().eaten == false)
                    {
                        score += 1;
                        aPellet.GetComponent<SpriteRenderer>().enabled = false;
                        aPellet.GetComponent<PelletInfo>().eaten = true;
                        text.text = "SCORE: " + score;
                    }

                }
                else
                {
                    chooseRandomDirection();
                }
            }
        }
    }

    
    public void setBegin(int groupNum) {
        begin = true;
        pelletGroups = GameObject.Find("Manager").GetComponent<MapLoader>().pelletGroups;

        //Set current pellet group num to pellet placed on.
        currentPelletGroup = groupNum;

        //find target pellet in group
        for (int i = 0; i < pelletGroups[currentPelletGroup].Count; i++)
        {
            if (pelletGroups[currentPelletGroup][i].GetComponent<PelletInfo>().eaten == false)
            {
                targetPellet = pelletGroups[currentPelletGroup][i];
                break;
            }
        }
        //chooseRandomDirection();
    }

    void chooseRandomDirection() {
        ArrayList goodDirec = new ArrayList();

        GameObject aPellet = thePellets[currentRow-1][currentCol].gameObject;
        if (finalMap[currentRow - 1][currentCol] == '.' && !aPellet.GetComponent<PelletInfo>().eaten)
        {
            goodDirec.Add("up");
        }
        aPellet = thePellets[currentRow + 1][currentCol].gameObject;
        if (finalMap[currentRow + 1][currentCol] == '.' && !aPellet.GetComponent<PelletInfo>().eaten) {
            goodDirec.Add("down");
        }
        aPellet = thePellets[currentRow][currentCol + 1].gameObject;
        if (finalMap[currentRow][currentCol + 1] == '.' && !aPellet.GetComponent<PelletInfo>().eaten)
        {
            goodDirec.Add("right");
        }
        aPellet = thePellets[currentRow][currentCol - 1].gameObject;
        if (finalMap[currentRow][currentCol - 1] == '.' && !aPellet.GetComponent<PelletInfo>().eaten)
        {
            goodDirec.Add("left");
        }
        if (goodDirec.Count > 0)
        {
            int random = Random.Range(0, goodDirec.Count);
            direction = (string)goodDirec[random];
        }
        else {
            chooseLameDirection();
        }
        

    }

    void chooseLameDirection() {
        ArrayList goodDirec = new ArrayList();
        
        if (finalMap[currentRow - 1][currentCol] == '.')
        {
            goodDirec.Add("up");
        }
        if (finalMap[currentRow + 1][currentCol] == '.')
        {
            goodDirec.Add("down");
        }
        if (finalMap[currentRow][currentCol + 1] == '.')
        {
            goodDirec.Add("right");
        }
        if (finalMap[currentRow][currentCol - 1] == '.')
        {
            goodDirec.Add("left");
        }

        int random = Random.Range(0, goodDirec.Count);
        direction = (string)goodDirec[random];
    }

}
