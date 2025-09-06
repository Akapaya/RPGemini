using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    public GameObject SubActionPanel;
    public GameObject SubTalkPanel;
    public GameObject SubTargetPanel;
    public GameObject SubPromptPanel;
    public TMP_Dropdown ObjectiveTexts;
    public TMP_Dropdown MoodTexts;
    public TMP_Dropdown TargetsTexts;

    public Actions CurrentAction;
    public string CurrentTarget;
    public TMP_Text CurrentPrompt;
    public Objective CurrentObjective;
    public Mood CurrentMood;

    public void EnableSubActionPanel()
    {
        DisableSubTalkPanel();
        DisableTargetPanel();
        SubActionPanel.SetActive(true);
    }

    public void DisableSubActionPanel()
    {
        SubActionPanel.SetActive(false);
    }

    public void EnableSubTalkPanel()
    {
        DisableSubActionPanel();
        DisableTargetPanel();
        PopulateTargetPanel();
        SubTalkPanel.SetActive(true);
    }

    public void DisableSubTalkPanel()
    {
        SubTalkPanel.SetActive(false);
    }

    public void EnableTargetPanel()
    {
        SubTargetPanel.SetActive(true);
        PopulateTargetPanel();
    }

    public void DisableTargetPanel()
    {
        SubTargetPanel.SetActive(false);
    }

    public void EnablePromptPanel()
    {
        SubPromptPanel.SetActive(true);
    }

    public void DisablePromptPanel()
    {
        SubPromptPanel.SetActive(false);
    }

    public void PopulateTargetPanel()
    {
        TargetsTexts.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        options.Add(new TMP_Dropdown.OptionData("Everyone"));
        var targets = SessionManager.instance.SessionData.InSceneCharactersName;
        for (int i = 0; i < targets.Count; i++)
        {
            options.Add(new TMP_Dropdown.OptionData(targets[i]));
        }
        TargetsTexts.AddOptions(options);
    }

    public void SelectAction(string action)
    {
        if (Enum.TryParse(action, true, out Actions parsedAction))
        {
            CurrentAction = parsedAction; // Ou armazene o enum diretamente
            EnableTargetPanel();
        }
        else
        {
            // Trate ação inválida (ex: log, definir um valor padrão, lançar exceção)
            Console.WriteLine($"Ação inválida: {action}");
            CurrentAction = Actions.Defend; // Valor padrão
        }
    }

    public void SelectTarget(TMP_Text target)
    {
        CurrentTarget = target.text;
    }

    public void ContinuePrompt()
    {
        PromptManager.instance.SendPrompt("Continue a cena");
    }

    public void SaveContextPrompt()
    {
        PromptManager.instance.AskContextPrompt("Me retorna o contexto atual para eu salvar");
    }

    public void SendTalkPrompt()
    {
        var Player = SessionManager.instance.SessionData.PlayerCharacter;
        System.Enum.TryParse(ObjectiveTexts.options[ObjectiveTexts.value].text, out CurrentObjective);
        System.Enum.TryParse(MoodTexts.options[MoodTexts.value].text, out CurrentMood);
        CurrentTarget = TargetsTexts.options[TargetsTexts.value].text;

        DialogueData dialogo = new DialogueData(Player,CurrentPrompt.text,CurrentObjective, CurrentMood, CurrentTarget);
        if (CurrentPrompt.text == "")
        {
            Debug.Log("Prompt Vazio ou invalido");
            return;
        }

        SubPromptPanel.SetActive(false);
        PromptManager.instance.SendTalkPrompt(dialogo);
        CurrentPrompt.text = string.Empty;
    }
}

public enum Actions
{
    Attack,
    Defend,
    Skill,
    Challenge,
    Stealth,
    Greet,
    Ask,
    Intimidate,
    Shout,
    Persuade,
    Talk
}