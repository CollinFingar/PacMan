using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ghost : MonoBehaviour {


    public int currentRow;
    public int currentCol;

    public GameObject pacman;
    Pacman pacmanScript;

    List<string> finalMap; //List of chars showing where obstacles are

    public bool begin = false;
    bool locationSet = false;
    public bool follow = false;
    public bool random = true;

    public int wanderDistance; //Distance in one direction to move
    int counter = 0;

    string randomVertDir; //Up, Down, No
    string randomHorizDir; //Left, Right, No

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        
        //FOLLOW MOVEMENT
        if (begin && follow)
        {
            ///MOVE TO PACMAN

            //If Pacman is above
            if (pacman.transform.position.y > transform.position.y)
            {

                //Move up and avoid obstacles
                if (finalMap[currentRow - 1][currentCol] == '.')
                {
                    transform.position = new Vector3(currentCol, -currentRow + 1, transform.position.z);
                    currentRow--;

                }
            }

            //If Pacman is below
            if (pacman.transform.position.y < transform.position.y)
            {

                //Move down and avoid obstacles
                if (finalMap[currentRow + 1][currentCol] == '.')
                {
                    transform.position = new Vector3(currentCol, -currentRow - 1, transform.position.z);
                    currentRow++;

                }
            }

            //If Pacman is left
            if (pacman.transform.position.x < transform.position.x)
            {

                //Move down and avoid obstacles
                if (finalMap[currentRow][currentCol - 1] == '.')
                {
                    transform.position = new Vector3(currentCol - 1, -currentRow, transform.position.z);
                    currentCol--;

                }
            }

            //If Pacman is right
            if (pacman.transform.position.x > transform.position.x)
            {

                //Move down and avoid obstacles
                if (finalMap[currentRow][currentCol + 1] == '.')
                {
                    transform.position = new Vector3(currentCol + 1, -currentRow, transform.position.z);
                    currentCol++;

                }
            }



        }

        //RANDOM MOVEMENT
        else if (begin && random)
        {
            //Move a random direction for a set amount of spaces

            //Pick direction when counter is reset
            if (counter == 0)
            {
                moveRandom();
            }

            //Up
            if (randomVertDir == "Up")
            { 
                //Move up and avoid obstacles
                if (finalMap[currentRow - 1][currentCol] == '.')
                {
                    transform.position = new Vector3(currentCol, -currentRow + 1, transform.position.z);
                    currentRow--;

                }

            }

            //Down
            else if (randomVertDir == "Down")
            {
                //Move down and avoid obstacles
                if (finalMap[currentRow + 1][currentCol] == '.')
                {
                    transform.position = new Vector3(currentCol, -currentRow - 1, transform.position.z);
                    currentRow++;

                }

            }

            //Left
            if (randomHorizDir == "Left")
            {
                //Move down and avoid obstacles
                if (finalMap[currentRow][currentCol - 1] == '.')
                {
                    transform.position = new Vector3(currentCol - 1, -currentRow, transform.position.z);
                    currentCol--;

                }

            }

            //Right
            else if (randomHorizDir == "Right")
            {
                //Move down and avoid obstacles
                if (finalMap[currentRow][currentCol + 1] == '.')
                {
                    transform.position = new Vector3(currentCol + 1, -currentRow, transform.position.z);
                    currentCol++;

                }

            }

            //Increment counter
            counter++;

            //Moved in dir too long. Switch directions
            if(counter > wanderDistance)
            {
                counter = 0;
            }

        }
        
    }

    void moveRandom()
    {
        //Move Randomly

        //Chance to move Up/Down/No. 33% for each
        int verticalChance = Random.Range(0, 2);

        //Up
        if (verticalChance == 0)
        {
            randomVertDir = "Up";
        }

        //Down
        else if (verticalChance == 1)
        {
            randomVertDir = "Down";
        }

        //No vertical movement
        else
        {
            randomVertDir = "No";
        }


        //Chance to move Left/Right/No
        int horizontalChance = Random.Range(0, 2);

        //Left
        if (horizontalChance == 0)
        {
            randomHorizDir = "Left";
        }

        //Right
        else if (horizontalChance == 1)
        {
            randomHorizDir = "Right";
        }

        //No horizontal movement
        else
        {
            randomHorizDir = "No";
        }
    }

    public void setFinalMap(List<string> map)
    {
        finalMap = map;
    }
    
    
    public void setPacman()
    {
        pacman = GameObject.FindGameObjectWithTag("Pacman");
        pacmanScript = pacman.GetComponent<Pacman>();

        //Begin program
        begin = true;
    }

    

 
}
