using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public ChatGPT chatGPT;

    private void Start()
    {
        chatGPT = FindObjectOfType<ChatGPT>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;


    }
    private void Update()
    {


        if (!chatGPT.IsChatActive)
        {
            Cursor.lockState = CursorLockMode.None;
            transform.position = cameraPosition.position;

        }

    }
}
