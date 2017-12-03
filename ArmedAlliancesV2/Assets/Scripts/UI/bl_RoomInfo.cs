using UnityEngine;
using UnityEngine.UI;

public class bl_RoomInfo : MonoBehaviour {

    public Text RoomNameText = null;
    public Text MapNameText = null;
    public Text PlayersText = null;
    public Text GameModeText = null;
    public Text PingText = null;
    public GameObject JoinButton = null;
    public GameObject FullText = null;

    private RoomInfo cacheInfo = null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    public void GetInfo(RoomInfo info)
    {
        cacheInfo = info;
        RoomNameText.text = info.name;
        MapNameText.text = (string)info.customProperties[PropiertiesKeys.CustomSceneName];
        GameModeText.text = (string)info.customProperties[PropiertiesKeys.GameModeKey];
        PlayersText.text = info.playerCount + "/" + info.maxPlayers;
        PingText.text = PhotonNetwork.GetPing().ToString();
        bool _active = (info.playerCount < info.maxPlayers) ? true : false;
        JoinButton.SetActive(_active);
        FullText.SetActive(!_active);
    }
    /// <summary>
    /// 
    /// </summary>
    public void JoinRoom()
    {
        if (cacheInfo.playerCount < cacheInfo.maxPlayers)
        {
            PhotonNetwork.JoinRoom(cacheInfo.name);
        }
    }
}