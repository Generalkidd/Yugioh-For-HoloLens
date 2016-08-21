using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Online : MonoBehaviour {

    float timeSinceLastCall=.5f;
    float oldTime = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    float getTimeSinceLastCall()
    {
        return timeSinceLastCall;
    }
    
    void addDeltaTime()
    {
        timeSinceLastCall = Time.time - oldTime;
        oldTime = Time.time;
    }

    void OnMouseDown()
    {
        SceneManager.LoadScene(2);
        GameObject camera = Camera.main.gameObject;
        NetworkManager nm = camera.AddComponent<NetworkManager>();
        nm.Connect();
    }

    void OnSelect()
    {
        addDeltaTime();
        if (getTimeSinceLastCall() > .5)
        {
            SceneManager.LoadScene(2);
            GameObject camera = Camera.main.gameObject;
            NetworkManager nm = camera.AddComponent<NetworkManager>();
            nm.Connect();
        }
    }
}
