using TMPro;
using UnityEngine;

public class ChatController : MonoBehaviour
{
    public TMP_Text OutputChat;

    private void Start()
    {
        PromptManager.instance.OnReceivePrompt += UpdateOutputChat;
    }

    private void OnDisable()
    {
        PromptManager.instance.OnReceivePrompt -= UpdateOutputChat;
    }

    public void UpdateOutputChat(string output)
    {
        OutputChat.text = output;
    }
}
