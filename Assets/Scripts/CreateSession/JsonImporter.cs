using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine;

public class JsonImporter : MonoBehaviour
{
    public SessionDataSO sessionDataSO;
    public List<CharacterDataSO> characterSOs;
    public DisplayManager displayManager;

    [ContextMenu("Debug")]
    public void DebugChar()
    {
        Debug.Log(characterSOs[0].Background);
        Debug.Log(characterSOs[1].Background);
        Debug.Log(characterSOs[2].Background);
        Debug.Log(characterSOs[3].Background);
    }

    public void ParseChatGeminiOutput(string chatGeminiOutput)
    {
        try
        {
            // Extrai o conteúdo JSON da string usando regex
            string json = ExtractJson(chatGeminiOutput);

            // Desserializa para objeto de dados
            SessionWrapper data = JsonUtility.FromJson<SessionWrapper>(json);

            // Preenche o ScriptableObject de sessão
            sessionDataSO.SessionName = data.SessionData.SessionName;
            sessionDataSO.SessionDescription = data.SessionData.SessionDescription;
            sessionDataSO.SessionArcDescription = data.SessionData.SessionArcDescription;

            // Preenche os personagens
            for (int i = 0; i < data.Characters.Count && i < characterSOs.Count; i++)
            {
                CharacterData charData = data.Characters[i];
                CharacterDataSO charSO = characterSOs[i];

                charSO.Name = charData.Name;
                charSO.Background = charData.Background;
                charSO.Visual = charData.Visual;
                charSO.Ambition = charData.Ambition;
                charSO.Quirk = charData.Quirk;
                charSO.MaxHealth = charData.MaxHealth;
                charSO.Health = charData.Health;
                charSO.Strength = charData.Strength;
                charSO.Dexterity = charData.Dexterity;
                charSO.Constitution = charData.Constitution;
                charSO.Intelligence = charData.Intelligence;
                charSO.Charisma = charData.Charisma;
            }

            displayManager.UpdateDisplay();
            Debug.Log("Dados aplicados com sucesso!");
        }
        catch (Exception ex)
        {
            Debug.LogError("Erro ao processar a resposta do Gemini: " + ex.Message);
        }
    }

    private string ExtractJson(string input)
    {
        // Regex que tenta pegar o conteúdo entre as primeiras chaves balanceadas
        Match match = Regex.Match(input, @"\{(?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!))\}");
        if (!match.Success)
            throw new Exception("JSON não encontrado na string.");

        return match.Value;
    }
}