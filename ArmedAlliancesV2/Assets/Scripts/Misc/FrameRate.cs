using UnityEngine;
using UnityEngine.UI;

public class FrameRate : MonoBehaviour
{
    public float accum;
    public int frames;
    public Text TextUI = null;
    //Privates
    private string framerate;
    private float timeleft;
    private float updateInterval = 0.5f;

    void Start()
    {
        this.timeleft = this.updateInterval;
    }

    void Update()
    {
        this.timeleft -= Time.deltaTime;
        this.accum += Time.timeScale / Time.deltaTime;
        this.frames++;
        float _rate = 0;
        if (this.timeleft <= 0)
        {
            _rate = this.accum / this.frames;
            this.framerate = string.Empty + _rate.ToString("000");
            this.timeleft = this.updateInterval;
            this.accum = 0;
            this.frames = 0;
            if (_rate < 30)
            {
                //if you receive this, you need optimize your game or decrease quality level.
              //  Debug.LogWarning("Your fps is very low for a multiplayer game, due when have more players this will be even worse!.");
            }
        }
        if (TextUI != null)
        {
            TextUI.text = "FPS: <color=orange>" + this.framerate + "</color>";
        }
        else
        {
            Destroy(this);
        }
             
    }
}