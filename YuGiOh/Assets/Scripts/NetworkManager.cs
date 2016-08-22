using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Scripts.BattleHandler.Game;
using Assets.Scripts.BattleHandler.Cards;

public class NetworkManager : MonoBehaviour
{

    List<string> chatMessages;
    int maxChatMessages = 12;
    bool chatWindowUp = false;
    string password = "";
    bool connecting = false;
    private string currentMessage = string.Empty;
    int returnCounter = 0;
    private bool wasSignedIn = false;
    Texture CardBackTexture;
    System.Random rand = new System.Random();
    Player p1;
    Player p2;
    bool hasMadeGameManager = false;
    Game g;
    bool gameCreated = false;
    bool multiplayerSelected = false;
    int gameSeed = -1;
    string returnsFromRPC = "-1";
    GameManager gm1 = null;

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.player.name = PlayerPrefs.GetString("Username", "");
        chatMessages = new List<string>();
    }


    public void OnMultiplayer()
    {
        multiplayerSelected = true;
    }

    public void OnChat(string message)
    {
        AddChatMessage(message);
    }

    void OnDestroy()
    {
        PlayerPrefs.SetString("Username", PhotonNetwork.player.name);
    }


    public string getLatestChatMsg()
    {
        if (chatMessages.Count > 0)
        {
            return chatMessages[chatMessages.Count - 1];
        }
        else
        {
            return "";
        }
    }

    public void AddChatMessage(string m)
    {
        GetComponent<PhotonView>().RPC("AddChatMessage_RPC", PhotonTargets.All, m);
    }

    public void AddChatNonRPC(string m)
    {
        while (chatMessages.Count >= maxChatMessages)
        {
            chatMessages.RemoveAt(0);
        }
        chatMessages.Add(m);
    }

    [PunRPC]
    void AddChatMessage_RPC(string m)
    {
        while (chatMessages.Count >= maxChatMessages)
        {
            chatMessages.RemoveAt(0);
        }
        chatMessages.Add(m);
    }

    public void Connect()
    {
        Debug.Log("Connect");
        PhotonNetwork.ConnectUsingSettings("1.0.0");
    }

    void OnGUI()
    {
        GUIStyle loc = new GUIStyle();
        loc.normal.textColor = Color.green;
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString(), loc);

        if ((PhotonNetwork.connected == false) && (connecting == false) && !wasSignedIn)
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            GUILayout.Label("YuGiOh for HoloLens!");
            GUILayoutOption[] options = new GUILayoutOption[2];
            options[0] = GUILayout.Width(200);
            options[1] = GUILayout.Height(30);

            PhotonNetwork.player.name = GUILayout.TextField(PhotonNetwork.player.name, options);
            GUILayout.EndHorizontal();

            /* GUILayout.BeginHorizontal();
             GUILayout.Label("Password: ");
             password = GUILayout.TextField(password, options);
             GUILayout.EndHorizontal(); */

            if (GUILayout.Button("Multiplayer") || multiplayerSelected)
            {
                connecting = true;
                Connect();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        else if (PhotonNetwork.connected == true && connecting == false && wasSignedIn)
        {
            if (!chatWindowUp)
            {

                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                foreach (string msg in chatMessages)
                {
                    GUIStyle local = new GUIStyle();
                    local.normal.textColor = Color.white;
                    GUILayout.Label(msg, local);
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();

            }
            else
            {
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                foreach (string m in chatMessages)
                {
                    GUIStyle local = new GUIStyle();
                    local.normal.textColor = Color.white;
                    GUILayout.Label(m, local);

                }

                GUILayout.BeginHorizontal(GUILayout.Width(300));
                if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
                {
                    if (!string.IsNullOrEmpty(currentMessage.Trim()))
                    {
                        Debug.Log("Sending Message");
                        AddChatMessage(PhotonNetwork.player.name + ": " + currentMessage);
                        currentMessage = string.Empty;
                    }
                    else
                    {
                        Debug.Log("Message was blank. Closing chat Window.");
                        chatWindowUp = false;
                    }
                }
                GUI.SetNextControlName("ChatTextField");
                currentMessage = GUILayout.TextField(currentMessage);
                GUI.FocusControl("ChatTextField");


                GUILayout.EndHorizontal();


                GUILayout.EndVertical();
                GUILayout.EndArea();
            }

        }
    }

    void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("onFailedJoinRandom");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    internal string AttackLP(int attackingCardIndexInMyMonsterZone, int attackingId)
    {
        GetComponent<PhotonView>().RPC("AttackLP_RPC", PhotonTargets.All, attackingCardIndexInMyMonsterZone, attackingId);
        while (returnsFromRPC == "-1")
        {

        }
        string toReturn = returnsFromRPC;
        returnsFromRPC = "-1";
        return toReturn;
    }

    [PunRPC]
    void AttackLP_RPC(int attackingCardIndexInMyMonsterZone, int attackingId)
    {
        Debug.Log("ID " + attackingId + " is trying to attack lifepoints");
        if (p1.id == attackingId)
        {
            if (attackingCardIndexInMyMonsterZone > p1.FaceDownCardsInMonsterZone.Count)
            {
                attackingCardIndexInMyMonsterZone = attackingCardIndexInMyMonsterZone - p1.FaceDownCardsInMonsterZone.Count;
                returnsFromRPC = p1.AttackLifePoints(p1.MeReadOnly.FaceUpMonsters[attackingCardIndexInMyMonsterZone]);
            }
            else
            {
                returnsFromRPC = p1.AttackLifePoints(p1.FaceDownCardsInMonsterZone[attackingCardIndexInMyMonsterZone]);
            }
        }
        else if (p2.id == attackingId)
        {
            if (attackingCardIndexInMyMonsterZone > p2.FaceDownCardsInMonsterZone.Count)
            {
                attackingCardIndexInMyMonsterZone = attackingCardIndexInMyMonsterZone - p2.FaceDownCardsInMonsterZone.Count;
                returnsFromRPC = p2.AttackLifePoints(p2.MeReadOnly.FaceUpMonsters[attackingCardIndexInMyMonsterZone]);
            }
            else
            {
                returnsFromRPC = p2.AttackLifePoints(p2.FaceDownCardsInMonsterZone[attackingCardIndexInMyMonsterZone]);
            }
        }
        Debug.Log("Tried to attack face up with result= " + returnsFromRPC);
        if (gm1 != null)
        {
            gm1.updateLayout();
        }
    }

    internal string AttackFU(int attackingCardIndexInMyMonsterZone, int attackingId, int defendingIndexInOppMonsterZone)
    {
        GetComponent<PhotonView>().RPC("AttackFU_RPC", PhotonTargets.All, attackingCardIndexInMyMonsterZone, attackingId, defendingIndexInOppMonsterZone);
        while (returnsFromRPC == "-1")
        {

        }
        string toReturn = returnsFromRPC;
        returnsFromRPC = "-1";
        return toReturn;
    }

    [PunRPC]
    void AttackFU_RPC(int attackingCardIndexInMyMonsterZone, int attackingId, int defendingIndexInOppMonsterZone)
    {
        Debug.Log("ID " + attackingId + " is trying to attack a face up card");
        if (p1.id == attackingId)
        {
            if (attackingCardIndexInMyMonsterZone > p1.FaceDownCardsInMonsterZone.Count)
            {
                attackingCardIndexInMyMonsterZone = attackingCardIndexInMyMonsterZone - p1.FaceDownCardsInMonsterZone.Count;
                returnsFromRPC = p1.AttackFaceUpOpponent(p1.MeReadOnly.FaceUpMonsters[attackingCardIndexInMyMonsterZone], p1.getOpponent().FaceUpMonsters[defendingIndexInOppMonsterZone]);
            }
            else
            {
                returnsFromRPC = p1.AttackFaceUpOpponent(p1.FaceDownCardsInMonsterZone[attackingCardIndexInMyMonsterZone], p1.getOpponent().FaceUpMonsters[defendingIndexInOppMonsterZone]);
            }
        }
        else if (p2.id == attackingId)
        {
            if (attackingCardIndexInMyMonsterZone > p2.FaceDownCardsInMonsterZone.Count)
            {
                attackingCardIndexInMyMonsterZone = attackingCardIndexInMyMonsterZone - p2.FaceDownCardsInMonsterZone.Count;
                returnsFromRPC = p2.AttackFaceUpOpponent(p2.MeReadOnly.FaceUpMonsters[attackingCardIndexInMyMonsterZone], p2.getOpponent().FaceUpMonsters[defendingIndexInOppMonsterZone]);
            }
            else
            {
                returnsFromRPC = p2.AttackFaceUpOpponent(p2.FaceDownCardsInMonsterZone[attackingCardIndexInMyMonsterZone], p2.getOpponent().FaceUpMonsters[defendingIndexInOppMonsterZone]);
            }
        }
        Debug.Log("Tried to attack face up with result= " + returnsFromRPC);
        if (gm1 != null)
        {
            gm1.updateLayout();
        }
    }

    internal string AttackFD(int attackingCardIndexInMyMonsterZone, int attackingId)
    {
        GetComponent<PhotonView>().RPC("AttackFD_RPC", PhotonTargets.All, attackingCardIndexInMyMonsterZone, attackingId);
        while (returnsFromRPC == "-1")
        {

        }
        string toReturn = returnsFromRPC;
        returnsFromRPC = "-1";
        return toReturn;
    }

    [PunRPC]
    void AttackFD_RPC(int attackingCardIndexInMyMonsterZone, int attackingId)
    {
        Debug.Log("ID " + attackingId + " is trying to attack a face down card");
        if (p1.id == attackingId)
        {
            if (attackingCardIndexInMyMonsterZone > p1.FaceDownCardsInMonsterZone.Count)
            {
                attackingCardIndexInMyMonsterZone = attackingCardIndexInMyMonsterZone - p1.FaceDownCardsInMonsterZone.Count;
                if (p1.MeReadOnly.FaceUpMonsters.Count > attackingCardIndexInMyMonsterZone && attackingCardIndexInMyMonsterZone > -1)
                {
                    returnsFromRPC = p1.AttackFaceDownOpponent(p1.MeReadOnly.FaceUpMonsters[attackingCardIndexInMyMonsterZone]);
                }
            }
            else
            {
                if (p1.FaceDownCardsInMonsterZone.Count > attackingCardIndexInMyMonsterZone && attackingCardIndexInMyMonsterZone > -1)
                {
                    returnsFromRPC = p1.AttackFaceDownOpponent(p1.FaceDownCardsInMonsterZone[attackingCardIndexInMyMonsterZone]);
                }
            }
        }
        else if (p2.id == attackingId)
        {
            if (attackingCardIndexInMyMonsterZone > p2.FaceDownCardsInMonsterZone.Count)
            {
                attackingCardIndexInMyMonsterZone = attackingCardIndexInMyMonsterZone - p2.FaceDownCardsInMonsterZone.Count;
                if (p2.MeReadOnly.FaceUpMonsters.Count > attackingCardIndexInMyMonsterZone && attackingCardIndexInMyMonsterZone > -1)
                {
                    returnsFromRPC = p2.AttackFaceDownOpponent(p2.MeReadOnly.FaceUpMonsters[attackingCardIndexInMyMonsterZone]);
                }
            }
            else
            {
                if (p2.FaceDownCardsInMonsterZone.Count > attackingCardIndexInMyMonsterZone && attackingCardIndexInMyMonsterZone > -1)
                {
                    returnsFromRPC = p2.AttackFaceDownOpponent(p2.FaceDownCardsInMonsterZone[attackingCardIndexInMyMonsterZone]);
                }
            }
        }
        Debug.Log("Tried to attack face down with result= " + returnsFromRPC);
        if (gm1 != null)
        {
            gm1.updateLayout();
        }
    }

    internal string Spell(int cardIndexInHand, int summoningId)
    {
        GetComponent<PhotonView>().RPC("Spell_RPC", PhotonTargets.All, cardIndexInHand, summoningId);
        while (returnsFromRPC == "-1")
        {

        }
        string toReturn = returnsFromRPC;
        returnsFromRPC = "-1";
        return toReturn;
    }

    [PunRPC]
    void Spell_RPC(int cardIndexInHand, int summoningId)
    {
        Debug.Log("ID " + summoningId + " is trying to sacrifice");
        if (p1.id == summoningId)
        {
            if (p1.Hand[cardIndexInHand] is SpellAndTrapCard)
            {
                Debug.Log("Player 1 spells card number " + cardIndexInHand);
                returnsFromRPC = p1.CastSpellOrTrap(p1.Hand[cardIndexInHand]);
            }
            else
            {
                returnsFromRPC = "Card is not a spell or trap. Cannot Cast.";
            }
        }
        else if (p2.id == summoningId)
        {
            if (p2.Hand[cardIndexInHand] is SpellAndTrapCard)
            {
                Debug.Log("Player 2 spells card number " + cardIndexInHand);
                returnsFromRPC = p2.CastSpellOrTrap(p2.Hand[cardIndexInHand]);
            }
            else
            {
                returnsFromRPC = "Card is not monster. Cannot Sacrifice.";
            }
        }
        Debug.Log("Tried to spell with result= " + returnsFromRPC);
        if (gm1 != null)
        {
            gm1.updateLayout();
        }
    }

    internal string Sacrifice(int sacrificingId, int indexOfMonsterToSacrifice)
    {
        GetComponent<PhotonView>().RPC("Sacrifice_RPC", PhotonTargets.All, indexOfMonsterToSacrifice, sacrificingId);
        while (returnsFromRPC == "-1")
        {

        }
        string toReturn = returnsFromRPC;
        returnsFromRPC = "-1";
        return toReturn;
    }

    [PunRPC]
    void Sacrifice_RPC(int cardIndexInHand, int summoningId)
    {
        Debug.Log("ID " + summoningId + " is trying to sacrifice");
        if (p1.id == summoningId)
        {
            if (p1.Hand[cardIndexInHand] is MonsterCard)
            {
                Debug.Log("Player 1 sacrifices card number " + cardIndexInHand);
                returnsFromRPC = p1.Sacrifice(p1.Hand[cardIndexInHand] as MonsterCard);
            }
            else
            {
                returnsFromRPC = "Card is not monster. Cannot Sacrifice.";
            }
        }
        else if (p2.id == summoningId)
        {
            if (p2.Hand[cardIndexInHand] is MonsterCard)
            {
                Debug.Log("Player 2 sacrifices card number " + cardIndexInHand);
                returnsFromRPC = p2.Sacrifice(p2.Hand[cardIndexInHand] as MonsterCard);
            }
            else
            {
                returnsFromRPC = "Card is not monster. Cannot Sacrifice.";
            }
        }
        Debug.Log("Tried to sacrifice with result= " + returnsFromRPC);
        if (gm1 != null)
        {
            gm1.updateLayout();
        }
    }

    internal string ChangeMode(int changingModeId, int sendZeroForFaceUpOrOneForFaceDown, int indexOfCardOnField)
    {
        GetComponent<PhotonView>().RPC("ChangeMode_RPC", PhotonTargets.All, changingModeId, sendZeroForFaceUpOrOneForFaceDown, indexOfCardOnField);
        while (returnsFromRPC == "-1")
        {

        }
        string toReturn = returnsFromRPC;
        returnsFromRPC = "-1";
        return toReturn;
    }

    [PunRPC]
    void ChangeMode_RPC(int changingModeId, int sendZeroForFaceUpOrOneForFaceDown, int cardIndexOnField)
    {
        Debug.Log("ID " + changingModeId + " is trying to change mode of card "+cardIndexOnField);
        if (p1.id == changingModeId)
        {
            if (sendZeroForFaceUpOrOneForFaceDown == 0)
            {
                Debug.Log("Player 1 changes mode face up card number " + cardIndexOnField);
                returnsFromRPC = p1.ChangeModeOfCard(p1.MeReadOnly.FaceUpMonsters[cardIndexOnField]);
            }
            else if(sendZeroForFaceUpOrOneForFaceDown==1)
            {
                Debug.Log("Player 1 changes mode of face down card number" + cardIndexOnField);
                returnsFromRPC = p1.ChangeModeOfCard(p1.FaceDownCardsInMonsterZone[cardIndexOnField]);
            }
        }
        if (p2.id == changingModeId)
        {
            if (sendZeroForFaceUpOrOneForFaceDown == 0)
            {
                Debug.Log("Player 2 changes mode face up card number " + cardIndexOnField);
                returnsFromRPC = p2.ChangeModeOfCard(p2.MeReadOnly.FaceUpMonsters[cardIndexOnField]);
            }
            else if (sendZeroForFaceUpOrOneForFaceDown == 1)
            {
                Debug.Log("Player 2 changes mode of face down card number" + cardIndexOnField);
                returnsFromRPC = p2.ChangeModeOfCard(p2.FaceDownCardsInMonsterZone[cardIndexOnField]);
            }
        }
        Debug.Log("Tried to normal summon with result= " + returnsFromRPC);
        if (gm1 != null)
        {
            gm1.updateLayout();
        }
    }

    internal string Equip(string cardToEquipToName, int indexInHandOfEquipableCard, int equippingId)
    {
        GetComponent<PhotonView>().RPC("Equip_RPC", PhotonTargets.All, equippingId, indexInHandOfEquipableCard, cardToEquipToName);
        while (returnsFromRPC == "-1")
        {

        }
        string toReturn = returnsFromRPC;
        returnsFromRPC = "-1";
        return toReturn;
    }

    [PunRPC]
    void Equip_RPC(int equipingId, int indexInHandOfEquipableCard, string cardToEquipTo)
    {
        Debug.Log("ID " + equipingId + " is trying to equip.");
        if (p1.id == equipingId)
        {
            Debug.Log("Player 1 equips");
            returnsFromRPC = p1.TryEquip(p1.Hand[indexInHandOfEquipableCard], cardToEquipTo);
        }
        else if (p2.id == equipingId)
        {
            Debug.Log("Player 2 equips");
            returnsFromRPC = p2.TryEquip(p1.Hand[indexInHandOfEquipableCard], cardToEquipTo);
        }
        Debug.Log("Tried to equip with result= " + returnsFromRPC);
        if (gm1 != null)
        {
            gm1.updateLayout();
        }
    }


    internal string EndTurn(int endingTurnId)
    {
        GetComponent<PhotonView>().RPC("EndTurn_RPC", PhotonTargets.All, endingTurnId);
        while (returnsFromRPC == "-1")
        {

        }
        string toReturn = returnsFromRPC;
        returnsFromRPC = "-1";
        return toReturn;
    }

    [PunRPC]
    void EndTurn_RPC(int endingTurnId)
    {
        Debug.Log("ID " + endingTurnId + " is trying to end turn.");
        if (p1.id == endingTurnId)
        {
            Debug.Log("Player 1 ends turn");
            returnsFromRPC = p1.EndTurn();
        }
        else if (p2.id == endingTurnId)
        {
            Debug.Log("Player 2 ends turn");
            returnsFromRPC = p2.EndTurn();
        }
        Debug.Log("Tried to end turn with result= " + returnsFromRPC);
        if (gm1 != null)
        {
            gm1.updateLayout();
        }
    }

    internal string Summon(int cardIndexInHand, int summoningId)
    {
        GetComponent<PhotonView>().RPC("Summon_RPC", PhotonTargets.All, cardIndexInHand, summoningId);
        while (returnsFromRPC == "-1")
        {

        }
        string toReturn = returnsFromRPC;
        returnsFromRPC = "-1";
        return toReturn;
    }

    [PunRPC]
    void Summon_RPC(int cardIndexInHand, int summoningId)
    {
        Debug.Log("ID " + summoningId + " is trying to summon");
        if (p1.id == summoningId)
        {
            Debug.Log("Player 1 summons card number " + cardIndexInHand);
            returnsFromRPC = p1.NormalSummon(p1.Hand[cardIndexInHand]);
        }
        else if (p2.id == summoningId)
        {
            Debug.Log("Player 2 summons card number " + cardIndexInHand);
            returnsFromRPC = p2.NormalSummon(p2.Hand[cardIndexInHand]);
        }
        Debug.Log("Tried to normal summon with result= " + returnsFromRPC);
        if (gm1 != null)
        {
            gm1.updateLayout();
        }
    }

    [PunRPC]
    void SendGame_RPC(int randomGameSeed)
    {
        try
        {
            if (PhotonNetwork.isMasterClient && !gameCreated)
            {
                try
                {
                    Debug.Log("More than two players. Building game.");
                    //Here is where code should go to build personal Decks. For now we make a random deck (first 40 cards in database).
                    //Both players will use the same deck for this test app.
                    MainDeckBuilder mdb = new MainDeckBuilder();
                    List<Assets.Scripts.BattleHandler.Cards.Card> randomDeck = mdb.getRandomDeck();
                    Debug.Log("Loaded Random Decks.");

                    //Initialize Card Back
                    CardBackTexture = mdb.getCardBack() as Texture;
                    Debug.Log("Loaded CardBack Texture.");

                    //Build the players.
                    gameSeed = randomGameSeed;

                    GameObject p1Object = Instantiate((Resources.Load("Player") as UnityEngine.Object)) as GameObject;
                    GameObject p2Object = Instantiate((Resources.Load("Player") as UnityEngine.Object)) as GameObject;
                    p1 = Player.MakePlayer(p1Object, PhotonNetwork.masterClient.ID, PhotonNetwork.masterClient.name);
                    foreach (PhotonPlayer p in PhotonNetwork.playerList)
                    {
                        if (p != PhotonNetwork.masterClient)
                        {
                            p2 = Player.MakePlayer(p2Object, p.ID, p.name);
                        }
                    }

                    Debug.Log("Instantiated 2 players on the network and assigned their names/ids");

                    //Now the network manager gives a handle to the users and to the game.
                    Debug.Log("Setting p1=" + p1.id + " p2=" + p2.id+" and game seed to "+gameSeed);
                    g = new Game(p1, p2, gameSeed);
                    g.RequestSetPlayer1Deck(randomDeck);
                    g.RequestSetPlayer2Deck(randomDeck);
                    g.setCurrentGame();
                    g.StartGame();
                    //PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "game", g } });
                    //GetComponent<PhotonView>().RPC("UpdateGame_RPC", PhotonTargets.All);
                    Debug.Log("Instantiated a game on the network and started it.");
                    gameCreated = true;
                }
                catch (Exception e)
                {
                    Debug.Log("Failed to create game: " + e.Message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }


    void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        SpawnMyPlayer();
        connecting = false;
        if (PhotonNetwork.playerList.Length == 2)
        {
            try
            {
                Debug.Log("More than two players. Building game.");
                //Here is where code should go to build personal Decks. For now we make a random deck (first 40 cards in database).
                //Both players will use the same deck for this test app.
                MainDeckBuilder mdb = new MainDeckBuilder();
                List<Assets.Scripts.BattleHandler.Cards.Card> randomDeck = mdb.getRandomDeck();
                Debug.Log("Loaded Random Decks.");

                //Initialize Card Back
                CardBackTexture = mdb.getCardBack() as Texture;
                Debug.Log("Loaded CardBack Texture.");

                //Build the players.
                int randomGameId = rand.Next();
                Debug.Log("Assigning gameId=" + randomGameId);

                GameObject p1Object = Instantiate((Resources.Load("Player") as UnityEngine.Object)) as GameObject;
                GameObject p2Object = Instantiate((Resources.Load("Player") as UnityEngine.Object)) as GameObject;
                p1 = Player.MakePlayer(p1Object, PhotonNetwork.masterClient.ID, PhotonNetwork.masterClient.name);
                foreach (PhotonPlayer p in PhotonNetwork.playerList)
                {
                    if (p != PhotonNetwork.masterClient)
                    {
                        p2 = Player.MakePlayer(p2Object, p.ID, p.name);
                    }
                }
                Debug.Log("Instantiated 2 players on the network and assigned their names/ids");

                //Now the network manager gives a handle to the users and to the game.
                g = new Game(p1, p2, randomGameId);
                Debug.Log("Setting p1=" + p1.id + " p2=" + p2.id);
                g.RequestSetPlayer1Deck(randomDeck);
                g.RequestSetPlayer2Deck(randomDeck);
                g.setCurrentGame();
                g.StartGame();
                Debug.Log("Instantiated a game on the network and started it.");
                gameCreated = true;
                GetComponent<PhotonView>().RPC("SendGame_RPC", PhotonTargets.All, randomGameId);
                Debug.Log("Sent other client id the game seed=" + randomGameId);
            }
            catch (Exception e)
            {
                Debug.Log("Failed to create game: " + e.Message);
            }
        }
    }

    void SpawnMyPlayer()
    {
        AddChatMessage(PhotonNetwork.player.name + " has joined.");
        wasSignedIn = true;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Return Hit " + returnCounter);
                returnCounter++;
                if (!chatWindowUp)
                {
                    Debug.Log("Chat Window Was down so Now it is up.");
                    chatWindowUp = true;
                    Event.current.keyCode = KeyCode.Dollar;
                }
                else if (chatWindowUp)
                {
                    chatWindowUp = false;
                }
            }

            if (!hasMadeGameManager)
            {
                if (g != null)
                {
                    GameObject field = Resources.Load("DuelingField") as GameObject;
                    Instantiate(field);
                    Debug.Log("Found Game GameObject, Loading game manager");
                    GameObject gmPrefab = Resources.Load("GameManagerPrefab") as GameObject;
                    GameObject gObject = Instantiate(gmPrefab);
                    GameManager gm = GameManager.MakeManager(gObject, g.myPlayer(PhotonNetwork.player.ID), CardBackTexture, this);
                    gm1 = gm;
                    Instantiate(gObject);
                    hasMadeGameManager = true;
                    GameObject[] playersGOs = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject gos in playersGOs)
                    {
                        if (gos.GetComponent<Player>() == g.myPlayer(PhotonNetwork.player.ID))
                        {
                            GameObject spawn = GameObject.Find("Player1");
                            gos.transform.position = spawn.transform.position;
                            gos.transform.parent = spawn.transform;
                        }
                        else
                        {
                            GameObject spawn = GameObject.Find("Player2");
                            gos.transform.position = spawn.transform.position;
                            gos.transform.parent = spawn.transform;
                        }
                        gos.AddComponent<Lifepoints>().setGameManager(gm);
                    }
                    Debug.Log("Created GameManager.Ready to Rock and Roll on the Network.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }
}
