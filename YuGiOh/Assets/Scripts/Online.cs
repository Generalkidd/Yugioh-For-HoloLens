using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Online : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnSelect()
    {
        SceneManager.LoadScene(2);
    }
}
