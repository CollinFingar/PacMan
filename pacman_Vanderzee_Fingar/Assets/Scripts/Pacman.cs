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

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (begin) {
            //Move Pacman TESTING
            if (Input.GetKey(KeyCode.W))
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
            }

            else if (Input.GetKey(KeyCode.A))
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
            }

            else if (Input.GetKey(KeyCode.S))
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
            }

            else if (Input.GetKey(KeyCode.D))
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
            }
        }
        

    }
}
