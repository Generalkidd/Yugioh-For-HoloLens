using UnityEngine;
using System.Collections;

public class EndTurn : MonoBehaviour
{
    static internal GameManager myGameManager;
    internal GameManager.CurrentlySelectedCardType myZone;
    // Use this for initialization
    void Start () {
        myGameManager = null;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            myGameManager.OnEndTurn();
        }
    }

    internal void setGameManager(GameManager gm)
    {
        Debug.Log("Setting game manager to: " + gm);
        myGameManager = gm;
        Debug.Log("Set game manager:" + myGameManager);
    }

    void OnSelect()
    {
        if(myGameManager!=null)
        {
            Debug.Log("Game Manager not null and End Turn selected->");
            myGameManager.OnEndTurn();
        }
    }
}
