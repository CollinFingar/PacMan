using UnityEngine;
using System.Collections;

public class PelletInfo : MonoBehaviour {

    public int row;
    public int col;

    public int groupNum;

    public bool eaten = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setupPellet(int arow, int acol)
    {
        row = arow;
        col = acol;

        //Add Pellet to a group

    }
}
