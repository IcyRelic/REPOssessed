using REPOssessed.Util;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Handler
{
    public class EnemyHandler
    {
        private Enemy enemy;

        public EnemyHandler(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public void Heal(int heal) => GetHealth()?.Reflect().Invoke("Hurt", -heal, new Vector3(0, 0, 0));
        public void Hurt(int damage) => GetHealth()?.Reflect().Invoke("Hurt", damage, new Vector3(0, 0, 0));
        public void Kill() => GetHealth()?.Reflect().Invoke("Death", new Vector3(0, 0, 0));
        public void Lure(PlayerAvatar player)
        {
            if (player == null) return;
            enemy.SetChaseTarget(player);
        }

        public bool IsDisabled() => !GetEnemyParent().EnableObject.activeSelf || !GetEnemyParent().Reflect().GetValue<bool>("Spawned");
        public bool IsDead() => GetHealth().Reflect().GetValue<bool>("dead");
        public string GetName() => GetEnemyParent().enemyName;
        public EnemyHealth GetHealth() => enemy.GetComponent<EnemyHealth>();
        public EnemyParent GetEnemyParent() => enemy.GetComponentInParent<EnemyParent>();
    }

    public static class EnemyExtensions
    {
        public static EnemyHandler Handle(this Enemy enemy) => new EnemyHandler(enemy);
        public static EnemyParent GetEnemyParent(this EnemySetup enemy) => enemy.spawnObjects.Select(o => o?.GetComponent<EnemyParent>()).FirstOrDefault(e => e != null);
        public static string GetName(this EnemySetup enemy) => enemy.GetEnemyParent().enemyName;
    }
}