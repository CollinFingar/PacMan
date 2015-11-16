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

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        if (begin)
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
