using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ghost : MonoBehaviour {


    public int currentRow;
    public int currentCol;

    public GameObject pacman;
    //Pacman pacmanScript;

    List<string> finalMap; //List of chars showing where obstacles are

    public bool begin = false;  //True when player hits start
    public bool follow = false; //True when close enough to follow Pacman
    public bool random = false;  //True when far from Pacman
    public bool psuedoRandom = false;   //True when close to Pacman but not close enough to follow

    public int wanderDistance; //Distance in one direction to move
    int counter = 0;

    string randomVertDir; //Up, Down, No
    string randomHorizDir; //Left, Right, No

    enum states {
        random,
        semiRandom,
        chasing
    }
    int ghostState = (int)states.random;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
    }

    void FixedUpdate() {
        //If the game has begun
        if (begin && transform.position != pacman.transform.position)
        {
            float distanceToPacman = Vector2.Distance(transform.position, pacman.transform.position);
            switch (ghostState)
            {
                //if in a random state
                case (int)states.random:
                    if (distanceToPacman < 20)  //switch to chasing if very close
                    {
                        ghostState = (int)states.chasing;
                    }
                    else if (distanceToPacman < 40) //switch to psuedo random moving if sort of close
                    {
                        ghostState = (int)states.semiRandom;
                        psuedoRandom = true;
                    }
                    else           //else just move randomly
                    {
                        randomMovement();
                    }
                    break;
                //if in the psuedo random state
                case (int)states.semiRandom:
                    if (distanceToPacman < 20)  //switch to chasing if very close
                    {
                        ghostState = (int)states.chasing;
                        psuedoRandom = false;
                    }
                    else if (distanceToPacman < 40)     //move psuedo randomly if kind of close
                    {
                        randomMovement();
                    }
                    else     //else switch to random movement
                    {
                        ghostState = (int)states.random;
                        psuedoRandom = false;
                    }
                    break;
                //If in a chasing state
                case (int)states.chasing:
                    if (distanceToPacman < 20)      //chase if still close
                    {
                        followMovement();
                    }
                    else if (distanceToPacman < 40)     //switch to psuedo random state if semi close
                    {
                        ghostState = (int)states.semiRandom;
                    }
                    else        //else switch to random movement
                    {
                        ghostState = (int)states.random;
                    }
                    break;
            }
        }
        else {
            if (transform.position == pacman.transform.position) {
                Debug.Log("End");
            }
        }
    }

    //Called when doing follow movement to pacman
    void followMovement()
    {
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
    
    //Called when doing random movement or psuedo random movement.
    void randomMovement()
    {
       
        //Move a random direction for a set amount of spaces

        //Pick direction when counter is reset
        if (counter == 0)
        {
            pickRandomDirection();
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
        if (counter > wanderDistance)
        {
            counter = 0;
        }

        
    }
    
    //Called to pick a new random direction to move. Includes psuedo random movement when applicable
    void pickRandomDirection()
    {
        //Pick random direction
        if (random)
        {

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

        //Pick psuedo random direction
        else if (psuedoRandom)
        {
            //VERTICAL
            int verticalChance = Random.Range(0, 10);

            //Pacman is above better chance to move up.
            if (pacman.transform.position.y > transform.position.y)
            {
                //60 % chance
                if (verticalChance <= 6)
                {
                    randomVertDir = "Up";
                }

                //20% chance to move down or not
                else if (verticalChance > 6 && verticalChance <= 8)
                {
                    randomVertDir = "Down";
                }

                else
                {
                    randomVertDir = "No";
                }
            }

            //Pacman is bellow better chance to move down.
            else if (pacman.transform.position.y < transform.position.y)
            {
                //60 % chance
                if (verticalChance <= 6)
                {
                    randomVertDir = "Down";
                }

                //20% chance to move down or not
                else if (verticalChance > 6 && verticalChance <= 8)
                {
                    randomVertDir = "Up";
                }

                else
                {
                    randomVertDir = "No";
                }
            }

            //HORIZONTAL
            int horizontalChance = Random.Range(0, 10);

            //Pacman is to the right. Better chance to move right
            if (pacman.transform.position.x > transform.position.x)
            {
                //60 % chance
                if (horizontalChance <= 6)
                {
                    randomHorizDir = "Right";
                }

                //20% chance to move Left or not
                else if (horizontalChance > 6 && horizontalChance <= 8)
                {
                    randomHorizDir = "Left";
                }

                else
                {
                    randomHorizDir = "No";
                }
            }
        

            //Pacman is to the left. Better chance to move left
            else if (pacman.transform.position.x < transform.position.x)
            {
                //60 % chance
                if (horizontalChance <= 6)
                {
                    randomHorizDir = "Left";
                }

                //20% chance to move Right or not
                else if (horizontalChance > 6 && horizontalChance <= 8)
                {
                    randomHorizDir = "Right";
                }

                else
                {
                    randomHorizDir = "No";
                }
            }
        }
    }

    public void setFinalMap(List<string> map)
    {
        finalMap = map;
    }
    
    public void setPacman()
    {
        pacman = GameObject.FindGameObjectWithTag("Pacman");
        //pacmanScript = pacman.GetComponent<Pacman>();

        //Begin program
        begin = true;
    }

    
}
