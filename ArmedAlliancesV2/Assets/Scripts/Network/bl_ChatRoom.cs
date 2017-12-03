using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class bl_ChatRoom : bl_PhotonHelper {

    public GUISkin m_Skin;
    [Space(5)]
    public Text ChatText;
    public bool IsVisible = true;
    public bool WithSound = true;
    public int MaxMsn = 7;
    private List<string> messages = new List<string>();
    private string inputLine = "";
    [Space(5)]
    public AudioClip MsnSound;

    public static readonly string ChatRPC = "Chat";
    private float m_alpha = 2f;
    private bool isChat = false;

    void Start()
    {
        Refresh();
    }

    public void OnGUI()
    {
        if (ChatText == null)
            return;

        if (m_alpha > 0.0f && !isChat)
        {
            m_alpha -= Time.deltaTime / 2;
        }
        else if (isChat)
        {
            m_alpha = 10;
        }

        Color t_color = ChatText.color;
        t_color.a = m_alpha;
        ChatText.color = t_color;

        
        GUI.skin = m_Skin;
        GUI.color = new Color(1, 1, 1, m_alpha);
        if (!this.IsVisible || PhotonNetwork.connectionStateDetailed != ClientState.Joined)
        {
            return;
        }

        if (Event.current.type == EventType.KeyDown &&(Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(this.inputLine) && isChat && bl_UtilityHelper.GetCursorState)
            {
                this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
                this.inputLine = "";
                GUI.FocusControl("");
                isChat = false;
                return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
            }
            else if (!isChat && bl_UtilityHelper.GetCursorState)
            {
                GUI.FocusControl("MyChatInput");
                isChat = true;
            }
            else
            {
                if (isChat)
                {
                    Closet();
                }
            }
        }

        if (Event.current.type == EventType.keyDown && (Event.current.keyCode == KeyCode.Tab || Event.current.character == '\t'))
        {
            Event.current.Use();
        }
        GUI.SetNextControlName("");

        GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height - 35, 300, 50));
        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("MyChatInput");
        inputLine = GUILayout.TextField(inputLine);
        GUI.SetNextControlName("None");
        if (GUILayout.Button("Send", "box", GUILayout.ExpandWidth(false)))
        {
            this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
            this.inputLine = "";
            GUI.FocusControl("");
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

    }

    /// <summary>
    /// 
    /// </summary>
    void Closet()
    {
        isChat = false;
        GUI.FocusControl("");
    }
    /// <summary>
    /// Sync Method
    /// </summary>
    /// <param name="newLine"></param>
    /// <param name="mi"></param>
    [PunRPC]
    public void Chat(string newLine, PhotonMessageInfo mi)
    {
        m_alpha = 7;
        string senderName = "anonymous";

        if (mi.sender != null)
        {
            if (!string.IsNullOrEmpty(mi.sender.name))
            {
                senderName = mi.sender.name;
            }
            else
            {
                senderName = "Player " + mi.sender.ID;
            }
        }

        this.messages.Add("[" + senderName + "]: " + newLine);
        if (MsnSound != null && WithSound)
        {
            GetComponent<AudioSource>().PlayOneShot(MsnSound);
        }
        if (messages.Count > MaxMsn)
        {
            messages.RemoveAt(0);
        }

        ChatText.text = "";
        foreach (string m in messages)
            ChatText.text += m + "\n";
    }
    /// <summary>
    /// Local Method
    /// </summary>
    /// <param name="newLine"></param>
    public void AddLine(string newLine)
    {
        m_alpha = 7;
        this.messages.Add(newLine);
        if (messages.Count > MaxMsn)
        {
            messages.RemoveAt(0);
        }
    }

    public void Refresh()
    {
        ChatText.text = "";
        foreach (string m in messages)
            ChatText.text += m + "\n";
    }
}
