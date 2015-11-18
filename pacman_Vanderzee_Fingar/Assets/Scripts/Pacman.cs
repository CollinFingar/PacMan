using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Pacman : MonoBehaviour {

    public int currentRow;
    public int currentCol;

    public List<string> finalMap;
    public int maxCol;
    public int groupSize;

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

    GameObject[] ghosts;

    bool ghostsFar = true;      //True to activate good stage. Follows pellets
    //bool ghostNearby = false;   //True to activiate middle stage. Ghost closish.

    bool noMoreHorizontalGroups = false;    //Used by the middle stage when looking for new pellet group.
    bool noMoreVerticalGroups = false;      //If there isn't any good groups to go to they will be true.
                                            //At which time, we should ignore and do normal pellet eating


    public List<GameObject> tilePath;

    // Use this for initialization
    void Start () {
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");
	}
	
	// Update is called once per frame
	void Update () {

        //TEST CODE FOR MIDDLE BEHAVIOR
        if (Input.GetMouseButtonDown(1))
        {
            //do middle state according to closest ghost.
            ghostsFar = false;
        }
    }

    void FixedUpdate() {
        if (begin)
        {
            
            //TESTING NEW BEHAVIOR
            if (!ghostsFar)
            {
                float closest = Mathf.Infinity;
                int closeIndex = 0;
                for(int i = 0; i < ghosts.Length; i++)
                {
                    float aDistance = Vector2.Distance(transform.position, ghosts[i].transform.position);
                    if(aDistance < closest)
                    {
                        closest = aDistance;
                        closeIndex = i;
                    }
                }

                //Switch pellet groups to avoid ghost
                findNeighboringGroup(ghosts[closeIndex]);
                ghostsFar = true;


            }

            //Normal Pellet following
            if (!usingAStarMovement & ghostsFar)
            {
                safeGroupMovement();
            }

            //Following the Astar Path
            else if(usingAStarMovement & ghostsFar)
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



    //Called when ghosts are closish to Pacman and should switch groups to be safe
    void findNeighboringGroup(GameObject ghost)
    {
        //Find which direction we need to go
        //Is the ghost closer in vertical or horizontal?
        float vertDistance = Mathf.Abs(transform.position.y - ghost.transform.position.y);
        float horizDistance = Mathf.Abs(transform.position.x - ghost.transform.position.x);

        Debug.Log("current group is " + currentPelletGroup);

        //Closer Horizontally
        if (vertDistance > horizDistance)
        {
            lookHorizontally(ghost);
        }

        //Closer Vertically
        else
        {
            lookVertically(ghost);
        }

        //If there isn't a smart group to move to, ignore and go back to normal pellet eating
        if(noMoreHorizontalGroups && noMoreVerticalGroups)
        {
            Debug.Log("No good groups to switch to");
            //ghostsFar = true;
        }

        Debug.Log("new group is " + currentPelletGroup);
        
    }

    //Called by findNeighboringGroup when we look vertically for a new group
    void lookVertically(GameObject ghost)
    {
        int groupsPerRow = maxCol / groupSize;
        int groupsPerCol = (finalMap.Count - 1) / groupSize;
        noMoreVerticalGroups = false;
        //Do we look up or down?
        //Ghost is up
        if (ghost.transform.position.y > transform.position.y)
        {
            //Look for a non empty group below
            bool keepLookingDown = true;
            int iter = groupsPerRow;
            while (keepLookingDown)
            {
                //Look down a row. currentgroup + groupsperrow
                //Don't go out of bounds
                if (currentPelletGroup + iter > pelletGroups.Count)
                {
                    //No more rows below
                    keepLookingDown = false;
                    noMoreVerticalGroups = true;
                    //Look horizontally instead
                    lookHorizontally(ghost);
                    break;
                }

                //Look at group below
                else
                {
                    //Is there an empty pellet
                    for (int i = 0; i < pelletGroups[currentPelletGroup + iter].Count; i++)
                    {
                        if (pelletGroups[currentPelletGroup + iter][i].GetComponent<PelletInfo>().eaten == false)
                        {
                            //Set new target pellet and group
                            targetPellet = pelletGroups[currentPelletGroup + iter][i];
                            currentPelletGroup = currentPelletGroup + iter;
                            keepLookingDown = false;
                            break;
                        }
                    }
                }
                iter += groupsPerRow;
            }

        }
        //Ghost is below
        else
        {
            //Look for a non empty group above
            bool keepLookingUp = true;
            int iter = groupsPerRow;
            while (keepLookingUp)
            {
                //Look up a row. currentgroup - groupsperrow
                //Don't go out of bounds
                if (currentPelletGroup - iter < 0)
                {
                    //No more rows below
                    keepLookingUp = false;
                    noMoreVerticalGroups = true;
                    //Look horizontally instead
                    lookHorizontally(ghost);
                    break;
                }

                //Look at group above
                else
                {
                    //Is there an empty pellet
                    for (int i = 0; i < pelletGroups[currentPelletGroup - iter].Count; i++)
                    {
                        if (pelletGroups[currentPelletGroup - iter][i].GetComponent<PelletInfo>().eaten == false)
                        {
                            //Set new target pellet and group
                            targetPellet = pelletGroups[currentPelletGroup - iter][i];
                            currentPelletGroup = currentPelletGroup - iter;
                            keepLookingUp = false;
                            break;
                        }
                    }
                }
                iter += groupsPerRow;
            }
        }
    }
    
    //Called by findNeighboringGroup when we look horizontally for a new group
    void lookHorizontally(GameObject ghost)
    {

        int groupsPerRow = maxCol / groupSize;
        int groupsPerCol = (finalMap.Count - 1) / groupSize;
        noMoreHorizontalGroups = false;

        //Do we look left or right?
        //Ghost is to the right
        if (ghost.transform.position.x > transform.position.x)
        {
            //Look for a non empty group to the left
            bool keepLookingLeft = true;
            int iter = 1;
            while (keepLookingLeft)
            {
                //Don't go up a row
                if ((currentPelletGroup - iter) % groupsPerRow == 0)
                {
                    //No more groups to the left
                    keepLookingLeft = false;
                    noMoreHorizontalGroups = true;
                    //Look vertically instead
                    lookVertically(ghost);
                    break;
                }
                //Look at group to the left
                else
                {
                    //Is there an empty pellet
                    for (int i = 0; i < pelletGroups[currentPelletGroup - iter].Count; i++)
                    {
                        if (pelletGroups[currentPelletGroup - iter][i].GetComponent<PelletInfo>().eaten == false)
                        {
                            //Set new target pellet and group
                            targetPellet = pelletGroups[currentPelletGroup - iter][i];
                            currentPelletGroup = currentPelletGroup - iter;
                            keepLookingLeft = false;
                            break;
                        }
                    }
                }
                iter++;
            }

        }
        //Ghost is to the left
        else
        {
            //Look for a non empty group to the right
            bool keepLookingRight = true;
            int iter = 1;
            while (keepLookingRight)
            {
                //Don't go down a row
                if ((currentPelletGroup + iter) % groupsPerRow == 0)
                {
                    //No more groups to the right
                    keepLookingRight = false;
                    noMoreHorizontalGroups = true;
                    //Look vertically instead
                    lookVertically(ghost);
                    break;
                }
                //Look at group to the right
                else
                {
                    //Is there an empty pellet
                    for (int i = 0; i < pelletGroups[currentPelletGroup + iter].Count; i++)
                    {
                        if (pelletGroups[currentPelletGroup + iter][i].GetComponent<PelletInfo>().eaten == false)
                        {
                            //Set new target pellet and group
                            targetPellet = pelletGroups[currentPelletGroup + iter][i];
                            currentPelletGroup = currentPelletGroup + iter;
                            keepLookingRight = false;
                            break;
                        }
                    }
                }
                iter++;
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
            aStarTile();
        }
    }

    //Called to manually move pacman along the a* path
    void aStarMovement()
    {
       
        //Move Pacman to next spot in path
        transform.position = tilePath[0].transform.position;
       
        //Set his new row and col correctly
        currentCol = tilePath[0].GetComponent<PelletInfo>().col;
        currentRow = tilePath[0].GetComponent<PelletInfo>().row;

       

        //Remove pellet if there is one
        removePellet();

        //Delete from array
        tilePath.RemoveAt(0);

       
        //Found goal
        if(tilePath.Count == 0)
        {
           
            usingAStarMovement = false;

            //Select new target
            findClosestGroup();
        }
    }

    //Find path using a*
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
               

                //Add tile path to array
                GameObject tileForArray = currentTile;
                while (tileForArray != null)
                {
                    tilePath.Insert(0, tileForArray);
                    tileForArray = tileForArray.GetComponent<PelletInfo>().parent;
                }

                //Switch to astar path movement
                usingAStarMovement = true;
                
                //Set targetpellet to first element of path
                targetPellet = tilePath[0];

                //Reset Pellet info
                
                currentTile.GetComponent<PelletInfo>().reset();
                for(int i = 0; i < closedList.Count; i++)
                {
                    closedList[i].GetComponent<PelletInfo>().reset();
                }
                for (int i = 0; i < openList.Count; i++)
                {
                    openList[i].GetComponent<PelletInfo>().reset();
                }
                
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
            if(iter > 40)
            {
                Debug.Log("Error: Pacman looking very far for a group");
                break;
               
            }

        }
        
    

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
