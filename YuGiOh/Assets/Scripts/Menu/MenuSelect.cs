using UnityEngine;
using System.Collections;

public class MenuSelect : MonoBehaviour
{
    public bool isReasons;
    public int reasonId;

    private string Action { get; set; }

    private GameObject tmp;
    
	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isReasons)
        {
            GameObject tmp = GameObject.Find("MenuTitle");
            if(tmp.GetComponent<MenuManager>().SelectedReason != reasonId)
            {
                this.GetComponent<TextMesh>().color = Color.green;
            }
            else
            {
                this.GetComponent<TextMesh>().color = Color.red;
            }
        }
    }

    void OnReleased()
    {
        tmp = GameObject.Find("MenuTitle");
        tmp.GetComponent<MenuManager>().Action = this.GetComponent<TextMesh>().text;
    }
}
