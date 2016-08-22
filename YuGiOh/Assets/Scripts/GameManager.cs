using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.BattleHandler.Game;
using System;
using Assets.Scripts.BattleHandler.Cards;

public class GameManager : MonoBehaviour
{
    //long frameCounter = long.MaxValue;
    static Player me;
    static Texture CardBackTexture;
    static NetworkManager netManager;
    static Card currentlySelectedCard;
    List<Assets.Scripts.BattleHandler.Cards.Card> hand = new List<Assets.Scripts.BattleHandler.Cards.Card>();
    private CurrentlySelectedCardType currentlySelectedCardType = CurrentlySelectedCardType.None;
    private Assets.Scripts.BattleHandler.Cards.Card myCurrentlySelectedCard;
    private Assets.Scripts.BattleHandler.Cards.Card toAttackCard;
    private int toAttackIndexIfFaceDown = -1;
    private Card myCurrentlySelectedCardObject;
    internal Quaternion rotation = new Quaternion(0f, 90f, 0f, 0f);
    bool attackWorked = false;

    public enum CurrentlySelectedCardType
    {
        Hand,
        Monster,
        None
    }

    public static GameManager MakeManager(GameObject toAddTo, Player pMe, Texture CardBack, NetworkManager networkManager)
    {
        GameManager myManager = toAddTo.GetComponent<GameManager>();
        //Debug.Log("pMe Id=" + pMe.id);
        me = pMe;
        CardBackTexture = CardBack;
        netManager = networkManager;
        return myManager;
    }

    void Start()
    {
        updateLayout();
        GameObject.Find("EndTurn").GetComponent<EndTurn>().setGameManager(this);
        GameObject.Find("Sacrifice").GetComponent<Sacrifice>().setGameManager(this);
        GameObject.Find("ChangeCardPosition").GetComponent<ChangeCardMode>().setGameManager(this);
        GameObject nBox = GameObject.Find("NotificationsBox");
        nBox.GetComponent<MeshCollider>().enabled = true;
        MeshRenderer[] mrs = nBox.GetComponents<MeshRenderer>();
        foreach (MeshRenderer mr in mrs)
        {
            mr.enabled = true;
        }
        nBox.GetComponentInChildren<MeshRenderer>().enabled = true;
        nBox.GetComponentInChildren<TextMesh>().text = "Started Game Manager";
    }

    public void updateLayout()
    {
        try
        {
            placeMyHandCardsOnGUI();
            placeMyMonsterCardOnGUI();
            placeMyTrapsOnGUI();
            placeOpponentsMonstersOnGUI();
            placeOpponentsTrapsOnGUI();
            placeOpponentsHandOnGUI();
            updateLifePoints();
            currentlySelectedCard = null;
            currentlySelectedCardType = CurrentlySelectedCardType.None;
            myCurrentlySelectedCard = null;
            myCurrentlySelectedCardObject = null;
        }
        catch (Exception e)
        {
            Debug.Log("Problem updating layout: " + e.Message);
            GameObject.Find("NotificationsBox").GetComponentInChildren<TextMesh>().text = "Couldn't Update Layout";
        }
    }

