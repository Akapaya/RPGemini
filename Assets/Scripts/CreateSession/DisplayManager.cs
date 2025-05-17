using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DisplayManager : MonoBehaviour
{
    [Header("Dados")]
    public SessionDataSO sessionData;
    public List<CharacterDataSO> characterSOs;

    [Header("UI")]
    public TMP_Dropdown dropdown;
    public TMP_Text displayText;

    public void UpdateDisplay()
    {
        PopulateDropdown();
        dropdown.onValueChanged.AddListener(UpdateDisplay);
        UpdateDisplay(0); // Mostrar dados da sessão inicialmente
    }

    void PopulateDropdown()
    {
        dropdown.ClearOptions();
        List<string> options = new List<string>
        {
            "Sessão"
        };

        foreach (var character in characterSOs)
        {
            options.Add(character.Name);
        }

        dropdown.AddOptions(options);
    }

    void UpdateDisplay(int index)
    {
        if (index == 0)
        {
            // Dados da sessão
            displayText.text = $"<b>Nome da Sessão:</b> {sessionData.SessionName}\n" +
                               $"<b>Descrição:</b> {sessionData.SessionDescription}\n" +
                               $"<b>Arco:</b> {sessionData.SessionArcDescription}";
        }
        else
        {
            // Dados do personagem
            CharacterDataSO character = characterSOs[index - 1]; // -1 porque 0 é a sessão
            displayText.text = $"<b>Nome:</b> {character.Name}\n" +
                               $"<b>Background:</b> {character.Background}\n" +
                               $"<b>Visual:</b> {character.Visual}\n" +
                               $"<b>Ambição:</b> {character.Ambition}\n" +
                               $"<b>Quirk:</b> {character.Quirk}\n\n" +
                               $"<b>Max HP:</b> {character.MaxHealth}\n" +
                               $"<b>HP Atual:</b> {character.Health}\n" +
                               $"<b>Força:</b> {character.Strength}\n" +
                               $"<b>Destreza:</b> {character.Dexterity}\n" +
                               $"<b>Constituição:</b> {character.Constitution}\n" +
                               $"<b>Inteligência:</b> {character.Intelligence}\n" +
                               $"<b>Carisma:</b> {character.Charisma}";
        }
    }
}
