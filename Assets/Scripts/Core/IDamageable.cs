using System.Collections.Generic;
using UnityEngine;

public enum DamageableTeam
{
    Player,
    Ally,
    Enemy
}

public interface IDamageable
{
    DamageableTeam Team { get; }
    bool IsDead { get; }
    Transform TargetTransform { get; }
    Collider2D HitCollider { get; }
    void TakeDamage(int amount);
}

public static class DamageableRegistry
{
    private static readonly List<IDamageable> damageables = new List<IDamageable>();

    public static IReadOnlyList<IDamageable> All => damageables;

    public static void Register(IDamageable damageable)
    {
        if (damageable == null) return;
        if (damageables.Contains(damageable)) return;
        damageables.Add(damageable);
    }

    public static void Unregister(IDamageable damageable)
    {
        if (damageable == null) return;
        damageables.Remove(damageable);
    }

    public static void Cleanup()
    {
        for (int i = damageables.Count - 1; i >= 0; i--)
        {
            IDamageable damageable = damageables[i];
            if (damageable == null || damageable.TargetTransform == null)
            {
                damageables.RemoveAt(i);
            }
        }
    }
}
