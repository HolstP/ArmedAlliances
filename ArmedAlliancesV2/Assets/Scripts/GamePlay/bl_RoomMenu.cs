/////////////////////////////////////////////////////////////////////////////////
////////////////////////////////bl_RoomMenu.cs///////////////////////////////////
/////////////////place this in a scena for handling menus of room////////////////
/////////////////////////////////////////////////////////////////////////////////
////////////////////////////////Briner Games/////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class bl_RoomMenu : bl_PhotonHelper
{
    [HideInInspector]
    public bool isPlaying = false;
    [HideInInspector]
    public float m_sensitive = 2.0f;
    [HideInInspector]
    public bool ShowWarningPing = false;
    [HideInInspector]
    public List<PhotonPlayer> m_playerlist = new List<PhotonPlayer>();
    [HideInInspector]
    public string PlayerStar = "";
    [HideInInspector]
    public bool showMenu = true;
    [HideInInspector]
    public bool isFinish = false;
    [HideInInspector]
    public bool SpectatorMode, WaitForSpectator = false;
    /// <summary>
    /// Reference of player class select
    /// </summary>
    public static PlayerClass m_playerclass = PlayerClass.Assault;

    [HideInInspector] public bool AutoTeamSelection = false;
    [Header("Global")]
    public string LeftRoomReturnScene = "MainMenu";
    [Header("Inputs")]
    public KeyCode ScoreboardKey = KeyCode.N;
    public KeyCode PauseMenuKey = KeyCode.Escape;
    public KeyCode ChangeClassKey = KeyCode.M;
    [Header("Ping Settings")]
    /// <summary>
    /// When ping is > at this, them show a message
    /// </summary>
    public int MaxPing = 500;
    /// <summary>
    /// When ping is too high show this message
    /// </summary>
    public string MsnMaxPing = "Your <color=yellow>ping is too high</color> \n <size=12>check your local coneccion.</size>";
    [Header("Map Camera")]
    /// <summary>
    /// Rotate room camera?
    /// </summary>
    public bool RotateCamera = true;
    /// <summary>
    /// Rotation Camera Speed
    /// </summary>
    public float RotSpeed = 5;
    [Space(5)]
    [Header("GUI")]
    public GUISkin SKin;
    public Texture2D WarningPing;
    public Texture2D FadeBlackTexture;
    [Range(0.0f,1.0f)]
    public float VigAlpha = 0.8f;
    public bool Use_Vignette = true;
    public static float m_alphafade = 3;
    [Header("LeftRoom")]
    [Range(0.0f,5)]
    public float DelayLeave = 1.5f;
    [Header("Others")]
    public Image VignetteImage = null;
    public GameObject ButtonsClassPlay = null;
    public Canvas m_CanvasRoot = null;
    [SerializeField]private GameObject FadeBlack;

    private bl_GameManager GM;  
    //Private
    private float m_volume = 1.0f;
    private float m_currentQuality = 2;
    private string[] m_stropicOptions = new string[] { "Disable", "Enable", "Force Enable" };
    private int m_stropic = 0;
    private int m_window;
    private Vector2 scroll_1;
    private Vector2 scroll_2;
    private bool CanSpawn = false;
    private bool AlredyAuto = false;
    private bool m_showScoreBoard = false;
    private bool m_showbuttons = false;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        if (!isConnected)
            return;

        GM = FindObjectOfType<bl_GameManager>();
        this.GetComponent<bl_ChatRoom>().AddLine("Play " + GetGameMode.ToString() + " Mode");
        ShowWarningPing = false;

        m_window = 1;
        showMenu = true;
        if (AutoTeamSelection)
        {
            StartCoroutine(CanSpawnIE());
        }
        if (VignetteImage && Use_Vignette)
        {
            VignetteImage.color = new Color(VignetteImage.color.r, VignetteImage.color.b, VignetteImage.color.g, VigAlpha);
        }
        StartCoroutine(FadeOut(1.5f));
        GetPrefabs();

    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(PauseMenuKey) && !SpectatorMode)
        {
            showMenu = true;
            m_showScoreBoard = false;
            bl_UtilityHelper.LockCursor(false);
        }
        else if (Input.GetKeyDown(PauseMenuKey) && SpectatorMode)
        {
            bl_UtilityHelper.LockCursor(false);
        }
        if (Input.GetKeyDown(ScoreboardKey) && !showMenu)
        {
                m_showScoreBoard = true;
        }
        else if(Input.GetKeyUp(ScoreboardKey) || showMenu)
        {
                m_showScoreBoard = false;
        }
        if (RotateCamera &&  !isPlaying && !SpectatorMode)
        {
            this.transform.Rotate(Vector3.up * Time.deltaTime * RotSpeed);
        }

        if (AutoTeamSelection && !AlredyAuto)
        {
            AutoTeam();
        }
        if (isPlaying && Input.GetKeyDown(ChangeClassKey) && ButtonsClassPlay != null)
        {
            m_showbuttons = !m_showbuttons;
            if (m_showbuttons)
            {
                if (!ButtonsClassPlay.activeSelf)
                {
                    ButtonsClassPlay.SetActive(true);
                    bl_UtilityHelper.LockCursor(false);
                }
            }
            else
            {
                if (ButtonsClassPlay.activeSelf)
                {
                    ButtonsClassPlay.SetActive(false);
                    bl_UtilityHelper.LockCursor(true);
                }
            }
        }
        if (bl_GameManager.isAlive && isPlaying)
        {
            if (m_CanvasRoot != null && !m_CanvasRoot.enabled)
            {
                m_CanvasRoot.enabled = true;
            }
        }
        else
        {
            if (m_CanvasRoot != null && m_CanvasRoot.enabled)
            {
                m_CanvasRoot.enabled = false;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        if (GetGameMode == GameMode.FFA)
        {
            m_playerlist.Clear();
            m_playerlist = GetPlayerList;
            if (m_playerlist.Count > 0 && m_playerlist != null)
            {
                m_playerlist.Sort(GetSortPlayerByKills);
                PlayerStar = m_playerlist[0].name;
            }
        }
    }
    /// <summary>
    /// Use for change player class for next Respawn
    /// </summary>
    /// <param name="m_class"></param>
    public void ChangeClass(int m_class)
    {
        switch (m_class)
        {
            case 0:
                m_playerclass = PlayerClass.Assault;
                break;
            case 1:
                m_playerclass = PlayerClass.Engineer;
                break;
            case 2:
                m_playerclass = PlayerClass.Recon;
                break;
            case 3:
                m_playerclass = PlayerClass.Support;
                break;
        }
        ButtonsClassPlay.SetActive(false);
        bl_UtilityHelper.LockCursor(true);
        m_showbuttons = false;
    }

    /// <summary>
    /// 
    /// </summary>
    void AutoTeam()
    {
        if (CanSpawn && !isPlaying && !AlredyAuto)
        {
            AlredyAuto = true;
            if (GetGameMode != GameMode.FFA)
            {
                if (GetPlayerInDeltaCount > GetPlayerInReconCount)
                {
                    bl_UtilityHelper.LockCursor(true);
                    showMenu = false;
                    GM.SpawnPlayer(Team.Recon);
                    bl_EventHandler.KillEvent(PhotonNetwork.player.name, "", "Joined in Recon", Team.Recon.ToString(), 777, 30);
                    isPlaying = true;
                }
                else if (GetPlayerInDeltaCount < GetPlayerInReconCount)
                {
                    bl_UtilityHelper.LockCursor(true);
                    showMenu = false;
                    GM.SpawnPlayer(Team.Delta);
                    bl_EventHandler.KillEvent(PhotonNetwork.player.name, "", "Joined in Delta", Team.Delta.ToString(), 777, 30);
                    isPlaying = true;
                }
                else if (GetPlayerInDeltaCount == GetPlayerInReconCount)
                {
                    bl_UtilityHelper.LockCursor(true);
                    showMenu = false;
                    GM.SpawnPlayer(Team.Delta);
                    bl_EventHandler.KillEvent(PhotonNetwork.player.name, "", "Joined in Delta", Team.Delta.ToString(), 777, 30);
                    isPlaying = true;
                }
            }                  
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnGUI()
    {
        GUI.skin = SKin;
        GUI.depth = 100;
        if (!RequestLeft)
        {
            if (showMenu && !SpectatorMode)
            {
                MainMenu();
            }
            if (ShowWarningPing && WarningPing != null)
            {
                GUI.color = new Color(1, 1, 1, 0.8f);
                GUI.DrawTexture(new Rect(Screen.width / 2 - 37, Screen.height - 76, 75, 75), WarningPing);
                GUI.Label(new Rect(Screen.width / 2 + 38, Screen.height - 43, 200, 60), MsnMaxPing);
                GUI.color = Color.white;
            }

        }
        if (FadeBlackTexture != null)
        {
            if (m_alphafade > 0.0f)
            {
                GUI.color = new Color(1, 1, 1, m_alphafade);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeBlackTexture);
            }
            GUI.color = Color.white;
        }
        if (!RequestLeft)
        {
            if (AutoTeamSelection && !isPlaying && !AlredyAuto && GetGameMode != GameMode.FFA)
            {
                bl_UtilityHelper.ShadowLabel(new Rect(Screen.width / 2 - 75, Screen.height / 2 + 50, 200, 30), "Wait For Select Team...");
            }
            OnlyScoreBoard();

            if (!isPlaying && !SpectatorMode)
            {
                if (GUI.Button(new Rect(0, 5, 170, 40), "Spectator"))
                {
                    this.GetComponentInChildren<bl_SpectatorCamera>().enabled = true;
                    this.GetComponentInChildren<bl_MouseLook>().enabled = true;
                    bl_UtilityHelper.LockCursor(true);
                    SpectatorMode = true;
                }
            }
            else if (!isPlaying && SpectatorMode)
            {
                if (GUI.Button(new Rect(0, 5, 170, 40), "Leave Spectator"))
                {
                    this.GetComponentInChildren<bl_SpectatorCamera>().enabled = false;
                    this.GetComponentInChildren<bl_MouseLook>().enabled = false;
                    this.GetComponentInChildren<Camera>().transform.localPosition = Vector3.zero;
                    this.GetComponentInChildren<Camera>().transform.rotation = Quaternion.identity;
                    bl_UtilityHelper.LockCursor(false);
                    SpectatorMode = false;
                }
                GUI.Box(new Rect(0, 50, 170, 40), "Total Players: " + PhotonNetwork.playerList.Length);
                GUI.Box(new Rect(0, 95, 170, 40), "Master: " + PhotonNetwork.masterClient.name);
                GUI.Label(new Rect(0, Screen.height - 25, 400, 25), "Move: A,W,D,S Look: Mouse Axis Up: 'E' Down: 'Q'");
            }
            //When die and go to spectator mode.
            if (WaitForSpectator)
            {
                if (GUI.Button(new Rect(150, 5, 150, 40), "Spawn Now"))
                {
                    this.GetComponent<bl_GameManager>().SpawnPlayer(GetTeamEnum(PhotonNetwork.player));
                    WaitForSpectator = false;
                }
            }
        }
        else
        {
            GUI.Box(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 15, 250, 30), "Returning to lobby...");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void LeftButton()
    {
        if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 200, 150, 30), "Left of Room"))
        {
            LeftOfRoom();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void LeftOfRoom()
    {
        //Good place to save info before reset statictics
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            bl_UtilityHelper.LoadLevel(LeftRoomReturnScene);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void MainMenu()
    {
        if (GetGameMode != GameMode.FFA)
        {
            TDMJoinButtons();
            MenuButtons();
            if (m_window == 1)
            {
                TDMScoreboard();
            }
            LeftButton();
            if (!isPlaying && !isFinish)
            {
                SelectClassMenu();
            }

        }
        else if (GetGameMode == GameMode.FFA)
        {

            FFAJoinButton();
            MenuButtons();
            LeftButton();
            if (m_window == 1)
            {
                FFAScoreboard();
            }
            if (!isPlaying && !isFinish)
            {
                SelectClassMenu();
            }
        }
        if (m_window == 2)
        {
            SettingMenu();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void MenuButtons()
    {
        if (isPlaying)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 180, 150, 30), "Resume"))
            {
                m_window = 1;
                bl_UtilityHelper.LockCursor(true);
                showMenu = false;
            }
            if (GUI.Button(new Rect(Screen.width / 2 - 225, Screen.height / 2 - 180, 150, 30), "Settings"))
            {
                m_window = 2;
            }
            if (GUI.Button(new Rect(Screen.width / 2 + 75, Screen.height / 2 - 180, 150, 30), "ScoreBoard"))
            {
                m_window = 1;
            }
            if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 200, 150, 30), "Suicide") && bl_GameManager.isAlive)
            {
                PhotonView view = PhotonView.Find(bl_GameManager.m_view);
                if (view != null)
                {

                    bl_PlayerDamageManager pdm = view.GetComponent<bl_PlayerDamageManager>();
                    pdm.Suicide();
                    m_window = 1;
                    bl_UtilityHelper.LockCursor(true);
                    showMenu = false;
                    if (view.isMine)
                    {
                        bl_GameManager.SuicideCount++;
                        //Debug.Log("Suicide " + bl_GameManager.SuicideCount + " times");
                        //if player is a joker o abuse of suicide, them kick of room
                        if (bl_GameManager.SuicideCount >= 3)//Max number de suicides  = 3, you can change
                        {
                            isPlaying = false;
                            bl_GameManager.isAlive = false;
                            bl_UtilityHelper.LockCursor(false);
                            LeftOfRoom();
                        }
                    }
                }
                else
                {
                    Debug.LogError("This view " + bl_GameManager.m_view + " is not found");
                }

            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void FFAJoinButton()
    {
        if (!isPlaying && !isFinish)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 190, 200, 40), "Join"))
            {
                m_window = 1;
                bl_UtilityHelper.LockCursor(true);
                showMenu = false;
                GM.SpawnPlayer(Team.All);
                bl_EventHandler.KillEvent(PhotonNetwork.player.name, "", "Joined in match", Team.All.ToString(), 777, 30);
                isPlaying = true;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void TDMJoinButtons()
    {
        if (!isPlaying && !isFinish && !AutoTeamSelection)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 190, 200, 40), "Join Delta"))
            {
                m_window = 1;
                bl_UtilityHelper.LockCursor(true);
                showMenu = false;
                GM.SpawnPlayer(Team.Delta);
                bl_EventHandler.KillEvent(PhotonNetwork.player.name, "", "Joined in Delta", Team.Delta.ToString(), 777, 30);
                isPlaying = true;
            }

            if (GUI.Button(new Rect(Screen.width / 2 + 100, Screen.height / 2 - 190, 200, 40), "Join Recon"))
            {
                m_window = 1;
                bl_UtilityHelper.LockCursor(true);
                showMenu = false;
                GM.SpawnPlayer(Team.Recon);
                bl_EventHandler.KillEvent(PhotonNetwork.player.name, "", "Joined in Recon", Team.Recon.ToString(), 777, 30);
                isPlaying = true;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void FFAScoreboard()
    {
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 350), "", ScoreBoardStyle);
        GUILayout.Space(5);
        GUILayout.BeginHorizontal(SKin.customStyles[6]);
        GUILayout.Label("Player", GUILayout.Width(175));
        GUILayout.Label("K", GUILayout.Width(50));
        GUILayout.Label("D", GUILayout.Width(50));
        GUILayout.Label("S", GUILayout.Width(50));
        GUILayout.Label("Ms", GUILayout.Width(50));
        GUILayout.EndHorizontal();
        scroll_2 = GUILayout.BeginScrollView(scroll_2, false, false);
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.name == PhotonNetwork.player.name)//if this player is Mine
            {
                GUI.color = new Color(1, 0.6f, 0, 1);
                GUILayout.BeginHorizontal("Box");
            }
            else
            {
                GUI.color = Color.white;
                GUILayout.BeginHorizontal();
            }
            GUILayout.Label((string)player.name, GUILayout.Width(173));
            GUILayout.Label(player.customProperties[PropiertiesKeys.KillsKey].ToString(), GUILayout.Width(50));
            GUILayout.Label(player.customProperties[PropiertiesKeys.DeathsKey].ToString(), GUILayout.Width(50));
            GUILayout.Label(player.customProperties[PropiertiesKeys.ScoreKey].ToString(), GUILayout.Width(50));
            int Ping = (int)player.customProperties[PropiertiesKeys.Ping];
            GUILayout.Label(Ping.ToString("000") + "<size=10>ms</size>", GUILayout.Width(50));
            GUILayout.EndHorizontal();
            GUI.color = Color.white;
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    /// <summary>
    /// 
    /// </summary>
    void TDMScoreboard()
    {
        //////////////////////////////////////////////RECON/////////////////////////////////////////////////////////
        GUILayout.BeginArea(new Rect(Screen.width / 2 + 5, Screen.height / 2 - 150, 400, 350), "", ScoreBoardStyle);
        GUILayout.Space(5);
        GUILayout.BeginHorizontal(SKin.customStyles[6]);
        GUILayout.Label("Player", GUILayout.Width(175));
        GUILayout.Label("K", GUILayout.Width(50));
        GUILayout.Label("D", GUILayout.Width(50));
        GUILayout.Label("S", GUILayout.Width(50));
        GUILayout.Label("Ms", GUILayout.Width(50));
        GUILayout.EndHorizontal();
        scroll_2 = GUILayout.BeginScrollView(scroll_2, false, false);
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.GetPlayerTeam() == Team.Recon)
            {
                if (player.name == PhotonNetwork.player.name)//if this player is Mine
                {
                    GUI.color = ColorKeys.MineColor;
                }
                else
                {
                    GUI.color = Color.white;
                }
                GUILayout.BeginHorizontal("box");
                GUILayout.Label((string)player.name, GUILayout.Width(175));
                GUILayout.Label(player.customProperties[PropiertiesKeys.KillsKey].ToString(), GUILayout.Width(50));
                GUILayout.Label(player.customProperties[PropiertiesKeys.DeathsKey].ToString(), GUILayout.Width(50));
                GUILayout.Label(player.customProperties[PropiertiesKeys.ScoreKey].ToString(), GUILayout.Width(50));
                int Ping = (int)player.customProperties[PropiertiesKeys.Ping];
                GUILayout.Label(Ping.ToString("000") + "<size=10>ms</size>", GUILayout.Width(50));
                GUILayout.EndHorizontal();
                GUI.color = Color.white;
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        /////////////////////////////////////////////DELTA////////////////////////////////////////////////////////////
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 400, Screen.height / 2 - 150, 400, 350), "", ScoreBoardStyle);
        GUILayout.Space(5);
        GUILayout.BeginHorizontal(SKin.customStyles[6]);
        GUILayout.Label("Player", GUILayout.Width(175));
        GUILayout.Label("K", GUILayout.Width(50));
        GUILayout.Label("D", GUILayout.Width(50));
        GUILayout.Label("S", GUILayout.Width(50));
        GUILayout.Label("Ms", GUILayout.Width(50));
        GUILayout.EndHorizontal();
        scroll_1 = GUILayout.BeginScrollView(scroll_1, false, false);
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.GetPlayerTeam() == Team.Delta)
            {
                if (player.name == PhotonNetwork.player.name)//if this player is Mine
                {
                    GUI.color = ColorKeys.MineColor;
                }
                else
                {
                    GUI.color = Color.white;
                }
                GUILayout.BeginHorizontal("box");
                GUILayout.Label((string)player.name, GUILayout.Width(175));
                GUILayout.Label(player.customProperties[PropiertiesKeys.KillsKey].ToString(), GUILayout.Width(50));
                GUILayout.Label(player.customProperties[PropiertiesKeys.DeathsKey].ToString(), GUILayout.Width(50));
                GUILayout.Label(player.customProperties[PropiertiesKeys.ScoreKey].ToString(), GUILayout.Width(50));
                int Ping = (int)player.customProperties[PropiertiesKeys.Ping];
                GUILayout.Label(Ping.ToString("000") + "<size=10>ms</size>", GUILayout.Width(50));
                GUILayout.EndHorizontal();
                GUI.color = Color.white;
            }
        }
        GUILayout.EndScrollView();


        GUILayout.EndArea();
    }

    /// <summary>
    /// 
    /// </summary>
    void OnlyScoreBoard()
    {
        if (m_showScoreBoard == true)
        {
            if (GetGameMode != GameMode.FFA)
            {
                TDMScoreboard();
            }
            else
            {
                FFAScoreboard();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void SelectClassMenu()
    {
        int width = 150;

        GUILayout.BeginArea(new Rect(0, Screen.height / 2 - 130, 250, 325));
        GUILayout.Label("<b>Select your \n<color=orange>Player Class</color></b>");
        GUILayout.BeginVertical();
        if (m_playerclass == PlayerClass.Assault)
        {
            GUI.color = ColorKeys.SelectColor;
        }
        else
        {
            GUI.color = Color.white;
        }
        if (GUILayout.Button("Assault", GUILayout.Width(width), GUILayout.Height(50)))
        {
            m_playerclass = PlayerClass.Assault;
        }
        if (m_playerclass == PlayerClass.Recon)
        {
            GUI.color = ColorKeys.SelectColor;
        }
        else
        {
            GUI.color = Color.white;
        }
        if (GUILayout.Button("Recon", GUILayout.Width(width), GUILayout.Height(50)))
        {
            m_playerclass = PlayerClass.Recon;
        }
        if (m_playerclass == PlayerClass.Support)
        {
            GUI.color = ColorKeys.SelectColor;
        }
        else
        {
            GUI.color = Color.white;
        }
        if (GUILayout.Button("Suport", GUILayout.Width(width), GUILayout.Height(50)))
        {
            m_playerclass = PlayerClass.Support;
        }
        if (m_playerclass == PlayerClass.Engineer)
        {
            GUI.color = ColorKeys.SelectColor;
        }
        else
        {
            GUI.color = Color.white;
        }
        if (GUILayout.Button("Engineer", GUILayout.Width(width), GUILayout.Height(50)))
        {
            m_playerclass = PlayerClass.Engineer;
        }
        GUI.color = Color.white;
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void SettingMenu()
    {
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 250, Screen.height / 2 - 150, 500, 350), "", "box");
        GUILayout.Space(10);
        GUILayout.Box("Settings");
        GUILayout.Label("Quality Level");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<<"))
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
        GUILayout.Box(QualitySettings.names[(int)m_currentQuality]);
        if (GUILayout.Button(">>"))
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
        GUILayout.EndHorizontal();
        GUILayout.Label("Anisotropic Filtering");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<<"))
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
        GUILayout.Box(m_stropicOptions[m_stropic]);
        if (GUILayout.Button(">>"))
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
        GUILayout.EndHorizontal();
        GUILayout.Label("Sound Volume");
        GUILayout.BeginHorizontal();
        m_volume = GUILayout.HorizontalSlider(m_volume, 0.0f, 1.0f);
        GUILayout.Label((m_volume * 100).ToString("000"), GUILayout.Width(30));
        GUILayout.EndHorizontal();
        GUILayout.Label("Sensitivity");
        GUILayout.BeginHorizontal();
        m_sensitive = GUILayout.HorizontalSlider(m_sensitive, 0.0f, 100.0f);
        GUILayout.Label(m_sensitive.ToString("000"), GUILayout.Width(30));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Apply"))
        {
            ApplySave();
        }
        GUILayout.EndArea();
    }

    void ApplySave()
    {
        QualitySettings.SetQualityLevel((int)m_currentQuality);
        AudioListener.volume = m_volume;
        if (m_stropic == 0)
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        }
        else if (m_stropic == 1)
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        }
        else
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        }
        //Save
        PlayerPrefs.SetFloat("volumen", m_volume);
        PlayerPrefs.SetFloat("sensitive", m_sensitive);
        PlayerPrefs.SetInt("quality", (int)m_currentQuality);
        PlayerPrefs.SetInt("anisotropic", m_stropic);
        Debug.Log("Save Done!");
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

    public GUIStyle ScoreBoardStyle
    {
        get
        {
            return SKin.customStyles[4];
            
        }
    }
    /// <summary>
    /// Get All Player in Room List
    /// </summary>
    public List<PhotonPlayer> GetPlayerList
    {
        get
        {
            List<PhotonPlayer> list = new List<PhotonPlayer>();
            foreach (PhotonPlayer players in PhotonNetwork.playerList)
            {
                list.Add(players);
            }
            return list;
        }
    }
    /// <summary>
    /// Get the total players in team Delta
    /// </summary>
    public int GetPlayerInDeltaCount
    {
        get
        {
            int count = 0;
            foreach (PhotonPlayer players in PhotonNetwork.playerList)
            {
                if ((string)players.customProperties[PropiertiesKeys.TeamKey] == Team.Delta.ToString())
                {
                    count++;
                }
            }
            return count;
        }
    }
    /// <summary>
    /// Get the total players in team Recon
    /// </summary>
    public int GetPlayerInReconCount
    {
        get
        {
            int count = 0;
            foreach (PhotonPlayer players in PhotonNetwork.playerList)
            {
                if ((string)players.customProperties[PropiertiesKeys.TeamKey] == Team.Recon.ToString())
                {
                    count++;
                }
            }
            return count;
        }
    }
    /// <summary>
    /// Sort Player by Kills,for more info wacht this: http://answers.unity3d.com/questions/233917/custom-sorting-function-need-help.html
    /// </summary>
    /// <param name="player1"></param>
    /// <param name="player2"></param>
    /// <returns></returns>
    private static int GetSortPlayerByKills(PhotonPlayer player1, PhotonPlayer player2)
    {
        if (player1.customProperties[PropiertiesKeys.KillsKey] != null && player2.customProperties[PropiertiesKeys.KillsKey] != null)
        {
            return (int)player2.customProperties[PropiertiesKeys.KillsKey] - (int)player1.customProperties[PropiertiesKeys.KillsKey];
        }
        else
        {
            return 0;
        }
    }

    IEnumerator CanSpawnIE()
    {
        yield return new WaitForSeconds(3);
        CanSpawn = true;
    }

    public IEnumerator FadeIn(float delay = 0.0f, bool load = false)
    {

        m_alphafade = 0;
        while (m_alphafade < 2.0f)
        {
            m_alphafade += Time.deltaTime;
            yield return 0;
        }
        yield return new WaitForSeconds(delay);
        if (load)
        {
            // back to main menu        
            bl_UtilityHelper.LoadLevel(LeftRoomReturnScene);
        }
    }

   public static IEnumerator FadeOut(float t_time)
    {
        m_alphafade = t_time;
        while (m_alphafade > 0.0f)
        {
            m_alphafade -= Time.deltaTime;
            yield return 0;
        }
    }

   private bool RequestLeft = false;
   public override void OnLeftRoom()
   {
       Debug.Log("OnLeftRoom (local)");
       
       RequestLeft = true;
       m_CanvasRoot.enabled = false;
       this.GetComponent<bl_RoundTime>().enabled = false;
       FadeBlack.SetActive(true);
       StartCoroutine(FadeIn(DelayLeave, true));
   }
}