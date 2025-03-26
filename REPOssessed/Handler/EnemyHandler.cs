using REPOssessed.Extensions;
using REPOssessed.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Handler
{
    public class EnemyHandler
    {
        private Enemy enemy = null;
        public EnemyHealth enemyHealth = null;
        public EnemyParent enemyParent = null;

        public EnemyHandler(Enemy enemy)
        {
            this.enemy = enemy;
            this.enemyHealth = enemy.GetComponentHierarchy<EnemyHealth>();
            this.enemyParent = enemy.GetComponentHierarchy<EnemyParent>();
        }

        public void Heal(int heal)
        {
            if (enemyHealth == null) return;
            enemyHealth.Reflect().Invoke("Hurt", -heal, new Vector3(0, 0, 0));
        }
        public void Hurt(int damage)
        {
            if (enemyHealth == null) return;
            enemyHealth.Reflect().Invoke("Hurt", damage, new Vector3(0, 0, 0));
        }
        public void Kill()
        {
            if (enemyHealth == null) return;
            enemyHealth.Reflect().Invoke("Death", new Vector3(0, 0, 0));
        }
        public void Lure(PlayerAvatar player)
        {
            if (player == null) return;
            enemy.SetChaseTarget(player);
        }
        public void Teleport(Vector3 position) => enemy.EnemyTeleported(position);
        public bool IsDisabled() => (!enemyParent?.EnableObject?.activeSelf ?? false) || (!enemyParent.Reflect()?.GetValue<bool>("Spawned") ?? false);
        public bool IsDead() => enemyHealth != null && enemyHealth.Reflect().GetValue<bool>("dead");
        public string GetName() => enemyParent != null ? enemyParent.enemyName : "Unknown";
        public int GetHealth() => enemyHealth != null ? enemyHealth.Reflect().GetValue<int>("healthCurrent") : 0;
        public int GetMaxHealth() => enemyHealth != null ? enemyHealth.health : 0;
        public PlayerAvatar GetEnemyTarget() => enemy.Reflect().GetValue<PlayerAvatar>("TargetPlayerAvatar");
    }

    public static class EnemyExtensions
    {
        public static Dictionary<Enemy, EnemyHandler> EnemyHandlers = new Dictionary<Enemy, EnemyHandler>();

        public static EnemyHandler Handle(this Enemy enemy)
        {
            if (enemy == null)
            {
                if (EnemyHandlers.ContainsKey(enemy)) EnemyHandlers.Remove(enemy);
                return null;
            }
            if (!EnemyHandlers.TryGetValue(enemy, out var handler))
            {
                handler = new EnemyHandler(enemy);
                EnemyHandlers[enemy] = handler;
            }
            return handler;
        }

        public static string GetName(this EnemySetup enemy) => enemy.spawnObjects.Select(o => o?.GetComponentHierarchy<EnemyParent>()).FirstOrDefault(e => e != null).enemyName;
    }
}