using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class bl_PhotonConnection : Photon.MonoBehaviour
{

    private LobbyState m_state = LobbyState.PlayerName;
    private string playerName;
    private string hostName; //Name of room
    [Header("Photon")]
    public string AppVersion = "1.0";
    [SerializeField]private string RoomNamePrefix = "LovattoRoom {0}";
    [SerializeField]private string PlayerNamePrefix = "Guest {0}";
    public float UpdateServerListEach = 2;
    public bool ShowPhotonStatus;
    public bool ShowPhotonStatistics;

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
    [SerializeField]private GameObject FadeImage;
    public Text PhotonStatusText = null;
    public Text PlayerNameText = null;
    public GameObject RoomInfoPrefab;
    [SerializeField]private GameObject PhotonStaticticsUI;
    public Transform RoomListPanel;
    public CanvasGroup CanvasGroupRoot = null;
    public Text MaxPlayerText = null;
    public Text RoundTimeText = null;
    public Text GameModeText = null;
    public Text MapNameText = null;
    public Text AntiStrpicText = null;
    public Text QualityText = null;
    public Text VolumenText = null;
    public Text SensitivityText = null;
    [SerializeField]private Text NoRoomText;
    public Image MapPreviewImage = null;
    public InputField PlayerNameField = null;
    public InputField RoomNameField = null;
    //OPTIONS
    private int m_currentQuality = 3;
    private float m_volume = 1.0f;
    private float m_sensitive = 15;
    private string[] m_stropicOptions = new string[] { "Disable", "Enable", "Force Enable" };
    private int m_stropic = 0;
    private bool GamePerRounds = false;
    private bool AutoTeamSelection = false;

    [Header("Effects")]
    public AudioClip a_Click;
    private float alpha = 2.0f;
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
        PhotonNetwork.autoJoinLobby = true;
        hostName = string.Format(RoomNamePrefix, Random.Range(10, 999));
        RoomNameField.text = hostName;
        GameModes = Enum.GetNames(typeof(GameMode));

        if (string.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            // generate a name for this player, if none is assigned yet
            if (String.IsNullOrEmpty(playerName))
            {
                playerName = string.Format(PlayerNamePrefix, Random.Range(1, 9999));
                PlayerNameField.text = playerName;
            }
            ChangeWindow(0);
        }
        else
        {
            StartCoroutine(Fade(LobbyState.MainMenu, 1.2f));
            if (!PhotonNetwork.connected)
            {
                ConnectPhoton();
            }
            ChangeWindow(2, 1);
        }
        SetUpOptionsHost();
        InvokeRepeating("UpdateServerList", 1, UpdateServerListEach);
        GetPrefabs();
        FadeImage.SetActive(false);
        if(PhotonStaticticsUI != null) { PhotonStaticticsUI.SetActive(ShowPhotonStatistics); }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t_state"></param>
    /// <returns></returns>
    IEnumerator Fade(LobbyState t_state, float t = 2.0f)
    {
        alpha = 0.0f;
        m_state = t_state;
        while (alpha < t)
        {
            alpha += Time.deltaTime;
            CanvasGroupRoot.alpha = alpha;
            yield return null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void ConnectPhoton()
    {
        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (!PhotonNetwork.connected || PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated)
        {
            PhotonNetwork.AuthValues = null;
            PhotonNetwork.ConnectUsingSettings(AppVersion);
            ChangeWindow(3);
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
                PlayerNameText.text = "<b><color=orange>PLAYER:</color>  " + PhotonNetwork.player.name + "</b>";
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
            NoRoomText.text = "THERE ARE NOT ROOMS CREATED YET, CREATE ONE TO PLAY.";
        }
    }

    /// <summary>
    /// Menu For Enter Name for UI 4.6 WIP
    /// </summary>
    public void EnterName(InputField field = null)
    {

        if (field == null || string.IsNullOrEmpty(field.text))
            return;

        playerName = field.text;
        playerName = playerName.Replace("\n", "");

        PlayAudioClip(a_Click, transform.position, 1.0f);
        StartCoroutine(Fade(LobbyState.MainMenu));

        PhotonNetwork.playerName = playerName;
        ConnectPhoton();

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
        StartCoroutine(Fade(LobbyState.MainMenu, 3f));
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
        if (a_Click != null)
        {
            AudioSource.PlayClipAtPoint(a_Click, this.transform.position, 1.0f);
        }
    }

    public void Disconect()
    {
        if (PhotonNetwork.connected)
        { PhotonNetwork.Disconnect(); }
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
            StartCoroutine(Fade(LobbyState.MainMenu, 3.2f));
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
        FadeImage.SetActive(true);
        FadeImage.GetComponent<Animator>().speed = 2;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    /// <returns></returns>
	AudioSource PlayAudioClip(AudioClip clip, Vector3 position, float volume)
    {
        GameObject go = new GameObject("One shot audio");
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.Play();
        Destroy(go, clip.length);
        return source;
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
        StartCoroutine(Fade(LobbyState.MainMenu));
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
        FadeImage.SetActive(false);
        Debug.LogWarning("OnFailedToConnectToPhoton: " + cause);
    }
    void OnConnectionFail(DisconnectCause cause)
    {
        FadeImage.SetActive(false);
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

    private int GetButtonSize(LobbyState t_state)
    {

        if (m_state == t_state)
        {
            return 55;
        }
        else
        {
            return 40;
        }
    }

    [System.Serializable]
    public enum LobbyType
    {
        UGUI,
        OnGUI,
    }
}