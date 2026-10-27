using UnityEngine;

public class DamageCheck : MonoBehaviour
{
    [SerializeField] private SpeciesData attacker;
    [SerializeField] private SpeciesData defender;

    void Start()
    {
        int dmg = DamageCalculator.CalculateDamage(attacker.Attack, 1f, defender.Shell);
        Debug.Log($"{attacker.DisplayName} đánh thường {defender.DisplayName}: {dmg}");
    }

}
