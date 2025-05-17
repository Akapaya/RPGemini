using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using UnityEngine.UIElements;

public class PromptManager : MonoBehaviour
{
    private string apiKey = "AIzaSyCKA3XNXBgnabXNMqIFoZNBpsHWoR4Tyqg"; // Replace with your actual API key
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    public Action<string> OnReceivePrompt;

    public static PromptManager instance;

    public void Awake() // Use Awake para garantir que a instância seja definida cedo
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: mantém o objeto entre cenas
        }
        else if (instance!= this)
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        GenerateText($"Você é um mestre de RPG que pegou um RPG em andamento o contexto do RPG atual é: {SessionManager.instance.SessionData.SessionArcDescription}, e o jogador é {SessionManager.instance.SessionData.PlayerCharacter.Name}, dê um resumo do e espere a ação do usuario.");
    }

    /// <summary>
    /// Starts the coroutine to send a prompt to the Gemini API.
    /// </summary>
    /// <param name="prompt">The text prompt to send to the API.</param>
    public void GenerateText(string prompt)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_API_KEY")
        {
            Debug.LogError("API Key not set. Please replace 'YOUR_API_KEY' with your actual API key in the PromptManager script.");
            return;
        }
        Debug.Log(prompt);
        //OnReceivePrompt?.Invoke(prompt);
        StartCoroutine(SendRequest(prompt));
    }

    /// <summary>
    /// Coroutine to handle the web request to the Gemini API.
    /// </summary>
    /// <param name="prompt">The text prompt.</param>
    IEnumerator SendRequest(string prompt)
    {
        // Construct the JSON payload string.
        // Using string interpolation for better readability.
        string jsonPayload = $"{{\"contents\": [{{ \"parts\": [ {{ \"text\": \"{prompt}\" }} ] }}]}}";
        string urlWithKey = $"{apiUrl}?key={apiKey}";

        // Create a new UnityWebRequest.
        using (UnityWebRequest webRequest = new UnityWebRequest(urlWithKey, "POST"))
        {
            // Encode the JSON payload into a byte array.
            // CORRECTED: bodyRaw should be byte[]
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

            // Assign the raw byte array to the upload handler.
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            // Create a new download handler to receive the response.
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            // Set the Content-Type header to application/json.
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Send the request and wait for the response.
            yield return webRequest.SendWebRequest();

            // Check for errors.
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {webRequest.error}");
                Debug.LogError($"Response Code: {webRequest.responseCode}");
                if (webRequest.downloadHandler != null)
                {
                    Debug.LogError($"Error Details: {webRequest.downloadHandler.text}");
                }
            }
            else
            {
                // Get the response text.
                string responseText = webRequest.downloadHandler.text;
                Debug.Log("Gemini Raw Response: " + responseText);
                //SessionManager.instance.SessionData.LastLogs.Add(responseText);               

                // Parse the JSON response using Newtonsoft.Json.
                try
                {
                    JObject jsonResponse = JObject.Parse(responseText);

                    // Navigate through the JSON structure to get the generated text.
                    // It's good practice to check if elements exist before accessing them.
                    if (jsonResponse["candidates"] is JArray candidates && candidates.Count > 0)
                    {
                        if (candidates[0] is JObject candidate && candidate["content"] is JObject content)
                        {
                            if (content["parts"] is JArray parts && parts.Count > 0)
                            {
                                if (parts[0] is JObject part && part["text"] is JValue textValue)
                                {
                                    string generatedText = textValue.ToString();
                                    Debug.Log("Generated Text: " + generatedText);
                                    OnReceivePrompt?.Invoke(generatedText);
                                    // TODO: Display the generated text in your game
                                    // (e.g., using a UI Text component or calling another method).
                                    // Example: FindObjectOfType<YourUITextScript>().DisplayText(generatedText);
                                }
                                else
                                {
                                    Debug.LogError("Error parsing JSON: 'text' field not found or not a JValue in the first part.");
                                }
                            }
                            else
                            {
                                Debug.LogError("Error parsing JSON: 'parts' array not found or empty in the first candidate's content.");
                            }
                        }
                        else
                        {
                            Debug.LogError("Error parsing JSON: 'content' object not found in the first candidate.");
                        }
                    }
                    else
                    {
                        // Check for API error messages if candidates array is missing or empty
                        if (jsonResponse["error"] is JObject errorDetails)
                        {
                            Debug.LogError($"API Error: {errorDetails["message"]}");
                        }
                        else
                        {
                            Debug.LogError("Error parsing JSON: 'candidates' array not found, empty, or API returned an error.");
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error parsing JSON response: {e.Message}\nStackTrace: {e.StackTrace}");
                }
            }
        }
    }

    public void AttackNPCPrompt(CharacterDataSO firstCharacter, CharacterDataSO secondCharacter)
    {
        int rollD20Character1 = UnityEngine.Random.Range(0, 21);
        int rollD20Character2 = UnityEngine.Random.Range(0, 21);
        string prompt = $"{firstCharacter.Name} rodou {rollD20Character1} e atacou o {secondCharacter.Name} que rodou defesa/esquiva {rollD20Character2} no combate" +
            $"\n" +
            $"########### \n" +
            $"O primeiro personagem tem o status atual de :\n" +
            $"Nome: {firstCharacter.Name}\n" +
            $"Quirk: {firstCharacter.Quirk}\n" +
            $"Health: {firstCharacter.Health}\n" +
            $"Força: {secondCharacter.Strength}\n" +
            $"Destreza: {secondCharacter.Dexterity}\n" +

        $"###########\n" +
        $"O segundo personagem tem o status atual de :\n" +
        $"Nome: {secondCharacter.Name}\n" +
        $"Quirk: {secondCharacter.Quirk}\n" +
        $"Health: {secondCharacter.Health}\n" +
        $"Destreza: {firstCharacter.Dexterity}\n" +
        $"Consistencia: {firstCharacter.Constitution}\n" +

        $"###########\n" +
        $"Descreva APENAS a cena deles e o resultado desta ação para o contexto do RPG.";

        GenerateText(prompt);
    }

    public void DefendNPCPrompt(CharacterDataSO firstCharacter)
    { 
        string prompt = $"{firstCharacter.Name} entrou em posição de defesa no combate" +
            $"\n" +
            $"########### \n" +
            $"O primeiro personagem tem o status atual de :\n" +
            $"Nome: {firstCharacter.Name}\n" +
            $"Quirk: {firstCharacter.Quirk}\n" +
            $"Health: {firstCharacter.Health}\n" +
            $"Força: {firstCharacter.Strength}\n" +
            $"Destreza: {firstCharacter.Dexterity}\n" +
            $"Consistencia: {firstCharacter.Constitution}\n" +

        $"###########\n" +
        $"Descreva APENAS a cena dele e o resultado desta ação para o contexto do RPG.";

        GenerateText(prompt);
    }

    public void SkillNPCPrompt(CharacterDataSO firstCharacter, CharacterDataSO secondCharacter)
    {
        int rollD20Character1 = UnityEngine.Random.Range(0, 21);
        int rollD20Character2 = UnityEngine.Random.Range(0, 21);
        string prompt = $"{firstCharacter.Name} rodou {rollD20Character1} e usou a skill {firstCharacter.Quirk} no {secondCharacter.Name} que rodou defesa/esquiva {rollD20Character2} no combate" +
            $"\n" +
            $"########### \n" +
            $"O primeiro personagem tem o status atual de :\n" +
            $"Nome: {firstCharacter.Name}\n" +
            $"Health: {firstCharacter.Health}\n" +
            $"Força: {secondCharacter.Strength}\n" +
            $"Destreza: {secondCharacter.Dexterity}\n" +

        $"###########\n" +
        $"O segundo personagem tem o status atual de :\n" +
        $"Nome: {secondCharacter.Name}\n" +
        $"Quirk: {secondCharacter.Quirk}\n" +
        $"Health: {secondCharacter.Health}\n" +
        $"Destreza: {firstCharacter.Dexterity}\n" +
        $"Consistencia: {firstCharacter.Constitution}\n" +

        $"###########\n" +
        $"Descreva APENAS a cena deles e o resultado desta ação para o contexto do RPG.";

        GenerateText(prompt);
    }

    public void ChallengeNPCPrompt(CharacterDataSO firstCharacter, CharacterDataSO secondCharacter)
    {
        string prompt = $"{firstCharacter.Name} desafiou o {secondCharacter.Name} para um combate de treinamento." +
            $"\n" +
            $"########### \n" +
            $"O primeiro personagem tem o status atual de :\n" +
            $"Nome: {firstCharacter.Name}\n" +
            $"Background: {firstCharacter.Background}\n" +
            $"Quirk: {firstCharacter.Quirk}\n" +
            $"Health: {firstCharacter.Health}\n" +
            $"Força: {firstCharacter.Strength}\n" +
            $"Destreza: {firstCharacter.Dexterity}\n" +
            $"Consistencia: {firstCharacter.Constitution}\n" +
            $"Inteligencia: {firstCharacter.Intelligence}\n" +
            $"Carisma: {firstCharacter.Charisma}\n" +

        $"###########\n" +
        $"O segundo personagem tem o status atual de :\n" +
        $"Nome: {secondCharacter.Name}\n" +
        $"Background: {secondCharacter.Background}\n" +
        $"Quirk: {secondCharacter.Quirk}\n" +
        $"Health: {secondCharacter.Health}\n" +
        $"Força: {secondCharacter.Strength}\n" +
        $"Destreza: {secondCharacter.Dexterity}\n" +
        $"Consistencia: {secondCharacter.Constitution}\n" +
        $"Inteligencia: {secondCharacter.Intelligence}\n" +
        $"Carisma: {secondCharacter.Charisma}\n" +
        $"###########\n" +
        $"Descreva APENAS a cena deles se desafiando para o contexto do RPG.";

        GenerateText(prompt);
    }

    public void StealthNPCPrompt(CharacterDataSO firstCharacter)
    {
        int rollD20Character1 = UnityEngine.Random.Range(0, 21);
        string prompt = $"{firstCharacter.Name} está tentando usar sua furtividade na cena atual ele rodou {rollD20Character1} no d20." +
            $"\n" +
            $"########### \n" +
            $"O primeiro personagem tem o status atual de :\n" +
            $"Nome: {firstCharacter.Name}\n" +
            $"Background: {firstCharacter.Background}\n" +
            $"Quirk: {firstCharacter.Quirk}\n" +
            $"Health: {firstCharacter.Health}\n" +
            $"Destreza: {firstCharacter.Dexterity}\n" +
            $"Consistencia: {firstCharacter.Constitution}\n" +

        $"###########\n" +
        $"Descreva APENAS a cena dele e o resultado para o contexto do RPG.";

        GenerateText(prompt);
    }

    public void GreetNPCPrompt(CharacterDataSO firstCharacter, CharacterDataSO secondCharacter)
    {
        string prompt = $"{firstCharacter.Name} comprimentou o {secondCharacter.Name} na cena atual" +
            $"\n" +
            $"########### \n" +
            $"O primeiro personagem tem o status atual de :\n" +
            $"Nome: {firstCharacter.Name}\n" +
            $"Background: {firstCharacter.Background}\n" +
            $"Quirk: {firstCharacter.Quirk}\n" +
            $"Carisma: {firstCharacter.Charisma}\n" +

        $"###########\n" +
        $"O segundo personagem tem o status atual de :\n" +
        $"Nome: {secondCharacter.Name}\n" +
        $"Background: {secondCharacter.Background}\n" +
        $"Carisma: {secondCharacter.Charisma}\n" +
        $"###########\n" +
        $"Descreva APENAS a cena deles se comprimentando para o contexto do RPG.";

        GenerateText(prompt);
    }

    public void AskNPCPrompt(CharacterDataSO firstCharacter, CharacterDataSO secondCharacter, string question)
    {
        string prompt = $"{firstCharacter.Name} perguntou para o {secondCharacter.Name} {question}" +
            $"\n" +
            $"########### \n" +
            $"O primeiro personagem tem o status atual de :\n" +
            $"Nome: {firstCharacter.Name}\n" +
            $"Background: {firstCharacter.Background}\n" +
            $"Quirk: {firstCharacter.Quirk}\n" +
            $"Carisma: {firstCharacter.Charisma}\n" +

        $"###########\n" +
        $"O segundo personagem tem o status atual de :\n" +
        $"Nome: {secondCharacter.Name}\n" +
        $"Background: {secondCharacter.Background}\n" +
        $"Carisma: {secondCharacter.Charisma}\n" +
        $"###########\n" +
        $"Descreva APENAS a cena deles e a resposta para o contexto do RPG.";

        GenerateText(prompt);
    }

    public void IntimidateNPCPrompt(CharacterDataSO firstCharacter, CharacterDataSO secondCharacter)
    {
        string prompt = $"{firstCharacter.Name} tentou intimidaro {secondCharacter.Name}" +
            $"\n" +
            $"########### \n" +
            $"O primeiro personagem tem o status atual de :\n" +
            $"Nome: {firstCharacter.Name}\n" +
            $"Background: {firstCharacter.Background}\n" +
            $"Quirk: {firstCharacter.Quirk}\n" +
            $"Carisma: {firstCharacter.Charisma}\n" +

        $"###########\n" +
        $"O segundo personagem tem o status atual de :\n" +
        $"Nome: {secondCharacter.Name}\n" +
        $"Background: {secondCharacter.Background}\n" +
        $"Carisma: {secondCharacter.Charisma}\n" +
        $"###########\n" +
        $"Descreva APENAS a cena deles e o resultado para o contexto do RPG.";

        GenerateText(prompt);
    }

    public void ShoutNPCPrompt(CharacterDataSO firstCharacter, CharacterDataSO secondCharacter, string question)
    {
        string prompt = $"{firstCharacter.Name} tentou gritar para {secondCharacter.Name} {question}" +
            $"\n" +
            $"########### \n" +
            $"O primeiro personagem tem o status atual de :\n" +
            $"Nome: {firstCharacter.Name}\n" +
            $"Background: {firstCharacter.Background}\n" +
            $"Quirk: {firstCharacter.Quirk}\n" +
            $"Carisma: {firstCharacter.Charisma}\n" +

        $"###########\n" +
        $"O segundo personagem tem o status atual de :\n" +
        $"Nome: {secondCharacter.Name}\n" +
        $"Background: {secondCharacter.Background}\n" +
        $"Carisma: {secondCharacter.Charisma}\n" +
        $"###########\n" +
        $"Descreva APENAS a cena deles e o resultado para o contexto do RPG.";

        GenerateText(prompt);
    }

    public void PersuadeNPCPrompt(CharacterDataSO firstCharacter, CharacterDataSO secondCharacter, string question)
    {
        int rollD20Character1 = UnityEngine.Random.Range(0, 21);

        string prompt = $"{firstCharacter.Name} tentou persuadir o {secondCharacter.Name} e rodou no d20 {rollD20Character1}, ele tentou para {question}" +
            $"\n" +
            $"########### \n" +
            $"O primeiro personagem tem o status atual de :\n" +
            $"Nome: {firstCharacter.Name}\n" +
            $"Background: {firstCharacter.Background}\n" +
            $"Quirk: {firstCharacter.Quirk}\n" +
            $"Carisma: {firstCharacter.Charisma}\n" +

        $"###########\n" +
        $"O segundo personagem tem o status atual de :\n" +
        $"Nome: {secondCharacter.Name}\n" +
        $"Background: {secondCharacter.Background}\n" +
        $"Carisma: {secondCharacter.Charisma}\n" +
        $"###########\n" +
        $"Descreva APENAS a cena deles e o resultado para o contexto do RPG.";

        GenerateText(prompt);
    }

    public void SendPrompt(Actions currentAction, string currentTarget, string prompt = "")
    {
        var player = SessionManager.instance.SessionData.PlayerCharacter;
        var target = SessionManager.instance.SessionData.CharactersData.First(o=> o.Name == currentTarget);
        switch (currentAction)
        {
            case Actions.Talk:
                {
                    AttackNPCPrompt(player, target);
                    break;
                }
        }
    }

    public void SendTalkPrompt(DialogueData dialogueData)
    {
        var player = SessionManager.instance.SessionData.PlayerCharacter;
        string targetComplement = "";

        if (dialogueData.Target != "Everyone")
        {
            var target = SessionManager.instance.SessionData.CharactersData.First(o => o.Name == dialogueData.Target);
            targetComplement = $"O alvo da fala é o personagem {target.Name}" +
                $"ele tem os seguintes status:" +
                $"Nome: {target.Name}\n" +
                $"Background: {target.Background}\n" +
                $"Quirk: {target.Quirk}\n" +
                $"Carisma: {target.Charisma}\n" +
                $"O relacionamento entre eles de 0-100 é: {UnityEngine.Random.Range(0, 101)}";
        }    
        else
        {
            targetComplement = "O personagem falou para todos na cena, esses personagens são: \n";
            foreach (var character in SessionManager.instance.SessionData.InSceneCharactersName)
            {
                var target = SessionManager.instance.SessionData.CharactersData.First(o => o.Name == character);
                targetComplement += $"Um dos personagens é {target.Name}" +
                    $"ele tem os seguintes status:" +
                    $"Nome: {target.Name}\n" +
                    $"Background: {target.Background}\n" +
                    $"Quirk: {target.Quirk}\n" +
                    $"Carisma: {target.Charisma}\n" +
                    $"O relacionamento entre eles de 0-100 é: {UnityEngine.Random.Range(0,101)} \n";
            }
        }

        string prompt = $"Essa é uma cena de RPG de uma fala, o contexto desta cena é {SessionManager.instance.SessionData.SessionArcDescription}" +
            $"########### \n" +
            $"{player.Name} falou para {dialogueData.Target} {dialogueData.Prompt}" +
            $"\n" +
            $"########### \n" +
            $"O jogador tem o status atual de :\n" +
            $"Nome: {player.Name}\n" +
            $"Background: {player.Background}\n" +
            $"Quirk: {player.Quirk}\n" +
            $"Carisma: {player.Charisma}\n" +
            $"Este personagem falou com o objetivo {dialogueData.Objective} e da seguinte forma {dialogueData.Mood}" +

        $"###########\n";

        prompt += targetComplement;

        string ResponseFormat = GetResponseFormat(Actions.Talk);

        if(ResponseFormat == string.Empty)
        {
            Debug.LogError("Älgum erro no formato da resposta");
            return;
        }
        prompt += ResponseFormat;

        GenerateText(prompt);
    }

    private string GetResponseFormat(Actions actions)
    {
        switch (actions)
        {
            case Actions.Talk:
                {
                    return $"########### \n" +
                    $"Descreva APENAS a cena deles e o resultado para o contexto do RPG e caso algum ouvinte tenha alguma outra fala junto com essa." +
                    $"Por fim escreva o impacto em cada ouvinte para a relação da seguinte formula:" +
                    $"Relacionamento:" +
                    $"ObjetoJson" +
                    $"{{" +
                    $"    NameCharacter;" +
                    $"    NameOfWhoTalked;" +
                    $"    Feedback;" +
                    $"}}" +
                    $"MuitoNegativo," +
                    $"Negativo," +
                    $"Neutro," +
                    $"Positivo," +
                    $"MuitoPositivo";
                }
        }

        return string.Empty;
    }

    public void SendPrompt(string v)
    {
        GenerateText(v);
    }

    public void AskContextPrompt(string v)
    {
        GenerateText(v);
        Debug.Log(SessionManager.instance.SessionData.LastLogs.Last());
    }
}

