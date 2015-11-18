using UnityEngine;
using System.Collections;

public class PelletInfo : MonoBehaviour {

    public int row;
    public int col;

    public int groupNum;

    public bool eaten = false;

    public int costFromStart = -1; //g(n) value
    public int costToGoal; //h(n) value

    public GameObject parent;

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

    public int totalCost()
    {
        if (costFromStart >= 0 && costToGoal >= 0)
        {
            return (int)(costToGoal) + costFromStart;
        }
        else
        {
            Debug.Log("Can't compute. negative cost");
            return -1;
        }
    }


    void setCostToGoal(GameObject Goal)
    {
        //Euclidean
        costToGoal = (int)Mathf.Sqrt(Mathf.Pow((transform.position.x - Goal.transform.position.x), 2) + Mathf.Pow((transform.position.y - Goal.transform.position.y), 2));
       
    }
}
