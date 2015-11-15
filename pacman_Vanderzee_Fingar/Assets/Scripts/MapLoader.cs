using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class MapLoader : MonoBehaviour {

    public GameObject treePrefab;
    public GameObject floorPrefab;
    public GameObject outsidePrefab;
    public GameObject pelletPrefab;
    public GameObject theCamera;

    List<string> fullMap;
    List<string> finalMap;
    public List<List<GameObject>> useableTiles;
    List<GameObject> thePellets;
    int oldRowNum;
    int oldColNum;
    int colNum = 0;

    GameObject map;
    GameObject pellets;
    public string mapName;

    // Use this for initialization
    void Start () {

        map = GameObject.Find("Map").gameObject;
        pellets = GameObject.Find("Pellets").gameObject;

        fullMap = new List<string>();
        finalMap = new List<string>();
        useableTiles = new List<List<GameObject>>();
        thePellets = new List<GameObject>();

        readMap();
        condenseMap();
        buildMap();
    }
	
	// Update is called once per frame
	void Update () {

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
                    arow.Add(newTile);

                    //Add Pellet in same location
                    GameObject aPellet = pelletPrefab;
                    GameObject newPellet = Instantiate(aPellet, new Vector3(col, -row, 0), Quaternion.identity) as GameObject;
                    PelletInfo infoScript = newPellet.GetComponent<PelletInfo>();
                    infoScript.row = row;
                    infoScript.col = col;
                    newPellet.transform.parent = pellets.transform;
                    thePellets.Add(newPellet);

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
            useableTiles.Insert(0, arow);
        }
    }
}
