using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public SessionDataSO SessionData;

    public static SessionManager instance;

    public void Start()
    {
        instance = this;
    }

    [ContextMenu("Start Challenge NPC Test")]
    public void ChallengeNPC()
    {
        PromptManager.instance.ChallengeNPCPrompt(SessionData.PlayerCharacter, SessionData.CharactersData[2]);
    }
}