    public void updateLifePoints()
    {
        GameObject myLPBox = GameObject.Find("MyLifePointsBox");
        GameObject oppLPBox = GameObject.Find("OppLifePointsBox");
        myLPBox.GetComponent<MeshCollider>().enabled = true;
        MeshRenderer[] mrs = myLPBox.GetComponents<MeshRenderer>();
        foreach (MeshRenderer mr in mrs)
        {
            mr.enabled = true;
        }
        myLPBox.GetComponentInChildren<MeshRenderer>().enabled = true;
        myLPBox.GetComponentInChildren<TextMesh>().text = PhotonNetwork.playerName + ": " + me.getLifePoints();
        oppLPBox.GetComponent<MeshCollider>().enabled = true;
        MeshRenderer[] mrs2 = oppLPBox.GetComponents<MeshRenderer>();
        foreach (MeshRenderer mr in mrs2)
        {
            mr.enabled = true;
        }
        PhotonPlayer[] pps = PhotonNetwork.playerList;
        foreach (PhotonPlayer pp in pps)
        {
            if (pp != PhotonNetwork.player)
            {
                oppLPBox.GetComponentInChildren<TextMesh>().text = pp.name + ": " + me.getOpponent().LifePoints;
            }
        }
        GameObject.Find("NotificationsBox").GetComponentInChildren<TextMesh>().text = "Updated Lifepoints";
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
            else if (i == 1)
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
        GameObject.Find("NotificationsBox").GetComponentInChildren<TextMesh>().text = "Updated Opponents Hand";

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
                Card c = monster.AddComponent<Card>();
                c.setGameManager(this);
                c.setCardName(i + "");
                c.myZone = CurrentlySelectedCardType.Monster;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;

            }
            catch (Exception e)
            {
                Debug.Log("Problem instantiating monster prefab. It Probably doesn't exist. error=" + e.Message + ". Summoning Dummy Instead.");
                GameObject monster = Instantiate(Resources.Load("dummy")) as GameObject;
                Card c = monster.AddComponent<Card>();
                c.setGameManager(this);
                c.setCardName(i + "");
                c.myZone = CurrentlySelectedCardType.Monster;
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
                Card c = monster.AddComponent<Card>();
                c.setCardName(me.MeReadOnly.FaceUpMonsters[i].CardName);
                c.setGameManager(this);
                c.myZone = CurrentlySelectedCardType.Monster;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;

            }
            catch (Exception e)
            {
                Debug.Log("Problem instantiating monster prefab. It Probably doesn't exist. error=" + e.Message + ". Summoning Dummy Instead.");
                GameObject monster = Instantiate(Resources.Load("dummy")) as GameObject;
                Card c = monster.AddComponent<Card>();
                c.setCardName(me.MeReadOnly.FaceUpMonsters[i].CardName);
                c.setGameManager(this);
                c.myZone = CurrentlySelectedCardType.Monster;
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
        GameObject.Find("NotificationsBox").GetComponentInChildren<TextMesh>().text = "Updated Opponents Monsters";

    }

    private void placeMyTrapsOnGUI()
    {
        // throw new NotImplementedException();
    }