public readonly struct DialogueData
{
    public readonly CharacterDataSO MainChar;
    public readonly string Prompt;
    public readonly Objective Objective;
    public readonly Mood Mood;
    public readonly string Target;

    public DialogueData(CharacterDataSO mainChar, string prompt, Objective objective, Mood mood, string target)
    {
        MainChar = mainChar;
        Prompt = prompt;
        Objective = objective;
        Mood = mood;
        Target = target;
    }
}

public enum Objective
{
    ApenasConversar,
    PedirConselhos,
    Reclamar,
    Informar,
    PedirAjuda,
    CompartilharExperiencia,
    ExpressarGratidao,
    Desabafar,
    Negociar,
    Ensinar,
    Aprender,
    Debater,
    PedirDesculpas,
    Reconciliar,
    Inspirar,
    Motivar,
    Persuadir,
    Entreter,
    Flertar
}

public enum Mood
{
    Neutro,
    Feliz,
    Triste,
    Agressivo,
    Ansioso,
    Confiante,
    Curioso,
    Deprimido,
    Empolgado,
    Frustrado,
    Culpado,
    Esperançoso,
    Solitario,
    Nostalgico,
    Otimista,
    Pensativo,
    Orgulhoso,
    Aliviado,
    Romantico,
    Cetico,
    Estressado,
    Surpreso,
    Cansado,
    Preocupado
}

public enum Feedback
{
    MuitoNegativo,
    Negativo,
    Neutro,
    Positivo,
    MuitoPositivo
}

public readonly struct RelationChangeData
{
    public readonly string NameCharacter;
    public readonly string NameOfWhoTalked;
    public readonly Feedback Feedback;

    public RelationChangeData(string nameCharacter, string nameOfWhoTalked, Feedback feedback)
    {
        NameCharacter = nameCharacter;
        NameOfWhoTalked = nameOfWhoTalked;
        Feedback = feedback;
    }
}