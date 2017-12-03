using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class bl_PhotonStatistics : MonoBehaviour
{
    [Header("Settings")]
    [Range(1,5)]public float RandomTime = 2;
    [Range(1,5)]public float UpdateEach = 10;
	[Header("References")]
    [SerializeField]private Text AllRoomText;
    [SerializeField]private Text AllPlayerText;
    [SerializeField]private Text AllPlayerInRoomText;
    [SerializeField]private Text AllPlayerInLobbyText;
    [SerializeField]private Text PingText;
    [SerializeField]private Image PingImage;

    private float GetTime;
    private int AllRooms;
    private int AllPlayers;
    private int AllPlayerInRoom;
    private int AllPlayerInLobby;

    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        Refresh();
        InvokeRepeating("UpdateRepeting", 1, UpdateEach);
    }

    /// <summary>
    /// 
    /// </summary>
    void UpdateRepeting()
    {
        Refresh();
    }

    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        CancelInvoke();
        StopAllCoroutines();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Refresh()
    {
        StopAllCoroutines();
        StartCoroutine(GetStaticsIE());
        GetPing();
    }

    /// <summary>
    /// 
    /// </summary>
    void GetPing()
    {
        int ping = PhotonNetwork.GetPing();
        if (ping <= 150)
        {
            PingImage.color = Color.green;
        }
        else if (ping > 150 && ping < 225)
        {
            PingImage.color = Color.yellow;
        }
        else if (ping > 225)
        {
            PingImage.color = Color.red;
        }
        float percet = ping * 100 / 500;
        PingImage.fillAmount = 1 - (percet * 0.01f);
        PingText.text = ping.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator GetStaticsIE()
    {
        GetTime = RandomTime;
        while(GetTime > 0)
        {
            GetTime -= Time.deltaTime;
            AllRooms = Random.Range(0, 999);
            AllPlayers = Random.Range(0, 999);
            AllPlayerInRoom = Random.Range(0, 999);
            AllPlayerInLobby = Random.Range(0, 999);
            Set();
            yield return new WaitForEndOfFrame();
        }
        GetPhotonStatics();
        Set();
    }

    /// <summary>
    /// 
    /// </summary>
    void GetPhotonStatics()
    {
        AllRooms = PhotonNetwork.countOfRooms;
        AllPlayers = PhotonNetwork.countOfPlayers;
        AllPlayerInRoom = PhotonNetwork.countOfPlayersInRooms;
        AllPlayerInLobby = PhotonNetwork.countOfPlayersOnMaster;
    }

    /// <summary>
    /// 
    /// </summary>
    void Set()
    {
        AllRoomText.text = string.Format("<b>ROOMS CREATED:</b> {0}", AllRooms);
        AllPlayerText.text = string.Format("<b>PLAYERS ONLINE:</b> {0}", AllPlayers);
        AllPlayerInRoomText.text = string.Format("<b>PLAYERS PLAYING:</b> {0}", AllPlayerInRoom);
        AllPlayerInLobbyText.text = string.Format("<b>PLAYERS IN LOBBY:</b> {0}", AllPlayerInLobby);
    }

}