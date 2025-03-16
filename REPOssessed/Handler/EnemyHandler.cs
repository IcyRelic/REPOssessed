using Photon.Realtime;
using REPOssessed.Extensions;
using REPOssessed.Util;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Handler
{
    public class EnemyHandler
    {
        private Enemy enemy;
        public EnemyHealth enemyHealth;
        public EnemyParent enemyParent;

        public EnemyHandler(Enemy enemy)
        {
            if (enemy == null) return; 
            this.enemy = enemy;
            enemyHealth = enemy.gameObject.GetComponentHierarchy<EnemyHealth>();
            enemyParent = enemy.gameObject.GetComponentHierarchy<EnemyParent>();
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

        public bool IsDisabled() => enemyParent == null || enemyParent.EnableObject == null || !enemyParent.EnableObject.activeSelf || !(enemyParent.Reflect()?.GetValue<bool>("Spawned") ?? false);
        public bool IsDead() => enemyHealth != null && enemyHealth.Reflect().GetValue<bool>("dead");
        public string GetName() => enemyParent != null ? enemyParent.enemyName : "Unknown";
        public int GetHealth() => enemyHealth != null ? enemyHealth.Reflect().GetValue<int>("healthCurrent") : 0;
        public int GetMaxHealth() => enemyHealth != null ? enemyHealth.health : 0;
        public PlayerAvatar GetEnemyTarget() => enemy.Reflect().GetValue<PlayerAvatar>("TargetPlayerAvatar");
    }

    public static class EnemyExtensions
    {
        public static EnemyHandler Handle(this Enemy enemy) => new EnemyHandler(enemy);
        public static string GetName(this EnemySetup enemy) => enemy.spawnObjects.Select(o => o?.GetComponentHierarchy<EnemyParent>()).FirstOrDefault(e => e != null).enemyName;
    }
}