using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour {

    public GameObject treePrefab;
    public GameObject floorPrefab;
    public GameObject outsidePrefab;
    public GameObject pelletPrefab;
    public GameObject theCamera;
    public GameObject pacmanPrefab;

    public GameObject[] ghosts;

    List<string> fullMap;
    List<string> finalMap;
    public List<List<GameObject>> thePellets;

    public int groupSize;
    public List<List<GameObject>> pelletGroups;

    //List<GameObject> thePellets;
    int oldRowNum;
    int oldColNum;
    int colNum = 0;

    GameObject map;
    GameObject pellets;
    GameObject pacman;
    public string mapName;

    bool hoveringOnGUI = false;
    bool pacmanPlaced = false;
    bool begin = false;

    Pacman pacmanScript;

    public float score = 0f;
    public Text text;

    // Use this for initialization
    void Start () {
        text.text = "SCORE: 0";

        map = GameObject.Find("Map").gameObject;
        pellets = GameObject.Find("Pellets").gameObject;

        fullMap = new List<string>();
        finalMap = new List<string>();
        //useableTiles = new List<List<GameObject>>();
        thePellets = new List<List<GameObject>>();
        

        readMap();
        condenseMap();
        buildMap();
        setupPelletGroups();
    }
	
	// Update is called once per frame
	void Update () {

        //Left click to place Pacman
        if (Input.GetMouseButtonDown(0) && !hoveringOnGUI)
        {
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);
            
            if (hit.transform.tag == "Pellet")
            {
                //Spawn Pacman
                pacman = Instantiate(pacmanPrefab, new Vector3(hit.transform.position.x, hit.transform.position.y, hit.transform.position.z - 1), Quaternion.identity) as GameObject;
                pacmanScript = pacman.GetComponent<Pacman>();
                pacmanScript.currentRow = hit.transform.gameObject.GetComponent<PelletInfo>().row;
                pacmanScript.currentCol = hit.transform.gameObject.GetComponent<PelletInfo>().col;
                pacmanScript.text = text;
                pacmanScript.finalMap = finalMap;
                pacmanScript.maxCol = colNum;
                pacmanScript.thePellets = thePellets;
                pacmanScript.groupSize = groupSize;

                pacmanPlaced = true;
                
            }
        }

        //If we have placed pacman and hit start
        if (begin)
        {
            
        }

        //Move Camera
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 cameraPos = theCamera.transform.position;
            cameraPos.x += 1;
            theCamera.transform.position = cameraPos;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 cameraPos = theCamera.transform.position;
            cameraPos.x -= 1;
            theCamera.transform.position = cameraPos;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 cameraPos = theCamera.transform.position;
            cameraPos.y += 1;
            theCamera.transform.position = cameraPos;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 cameraPos = theCamera.transform.position;
            cameraPos.y -= 1;
            theCamera.transform.position = cameraPos;
        }
    }


    void setupPelletGroups()
    {

        int groupsPerRow = colNum / groupSize;
        int groupsPerCol = (finalMap.Count - 1) / groupSize;
        int currentGroupNum = 0;
        pelletGroups = new List<List<GameObject>>(groupsPerCol * groupsPerRow);

        List<GameObject> firstList = new List<GameObject>();
        pelletGroups.Add(firstList);

        int numberOfGroups = 0;
        for (int i = 0; i <finalMap.Count; i++)
        {
            for (int j = 0; j < colNum; j++)
            {
                //Only if its a pellet
                if (thePellets[i][j].tag == "Pellet")
                {
                    int groupToAddTo = currentGroupNum + (j / groupSize);

                    //Add more lists as we need them
                    if(numberOfGroups < groupToAddTo)
                    {
                        for(int k = 0; k < groupToAddTo; k++)
                        {
                            List<GameObject> aGroup = new List<GameObject>();
                            pelletGroups.Add(aGroup);
                            numberOfGroups = groupToAddTo;
                        }
                    }
                    pelletGroups[groupToAddTo].Add(thePellets[i][j]);
                    thePellets[i][j].GetComponent<PelletInfo>().groupNum = groupToAddTo;
                }
            }

            //Increase group num
            if( i%groupSize == 0)
            {
                currentGroupNum += groupSize;
            }
        }
    }

    void OnGUI()
    {
        //Start Button
        if (GUI.Button(new Rect(10, 10, 50, 30), "Start"))
        {
            //Only run when Pacman has been placed
            if (pacmanPlaced)
            {
                //Remove Pellet
                GameObject aPellet = thePellets[pacmanScript.currentRow][pacmanScript.currentCol].gameObject;
                aPellet.GetComponent<SpriteRenderer>().enabled = false;
                aPellet.GetComponent<PelletInfo>().eaten = true;
                

                //Set pacman in the ghosts
                for (int i = 0; i < ghosts.Length; i++)
                {
                    ghosts[i].GetComponent<Ghost>().setPacman();
                }
                pacmanScript.setBegin(aPellet.GetComponent<PelletInfo>().groupNum);
                begin = true;
            }
            else
            {
                Debug.Log("Must pick valid location for Pacman. Use mouse");
            }


        }

        //If were hovering over a button
        if (new Rect(10, 10, 50, 30).Contains(Event.current.mousePosition)) hoveringOnGUI = true;
        else if (hoveringOnGUI) hoveringOnGUI = false;
    }

    void readMap()
    {
        StreamReader reader = new StreamReader(mapName, Encoding.Default);
        string line;
        using (reader)
        {
            do
            {
                line = reader.ReadLine();

                if (line != null)
                {
                    fullMap.Add(line);
                }
            }
            while (line != null);
            reader.Close();
        }
    }


    void condenseMap()
    {
        //Set num of rows and cols
        string rows = fullMap[1].Split(null)[1];
        string cols = fullMap[2].Split(null)[1];
        oldRowNum = int.Parse(rows);
        oldColNum = int.Parse(cols);

        //Get rid of unneeded first rows
        fullMap.RemoveRange(0, 4);
        //Look at squares of map and condense map into final array to put on screen
        for (int line = 0; line < oldRowNum - 2; line += 2)
        {
            string aLine = "";

            for (int col = 0; col < oldColNum - 2; col += 2)
            {
                int trees = 0;
                int floor = 0;
                int outside = 0;
                char[] square = new char[4];

                square[0] = fullMap[line][col];
                square[1] = fullMap[line][col + 1];
                square[2] = fullMap[line + 1][col];
                square[3] = fullMap[line + 1][col + 1];

                for (int i = 0; i < 4; i++)
                {
                    if (square[i] == '.')
                    {
                        floor++;
                    }

                    else if (square[i] == 'T')
                    {
                        trees++;
                    }

                    else if (square[i] == '@')
                    {
                        outside++;
                    }
                }

                if (floor >= 2)
                {
                    aLine = aLine + '.';
                }
                else if (trees >= 2)
                {
                    aLine = aLine + 'T';
                }
                else if (outside >= 2)
                {
                    aLine = aLine + '@';
                }

            }
            colNum = aLine.Length;
            finalMap.Add(aLine);

        }
        //Send finalmap to ghosts for obstacle detection
        for(int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].GetComponent<Ghost>().setFinalMap(finalMap);
        }
    }


    void buildMap()
    {
        //Go through array and place corresponding prefab for each char
        //Need to start at last row so we build up

        for (int row = finalMap.Count - 1; row > -1; row--)
        {
            List<GameObject> arow = new List<GameObject>();
            for (int col = 0; col < colNum; col++)
            {
                GameObject tile = outsidePrefab;
                
                //Instantiate and parent the correct prefab
                if (finalMap[row][col] == '.')
                {
                    tile = floorPrefab;
                    GameObject newTile = Instantiate(tile, new Vector3(col, -row, 1), Quaternion.identity) as GameObject;
                    newTile.transform.parent = map.transform;

                    if ((row + col) % 2 == 0) {
                        SpriteRenderer sr = newTile.GetComponent<SpriteRenderer>();
                        sr.color = new Color(.9f, .9f, .9f, 1);
                    }

                    //arow.Add(newTile);

                    //Add Pellet in same location
                    GameObject aPellet = pelletPrefab;
                    GameObject newPellet = Instantiate(aPellet, new Vector3(col, -row, 0), Quaternion.identity) as GameObject;
                    PelletInfo infoScript = newPellet.GetComponent<PelletInfo>();
                    infoScript.setupPellet(row, col);
                    newPellet.transform.parent = pellets.transform;
                    arow.Add(newPellet);

                }

                else if (finalMap[row][col] == 'T')
                {
                    tile = treePrefab;
                    GameObject newTile = Instantiate(tile, new Vector3(col, -row, 0), Quaternion.identity) as GameObject;
                    newTile.transform.parent = map.transform;
                    arow.Add(newTile);
                }

                else if (finalMap[row][col] == '@')
                {
                    tile = outsidePrefab;
                    GameObject newTile = Instantiate(tile, new Vector3(col, -row, 0), Quaternion.identity) as GameObject;
                    newTile.transform.parent = map.transform;
                    arow.Add(newTile);
                }
            }
            //Add the list of Gameobjects to the big list to create 2d. put at beginning because order is reversed
            thePellets.Insert(0, arow);
        }
    }
}
