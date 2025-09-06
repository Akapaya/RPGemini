using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering.VirtualTexturing;

public class RPGChatClient : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text chatOutput;

    private const string SYSTEM =
        "Você é um Mestre de RPG veterano. Sempre responda narrando ações, cenários e " +
        "reações dos NPCs usando apenas as regras fornecidas. Nunca diga que seu conhecimento " +
        "é limitado ou mencione o manual.";

    private void Start()
    {
        //OnSendMessage();
    }

    // Lista de tuplas (remetente, texto)
    private List<(string speaker, string text)> history = new List<(string, string)>();

    // Número máximo de entradas que você quer enviar de contexto
    private const int MaxHistoryEntries = 20;

    public void OnSendMessage()
    {
        string playerMessage = inputField.text;
        AppendToHistory("Jogador", playerMessage);
        //StartCoroutine(SendMessageToServer());
    }

    private void AppendToHistory(string speaker, string text)
    {
        history.Add((speaker, text));
        // Limita o tamanho da lista
        if (history.Count > MaxHistoryEntries)
            history.RemoveAt(0);
    }

    // Monta um único string de prompt com as últimas entradas
    private string BuildPrompt()
    {
        var sb = new System.Text.StringBuilder();
        // 1) System prompt
        sb.AppendLine("Você é um Mestre de RPG veterano. " +
                      "Sempre responda narrando ações, cenários e reações dos NPCs " +
                      "usando apenas as regras fornecidas. Nunca diga que seu conhecimento " +
                      "é limitado ou mencione o manual.");
        // 2) Histórico estruturado
        foreach (var (speaker, text) in history)
            sb.AppendLine($"{speaker}: {text}");
        // 3) Prepare a vez do Mestre
        sb.Append("Mestre:");
        return sb.ToString();
    }

    IEnumerator SendMessageToServer(string playerMessage, bool useFilteredContext, string filteredContext = "")
    {
        // 1) Monta o prompt final
        string prompt;
        if (useFilteredContext)
        {
            prompt = $"{SYSTEM}\n{filteredContext}\nJogador: {playerMessage}\nMestre:";
        }
        else
        {
            // contexto completo
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(SYSTEM);
            foreach (var (speaker, text) in history)
                sb.AppendLine($"{speaker}: {text}");
            sb.AppendLine($"Jogador: {playerMessage}");
            sb.Append("Mestre:");
            prompt = sb.ToString();
        }

        // 2) Prepara JSON
        var req = new ChatRequest { prompt = prompt };
        string jsonData = JsonUtility.ToJson(req);

        // 3) Chama o Flask
        var request = new UnityWebRequest("http://localhost:5000/chat", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var responseData = JsonUtility.FromJson<ChatRequest>(request.downloadHandler.text);

            // armazena e exibe
            AppendToHistory("Mestre", responseData.prompt);
            chatOutput.text += $"\nMestre: {responseData.prompt}";
        }
        else
        {
            chatOutput.text += "\n[Mestre: erro de comunicação]";
        }
    }

    [ContextMenu("1")]
    public void Message1()
    {
        string playerMessage = "Quero jogar um RPG";
        AppendToHistory("Jogador", playerMessage);
        StartCoroutine(SendMessageToServer(playerMessage, useFilteredContext: false));
    }

    [ContextMenu("2")]
    public void Message2()
    {
        string playerMessage = "Quero ser um heroi que tem o poder das trevas";
        AppendToHistory("Jogador", playerMessage);
        StartCoroutine(SendMessageToServer(playerMessage, useFilteredContext: false));
    }

    [ContextMenu("3")]
    public void Message3()
    {
        string playerMessage = "Pode começar a aventura";
        AppendToHistory("Jogador", playerMessage);
        StartCoroutine(SendMessageToServer(playerMessage, useFilteredContext: false));
    }

    [ContextMenu("4")]
    public void Message4()
    {
        string playerMessage = "Pode me lembrar qual foi minha primeira mensagem e qual poder eu tenho?";
        AppendToHistory("Jogador", playerMessage);
        StartCoroutine(HandleMemoryQuestion(playerMessage));
    }

    IEnumerator HandleMemoryQuestion(string question)
    {
        // 1) extrai palavras‑chave simples da pergunta
        var keywords = ExtractKeywords(question);

        // 2) filtra entradas do history que contenham algum keyword
        var relevant = history
            .Where(entry => keywords.Any(kw => entry.text.ToLower().Contains(kw)))
            .ToList();

        // 3) monta prompt só com essas entradas
        string filteredContext = string.Join("\n", relevant
            .Select(e => $"{e.speaker}: {e.text}"));

        // 4) envia ao servidor usando esse mini‑contexto
        yield return SendMessageToServer(question, useFilteredContext: true, filteredContext);
    }

    IEnumerable<string> ExtractKeywords(string text)
    {
        // quebra em palavras e retorna só as que têm 4+ caracteres
        return text
            .ToLower()
            .Split(new[] { ' ', ',', '.', '?', '!', ';' }, System.StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length >= 4)
            .Distinct();
    }

    [System.Serializable]
    public class ChatRequest
    {
        public string prompt;
    }
}
