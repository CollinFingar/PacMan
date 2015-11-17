using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Pacman : MonoBehaviour {

    public int currentRow;
    public int currentCol;

    public List<string> finalMap;

    private int score = 0;
    public Text text;
    public List<List<GameObject>> thePellets;

    public bool begin = false;

    private string direction = "up";

    List<List<GameObject>> pelletGroups;
    GameObject targetPellet;

    int currentPelletGroup;

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
            safeGroupMovement();
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
        if (transform.position.y < targetPellet.transform.position.y)
        {

            //Move up and avoid obstacles
            if (thePellets[currentRow - 1][currentCol].tag == "Pellet")
            {
                transform.position = new Vector3(currentCol, -currentRow + 1, transform.position.z);
                currentRow--;
                direction = "up";
                removePellet();

            }
            //Obstacle
            else
            {
                upObstacle = true;
            }
        }

        //If Pellet is below
        else if (transform.position.y > targetPellet.transform.position.y)
        {

            //Move down and avoid obstacles
            if (thePellets[currentRow + 1][currentCol].tag == "Pellet")
            {
                transform.position = new Vector3(currentCol, -currentRow - 1, transform.position.z);
                currentRow++;
                direction = "down";
                removePellet();
            }

            //obstacle
            else
            {
                downObstacle = true;
            }
        }

        
        //If Pellet is left
        if (transform.position.x > targetPellet.transform.position.x)
        {

            //Move left and avoid obstacles
            if (thePellets[currentRow][currentCol -1].tag == "Pellet")
            {
                transform.position = new Vector3(currentCol -1, -currentRow, transform.position.z);
                currentCol--;
                direction = "left";
                removePellet();
            }

            //Obstacle
            else
            {
                leftObstacle = true;
            }
        }

        //If Pellet is right
        else if (transform.position.x < targetPellet.transform.position.x)
        {

            //Move right and avoid obstacles
            if (thePellets[currentRow][currentCol  +1].tag == "Pellet")
            {
                transform.position = new Vector3(currentCol + 1, -currentRow, transform.position.z);
                currentCol++;
                direction = "right";
                removePellet();
            }

            //Obstacle
            else
            {
                rightObstacle = true;
            }
        }

        //Deal with obstacle avoidance here
        
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


    //Called when Pacman needs to find the closest group of pellets to begin eating
    void findClosestGroup()
    {
        //Iterate until we get the closest pellet
        bool foundClosest = false;
        int iter = 1;
        while (!foundClosest)
        {
            //Look Up
            if (thePellets[currentRow - iter][currentCol].tag == "Pellet")
            {
                if(thePellets[currentRow - iter][currentCol].GetComponent<PelletInfo>().eaten == false)
                {
                    foundClosest = true;
                    currentPelletGroup = thePellets[currentRow - iter][currentCol].GetComponent<PelletInfo>().groupNum;
                }
            }

            //Look Down
            else if (thePellets[currentRow + iter][currentCol].tag == "Pellet")
            {
                if (thePellets[currentRow + iter][currentCol].GetComponent<PelletInfo>().eaten == false)
                {
                    foundClosest = true;
                    currentPelletGroup = thePellets[currentRow + iter][currentCol].GetComponent<PelletInfo>().groupNum;
                }
            }

            //Look Left
            else if (thePellets[currentRow][currentCol - iter].tag == "Pellet")
            {
                if (thePellets[currentRow][currentCol - iter].GetComponent<PelletInfo>().eaten == false)
                {
                    foundClosest = true;
                    currentPelletGroup = thePellets[currentRow][currentCol - iter].GetComponent<PelletInfo>().groupNum;
                }
            }

            //Look Right
            else if (thePellets[currentRow][currentCol + iter].tag == "Pellet")
            {
                if (thePellets[currentRow][currentCol + iter].GetComponent<PelletInfo>().eaten == false)
                {
                    foundClosest = true;
                    currentPelletGroup = thePellets[currentRow][currentCol + iter].GetComponent<PelletInfo>().groupNum;
                }
            }

            //Look Up/Left
            else if (thePellets[currentRow - iter][currentCol - iter].tag == "Pellet")
            {
                if (thePellets[currentRow - iter][currentCol - iter].GetComponent<PelletInfo>().eaten == false)
                {
                    foundClosest = true;
                    currentPelletGroup = thePellets[currentRow - iter][currentCol - iter].GetComponent<PelletInfo>().groupNum;
                }
            }

            //Look Up/Right
            else if (thePellets[currentRow - iter][currentCol + iter].tag == "Pellet")
            {
                if (thePellets[currentRow - iter][currentCol + iter].GetComponent<PelletInfo>().eaten == false)
                {
                    foundClosest = true;
                    currentPelletGroup = thePellets[currentRow - iter][currentCol + iter].GetComponent<PelletInfo>().groupNum;
                }
            }

            //Look Down/Left
            else if (thePellets[currentRow + iter][currentCol - iter].tag == "Pellet")
            {
                if (thePellets[currentRow + iter][currentCol - iter].GetComponent<PelletInfo>().eaten == false)
                {
                    foundClosest = true;
                    currentPelletGroup = thePellets[currentRow + iter][currentCol - iter].GetComponent<PelletInfo>().groupNum;
                }
            }

            //Look Down/Right
            else if (thePellets[currentRow + iter][currentCol + iter].tag == "Pellet")
            {
                if (thePellets[currentRow + iter][currentCol + iter].GetComponent<PelletInfo>().eaten == false)
                {
                    foundClosest = true;
                    currentPelletGroup = thePellets[currentRow + iter][currentCol + iter].GetComponent<PelletInfo>().groupNum;
                }
            }

            //None this distance away. Increase distance to look
            else
            {
                iter++;
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
