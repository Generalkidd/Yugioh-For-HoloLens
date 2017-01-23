using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private const string barcode = "+0002F1";

    public MenuManager()
    {
        SelectedReason = -1;
    }

    public string Action { get; set; }

    private GameObject GameObject { get; set; }

    public int SelectedReason { get; set; }

    private int Count { get; set; }

    private int index = 0;

    // Use this for initialization
    void Start()
    {
        //StartCoroutine(Get());
    }

    IEnumerator Get()
    {
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        switch (Action)
        {
            case "Single User Test":
                SceneManager.LoadScene(1);
                Action = "";
                break;
            case "Multiplayer":
                SceneManager.LoadScene(2);
                Action = "";
                break;
            case "Mini Mode":
                SceneManager.LoadScene(3);
                Action = "";
                break;
            case "Duel Disk Mode":
                SceneManager.LoadScene(7);
                Action = "";
                break;
        }
    }

}
