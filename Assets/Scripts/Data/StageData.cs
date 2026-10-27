using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bug Battle/Stage", fileName = "Stage_")]
public class StageData : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; private set; }  // ví dụ "1-1"
    [field: SerializeField, Min(1)] public int Columns { get; private set; } = 3;
    [field: SerializeField, Min(1)] public int Lanes { get; private set; } = 3;
    [field: SerializeField] public List<UnitSlot> Enemies { get; private set; } = new();
}
