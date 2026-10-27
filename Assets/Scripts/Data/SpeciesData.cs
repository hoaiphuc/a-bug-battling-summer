using UnityEngine;

[CreateAssetMenu(menuName = "Bug Battle/Species", fileName = "Species_")]
public class SpeciesData : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField, Min(1)] public int Courage { get; private set; } = 80; // Dũng khí
    [field: SerializeField, Min(0)] public int Attack { get; private set; } = 70;
    [field: SerializeField, Min(0)] public int Shell { get; private set; } = 60;
    [field: SerializeField, Min(1)] public int Speed { get; private set; } = 70;
    [field: SerializeField] public Color Color { get; private set; } = Color.white;

    [field: SerializeField] public SkillData Skill1 { get; private set; }
    [field: SerializeField] public SkillData Skill2 { get; private set; }
}
