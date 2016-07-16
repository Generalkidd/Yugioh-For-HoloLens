using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.BattleHandler.Game;
using System;

public class GameManager : MonoBehaviour
{
    List<Assets.Scripts.BattleHandler.Cards.Card> hand = new List<Assets.Scripts.BattleHandler.Cards.Card>();
    Texture CardBackTexture;
    System.Random rand = new System.Random();

    long frameCounter = long.MaxValue;

    //In multiplayer we would not store both players
    Player p1;
    Player p2;
    Player me;

    private float x = -1f;
    // Use this for initialization
    void Start()
    {
        //Old Code
        /*
	    foreach(GameObject c in GameObject.FindGameObjectsWithTag("Card"))
        {
            deck.Add(c);
        }

        System.Random rand = new System.Random();
        deck = deck.OrderBy(item => rand.Next()).ToList();

        for(int i = 0; i < 6; i++)
        {
            hand.Add(deck[i]);
            
            GameObject card;
            card = (GameObject)Instantiate(hand[i], new Vector3(x, 0, 3f), new Quaternion(0, 180, 0, 0));
            x += 0.5f;
        }

        for(int i = 0; i < 6; i++)
        {
            //deck.RemoveAt(i);
        }*/

        //Here is where code should go to build personal Decks. For now we make a random deck (first 40 cards in database).
        //Both players will use the same deck for this test app.
        MainDeckBuilder mdb = new MainDeckBuilder();
        List<Assets.Scripts.BattleHandler.Cards.Card> randomDeck = mdb.getRandomDeck();

        //Initialize Card Back
        CardBackTexture = mdb.getCardBack() as Texture;

        //Build the players just like a Network Manager would do.
        int random1 = rand.Next();
        int random2 = rand.Next();
        int randomGameId = rand.Next();
        p1 = new Player(random1, "SethRocks!");
        p2 = new Player(random2, "BobSucks!");

        //Now the network manager would give a handle to the users and to the game.
        //Since this is just for testing we will control both players from this one class.
        Game g = new Game(p1, p2, randomGameId);
        g.RequestSetPlayer1Deck(randomDeck);
        g.RequestSetPlayer2Deck(randomDeck);
        g.StartGame();
        //Normally only the game and corresponding player will be able to use the get hand function
        //because they will be the only entities which have a handle to the player
        me = p1;
        hand = me.Hand;
    }

	
	// Update is called once per frame
	void Update () {
        //Update every 1400 frames
        if (frameCounter > 1400)
        {
            placeMyHandCardsOnGUI();
            placeMyMonsterCardOnGUI();
            placeMyTrapsOnGUI();
            placeOpponentsMonstersOnGUI();
            placeOpponentsTrapsOnGUI();
            placeOpponentsHandOnGUI();
            frameCounter = 0;
        }
        frameCounter++;
	}

    private void placeOpponentsHandOnGUI()
    {
        //throw new NotImplementedException();
    }

    private void placeOpponentsTrapsOnGUI()
    {
       // throw new NotImplementedException();
    }

    private void placeOpponentsMonstersOnGUI()
    {
        //throw new NotImplementedException();
    }

    private void placeMyTrapsOnGUI()
    {
       // throw new NotImplementedException();
    }

    private void placeMyMonsterCardOnGUI()
    {
       // throw new NotImplementedException();
    }

