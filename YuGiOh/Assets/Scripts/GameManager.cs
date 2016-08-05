using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.BattleHandler.Game;
using System;
using Assets.Scripts.BattleHandler.Cards;

public class GameManager : MonoBehaviour
{
    long frameCounter = long.MaxValue;
    static Player me;
    static Texture CardBackTexture;
    static NetworkManager netManager;
    static Card currentlySelectedCard;
    List<Assets.Scripts.BattleHandler.Cards.Card> hand = new List<Assets.Scripts.BattleHandler.Cards.Card>();
    private CurrentlySelectedCardType currentlySelectedCardType=CurrentlySelectedCardType.None;
    private Assets.Scripts.BattleHandler.Cards.Card myCurrentlySelectedCard;
    private Card myCurrentlySelectedCardObject;
    internal Quaternion rotation = new Quaternion(0f, 90f, 0f, 0f);

    public enum CurrentlySelectedCardType
    {
        Hand,
        Monster,
        None
    }

    public static GameManager MakeManager(GameObject toAddTo, Player pMe, Texture CardBack, NetworkManager networkManager)
    {
        GameManager myManager = toAddTo.AddComponent<GameManager>();
        Debug.Log("pMe Id=" + pMe.id);
        me = pMe;
        CardBackTexture = CardBack;
        netManager = networkManager;
        return myManager;
    }

    void Start()
    {
        placeMyHandCardsOnGUI();
        placeMyMonsterCardOnGUI();
        placeMyTrapsOnGUI();
        placeOpponentsMonstersOnGUI();
        placeOpponentsTrapsOnGUI();
        placeOpponentsHandOnGUI();
    }

