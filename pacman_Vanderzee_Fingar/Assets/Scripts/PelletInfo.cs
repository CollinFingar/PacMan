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

        

    }

    public int totalCost(GameObject Goal)
    {
        if (costFromStart >= 0 && setCostToGoal(Goal) >= 0)
        {
            return (int)(setCostToGoal(Goal)) + costFromStart;
        }
        else
        {
            Debug.Log("Can't compute. negative cost");
            return -1;
        }
    }


    int setCostToGoal(GameObject Goal)
    {
        //Euclidean
        costToGoal = (int)Mathf.Sqrt(Mathf.Pow((transform.position.x - Goal.transform.position.x), 2) + Mathf.Pow((transform.position.y - Goal.transform.position.y), 2));
        return costToGoal;
    }

    public void reset()
    {
        parent = null;

        costFromStart = -1;
    }
}