    private void placeMyMonsterCardOnGUI()
    {
        //Debug.Log("Trying to instantiate monster");
        for (int i = 0; i < me.FaceDownCardsInMonsterZone.Count; i++)
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
                GameObject monster = Instantiate(Resources.Load(me.FaceDownCardsInMonsterZone[i].CardName)) as GameObject;
                Card c = monster.AddComponent<Card>();
                c.setCardName(me.FaceDownCardsInMonsterZone[i].CardName);
                c.setGameManager(this);
                c.myZone = CurrentlySelectedCardType.Monster;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;

            }
            catch (Exception e)
            {
                Debug.Log("Problem instantiating monster prefab. It Probably doesn't exist. error=" + e.Message + ". Summoning Dummy Instead.");
                GameObject monster = Instantiate(Resources.Load("dummy")) as GameObject;
                Card c = monster.AddComponent<Card>();
                c.setCardName(me.FaceDownCardsInMonsterZone[i].CardName);
                c.setGameManager(this);
                c.setZone(CurrentlySelectedCardType.Monster);
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
                Card c = monster.AddComponent<Card>();
                c.setCardName(me.MeReadOnly.FaceUpMonsters[i].CardName);
                c.setGameManager(this);
                c.myZone = CurrentlySelectedCardType.Monster;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;

            }
            catch (Exception e)
            {
                Debug.Log("Problem instantiating monster prefab. It Probably doesn't exist. error=" + e.Message + ". Summoning Dummy Instead.");
                GameObject monster = Instantiate(Resources.Load("dummy")) as GameObject;
                Card c = monster.AddComponent<Card>();
                c.setCardName(me.MeReadOnly.FaceUpMonsters[i].CardName);
                c.setGameManager(this);
                c.myZone = CurrentlySelectedCardType.Monster;
                monster.transform.parent = spawnSpot.transform;
                monster.transform.position = spawnSpot.transform.position;
            }
        }
        for (int i = me.FaceDownCardsInMonsterZone.Count + me.MeReadOnly.FaceUpMonsters.Count; i < 6; i++)
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
        GameObject.Find("NotificationsBox").GetComponentInChildren<TextMesh>().text = "Updated My Monsters";

    }

    public void setSelectedCard(Card toSelect, CurrentlySelectedCardType toSelectType)
    {
        //Return Old Selected Cards to Normal Positions
        if (currentlySelectedCardType == CurrentlySelectedCardType.Monster || currentlySelectedCardType == CurrentlySelectedCardType.Hand)
        {
            myCurrentlySelectedCardObject.gameObject.transform.position = new Vector3(myCurrentlySelectedCardObject.gameObject.transform.position.x, myCurrentlySelectedCardObject.gameObject.transform.position.y - .3f, myCurrentlySelectedCardObject.gameObject.transform.position.z);
        }
        attackWorked = false;

        //If the same card is hit twice
        if (myCurrentlySelectedCardObject == toSelect && currentlySelectedCardType == CurrentlySelectedCardType.Hand)
        {
            Debug.Log("Same card has been clicked twice. Summoning Or SpellCasting...");
            //Summon It
            if (myCurrentlySelectedCard is MonsterCard)
            {
                OnSummon();
                if (currentlySelectedCard != null)
                {
                    GameObject.Find("NotificationsBox").GetComponentInChildren<TextMesh>().text = "Set " + currentlySelectedCard.CardName;
                }

            }
            else
            {
                OnSpell();
                if (currentlySelectedCard != null)
                {
                    GameObject.Find("NotificationsBox").GetComponentInChildren<TextMesh>().text = "Casted " + currentlySelectedCard.CardName;
                }
            }
            //Reset
            myCurrentlySelectedCardObject = null;
            setCurrentlySelectedCard(null);
            currentlySelectedCardType = CurrentlySelectedCardType.None;

        }
        else
        {
            //If we are currently selecting a monster and click on another monster, it is probable we are trying to attack
            if (currentlySelectedCardType == CurrentlySelectedCardType.Monster && toSelectType == CurrentlySelectedCardType.Monster)
            {
                Debug.Log("Both Selected And About to Select are monsters...trying to attack-->");
                //Reset toAttackCard and Check if opponents field has the monster to attack
                toAttackCard = null;
                toAttackIndexIfFaceDown = -1;

                //If toSelect's card name is an index number it is facedown.
                if (toSelect.CardName == "0")
                {
                    toAttackIndexIfFaceDown = 0;
                }
                else if (toSelect.CardName == "1")
                {
                    toAttackIndexIfFaceDown = 1;
                }
                else if (toSelect.CardName == "2")
                {
                    toAttackIndexIfFaceDown = 2;
                }
                else if (toSelect.CardName == "3")
                {
                    toAttackIndexIfFaceDown = 3;
                }
                else if (toSelect.CardName == "4")
                {
                    toAttackIndexIfFaceDown = 4;
                }
                else if (toSelect.CardName == "5")
                {
                    toAttackIndexIfFaceDown = 5;
                }
                if (toAttackIndexIfFaceDown == -1)
                {
                    for (int i = 0; i < me.getOpponent().FaceUpMonsters.Count; i++)
                    {
                        if (me.getOpponent().FaceUpMonsters[i].CardName == toSelect.CardName)
                        {
                            toAttackCard = me.getOpponent().FaceUpMonsters[i];
                        }
                    }
                }
                Debug.Log("toAttackCard=" + toAttackCard + "  index=" + toAttackIndexIfFaceDown);
                if (toAttackCard != null || toAttackIndexIfFaceDown != -1)
                {
                    OnAttack();
                    if (currentlySelectedCard != null)
                    {
                        if (toAttackIndexIfFaceDown != -1)
                        {
                            GameObject.Find("NotificationsBox").GetComponentInChildren<TextMesh>().text = currentlySelectedCard.CardName + " Attacked Face Down Monster";
                        }
                        else
                        {
                            GameObject.Find("NotificationsBox").GetComponentInChildren<TextMesh>().text = currentlySelectedCard.CardName + " Attacked " + toAttackCard.CardName;
                        }
                    }
                }
            }
            //If we are currently selecting a spell card and we click on a monster we are likely trying to equip.
            if (currentlySelectedCardType == CurrentlySelectedCardType.Hand && myCurrentlySelectedCard.Attribute==CardAttributeOrType.Spell && toSelectType == CurrentlySelectedCardType.Monster)
            {
                Debug.Log("Selected is Spell and And About to Select is Monster...trying to equip-->");
                foreach (Assets.Scripts.BattleHandler.Cards.Card c in me.FaceDownCardsInMonsterZone)
                {
                    if (c.CardName == toSelect.getCardName())
                    {
                        OnEquip(c as MonsterCard);
                    }
                }
                foreach (Assets.Scripts.BattleHandler.Cards.Card c in me.MeReadOnly.FaceUpMonsters)
                {
                    if (c.CardName == toSelect.getCardName())
                    {
                        OnEquip(c as MonsterCard);
                    }
                }
            }
            if (attackWorked == false)
            {
                Debug.Log("Attack either failed or was never tried....lifting newly selected card-->");
                //Lift Newly Selected Cards On GUI so User knows it is selected
                toSelect.gameObject.transform.position = new Vector3(toSelect.gameObject.transform.position.x, toSelect.gameObject.transform.position.y + .3f, toSelect.gameObject.transform.position.z);
                myCurrentlySelectedCardObject = toSelect;
                currentlySelectedCardType = toSelectType;
                if (toSelectType == CurrentlySelectedCardType.Hand)
                {
                    foreach (Assets.Scripts.BattleHandler.Cards.Card c in me.Hand)
                    {
                        if (c.CardName == toSelect.getCardName())
                        {
                            setCurrentlySelectedCard(c);
                            if (currentlySelectedCard != null)
                            {
                                GameObject.Find("NotificationsBox").GetComponentInChildren<TextMesh>().text = "Currently Selecting Hand Card" + currentlySelectedCard.CardName;
                            }
                        }
                    }
                }
                else if (toSelectType == CurrentlySelectedCardType.Monster)
                {
                    foreach (Assets.Scripts.BattleHandler.Cards.Card c in me.FaceDownCardsInMonsterZone)
                    {
                        if (c.CardName == toSelect.getCardName())
                        {
                            setCurrentlySelectedCard(c);
                            if (currentlySelectedCard != null)
                            {
                                GameObject.Find("NotificationsBox").GetComponentInChildren<TextMesh>().text = "Currently Selecting Face Down Monster " + currentlySelectedCard.CardName;
                            }
                        }
                    }
                    foreach (Assets.Scripts.BattleHandler.Cards.Card c in me.MeReadOnly.FaceUpMonsters)
                    {
                        if (c.CardName == toSelect.getCardName())
                        {
                            setCurrentlySelectedCard(c);
                            if (currentlySelectedCard != null)
                            {
                                GameObject.Find("NotificationsBox").GetComponentInChildren<TextMesh>().text = "Currently Selecting Face Up Monster " + currentlySelectedCard.CardName;
                            }
                       }
                    }
                }
            }
        }
    }

    Assets.Scripts.BattleHandler.Cards.Card getCurrentlySelectedCard()
    {
        return myCurrentlySelectedCard;
    }

    Assets.Scripts.BattleHandler.Cards.Card getToAttackCard()
    {
        return toAttackCard;
    }

    int getToAttackFaceDownIndex()
    {
        return toAttackIndexIfFaceDown;
    }

    void setCurrentlySelectedCard(Assets.Scripts.BattleHandler.Cards.Card toSet)
    {
        if (toSet != null)
        {
            Debug.Log("Setting currently selected card to " + toSet.CardName);
        }
        myCurrentlySelectedCard = toSet;
    }

    void OnSummon()
    {
        Debug.Log("Trying to summon: " + getCurrentlySelectedCard().CardName + ". My Id is=" + me.id);
        for (int i = 0; i < me.Hand.Count; i++)
        {
            if (me.Hand[i] == getCurrentlySelectedCard())
            {
                netManager.Summon(i, me.id);
            }
        }
        updateLayout();
    }

    void OnEquip(MonsterCard toEquip)
    {
        if (getCurrentlySelectedCard() != null)
        {
            Debug.Log("Trying to equip: " + getCurrentlySelectedCard().CardName + " to "+toEquip.CardName+". My Id is=" + me.id);
            for (int i = 0; i < me.Hand.Count; i++)
            {
                if (me.Hand[i] == getCurrentlySelectedCard())
                {
                    netManager.Equip(toEquip.CardName, i, me.id);
                }
            }
            updateLayout();
        }
    }

    void OnSpell()
    {
        if (getCurrentlySelectedCard() != null)
        {
            Debug.Log("Trying to cast: " + getCurrentlySelectedCard().CardName + ". My Id is=" + me.id);
            for (int i = 0; i < me.Hand.Count; i++)
            {
                if (me.Hand[i] == getCurrentlySelectedCard())
                {
                    netManager.Spell(i, me.id);
                }
            }
            updateLayout();
        }
    }

    void OnSet()
    {

    }

    internal void OnAttackLifePoints()
    {
        if (getCurrentlySelectedCard() != null)
        {
            Debug.Log("Trying to attack lifepoints with : " + getCurrentlySelectedCard().CardName + ". My Id is=" + me.id);
            for (int i = 0; i < me.FaceDownCardsInMonsterZone.Count; i++)
            {
                if (me.FaceDownCardsInMonsterZone[i] == getCurrentlySelectedCard())
                {
                    netManager.AttackLP(i, me.id);
                }
            }
            for (int i = 0; i < me.MeReadOnly.FaceUpMonsters.Count; i++)
            {
                if (me.MeReadOnly.FaceUpMonsters[i] == getCurrentlySelectedCard())
                {
                    netManager.AttackLP(i + me.FaceDownCardsInMonsterZone.Count, me.id);
                }
            }
            updateLayout();
        }
    }

    void OnAttack()
    {
        Debug.Log(getCurrentlySelectedCard().CardName + " is trying to attack: ");
        if (getToAttackCard() != null)
        {
            Debug.Log(getToAttackCard().CardName);
        }
        Debug.Log(" or a facedown card at index " + getToAttackFaceDownIndex() + ". My Id is=" + me.id);
        string result = "idk";
        if (getToAttackCard() != null)
        {
            for (int i = 0; i < me.FaceDownCardsInMonsterZone.Count; i++)
            {
                if (me.FaceDownCardsInMonsterZone[i] == getCurrentlySelectedCard())
                {
                    for (int j = 0; j < me.getOpponent().FaceUpMonsters.Count; j++)
                    {
                        if (me.getOpponent().FaceUpMonsters[j] == getToAttackCard())
                        {
                            result = netManager.AttackFU(i, me.id, j);
                        }
                    }
                }
            }
            for (int i = 0; i < me.MeReadOnly.FaceUpMonsters.Count; i++)
            {
                if (me.MeReadOnly.FaceUpMonsters[i] == getCurrentlySelectedCard())
                {
                    for (int j = 0; j < me.getOpponent().FaceUpMonsters.Count; j++)
                    {
                        if (me.getOpponent().FaceUpMonsters[j] == getToAttackCard())
                        {
                            result = netManager.AttackFU(i + me.FaceDownCardsInMonsterZone.Count, me.id, j);
                        }
                    }
                }
            }
        }
        else if (getToAttackFaceDownIndex() != -1)
        {
            for (int i = 0; i < me.FaceDownCardsInMonsterZone.Count; i++)
            {
                if (me.FaceDownCardsInMonsterZone[i] == getCurrentlySelectedCard())
                {
                    result = netManager.AttackFD(i, me.id);
                }
            }
            for (int i = 0; i < me.MeReadOnly.FaceUpMonsters.Count; i++)
            {
                if (me.MeReadOnly.FaceUpMonsters[i] == getCurrentlySelectedCard())
                {
                    result = netManager.AttackFD(i + me.FaceDownCardsInMonsterZone.Count, me.id);
                }
            }
        }

        if (result == "")
        {
            attackWorked = true;
        }
        updateLayout();
    }

    internal void OnSacrifice()
    {
        Debug.Log("Trying to sacrifice: " + getCurrentlySelectedCard().CardName + ". My Id is=" + me.id);
        for (int i = 0; i < me.Hand.Count; i++)
        {
            if (me.Hand[i] == getCurrentlySelectedCard())
            {
                netManager.Sacrifice(i, me.id);
            }
        }
        updateLayout();
    }

    internal void OnChangeMode()
    {
        if (myCurrentlySelectedCard is MonsterCard && me.FaceDownCardsInMonsterZone.Contains(myCurrentlySelectedCard as MonsterCard))
        {
            Debug.Log("Trying to change my card's mode" + ". My Id is=" + me.id);
            netManager.ChangeMode(me.id,1,me.FaceDownCardsInMonsterZone.IndexOf(myCurrentlySelectedCard as MonsterCard));
        }
        else if(myCurrentlySelectedCard is MonsterCard && me.MeReadOnly.FaceUpMonsters.Contains(myCurrentlySelectedCard as MonsterCard))
        {
            Debug.Log("Trying to change my card's mode" + ". My Id is=" + me.id);
            netManager.ChangeMode(me.id, 0, me.MeReadOnly.FaceUpMonsters.IndexOf(myCurrentlySelectedCard as MonsterCard));
        }
    }

    internal void OnEndTurn()
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
                //Debug.Log("Found Card GameObject, Loading specific Card");
                Card card = myCardGO.AddComponent<Card>();
                if (me.Hand[i] is MonsterCard)
                {
                    //  Debug.Log("Setting CardName to " + (me.Hand[i] as MonsterCard).CardName);
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
                //Debug.Log("Found Card GameObject, Loading specific Card");
                Card card = myCardGO.AddComponent<Card>();
                if (me.Hand[i] is MonsterCard)
                {
                    //  Debug.Log("Setting CardName to " + (me.Hand[i] as MonsterCard).CardName);
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
                //Debug.Log("Found Card GameObject, Loading specific Card");
                Card card = myCardGO.AddComponent<Card>();
                if (me.Hand[i] is MonsterCard)
                {
                    // Debug.Log("Setting CardName to " + (me.Hand[i] as MonsterCard).CardName);
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
                //Debug.Log("Found Card GameObject, Loading specific Card");
                Card card = myCardGO.AddComponent<Card>();
                if (me.Hand[i] is MonsterCard)
                {
                    //Debug.Log("Setting CardName to " + (me.Hand[i] as MonsterCard).CardName);
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
                //Debug.Log("Found Card GameObject, Loading specific Card");
                Card card = myCardGO.AddComponent<Card>();
                if (me.Hand[i] is MonsterCard)
                {
                    // Debug.Log("Setting CardName to " + (me.Hand[i] as MonsterCard).CardName);
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
                // Debug.Log("Found Card GameObject, Loading specific Card");
                Card card = myCardGO.AddComponent<Card>();
                if (me.Hand[i] is MonsterCard)
                {
                    //  Debug.Log("Setting CardName to " + (me.Hand[i] as MonsterCard).CardName);
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
            Debug.Log("Placed " + me.Hand[i].CardName + " into hand on GUI");
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
