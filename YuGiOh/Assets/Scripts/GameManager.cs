using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.BattleHandler.Game;
using System;

public class GameManager : MonoBehaviour
{
    long frameCounter = long.MaxValue;
    static Player me;
    static Texture CardBackTexture;
    List<Assets.Scripts.BattleHandler.Cards.Card> hand = new List<Assets.Scripts.BattleHandler.Cards.Card>();

    public static GameManager MakeManager(GameObject toAddTo, Player pMe, Texture CardBack)
    {
        GameManager myManager = toAddTo.AddComponent<GameManager>();
        me = pMe;
        CardBackTexture = CardBack;
        return myManager;
    }

    // Update is called once per frame
    void Update () {
        //Update every 1400 frames
        if (frameCounter > 5000)
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
                plane.AddComponent<BoxCollider>();
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
                plane.AddComponent<BoxCollider>();
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
                plane.AddComponent<BoxCollider>();
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
                plane.AddComponent<BoxCollider>();
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
                plane.AddComponent<BoxCollider>();
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
                plane.AddComponent<BoxCollider>();
                GameObject planeBack = GameObject.CreatePrimitive(PrimitiveType.Plane);
                planeBack.transform.Rotate(90, 0, 0);
                planeBack.transform.position = spawnPoint.transform.position;
                planeBack.GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                planeBack.transform.parent = spawnPoint.transform;
            }
        }
    }
}
