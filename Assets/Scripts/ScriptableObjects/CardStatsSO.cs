using UnityEngine;

[CreateAssetMenu()]
public class CardStatsSO : ScriptableObject
{
    public string cardName;
    public string description;
    public int manaCost;
    public int attack;
    public int hp;
}
