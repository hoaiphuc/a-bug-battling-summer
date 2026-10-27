using UnityEngine;

[CreateAssetMenu(menuName = "Bug Battle/Skill", fileName = "Skill_")]
public class SkillData : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField, Range(0, 3)] public int EnergyCost { get; private set; } = 1;
    [field: SerializeField, Min(0f)] public float Power { get; private set; } = 1f; // hệ số sát thương
    [field: SerializeField] public TargetType Targeting { get; private set; }
}
