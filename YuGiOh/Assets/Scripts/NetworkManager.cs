using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Scripts.BattleHandler.Game;

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
        PhotonNetwork.CreateRoom(null,roomOptions,null);
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
                    
                    GameObject p1Object = Instantiate((Resources.Load("Player") as UnityEngine.Object), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    GameObject p2Object = Instantiate((Resources.Load("Player") as UnityEngine.Object), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    p1 = Player.MakePlayer(p1Object, PhotonNetwork.masterClient.ID, PhotonNetwork.masterClient.name);
                    foreach(PhotonPlayer p in PhotonNetwork.playerList)
                    {
                        if(p != PhotonNetwork.masterClient)
                        {
                            p2 = Player.MakePlayer(p2Object, p.ID, p.name);
                        }
                    }
                    
                    Debug.Log("Instantiated 2 players on the network and assigned their names/ids");

                    //Now the network manager gives a handle to the users and to the game.
                    Debug.Log("Setting p1=" + p1.id + " p2=" + p2.id);
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

                GameObject p1Object = Instantiate((Resources.Load("Player") as UnityEngine.Object), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                GameObject p2Object = Instantiate((Resources.Load("Player") as UnityEngine.Object), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
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
                    GameObject gObject = Resources.Load("GameManagerPrefab") as GameObject;
                    GameManager gm = GameManager.MakeManager(gObject, g.myPlayer(PhotonNetwork.player.ID), CardBackTexture, this);
                    Instantiate(gObject);
                    hasMadeGameManager = true;
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
