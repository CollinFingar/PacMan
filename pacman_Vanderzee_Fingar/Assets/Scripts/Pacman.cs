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

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        

    }

    void FixedUpdate() {
        iterate();
    }


    void iterate() {

        if (begin)
        {
            //Move Pacman TESTING
            if (string.Compare(direction, "up") == 0)
            {
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





    public void setBegin() {
        begin = true;
        chooseRandomDirection();
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
