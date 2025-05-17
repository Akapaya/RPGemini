using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class CreateSessionController : MonoBehaviour
{
    public TMP_Dropdown ThemeRpg;
    public TMP_Dropdown SystemRpg;
    public TMP_Dropdown InitialValueRpg;

    public JsonImporter jsonImporter;

    private void Start()
    {
        PromptManager.instance.OnReceivePrompt += ShowSession;
    }

    private void OnDisable()
    {
        PromptManager.instance.OnReceivePrompt -= ShowSession;
    }

    public void CreateSession()
    {
        string prompt = "Monte um RPG para mim com uma sessão e cinco personagens. " +
"Eu preciso que o formato das informações do RPG seja em JSON. É crucial que a estrutura do JSON e os nomes das chaves sigam EXATAMENTE os modelos das classes C# fornecidas abaixo. Isso significa que o nome de cada chave no JSON deve ser IDÊNTICO ao nome da propriedade pública correspondente na classe C#. " +
"Modelo para os dados da sessão (SessionDataSO): " +
"As chaves JSON para os dados da sessão devem ser SessionName, SessionDescription, e SessionArcDescription. " +
"public class SessionDataSO : ScriptableObject { public string SessionName; public string SessionDescription; public string SessionArcDescription; } " +
"Modelo para os dados dos personagens (CharacterDataSO): " +
"As chaves JSON para cada personagem devem ser Name, Background, Visual, Ambition, Quirk, MaxHealth, Health, Strength, Dexterity, Constitution, Intelligence, e Charisma. Preste atenção especial para que a chave seja Visual e não outra variação. " +
"public class CharacterDataSO : ScriptableObject { public string Name; public string Background; public string Visual; public string Ambition; public string Quirk; public int MaxHealth; public int Health; public int Strength; public int Dexterity; public int Constitution; public int Intelligence; public int Charisma; } " +
"Exemplo da ESTRUTURA EXATA do JSON esperado (você preencherá os valores): " +
"{ " +
"  \\\"SessionData\\\": { " +
"    \\\"SessionName\\\": \\\"NOME_DA_SESSAO_AQUI\\\", " +
"    \\\"SessionDescription\\\": \\\"DESCRICAO_DA_SESSAO_AQUI\\\", " +
"    \\\"SessionArcDescription\\\": \\\"DESCRICAO_DO_ARCO_DA_SESSAO_AQUI\\\" " +
"  }, " +
"  \\\"Characters\\\": [ " +
"    { " +
"      \\\"Name\\\": \\\"NOME_PERSONAGEM_1\\\", " +
"      \\\"Background\\\": \\\"BACKGROUND_PERSONAGEM_1\\\", " +
"      \\\"Visual\\\": \\\"VISUAL_PERSONAGEM_1\\\", " +
"      \\\"Ambition\\\": \\\"AMBICAO_PERSONAGEM_1\\\", " +
"      \\\"Quirk\\\": \\\"QUIRK_PERSONAGEM_1\\\", " +
"      \\\"MaxHealth\\\": 0, " +
"      \\\"Health\\\": 0, " +
"      \\\"Strength\\\": 0, " +
"      \\\"Dexterity\\\": 0, " +
"      \\\"Constitution\\\": 0, " +
"      \\\"Intelligence\\\": 0, " +
"      \\\"Charisma\\\": 0 " +
"    } " +
"  ] " +
"} " +
$"Dados do RPG para preencher o JSON: Tema: {ThemeRpg.options[ThemeRpg.value].text}. " +
$"Sistema: {SystemRpg.options[SystemRpg.value].text}. " +
$"Soma dos valores de atributos base para cada personagem: {InitialValueRpg.options[InitialValueRpg.value].text} (distribua este total entre Strength, Dexterity, Constitution, Intelligence, Charisma para cada personagem de forma que faça sentido para o conceito do personagem). " +
"Número de Personagens: 5. " +
"Por favor, gere o JSON completo. Certifique-se de que todos os cinco personagens estejam presentes na lista Characters e que todas as chaves JSON correspondam exatamente aos nomes das propriedades das classes C# fornecidas.";


        PromptManager.instance.GenerateText(prompt);
    }

    public void ShowSession(string output)
    {
        jsonImporter.ParseChatGeminiOutput(output);
        Debug.Log(output);
    }

    
}

[System.Serializable]
public class SessionWrapper
{
    public SessionData SessionData;
    public List<CharacterData> Characters;
}

[System.Serializable]
public class SessionData
{
    public string SessionName;
    public string SessionDescription;
    public string SessionArcDescription;
}

[System.Serializable]
public class CharacterData
{
    public string Name;
    public string Background;
    public string Visual;
    public string Ambition;
    public string Quirk;
    public int MaxHealth;
    public int Health;
    public int Strength;
    public int Dexterity;
    public int Constitution;
    public int Intelligence;
    public int Charisma;
}

