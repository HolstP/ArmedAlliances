using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class PhotonConnection : Photon.MonoBehaviour
{

    private LobbyState m_state = LobbyState.PlayerName;
    public string email;
    private string playerName;
    private string clanName;
    private string hostName; //Name of room
    [Header("Photon")]
    public string AppVersion = "Dev 0.0.2.0";
    [SerializeField]private string RoomNamePrefix = "Room {0}";
    public float UpdateServerListEach = 2;
    public bool ShowPhotonStatus;
    public bool ShowPhotonStatistics;

    [Header("Login")]
    public InputField emailField;
    public InputField passwordField;
    public Text errorMsg;
    public Text versionText;
    public string myUserID;
    public string LoginUrl = "http://varygames.com/armedalliances/login.php";
    public string GetUserIDURL = "http://www.varygames.com/armedalliances/getuserid.php";
    public string CheckUserURL = "http://www.varygames.com/armedalliances/checkuser.php";
    public string GetCharacterDataURL = "http://www.varygames.com/armedalliances/characterinfo.php";
    public string[] userid;

    [Header("Connecting")]
    public Text connectingText;

    [Header("User Info")]
    public Text PlayerNameText = null;
    public Text clanNameText = null;
    public string[] characterinfo;

    [Header("Room Options")]
    //Max players in game
    public int[] maxPlayers;
    private int players;
    //Room Time in seconds
    public int[] RoomTime;
    private int r_Time;

    private string[] GameModes;
    private int CurrentGameMode = 0;

    [Header("Refrences")]
    public List<GameObject> MenusUI = new List<GameObject>();
    public Text PhotonStatusText = null;
    public GameObject RoomInfoPrefab;
    [SerializeField]private GameObject PhotonStaticticsUI;
    public Transform RoomListPanel;
    public CanvasGroup CanvasGroupRoot = null;
    public Text MaxPlayerText = null;
    public Text RoundTimeText = null;
    public Text GameModeText = null;
    [SerializeField]private Text NoRoomText;

    [Header("Room Creation")]
    public Image MapPreviewImage = null;
    public InputField RoomNameField = null;
    public Text MapNameText = null;

    [Header("Settings Menu")]
    public Text AntiStrpicText = null;
    public Text QualityText = null;
    public Text VolumenText = null;
    public Text SensitivityText = null;
    private int m_currentQuality = 3;
    private float m_volume = 1.0f;
    private float m_sensitive = 15;
    private string[] m_stropicOptions = new string[] { "Disable", "Enable", "Force Enable" };
    private int m_stropic = 0;
    private bool GamePerRounds = false;
    private bool AutoTeamSelection = false;

    [Serializable]
    public class AllScenes
    {
        public string m_name;
        public string m_SceneName;
        public Sprite m_Preview;
    }
    [Header("Levels Manager")]
    public List<AllScenes> m_scenes = new List<AllScenes>();
    private List<GameObject> CacheRoomList = new List<GameObject>();
    private int CurrentScene = 0;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {

        errorMsg.text = "";
        email = "";
        myUserID = "";
        //characternameField.text = "";
        //charCreationErrorMsg.text = "";

        versionText.text = AppVersion;

        string path = "test.txt";

        StreamReader reader = new StreamReader(path);
        emailField.text = reader.ReadToEnd();
        reader.Close();

        PhotonNetwork.autoJoinLobby = true;
        hostName = string.Format(RoomNamePrefix, Random.Range(10, 999));
        RoomNameField.text = hostName;
        GameModes = Enum.GetNames(typeof(GameMode));

        if (string.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            // generate a name for this player, if none is assigned yet
            if (String.IsNullOrEmpty(playerName))
            {
                Debug.Log("Playername is empty!");
            }
            ChangeWindow(0);
        }
        else
        {
            if (!PhotonNetwork.connected)
            {
                ConnectPhoton();
            }
            ChangeWindow(2, 1);
        }
        SetUpOptionsHost();
        InvokeRepeating("UpdateServerList", 1, UpdateServerListEach);
        GetPrefabs();
        if(PhotonStaticticsUI != null) { PhotonStaticticsUI.SetActive(ShowPhotonStatistics); }
    }

    void Update()
    {

        /*if (Input.GetKeyDown(KeyCode.Return))
        {
            if (MenusUI[0])
            {
                LogIn();
            }
            /*else if (characterCreationScreen.gameObject.activeSelf == true)
            {
                CreateCharacter();
            }
        }

        if (MenusUI.FindIndex)
        {


            if (Input.GetKeyDown(KeyCode.Tab) && emailField.isFocused == true)
            {
                EventSystem.current.SetSelectedGameObject(passwordField.gameObject, null);
                passwordField.OnPointerClick(new PointerEventData(EventSystem.current));
            }
            else if (Input.GetKeyDown(KeyCode.Tab) && passwordField.isFocused == true)
            {
                EventSystem.current.SetSelectedGameObject(emailField.gameObject, null);
                emailField.OnPointerClick(new PointerEventData(EventSystem.current));
            }
        }*/
    }

    public void LogIn()
    {
        errorMsg.text = "";
        email = emailField.text;

        if (emailField.text == "" || passwordField.text == "")
            errorMsg.text = "Please fill out both Email and Password";

        else
        {
            // Starts login proces
            /*if (rememberMe.isOn)
            {
                //Save E-mail in .txt document
                File.Delete("test.txt");
                Debug.Log("Saving user E-Mail");
                string path = "test.txt";

                StreamWriter writer = new StreamWriter(path, true);
                writer.WriteLine(email);
                writer.Close();
            }*/
            Debug.Log("Login started.");
            WWWForm form = new WWWForm();
            form.AddField("email", emailField.text);
            form.AddField("password", passwordField.text);
            form.AddField("clientVersion", AppVersion);
            WWW w = new WWW(LoginUrl, form);
            Debug.Log("URL: " + LoginUrl);
            ChangeWindow(3);
            connectingText.text = "Logging in...";
            StartCoroutine(LogIn(w));
        }
    }

    public IEnumerator LogIn(WWW _w)
    {
        yield return _w;

        Debug.Log("Checking game version and login information.");

        connectingText.text = "Checking login credentials.";

        if (_w.text == "Please fill out Email.")
        {
            ChangeWindow(0);
            errorMsg.text = "Please fill out Email.";
            Debug.Log("Email field is blank.");
        }
        else if (_w.text == "Please fill out Password.")
        {
            ChangeWindow(0);
            errorMsg.text = "Please fill out Password.";
            Debug.Log("Password field is blank.");
        }
        else if (_w.text == "Outdated")
        {
            ChangeWindow(0);
            errorMsg.text = "Game version is outdated.";
            Debug.Log("Game version is outdated.");
        }
        else if (_w.text == "Incorrect Email or Password.")
        {
            ChangeWindow(0);
            errorMsg.text = "Email or password is incorrect.";
            Debug.Log("Incorrect Email or Password.");
        }
        else if (_w.text == "Log in successful!")
        {
            connectingText.text = "Logged in.";
            Debug.Log("User logged in.");
            StartCoroutine(GetUserID());
        }
        else
        {
            ChangeWindow(0);
            Debug.Log("ERROR:" + _w.error);
            errorMsg.text = "Error connecting.";
        }
    }

    private IEnumerator GetUserID()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", emailField.text);
        WWW userID = new WWW(GetUserIDURL, form);
        yield return userID;

        string userIDString = userID.text;
        userid = userIDString.Split(';');
        myUserID = (GetUserID(userid[0], "User_ID"));
        Debug.Log("UserID fetched UserID is: " + myUserID);
        StartCoroutine(CheckUser());
    }

    string GetUserID(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("|")) value = value.Remove(value.IndexOf("|"));
        return value;
    }

    private IEnumerator CheckUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", myUserID);
        WWW checkUser = new WWW(CheckUserURL, form);
        yield return checkUser;
        if (checkUser.text == "Player has 0 characters.")
        {
            //ChangeWindow to character Creation
            Debug.Log("No characters found, redirecting user to character creation.");
        }
        else if (checkUser.text == "Player has 1 characters.")
        {
            StartCoroutine(GetCharacterInfo());
            Debug.Log("One character found.");
        }
        else
        {
            Debug.Log("An error occured.");
        }
    }

    string CheckUserAcc(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("|")) value = value.Remove(value.IndexOf("|"));
        return value;
    }

    private IEnumerator GetCharacterInfo()
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", myUserID);
        WWW characterInfo = new WWW(GetCharacterDataURL, form);
        yield return characterInfo;
        if (characterInfo.text == "No character found.") // If character name is not found. Redirect to character creation screen.
        {
            Debug.Log("User was not found.");
        }
        else
        {
            //Change to lobby Screen
            string characterInfoString = characterInfo.text;
            characterinfo = characterInfoString.Split(';');
            playerName = (GetCharacterInfo(characterinfo[0], "Charactername"));
            clanName = (GetCharacterInfo(characterinfo[0], "Clan"));
            //level.text = (GetCharacterInfo(characterinfo[0], "Level"));
            ConnectPhoton();
            PhotonNetwork.playerName = playerName;
            Debug.Log("Users info succesfully retrived.");
        }
    }


    string GetCharacterInfo(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("|")) value = value.Remove(value.IndexOf("|"));
        return value;
    }

    void ConnectPhoton()
    {
        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (!PhotonNetwork.connected || PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated)
        {
            connectingText.text = "Connecting To Game Server.";
            PhotonNetwork.AuthValues = null;
            PhotonNetwork.ConnectUsingSettings(AppVersion);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void UpdateServerList()
    {
        ServerList();
    }

    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        if (PhotonNetwork.connected)
        {
            if (ShowPhotonStatus)
            {
                PhotonStatusText.text = "<b><color=orange>STATUS:</color>  " + PhotonNetwork.connectionStateDetailed.ToString().ToUpper() + "</b>";
                PlayerNameText.text = PhotonNetwork.player.name;
                clanNameText.text = clanName;
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public void ServerList()
    {
        //Removed old list
        if (CacheRoomList.Count > 0)
        {
            foreach (GameObject g in CacheRoomList)
            {
                Destroy(g);
            }
            CacheRoomList.Clear();
        }
        //Update List
        RoomInfo[] ri = PhotonNetwork.GetRoomList();
        if (ri.Length > 0)
        {
            NoRoomText.text = string.Empty;
            for (int i = 0; i < ri.Length; i++)
            {
                GameObject r = Instantiate(RoomInfoPrefab) as GameObject;
                CacheRoomList.Add(r);
                r.GetComponent<bl_RoomInfo>().GetInfo(ri[i]);
                r.transform.SetParent(RoomListPanel, false);
            }
           
        }
        else
        {
            NoRoomText.text = "No Rooms Found.";
        }
    }

    #region UGUI
    /// <summary>
    /// For button can call this 
    /// </summary>
    /// <param name="id"></param>
    public void ChangeWindow(int id)
    {
        ChangeWindow(id, -1);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public void ChangeWindow(int id, int id2)
    {
        for (int i = 0; i < MenusUI.Count; i++)
        {
            if (i == id || i == id2)
            {
                MenusUI[i].SetActive(true);
            }
            else
            {
                if (i != 1)//1 = mainmenu buttons
                {
                    MenusUI[i].SetActive(false);
                }
                if (id == 6 || id == 8)
                {
                    MenusUI[1].SetActive(false);
                }
            }
        }
    }

    public void Disconect()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    public void LoadLocalLevel(string level)
    {
        if (PhotonNetwork.connected) { PhotonNetwork.Disconnect(); }
        bl_UtilityHelper.LoadLevel(level);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="plus"></param>
    public void ChangeMaxPlayer(bool plus)
    {
        if (plus)
        {
            if (players < maxPlayers.Length)
            {
                players++;
                if (players > (maxPlayers.Length - 1)) players = 0;

            }
        }
        else
        {
            if (players < maxPlayers.Length)
            {
                players--;
                if (players < 0) players = maxPlayers.Length - 1;
            }
        }
        MaxPlayerText.text = maxPlayers[players] + " Players";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="plus"></param>
    public void ChangeRoundTime(bool plus)
    {
        if (!plus)
        {
            if (r_Time < RoomTime.Length)
            {
                r_Time--;
                if (r_Time < 0)
                {
                    r_Time = RoomTime.Length - 1;

                }
            }
        }
        else
        {
            if (r_Time < RoomTime.Length)
            {
                r_Time++;
                if (r_Time > (RoomTime.Length - 1))
                {
                    r_Time = 0;

                }

            }
        }
        RoundTimeText.text = (RoomTime[r_Time] / 60) + " Minutes";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="plus"></param>
    public void ChangeGameMode(bool plus)
    {
        if (plus)
        {
            if (CurrentGameMode < GameModes.Length)
            {
                CurrentGameMode++;
                if (CurrentGameMode > (GameModes.Length - 1))
                {
                    CurrentGameMode = 0;
                }
            }
        }
        else
        {
            if (CurrentGameMode < GameModes.Length)
            {
                CurrentGameMode--;
                if (CurrentGameMode < 0)
                {
                    CurrentGameMode = GameModes.Length - 1;

                }
            }
        }
        GameModeText.text = GameModes[CurrentGameMode];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="plus"></param>
    public void ChangeMap(bool plus)
    {
        if (!plus)
        {
            if (CurrentScene < m_scenes.Count)
            {
                CurrentScene--;
                if (CurrentScene < 0)
                {
                    CurrentScene = m_scenes.Count - 1;

                }
            }
        }
        else
        {
            if (CurrentScene < m_scenes.Count)
            {
                CurrentScene++;
                if (CurrentScene > (m_scenes.Count - 1))
                {
                    CurrentScene = 0;
                }
            }
        }
        MapNameText.text = m_scenes[CurrentScene].m_name;
        MapPreviewImage.sprite = m_scenes[CurrentScene].m_Preview;
    }

    public void ChangeAntiStropic(bool plus)
    {
        if (!plus)
        {
            if (m_stropic < m_stropicOptions.Length)
            {
                m_stropic--;
                if (m_stropic < 0)
                {
                    m_stropic = m_stropicOptions.Length - 1;

                }
            }
        }
        else
        {
            if (m_stropic < m_stropicOptions.Length)
            {
                m_stropic++;
                if (m_stropic > (m_stropicOptions.Length - 1))
                {
                    m_stropic = 0;
                }
            }
        }
        AntiStrpicText.text = m_stropicOptions[m_stropic];
    }

    public void ChangeQuality(bool plus)
    {
        if (!plus)
        {
            if (m_currentQuality < QualitySettings.names.Length)
            {
                m_currentQuality--;
                if (m_currentQuality < 0)
                {
                    m_currentQuality = QualitySettings.names.Length - 1;

                }
            }
        }
        else
        {
            if (m_currentQuality < QualitySettings.names.Length)
            {
                m_currentQuality++;
                if (m_currentQuality > (QualitySettings.names.Length - 1))
                {
                    m_currentQuality = 0;
                }
            }
        }
        QualityText.text = QualitySettings.names[m_currentQuality];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void QuitGame(bool b)
    {
        if (b)
        {
            Application.Quit();
            Debug.Log("Game Exit, this only work in standalone version");
        }
        else
        {
            ChangeWindow(2, 1);
        }
    }

    public void ChangeAutoTeamSelection(bool b) { AutoTeamSelection = b; }
    public void ChangeGamePerRound(bool b) { GamePerRounds = b; }
    public void ChangeRoomName(string t) { hostName = t; }
    public void ChangeVolume(float v) { m_volume = v; VolumenText.text = (m_volume * 100).ToString("00") + "%"; }
    public void ChangeSensitivity(float s) { m_sensitive = s; SensitivityText.text = m_sensitive.ToString("00") + "%"; }
    
    /// <summary>
    /// 
    /// </summary>
    void SetUpOptionsHost()
    {
        MaxPlayerText.text = maxPlayers[players] + " Players";
        RoundTimeText.text = (RoomTime[r_Time] / 60) + " Minutes";
        GameModeText.text = GameModes[CurrentGameMode];
        MapNameText.text = m_scenes[CurrentScene].m_name;
        MapPreviewImage.sprite = m_scenes[CurrentScene].m_Preview;
        AntiStrpicText.text = m_stropicOptions[m_stropic];
        SensitivityText.text = m_sensitive.ToString("00") + "%";
        VolumenText.text = (m_volume * 100).ToString("00") + "%";
        QualityText.text = QualitySettings.names[m_currentQuality];
    }
    public void Save()
    {
        PlayerPrefs.SetFloat("volumen", m_volume);
        PlayerPrefs.SetFloat("sensitive", m_sensitive);
        PlayerPrefs.SetInt("quality", m_currentQuality);
        PlayerPrefs.SetInt("anisotropic", m_stropic);
        Debug.Log("Save Done!");
    }
    #endregion



    /// <summary>
    /// 
    /// </summary>
    public void CreateRoom()
    {
        PhotonNetwork.playerName = playerName;
        //Save Room properties for load in room
        ExitGames.Client.Photon.Hashtable roomOption = new ExitGames.Client.Photon.Hashtable();
        roomOption[PropiertiesKeys.TimeRoomKey] = RoomTime[r_Time];
        roomOption[PropiertiesKeys.GameModeKey] = GameModes[CurrentGameMode];
        roomOption[PropiertiesKeys.SceneNameKey] = m_scenes[CurrentScene].m_SceneName;
        roomOption[PropiertiesKeys.RoomRoundKey] = GamePerRounds ? "1" : "0";
        roomOption[PropiertiesKeys.TeamSelectionKey] = AutoTeamSelection ? "1" : "0";
        roomOption[PropiertiesKeys.CustomSceneName] = m_scenes[CurrentScene].m_name;

        string[] properties = new string[6];
        properties[0] = PropiertiesKeys.TimeRoomKey;
        properties[1] = PropiertiesKeys.GameModeKey;
        properties[2] = PropiertiesKeys.SceneNameKey;
        properties[3] = PropiertiesKeys.RoomRoundKey;
        properties[4] = PropiertiesKeys.TeamSelectionKey;
        properties[5] = PropiertiesKeys.CustomSceneName;

        PhotonNetwork.CreateRoom(hostName, new RoomOptions()
        {
            MaxPlayers = (byte)maxPlayers[players],
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = roomOption,
            CleanupCacheOnLeave = true,
            CustomRoomPropertiesForLobby = properties
        }, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveToGameScene()
    {
        //Wait for check
        while (PhotonNetwork.room == null)
        {
            yield return null;
        }
        PhotonNetwork.isMessageQueueRunning = false;
        bl_UtilityHelper.LoadLevel((string)PhotonNetwork.room.customProperties[PropiertiesKeys.SceneNameKey]);
    }
    // LOBBY EVENTS

    void OnJoinedLobby()
    {
        Debug.Log("We joined the lobby.");
        ChangeWindow(2, 1);

    }

    void OnLeftLobby()
    {
        Debug.Log("We left the lobby.");
    }

    // ROOMLIST

    void OnReceivedRoomList()
    {
        Debug.Log("We received a new room list, total rooms: " + PhotonNetwork.GetRoomList().Length);
    }

    void OnReceivedRoomListUpdate()
    {
        Debug.Log("We received a room list update, total rooms now: " + PhotonNetwork.GetRoomList().Length);
    }

    void OnJoinedRoom()
    {
        Debug.Log("We have joined a room.");
        StartCoroutine(MoveToGameScene());
    }
    void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogWarning("OnFailedToConnectToPhoton: " + cause);
        Debug.LogWarning("Failed To Connect To Game Server.");
    }
    void OnConnectionFail(DisconnectCause cause)
    {
        Debug.LogWarning("OnConnectionFail: " + cause);
    }

    void GetPrefabs()
    {
        if (PlayerPrefs.HasKey("volumen"))
        {
            m_volume = PlayerPrefs.GetFloat("volumen");
            AudioListener.volume = m_volume;
        }
        if (PlayerPrefs.HasKey("sensitive"))
        {
            m_sensitive = PlayerPrefs.GetFloat("sensitive");
        }
        if (PlayerPrefs.HasKey("quality"))
        {
            m_currentQuality = PlayerPrefs.GetInt("quality");
        }
        if (PlayerPrefs.HasKey("anisotropic"))
        {
            m_stropic = PlayerPrefs.GetInt("anisotropic");
        }
    }

    [System.Serializable]
    public enum LobbyType
    {
        UGUI,
        OnGUI,
    }
}