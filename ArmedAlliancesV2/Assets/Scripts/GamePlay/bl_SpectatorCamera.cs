using UnityEngine;
using System.Collections;

public class bl_SpectatorCamera : MonoBehaviour {

    //initial speed
    public int speed = 20;

    private Transform _Transform = null;
    private Camera _Cam = null;
    // Update is called once per frame
    void Update()
    {

        //press shift to move faster
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed = 40;

        }
        else
        {
            //if shift is not pressed, reset to default speed
            speed = 20;
        }
        //For the following 'if statements' don't include 'else if', so that the user can press multiple buttons at the same time
        //move camera to the left
        if (Input.GetKey(KeyCode.A))
        {
            Transform.position = Transform.position + UseCamera.transform.right * -1 * speed * Time.deltaTime;
        }

        //move camera backwards
        if (Input.GetKey(KeyCode.S))
        {
            Transform.position = Transform.position + UseCamera.transform.forward * -1 * speed * Time.deltaTime;

        }
        //move camera to the right
        if (Input.GetKey(KeyCode.D))
        {
            Transform.position = Transform.position + UseCamera.transform.right * speed * Time.deltaTime;

        }
        //move camera forward
        if (Input.GetKey(KeyCode.W))
        {

            Transform.position = Transform.position + UseCamera.transform.forward * speed * Time.deltaTime;
        }
        //move camera upwards
        if (Input.GetKey(KeyCode.E))
        {
            Transform.position = Transform.position + UseCamera.transform.up * speed * Time.deltaTime;
        }
        //move camera downwards
        if (Input.GetKey(KeyCode.Q))
        {
            Transform.position = Transform.position + UseCamera.transform.up * -1 * speed * Time.deltaTime;
        }

    }

    public Camera UseCamera
    {
        get
        {
            if (_Cam == null)
            {
                _Cam = this.GetComponent<Camera>();
            }
            return _Cam;
        }
    }
    public Transform Transform
    {
        get
        {
            if (_Transform == null)
            {
                _Transform = this.GetComponent<Transform>();
            }
            return _Transform;
        }
    }
}