using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character/CharacterSO")]
public class CharacterDataSO : ScriptableObject
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
    public List<string> InventaryList = new();
    public List<string> SpecialAbility = new();
    public Dictionary<string, int> Relations = new();

    public int GetRelationScoreOf(string name)
    {
        return Relations[name];
    }

    public void DoCalculationInRelationScore(Feedback feedback, string name)
    {
        switch (feedback)
        {
            case Feedback.MuitoNegativo:
                {
                    Relations[name] = Relations[name] - 10;
                    break;
                }
            case Feedback.Negativo:
                {
                    Relations[name] = Relations[name] - 5;
                    break;
                }
            case Feedback.Neutro:
                {
                    break;
                }
            case Feedback.Positivo:
                {
                    Relations[name] = Relations[name] + 5;
                    break;
                }
            case Feedback.MuitoPositivo:
                {
                    Relations[name] = Relations[name] + 10;
                    break;
                }
        }
    }
}