    public void updateLayout()
    {
        placeMyHandCardsOnGUI();
        placeMyMonsterCardOnGUI();
        placeMyTrapsOnGUI();
        placeOpponentsMonstersOnGUI();
        placeOpponentsTrapsOnGUI();
        placeOpponentsHandOnGUI();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("space"))
        {
            OnEndTurn();
        }
    }

    ///Returns a monster card air tapped by user from his/her monster zone.
    public MonsterCard PromptForOneOfMyMonstersOnField()
    {
        //TODO CODE
        return null;
    }

    ///<summary>
    ///Returns either a face up monster card air tapped by user from his/her opponent's monster zone
    ///OR the index of a face down monster card air tapped by user from his/her opponent's monster zone
    ///</summary>
    public void PromptForOneOfOpponentsMonstersOnField(out MonsterCard faceUpMonster, out int faceDownMonsterIndex)
    {
        //TODO CODE
        faceUpMonster = null;
        faceDownMonsterIndex = -1;
    }

    ///<summary>
    ///Returns either a face up Spell/Trap card air tapped by user from his/her opponent's Spell/Trap zone
    ///OR the index of a face down Spell/Trap card air tapped by user from his/her opponent's Spell/Trap zone
    ///</summary>
    public void PromptForOneOfOpponentsSpellsOrTrapsOnField(out SpellAndTrapCard faceUpSpellOrTrap, out int faceDownSpellOrTrapIndex)
    {
        //TODO CODE
        faceUpSpellOrTrap = null;
        faceDownSpellOrTrapIndex = -1;
    }


    private void placeOpponentsHandOnGUI()
    {
        GameObject spawnPoint = null;
        for (int i = 0; i < me.getOpponent().NumberOfCardsInHand; i++)
        {
            if (i == 0)
            {
                spawnPoint = GameObject.Find("Player2Hand1");
            }
            else if(i==1)
            {
                spawnPoint = GameObject.Find("Player2Hand2");
            }
            else if (i == 2)
            {
                spawnPoint = GameObject.Find("Player2Hand3");
            }
            else if (i == 3)
            {
                spawnPoint = GameObject.Find("Player2Hand4");
            }
            else if (i == 4)
            {
                spawnPoint = GameObject.Find("Player2Hand5");
            }
            else if (i == 5)
            {
                spawnPoint = GameObject.Find("Player2Hand6");
            }
            //Destroy the Old Card
            var children = new List<GameObject>();
            foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
            children.ForEach(child => Destroy(child));

            //Add The new Card
            GameObject cardGO = Resources.Load("Card") as GameObject;
            GameObject myCardGO = Instantiate(cardGO);
            myCardGO.transform.parent = spawnPoint.transform;
            myCardGO.transform.position = spawnPoint.transform.position;
            myCardGO.GetComponent<Renderer>().material.mainTexture = CardBackTexture;
        }
        for (int i = me.getOpponent().NumberOfCardsInHand; i < 6; i++)
        {
            if (i == 0)
            {
                spawnPoint = GameObject.Find("Player2Hand1");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));
            }
            else if (i == 1)
            {
                spawnPoint = GameObject.Find("Player2Hand2");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
            else if (i == 2)
            {
                spawnPoint = GameObject.Find("Player2Hand3");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));
            }
            else if (i == 3)
            {
                spawnPoint = GameObject.Find("Player2Hand4");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
            else if (i == 4)
            {
                spawnPoint = GameObject.Find("Player2Hand5");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
            else if (i == 5)
            {
                spawnPoint = GameObject.Find("Player2Hand6");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
        }

    }

    private void placeOpponentsTrapsOnGUI()
    {
       // throw new NotImplementedException();
    }

    private void placeOpponentsMonstersOnGUI()
    {
        for (int i = 0; i < me.getOpponent().NumberOfFaceDownCardsInMonsterZone; i++)
        {
            GameObject spawnSpot = null;
            if (i == 0)
            {
                spawnSpot = GameObject.Find("Player2Monster1");
            }
            else if (i == 1)
            {
                spawnSpot = GameObject.Find("Player2Monster2");
            }
            else if (i == 2)
            {
                spawnSpot = GameObject.Find("Player2Monster3");
            }
            else if (i == 3)
            {
                spawnSpot = GameObject.Find("Player2Monster4");
            }
            else if (i == 4)
            {
                spawnSpot = GameObject.Find("Player2Monster5");
            }
            else if (i == 5)
            {
                spawnSpot = GameObject.Find("Player2Monster6");
            }
            try
            {
                var children = new List<GameObject>();
                foreach (Transform child in spawnSpot.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Monster
                GameObject monster = Instantiate(Resources.Load("FaceDownCard")) as GameObject;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;

            }
            catch (Exception e)
            {
                Debug.Log("Problem instantiating monster prefab. It Probably doesn't exist. error=" + e.Message + ". Summoning Dummy Instead.");
                GameObject monster = Instantiate(Resources.Load("dummy")) as GameObject;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;
            }
        }
        for (int i = me.getOpponent().NumberOfFaceDownCardsInMonsterZone; i < me.getOpponent().FaceUpMonsters.Count; i++)
        {
            GameObject spawnSpot = null;
            if (i == 0)
            {
                spawnSpot = GameObject.Find("Player2Monster1");
            }
            else if (i == 1)
            {
                spawnSpot = GameObject.Find("Player2Monster2");
            }
            else if (i == 2)
            {
                spawnSpot = GameObject.Find("Player2Monster3");
            }
            else if (i == 3)
            {
                spawnSpot = GameObject.Find("Player2Monster4");
            }
            else if (i == 4)
            {
                spawnSpot = GameObject.Find("Player2Monster5");
            }
            else if (i == 5)
            {
                spawnSpot = GameObject.Find("Player2Monster6");
            }
            try
            {
                var children = new List<GameObject>();
                foreach (Transform child in spawnSpot.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Monster
                GameObject monster = Instantiate(Resources.Load(me.MeReadOnly.FaceUpMonsters[i].CardName)) as GameObject;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;

            }
            catch (Exception e)
            {
                Debug.Log("Problem instantiating monster prefab. It Probably doesn't exist. error=" + e.Message + ". Summoning Dummy Instead.");
                GameObject monster = Instantiate(Resources.Load("dummy")) as GameObject;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;
            }
        }

        for (int i = me.getOpponent().NumberOfFaceDownCardsInMonsterZone + me.getOpponent().FaceUpMonsters.Count; i < 6; i++)
        {
            if (i == 0)
            {
                GameObject spawnPoint = GameObject.Find("Player2Monster1");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));
            }
            else if (i == 1)
            {
                GameObject spawnPoint = GameObject.Find("Player2Monster2");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
            else if (i == 2)
            {
                GameObject spawnPoint = GameObject.Find("Player2Monster3");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));
            }
            else if (i == 3)
            {
                GameObject spawnPoint = GameObject.Find("Player2Monster4");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
            else if (i == 4)
            {
                GameObject spawnPoint = GameObject.Find("Player2Monster5");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
            else if (i == 5)
            {
                GameObject spawnPoint = GameObject.Find("Player2Monster6");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
        }

    }

    private void placeMyTrapsOnGUI()
    {
       // throw new NotImplementedException();
    }

    private void placeMyMonsterCardOnGUI()
    {
        Debug.Log("Trying to instantiate monster");
        for(int i=0; i<me.FaceDownCardsInMonsterZone.Count; i++)
        {
            GameObject spawnSpot = null;
            if(i==0)
            {
                spawnSpot= GameObject.Find("Player1Monster1");
            }
            else if(i==1)
            {
                spawnSpot = GameObject.Find("Player1Monster2");
            }
            else if (i == 2)
            {
                spawnSpot = GameObject.Find("Player1Monster3");
            }
            else if (i == 3)
            {
                spawnSpot = GameObject.Find("Player1Monster4");
            }
            else if (i == 4)
            {
                spawnSpot = GameObject.Find("Player1Monster5");
            }
            else if (i == 5)
            {
                spawnSpot = GameObject.Find("Player1Monster6");
            }
            try
            {
                var children = new List<GameObject>();
                foreach (Transform child in spawnSpot.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Monster
                GameObject monster = Instantiate(Resources.Load(me.FaceDownCardsInMonsterZone[i].CardName)) as GameObject;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;
               
            }
            catch (Exception e)
            {
                Debug.Log("Problem instantiating monster prefab. It Probably doesn't exist. error=" + e.Message + ". Summoning Dummy Instead.");
                GameObject monster = Instantiate(Resources.Load("dummy")) as GameObject;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;
            }
        }
        for (int i = me.FaceDownCardsInMonsterZone.Count; i < me.MeReadOnly.FaceUpMonsters.Count; i++)
        {
            GameObject spawnSpot = null;
            if (i == 0)
            {
                spawnSpot = GameObject.Find("Player1Monster1");
            }
            else if (i == 1)
            {
                spawnSpot = GameObject.Find("Player1Monster2");
            }
            else if (i == 2)
            {
                spawnSpot = GameObject.Find("Player1Monster3");
            }
            else if (i == 3)
            {
                spawnSpot = GameObject.Find("Player1Monster4");
            }
            else if (i == 4)
            {
                spawnSpot = GameObject.Find("Player1Monster5");
            }
            else if (i == 5)
            {
                spawnSpot = GameObject.Find("Player1Monster6");
            }
            try
            {
                var children = new List<GameObject>();
                foreach (Transform child in spawnSpot.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Monster
                GameObject monster = Instantiate(Resources.Load(me.MeReadOnly.FaceUpMonsters[i].CardName)) as GameObject;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;

            }
            catch (Exception e)
            {
                Debug.Log("Problem instantiating monster prefab. It Probably doesn't exist. error=" + e.Message + ". Summoning Dummy Instead.");
                GameObject monster = Instantiate(Resources.Load("dummy")) as GameObject;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;
            }
        }
        for (int i = me.FaceDownCardsInMonsterZone.Count+ me.MeReadOnly.FaceUpMonsters.Count; i < 6; i++)
        {
            if (i == 0)
            {
                GameObject spawnPoint = GameObject.Find("Player1Monster1");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));
            }
            else if (i == 1)
            {
                GameObject spawnPoint = GameObject.Find("Player1Monster2");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
            else if (i == 2)
            {
                GameObject spawnPoint = GameObject.Find("Player1Monster3");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));
            }
            else if (i == 3)
            {
                GameObject spawnPoint = GameObject.Find("Player1Monster4");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
            else if (i == 4)
            {
                GameObject spawnPoint = GameObject.Find("Player1Monster5");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
            else if (i == 5)
            {
                GameObject spawnPoint = GameObject.Find("Player1Monster6");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
        }

    }

    public void setSelectedCard(Card toSelect, CurrentlySelectedCardType toSelectType)
    {
        //Return Old Selected Cards to Normal Positions
        if (currentlySelectedCardType == CurrentlySelectedCardType.Hand)
        {
            myCurrentlySelectedCardObject.gameObject.transform.position = new Vector3(myCurrentlySelectedCardObject.gameObject.transform.position.x, myCurrentlySelectedCardObject.gameObject.transform.position.y -.3f, myCurrentlySelectedCardObject.gameObject.transform.position.z);
        }

        //If the same card is hit twice
        if (myCurrentlySelectedCardObject == toSelect)
        {
            Debug.Log("Same card has been clicked twice. Summoning...");
            //Summon It
            OnSummon();
            //Reset
            myCurrentlySelectedCardObject = null;
            setCurrentlySelectedCard(null);
            currentlySelectedCardType = CurrentlySelectedCardType.None;
        }
        else
        {
            //Lift Newly Selected Cards On GUI so User knows it is selected
            toSelect.gameObject.transform.position = new Vector3(toSelect.gameObject.transform.position.x, toSelect.gameObject.transform.position.y + .3f, toSelect.gameObject.transform.position.z);
            myCurrentlySelectedCardObject = toSelect;
            currentlySelectedCardType = toSelectType;
            if (toSelectType == CurrentlySelectedCardType.Hand)
            {
                foreach (Assets.Scripts.BattleHandler.Cards.Card c in me.Hand)
                {
                    Debug.Log("Checking " + c.CardName+ " against object"+toSelect.getCardName());
                    if (c.CardName == toSelect.getCardName())
                    {
                        setCurrentlySelectedCard(c);
                    }
                }
            }
            else if(toSelectType==CurrentlySelectedCardType.Monster)
            {
                foreach (Assets.Scripts.BattleHandler.Cards.Card c in me.FaceDownCardsInMonsterZone)
                {
                    Debug.Log("Checking " + c.CardName + " against object" + toSelect.getCardName());
                    if (c.CardName == toSelect.getCardName())
                    {
                        setCurrentlySelectedCard(c);
                    }
                }
                foreach(Assets.Scripts.BattleHandler.Cards.Card c in me.MeReadOnly.FaceUpMonsters)
                {
                    Debug.Log("Checking " + c.CardName + " against object" + toSelect.getCardName());
                    if (c.CardName==toSelect.getCardName())
                    {
                        setCurrentlySelectedCard(c);
                    }
                }
            }
        }
    }

    Assets.Scripts.BattleHandler.Cards.Card getCurrentlySelectedCard()
    {
        return myCurrentlySelectedCard;
    }

    void setCurrentlySelectedCard(Assets.Scripts.BattleHandler.Cards.Card toSet)
    {
        Debug.Log("Setting currently selected card to "+toSet);
        myCurrentlySelectedCard = toSet;
        Debug.Log("Set currently selected card to " + toSet);
    }

    void OnSummon()
    {
        Debug.Log("Trying to summon: " + getCurrentlySelectedCard()+". My Id is="+me.id);
        for (int i = 0; i < me.Hand.Count; i++)
        {
            if (me.Hand[i] == getCurrentlySelectedCard())
            {
                netManager.Summon(i, me.id);
            }
        }
        updateLayout();
    }

    void OnSet()
    {
        
    }

    void OnAttack()
    {
        
    }

    void OnEndTurn()
    {
        Debug.Log("Trying to End Turn" + ". My Id is=" + me.id);
        netManager.EndTurn(me.id);
        updateLayout();
    }

    private void placeMyHandCardsOnGUI()
    {
        hand = me.Hand;
        me.myGm = this;
        for (int i = 0; i < hand.Count; i++)
        {
            if (i == 0)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand1");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Card
                GameObject cardGO = Resources.Load("Card") as GameObject;
                GameObject myCardGO = Instantiate(cardGO);
                myCardGO.transform.parent = spawnPoint.transform;
                myCardGO.transform.position = spawnPoint.transform.position;
                Debug.Log("Found Card GameObject, Loading specific Card");
                Card card = myCardGO.AddComponent<Card>();
                if (me.Hand[i] is MonsterCard)
                {
                    Debug.Log("Setting CardName to " + (me.Hand[i] as MonsterCard).CardName);
                    card.setCardName((me.Hand[i] as MonsterCard).CardName);
                    card.attack = (me.Hand[i] as MonsterCard).AttackPoints;
                    card.defense = (me.Hand[i] as MonsterCard).DefensePoints;
                    card.CardType = "Monster";
                    card.level = (me.Hand[i] as MonsterCard).Level;
                    card.setGameManager(this);
                    card.myZone = CurrentlySelectedCardType.Hand;
                    myCardGO.GetComponent<Renderer>().material.mainTexture = hand[i].CardImage;
                    myCardGO.transform.Find("CardBack").GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                }
                else if (me.Hand[i] is SpellAndTrapCard)
                {
                    card.setCardName((me.Hand[i] as SpellAndTrapCard).CardName);
                    card.CardType = "Spell";
                    card.setGameManager(this);
                    card.myZone = CurrentlySelectedCardType.Hand;
                    myCardGO.GetComponent<Renderer>().material.mainTexture = hand[i].CardImage;
                    myCardGO.transform.Find("CardBack").GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                }
                /*
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
                planeBack.transform.parent = spawnPoint.transform;*/
            }
            else if (i == 1)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand2");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Card
                GameObject cardGO = Resources.Load("Card") as GameObject;
                GameObject myCardGO = Instantiate(cardGO);
                myCardGO.transform.parent = spawnPoint.transform;
                myCardGO.transform.position = spawnPoint.transform.position;
                Debug.Log("Found Card GameObject, Loading specific Card");
                Card card = myCardGO.AddComponent<Card>();
                if (me.Hand[i] is MonsterCard)
                {
                    Debug.Log("Setting CardName to " + (me.Hand[i] as MonsterCard).CardName);
                    card.setCardName((me.Hand[i] as MonsterCard).CardName);
                    card.attack = (me.Hand[i] as MonsterCard).AttackPoints;
                    card.defense = (me.Hand[i] as MonsterCard).DefensePoints;
                    card.CardType = "Monster";
                    card.level = (me.Hand[i] as MonsterCard).Level;
                    card.setGameManager(this);
                    card.myZone = CurrentlySelectedCardType.Hand;
                    myCardGO.GetComponent<Renderer>().material.mainTexture = hand[i].CardImage;
                    myCardGO.transform.Find("CardBack").GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                }
                else if (me.Hand[i] is SpellAndTrapCard)
                {
                    card.setCardName((me.Hand[i] as SpellAndTrapCard).CardName);
                    card.CardType = "Spell";
                    card.setGameManager(this);
                    card.myZone = CurrentlySelectedCardType.Hand;
                    myCardGO.GetComponent<Renderer>().material.mainTexture = hand[i].CardImage;
                    myCardGO.transform.Find("CardBack").GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                }
            }
            else if (i == 2)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand3");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Card
                GameObject cardGO = Resources.Load("Card") as GameObject;
                GameObject myCardGO = Instantiate(cardGO);
                myCardGO.transform.parent = spawnPoint.transform;
                myCardGO.transform.position = spawnPoint.transform.position;
                Debug.Log("Found Card GameObject, Loading specific Card");
                Card card = myCardGO.AddComponent<Card>();
                if (me.Hand[i] is MonsterCard)
                {
                    Debug.Log("Setting CardName to " + (me.Hand[i] as MonsterCard).CardName);
                    card.setCardName((me.Hand[i] as MonsterCard).CardName);
                    card.attack = (me.Hand[i] as MonsterCard).AttackPoints;
                    card.defense = (me.Hand[i] as MonsterCard).DefensePoints;
                    card.CardType = "Monster";
                    card.level = (me.Hand[i] as MonsterCard).Level;
                    card.setGameManager(this);
                    card.myZone = CurrentlySelectedCardType.Hand;
                    myCardGO.GetComponent<Renderer>().material.mainTexture = hand[i].CardImage;
                    myCardGO.transform.Find("CardBack").GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                }
                else if (me.Hand[i] is SpellAndTrapCard)
                {
                    card.setCardName((me.Hand[i] as SpellAndTrapCard).CardName);
                    card.CardType = "Spell";
                    card.setGameManager(this);
                    card.myZone = CurrentlySelectedCardType.Hand;
                    myCardGO.GetComponent<Renderer>().material.mainTexture = hand[i].CardImage;
                    myCardGO.transform.Find("CardBack").GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                }
            }
            else if (i == 3)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand4");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Card
                GameObject cardGO = Resources.Load("Card") as GameObject;
                GameObject myCardGO = Instantiate(cardGO);
                myCardGO.transform.parent = spawnPoint.transform;
                myCardGO.transform.position = spawnPoint.transform.position;
                Debug.Log("Found Card GameObject, Loading specific Card");
                Card card = myCardGO.AddComponent<Card>();
                if (me.Hand[i] is MonsterCard)
                {
                    Debug.Log("Setting CardName to " + (me.Hand[i] as MonsterCard).CardName);
                    card.setCardName((me.Hand[i] as MonsterCard).CardName);
                    card.attack = (me.Hand[i] as MonsterCard).AttackPoints;
                    card.defense = (me.Hand[i] as MonsterCard).DefensePoints;
                    card.CardType = "Monster";
                    card.level = (me.Hand[i] as MonsterCard).Level;
                    card.setGameManager(this);
                    card.myZone = CurrentlySelectedCardType.Hand;
                    myCardGO.GetComponent<Renderer>().material.mainTexture = hand[i].CardImage;
                    myCardGO.transform.Find("CardBack").GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                }
                else if (me.Hand[i] is SpellAndTrapCard)
                {
                    card.setCardName((me.Hand[i] as SpellAndTrapCard).CardName);
                    card.CardType = "Spell";
                    card.setGameManager(this);
                    card.myZone = CurrentlySelectedCardType.Hand;
                    myCardGO.GetComponent<Renderer>().material.mainTexture = hand[i].CardImage;
                    myCardGO.transform.Find("CardBack").GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                }
            }
            else if (i == 4)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand5");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Card
                GameObject cardGO = Resources.Load("Card") as GameObject;
                GameObject myCardGO = Instantiate(cardGO);
                myCardGO.transform.parent = spawnPoint.transform;
                myCardGO.transform.position = spawnPoint.transform.position;
                Debug.Log("Found Card GameObject, Loading specific Card");
                Card card = myCardGO.AddComponent<Card>();
                if (me.Hand[i] is MonsterCard)
                {
                    Debug.Log("Setting CardName to " + (me.Hand[i] as MonsterCard).CardName);
                    card.setCardName((me.Hand[i] as MonsterCard).CardName);
                    card.attack = (me.Hand[i] as MonsterCard).AttackPoints;
                    card.defense = (me.Hand[i] as MonsterCard).DefensePoints;
                    card.CardType = "Monster";
                    card.level = (me.Hand[i] as MonsterCard).Level;
                    card.setGameManager(this);
                    card.myZone = CurrentlySelectedCardType.Hand;
                    myCardGO.GetComponent<Renderer>().material.mainTexture = hand[i].CardImage;
                    myCardGO.transform.Find("CardBack").GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                }
                else if (me.Hand[i] is SpellAndTrapCard)
                {
                    card.setCardName((me.Hand[i] as SpellAndTrapCard).CardName);
                    card.CardType = "Spell";
                    card.setGameManager(this);
                    card.myZone = CurrentlySelectedCardType.Hand;
                    myCardGO.GetComponent<Renderer>().material.mainTexture = hand[i].CardImage;
                    myCardGO.transform.Find("CardBack").GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                }
            }
            else if (i == 5)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand6");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

                //Add The new Card
                GameObject cardGO = Resources.Load("Card") as GameObject;
                GameObject myCardGO = Instantiate(cardGO);
                myCardGO.transform.parent = spawnPoint.transform;
                myCardGO.transform.position = spawnPoint.transform.position;
                Debug.Log("Found Card GameObject, Loading specific Card");
                Card card = myCardGO.AddComponent<Card>();
                if (me.Hand[i] is MonsterCard)
                {
                    Debug.Log("Setting CardName to " + (me.Hand[i] as MonsterCard).CardName);
                    card.setCardName((me.Hand[i] as MonsterCard).CardName);
                    card.attack = (me.Hand[i] as MonsterCard).AttackPoints;
                    card.defense = (me.Hand[i] as MonsterCard).DefensePoints;
                    card.CardType = "Monster";
                    card.level = (me.Hand[i] as MonsterCard).Level;
                    card.setGameManager(this);
                    card.myZone = CurrentlySelectedCardType.Hand;
                    myCardGO.GetComponent<Renderer>().material.mainTexture = hand[i].CardImage;
                    myCardGO.transform.Find("CardBack").GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                }
                else if (me.Hand[i] is SpellAndTrapCard)
                {
                    card.setCardName((me.Hand[i] as SpellAndTrapCard).CardName);
                    card.CardType = "Spell";
                    card.setGameManager(this);
                    card.myZone = CurrentlySelectedCardType.Hand;
                    myCardGO.GetComponent<Renderer>().material.mainTexture = hand[i].CardImage;
                    myCardGO.transform.Find("CardBack").GetComponent<Renderer>().material.mainTexture = CardBackTexture;
                }
            }
        }
        for (int i = hand.Count; i < 6; i++)
        {
            if (i == 0)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand1");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));
            }
            else if (i == 1)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand2");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
            else if (i == 2)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand3");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));
            }
            else if (i == 3)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand4");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
            else if (i == 4)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand5");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
            else if (i == 5)
            {
                GameObject spawnPoint = GameObject.Find("Player1Hand6");
                //Destroy the Old Card
                var children = new List<GameObject>();
                foreach (Transform child in spawnPoint.transform) children.Add(child.gameObject);
                children.ForEach(child => Destroy(child));

            }
        }
    }
}