    private void placeMyHandCardsOnGUI()
    {
        hand = me.Hand;
        for(int i=0; i<hand.Count; i++)
        {
            if(i==0)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand1");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Card
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.Rotate(90, 180, 0);
                Vector3 scale = new Vector3(0.035f, 0.05f, 0.05f);
                plane.transform.localScale = scale;
                plane.transform.position = spawnPoint.transform.position;
                plane.GetComponent<Renderer>().material.mainTexture = hand[0].CardImage;
                plane.transform.parent = spawnPoint.transform;
                GameObject planeBack = GameObject.CreatePrimitive(PrimitiveType.Plane);
                planeBack.transform.Rotate(90, 0, 0);
                planeBack.transform.position = spawnPoint.transform.position;
                planeBack.transform.localScale = scale;
                planeBack.GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                planeBack.transform.parent = spawnPoint.transform;
            }
            else if(i==1)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand2");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Card
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.Rotate(90, 180, 0);
                plane.transform.position = spawnPoint.transform.position;
                Vector3 scale = new Vector3(0.035f, 0.05f, 0.05f);
                plane.transform.localScale = scale;
                plane.GetComponent<Renderer>().material.mainTexture = hand[1].CardImage;
                plane.transform.parent = spawnPoint.transform;
                GameObject planeBack = GameObject.CreatePrimitive(PrimitiveType.Plane);
                planeBack.transform.Rotate(90, 0, 0);
                planeBack.transform.localScale = scale;
                planeBack.transform.position = spawnPoint.transform.position;
                planeBack.GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                planeBack.transform.parent = spawnPoint.transform;
            }
            else if (i == 2)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand3");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Card
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.Rotate(90, 180, 0);
                plane.transform.position = spawnPoint.transform.position;
                Vector3 scale = new Vector3(0.035f, 0.05f, 0.05f);
                plane.transform.localScale = scale;
                plane.GetComponent<Renderer>().material.mainTexture = hand[2].CardImage;
                plane.transform.parent = spawnPoint.transform;
                GameObject planeBack = GameObject.CreatePrimitive(PrimitiveType.Plane);
                planeBack.transform.Rotate(90, 0, 0);
                planeBack.transform.position = spawnPoint.transform.position;
                planeBack.transform.localScale = scale;
                planeBack.GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                planeBack.transform.parent = spawnPoint.transform;
            }
            else if (i == 3)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand4");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Card
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.Rotate(90, 180, 0);
                plane.transform.position = spawnPoint.transform.position;
                Vector3 scale = new Vector3(0.035f, 0.05f, 0.05f);
                plane.transform.localScale = scale;
                plane.GetComponent<Renderer>().material.mainTexture = hand[3].CardImage;
                plane.transform.parent = spawnPoint.transform;
                GameObject planeBack = GameObject.CreatePrimitive(PrimitiveType.Plane);
                planeBack.transform.Rotate(90, 0, 0);
                planeBack.transform.position = spawnPoint.transform.position;
                planeBack.GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                planeBack.transform.parent = spawnPoint.transform;
                planeBack.transform.localScale = scale;
            }
            else if (i == 4)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand5");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Card
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.Rotate(90, 180, 0);
                plane.transform.position = spawnPoint.transform.position;
                Vector3 scale = new Vector3(0.035f, 0.05f, 0.05f);
                plane.transform.localScale = scale;
                plane.GetComponent<Renderer>().material.mainTexture = hand[4].CardImage;
                plane.transform.parent = spawnPoint.transform;
                GameObject planeBack = GameObject.CreatePrimitive(PrimitiveType.Plane);
                planeBack.transform.Rotate(90, 0, 0);
                planeBack.transform.position = spawnPoint.transform.position;
                planeBack.transform.localScale = scale;
                planeBack.GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                planeBack.transform.parent = spawnPoint.transform;
            }
            else if (i == 5)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand6");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Card
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.Rotate(90, 180, 0);
                plane.transform.position = spawnPoint.transform.position;
                Vector3 scale = new Vector3(0.035f, 0.05f, 0.05f);
                plane.transform.localScale = scale;
                plane.GetComponent<Renderer>().material.mainTexture = hand[5].CardImage;
                plane.transform.parent = spawnPoint.transform;
                GameObject planeBack = GameObject.CreatePrimitive(PrimitiveType.Plane);
                planeBack.transform.Rotate(90, 0, 0);
                planeBack.transform.position = spawnPoint.transform.position;
                planeBack.GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                planeBack.transform.parent = spawnPoint.transform;
            }
        }
    }

    void OnReset()
    {
        hand.Clear();
        Start();
    }
}
