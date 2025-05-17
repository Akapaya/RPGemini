using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Session", menuName = "Session/SessionSO")]
public class SessionDataSO : ScriptableObject
{
    public string SessionName;
    public string SessionDescription;
    public string SessionArcDescription;
    public List<string> LastLogs = new();

    public CharacterDataSO PlayerCharacter;
    public List<CharacterDataSO> CharactersData = new();
    public List<string> InSceneCharactersName = new();
}
