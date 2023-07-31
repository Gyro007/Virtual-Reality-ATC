using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        public PlayerMovement playerMovement;


        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private float height;
        private OpenAIApi openai = new OpenAIApi();
        public bool IsChatActive { get; private set; } = false;

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Act as a random stranger in a chat room and reply to the questions. Don't break character. Don't ever mention that you are an AI model.";

        private void Start()
        {
            button.onClick.AddListener(SendReply);
             playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement script not found in the scene.");
                return;
            }

        }
        private void Update()
        {
            HandlePlayerInput(); // Add this line to call HandlePlayerInput()
        }

            private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }


        private void HandlePlayerInput()
        {
            // Check for chat activation key (e.g., "E" key).
            if (Keyboard.current.eKey.wasPressedThisFrame)

            {
            if (!inputField.gameObject.activeSelf)
                {
                    // Activate the chat when the input field is not active.
                    ActivateChat();
                }

            }

            // Check for chat deactivation key (e.g., "Escape" key).
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                DeactivateChat();
            }

            // Check for "Enter" key press to send the chat message.
            if (inputField.gameObject.activeSelf && Keyboard.current.enterKey.wasPressedThisFrame)
            {
                SendReply();
            }
        }


        private void ToggleChatInteraction()
        {
            if (inputField == null || button == null)
            {
                Debug.LogWarning("InputField or Button is not assigned properly.");
                return;
            }

            IsChatActive = !IsChatActive;

            // Enable or disable the chat interaction based on the chat activation state.
            inputField.gameObject.SetActive(IsChatActive);
            button.gameObject.SetActive(IsChatActive);
            scroll.gameObject.SetActive(IsChatActive);


            inputField.enabled = IsChatActive;
            button.enabled = IsChatActive;
            scroll.enabled = IsChatActive;


            // Focus on the input field when the chat is activated.
            if (IsChatActive)
            {
                inputField.ActivateInputField();
            }
        }


        private void ActivateChat()
        {
            inputField.gameObject.SetActive(true);
            button.gameObject.SetActive(true);
            scroll.gameObject.SetActive(true);

            // Use new Unity Input System to enable the input field
            inputField.interactable = true;

            button.interactable = true;
            inputField.ActivateInputField();
            playerMovement.SetCanMove(false);

        }

        private void DeactivateChat()
        {
            inputField.gameObject.SetActive(false);
            button.gameObject.SetActive(false);
            scroll.gameObject.SetActive(false);

            // Use new Unity Input System to disable the input field
            inputField.interactable = false;

            button.interactable = false;
            playerMovement.SetCanMove(true);

        }




        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            
            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text; 
            
            messages.Add(newMessage);
            
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                
                messages.Add(message);
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            button.enabled = true;
            inputField.enabled = true;
        }
    }
}
