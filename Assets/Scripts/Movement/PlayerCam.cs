using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;
    public ChatGPT chatGPT;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        chatGPT = FindObjectOfType<ChatGPT>();

    }

    private void Update()
    {
        // get mouse input

        // Your existing camera movement code
        // ...

        if (!chatGPT.IsChatActive)
        {
            // Get mouse input using new Input System
            var mouseDelta = Mouse.current.delta.ReadValue();

            float mouseX = mouseDelta.x * Time.deltaTime * sensX;
            float mouseY = mouseDelta.y * Time.deltaTime * sensY;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Rotate cam and orientation
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        
    }
}