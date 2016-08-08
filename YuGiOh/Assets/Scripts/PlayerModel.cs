using UnityEngine;
using System.Collections;

public class PlayerModel : MonoBehaviour
{
    GameObject g;
	// Use this for initialization
	void Start ()
    {
        g = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update ()
    {
        this.transform.position = new Vector3(g.transform.position.x, -1.5f, g.transform.position.z);
	}
}
