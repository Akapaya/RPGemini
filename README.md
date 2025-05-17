# RPGemini

![Unity Version](https://img.shields.io/badge/Unity-2023.3%2B-blue) ![Status do Projeto](https://img.shields.io/badge/Status-Em%20Desenvolvimento-orange)

Um jogo de RPG Solo e Ferramenta de Criação de Campanhas utilizando Unity e a API Gemini para superar as limitações de contexto em sessões de RPG baseadas em IA.

## Sumário
- [Introdução](#introdução)
- [O Problema](#o-problema)
- [A Solução](#a-solução)
- [Arquitetura e Conceitos Principais](#arquitetura-e-conceitos-principais)
- [Detalhes da Implementação (Protótipo Atual)](#detalhes-da-implementação-protótipo-atual)
  - [Integração da API Gemini](#integração-da-api-gemini)
  - [Modelagem de Dados com Scriptable Objects](#modelagem-de-dados-com-scriptable-objects)
  - [Interface do Usuário e Fluxo de Ações](#interface-do-usuário-e-fluxo-de-ações)
  - [Engenharia de Prompts e Processamento de Respostas](#engenharia-de-prompts-e-processamento-de-respostas)
  - [Criação Inicial de Campanha](#criação-inicial-de-campanha)
- [O Papel da IA (API Gemini)](#o-papel-da-ia-api-gemini)
- [Próximos Passos / Roadmap](#próximos-passos--roadmap)
- [Como Começar (Em Desenvolvimento)](#como-começar-em-desenvolvimento)
- [Licença](#licença)

## Introdução

RPGemini é um projeto inovador que combina a flexibilidade da **Unity** como engine de jogo e ferramenta de gerenciamento de dados com a capacidade de geração textual da **API Gemini** do Google. O objetivo principal é criar uma experiência robusta para **jogar RPG de mesa solo** e **auxiliar na criação de campanhas**, superando as limitações comuns encontradas ao usar chatbots tradicionais para esse fim.

## O Problema

Muitos entusiastas de RPG de mesa enfrentam a dificuldade de reunir jogadores regularmente. Uma alternativa buscada é a utilização de chatbots para simular um mestre de jogo ou companheiros. No entanto, uma limitação recorrente e frustrante nesses testes é a **perda de contexto** ao longo de uma sessão estendida. Informações cruciais estabelecidas no início da conversa (como habilidades de um personagem, detalhes do cenário, ou eventos passados) são frequentemente "esquecidas" pelo chatbot à medida que o histórico da conversa cresce. Isso quebra a imersão e inviabiliza campanhas mais longas e complexas.

O motivo, como explicado por modelos de linguagem, reside na **janela de contexto limitada**. O modelo processa primariamente as mensagens mais recentes, fazendo com que dados antigos se percam gradualmente.

![image](https://github.com/user-attachments/assets/acc09cb3-7285-4657-acfa-21f9bf236afc)
*Ilustração da perda de contexto em chatbots tradicionais.*

## A Solução

A ideia central por trás do RPGemini é **transferir o gerenciamento dos dados persistentes da campanha da memória volátil da IA para um sistema estruturado dentro da Unity**. Em vez de depender do contexto do chat para lembrar detalhes da campanha, personagens, ações, etc., esses dados são armazenados de forma permanente (em saves de jogo, por exemplo) utilizando recursos da Unity.

A API Gemini, neste cenário, atua como um **narrador e interpretador**, recebendo prompts cuidadosamente elaborados pela Unity contendo apenas as informações **necessárias e contextualmente relevantes para a ação atual**. A Unity processa a resposta da IA, atualiza seus próprios dados conforme necessário e gerencia o estado do jogo.

**Benefícios dessa Abordagem:**

* **Persistência de Dados:** Informações cruciais da campanha e dos personagens são salvas e nunca são esquecidas.
* **Maior Controle:** O desenvolvedor (ou futuro mestre/jogador) tem controle total sobre os dados e a lógica do jogo.
* **Campanhas Múltiplas:** Possibilidade de ter diversas campanhas em andamento simultaneamente.
* **Experiência de RPG Solo Aprimorada:** Um ambiente dedicado para jogar campanhas completas.
* **Projeto Educacional e Portfólio:** Uma implementação prática e complexa que demonstra proficiência.

Este é um projeto de longo prazo devido à sua complexidade, mas a base e a prova de conceito já foram estabelecidas.

## Arquitetura e Conceitos Principais

O projeto se baseia na seguinte estrutura:

* **Unity:** A engine principal responsável por:
    * Gerenciamento da interface do usuário.
    * Armazenamento e gerenciamento de dados persistentes da campanha e personagens.
    * Processamento da lógica de jogo (ações, testes, combate - *futuro*).
    * Construção e envio de prompts estruturados para a API Gemini.
    * Processamento e aplicação das respostas da API Gemini.
* **API Gemini (Google AI):** Responsável por:
    * Gerar texto narrativo e descritivo com base nos prompts recebidos.
    * Interpretar o contexto fornecido nos prompts para gerar respostas coerentes.
* **Scriptable Objects (Unity):** Utilizados como a principal ferramenta para modelar e armazenar os dados estruturados da campanha (Sessão, Personagens, Locais, Itens, etc.), permitindo fácil serialização e gerenciamento.

## Detalhes da Implementação (Protótipo Atual)

Esta seção detalha os passos iniciais e o estado atual do protótipo:

### Integração da API Gemini

A integração da API Gemini na Unity foi realizada utilizando bibliotecas adequadas (como pacotes de JSON para parseamento). Testes iniciais validaram a comunicação e a capacidade de receber respostas da API.

![Screenshot 2025-05-15 142129](https://github.com/user-attachments/assets/5e0fa728-789e-47fc-ad7b-27f86de73896)
*Teste inicial de comunicação com a API Gemini.*

### Modelagem de Dados com Scriptable Objects

A estrutura de dados fundamental da campanha está sendo construída utilizando Scriptable Objects (SOs). Isso permite definir *templates* para diferentes tipos de dados e criar instâncias persistentes desses dados no projeto.

* **`SessionData` SO:** Contém informações globais da campanha (título, descrição, arco atual, lista de todos os personagens, personagens na cena atual, logs, etc.).
* **`CharacterData` SO:** Contém atributos e detalhes específicos de um personagem (nome, background, habilidades, atributos como força/inteligência, etc.).

![image](https://github.com/user-attachments/assets/341e1549-6d43-434b-ae89-5628b9db65f6)
*Exemplo da estrutura de dados `SessionData`.*

![image](https://github.com/user-attachments/assets/5a490474-0cdb-4c42-a5cb-d58c93fb35a9)
*Exemplo da estrutura de dados `CharacterData`.*

### Interface do Usuário e Fluxo de Ações

Foi desenvolvida uma interface de usuário básica (mockup) para testar as interações. Botões para ações comuns de RPG (Falar, Agir, Verificar) foram implementados para simular o fluxo de jogo. O foco inicial foi na ação de "Falar" para testar a passagem de contexto para a IA.

https://github.com/user-attachments/assets/91888bf2-4e7e-4efe-b518-ab0ff29287e2
*Vídeo: Interface do usuário inicial e interação básica.*

### Engenharia de Prompts e Processamento de Respostas

Um aspecto crucial é a formatação dos prompts enviados para a API Gemini e o processamento de suas respostas. Para garantir consistência e evitar erros inesperados, os prompts são gerados de forma estruturada pela Unity, incluindo exemplos de formato de resposta esperado (especialmente para JSON).

![image](https://github.com/user-attachments/assets/1a7e7bc2-6fa6-4e3a-9107-e398fb41d1cb)
*Exemplo de estrutura de prompt para a ação de Falar.*

**Exemplo - Ação Falar:**
Ao escolher um personagem para falar e o que ele diz, a Unity constrói o prompt incluindo:

* Identificação do personagem falante e do personagem ouvinte.
* Trechos relevantes do background ou estado atual dos personagens.
* O texto exato que o personagem está falando.

Isso permite que a API Gemini gere uma resposta narrativa coerente, considerando nuances como, por exemplo, um personagem que não pode falar reagir apenas por gestos.

https://github.com/user-attachments/assets/4a8447df-c30e-4466-bb64-ba06567be540
*Vídeo: Exemplo inicial da ação de Falar, com resposta da IA baseada em contexto limitado.*

Melhorias subsequentes na ação "Falar" incluíram a adição de campos para o objetivo e tom da fala, dando ao jogador mais controle e fornecendo à IA contexto adicional para a narração.

![image](https://github.com/user-attachments/assets/53cd8c8d-6582-45db-b46e-76c92c772235)
*UI aprimorada para a ação de Falar, incluindo objetivo e tom.*

https://github.com/user-attachments/assets/5f3930a2-2e1e-4683-8bf2-5cb46b845d85
*Vídeo: Ação de Falar aprimorada com objetivo e tom.*

### Criação Inicial de Campanha

Para testar a capacidade de consumir dados estruturados gerados pela IA, foi implementado um fluxo de criação inicial de campanha. O jogador seleciona o tema da campanha, o sistema de RPG base (conceitual) e a base inicial de atributos para os personagens.

A Unity envia um prompt para a API Gemini solicitando a geração de dados para a sessão e personagens iniciais em formato JSON, seguindo uma estrutura pré-definida.

![image](https://github.com/user-attachments/assets/d2cd0cee-ab3c-4c92-99f1-7d1ff9bd7f38)
*Estrutura do Prompt de criação da campanha.*

Um leitor de JSON na Unity processa a resposta da API, extrai o JSON relevante e popula SOs temporários. Esses dados são exibidos na UI para revisão do jogador, que pode aceitar a campanha gerada ou solicitar uma nova.

![image](https://github.com/user-attachments/assets/8ddcf912-eb3a-4ccc-986e-c5e639f1e38f)
*Json Importer que lidará com os dados recebidos em json*

https://github.com/user-attachments/assets/294abf31-9488-4432-9aa4-a411f12cbdb0
*Vídeo: Fluxo de criação inicial de campanha, geração e exibição de dados pela IA.*

## O Papel da IA (API Gemini)

É fundamental entender a divisão de responsabilidades neste projeto:

* **Unity:** É o "cérebro" do jogo. Gerencia *todos* os dados, a lógica do jogo (movimentação, combate, interações baseadas em atributos, etc.) e o estado da campanha.
* **API Gemini:** Atua como o **narrador**. Sua função é receber o contexto de uma ação (fornecido pela Unity) e gerar texto descritivo e envolvente para essa cena.

**Exemplo:** Se um personagem ataca um inimigo:
1.  A **Unity** processa a ação do jogador, calcula o sucesso/falha do ataque, o dano causado com base nos atributos e rolamentos (simulados ou reais).
2.  A **Unity** então cria um prompt para a **API Gemini** descrevendo o *resultado* da ação (Ex: "O Personagem X acertou um golpe crítico no Inimigo Y, causando [X] de dano. Eles estão em uma masmorra escura.").
3.  A **API Gemini** recebe o prompt e gera uma **descrição narrativa** vívida da cena (Ex: "Com um grito de guerra, [Nome do Personagem X] desfere um golpe perfeito que encontra a armadura do [Nome do Inimigo Y] com um barulho metálico estrondoso. O impacto é massivo, fazendo o inimigo cambalear para trás, com a fumaça sutilmente subindo do ponto de impacto.").

A IA não decide se o ataque acerta ou quanto dano ele causa; ela apenas descreve o *resultado* da ação que foi processada pela Unity. Isso garante consistência e controle sobre a jogabilidade.

## Conclusão

Este projeto está em fase inicial de prototipagem, É um projeto ambicioso, mas os resultados iniciais são promissores.
