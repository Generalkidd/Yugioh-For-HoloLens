using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LegacyGameManager : MonoBehaviour
{
    List<GameObject> hand = new List<GameObject>();
    List<GameObject> deck = new List<GameObject>();

    private float x = -1f;
    // Use this for initialization
    void Start()
    {
        foreach (GameObject c in GameObject.FindGameObjectsWithTag("Card"))
        {
            deck.Add(c);
        }

        System.Random rand = new System.Random();
        deck = deck.OrderBy(item => rand.Next()).ToList();

        for (int i = 0; i < 6; i++)
        {
            hand.Add(deck[i]);

            GameObject card;
            card = (GameObject)Instantiate(hand[i], new Vector3(x, 0, 3f), new Quaternion(0, 180, 0, 0));
            x += 0.5f;
        }

        for (int i = 0; i < 6; i++)
        {
            //deck.RemoveAt(i);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnReset()
    {
        hand.Clear();
        Start();
    }
}
