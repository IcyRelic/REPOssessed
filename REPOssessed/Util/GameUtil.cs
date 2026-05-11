using REPOssessed.Handler;
using REPOssessed.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Util
{
    public class GameUtil
    {
        public static List<Level> GetLevels()
        {
            List<Level> levels = new List<Level>();
            RunManager runManager = RunManager.instance;
            if (runManager != null)
            {
                levels.AddRange(RunManager.instance.levels);
                levels.AddRange(RunManager.instance.levelShop);
                levels.Add(RunManager.instance.levelTutorial);
                levels.Add(RunManager.instance.levelRecording);
                levels.AddRange(RunManager.instance.levelArena);
                levels.Add(RunManager.instance.levelLobby);
                levels.Add(RunManager.instance.levelLobbyMenu);
            }
            return levels;
        }

        public static Dictionary<GameObject, string> GetAllItems()
        {
            Dictionary<GameObject, string> items = new Dictionary<GameObject, string>();
            GetLevels().SelectMany(l => l.ValuablePresets).Where(v => v != null).ToList().ForEach(v =>
            {
                v.tiny.Select(p => p?.Prefab).Where(p => p != null).Cast<GameObject>().ToList().ForEach(p => items[p] = ValuableDirector.instance?.Reflect()?.GetValue<string>("tinyPath") ?? "");
                v.small.Select(p => p?.Prefab).Where(p => p != null).Cast<GameObject>().ToList().ForEach(p => items[p] = ValuableDirector.instance?.Reflect()?.GetValue<string>("smallPath") ?? "");
                v.medium.Select(p => p?.Prefab).Where(p => p != null).Cast<GameObject>().ToList().ForEach(p => items[p] = ValuableDirector.instance?.Reflect()?.GetValue<string>("mediumPath") ?? "");
                v.big.Select(p => p?.Prefab).Where(p => p != null).Cast<GameObject>().ToList().ForEach(p => items[p] = ValuableDirector.instance?.Reflect()?.GetValue<string>("bigPath") ?? "");
                v.wide.Select(p => p?.Prefab).Where(p => p != null).Cast<GameObject>().ToList().ForEach(p => items[p] = ValuableDirector.instance?.Reflect()?.GetValue<string>("widePath") ?? "");
                v.tall.Select(p => p?.Prefab).Where(p => p != null).Cast<GameObject>().ToList().ForEach(p => items[p] = ValuableDirector.instance?.Reflect()?.GetValue<string>("tallPath") ?? "");
                v.veryTall.Select(p => p?.Prefab).Where(p => p != null).Cast<GameObject>().ToList().ForEach(p => items[p] = ValuableDirector.instance?.Reflect()?.GetValue<string>("veryTallPath") ?? "");
            });
            AssetManager assetManager = AssetManager.instance;
            if (assetManager != null)
            {
                GameObject surplusValuableSmall = assetManager.surplusValuableSmall;
                if (surplusValuableSmall != null) items[surplusValuableSmall] = "surplus";
                GameObject surplusValuableMedium = assetManager.surplusValuableMedium;
                if (surplusValuableMedium != null) items[surplusValuableMedium] = "surplus";
                GameObject surplusValuableBig = assetManager.surplusValuableBig;
                if (surplusValuableBig != null) items[surplusValuableBig] = "surplus";
                GameObject enemyValuableSmall = assetManager.enemyValuableSmall;
                if (enemyValuableSmall != null) items[enemyValuableSmall] = "enemy";
                GameObject enemyValuableMedium = assetManager.enemyValuableMedium;
                if (enemyValuableMedium != null) items[enemyValuableMedium] = "enemy";
                GameObject enemyValuableBig = assetManager.enemyValuableBig;
                if (enemyValuableBig != null) items[enemyValuableBig] = "enemy";
            }
            StatsManager.instance?.itemDictionary?.Values?.Select(i => i?.prefab?.Prefab).Where(p => p != null).Cast<GameObject>().ToList().ForEach(p => items[p] = "shop");
            return items.Where(i => !string.IsNullOrEmpty(i.Value)).ToDictionary(i => i.Key, i => i.Value);
        }

        public static List<EnemySetup> GetEnemySetups()
        {
            List<EnemySetup> enemies = new List<EnemySetup>();
            EnemyDirector enemyDirector = EnemyDirector.instance;
            if (enemyDirector != null)
            {
                enemies.AddRange(enemyDirector.enemiesDifficulty1);
                enemies.AddRange(enemyDirector.enemiesDifficulty2);
                enemies.AddRange(enemyDirector.enemiesDifficulty3);
            }
            return enemies.Where(e => !e.name.Contains("Enemy Group")).ToList();
        }

        public static void SpawnEnemy(EnemySetup enemySetup, int amount)
        {
            LevelGenerator levelGenerator = LevelGenerator.Instance;
            if (levelGenerator == null || enemySetup == null) return;
            Transform? roomVolumeTransform = Object.FindObjectsOfType<RoomVolume>()?.FirstOrDefault(i => i.Truck)?.transform;
            if (roomVolumeTransform == null) return;
            Transform? levelPointTransform = levelGenerator.LevelPathPoints?.OrderByDescending(p => Vector3.Distance(p.transform.position, roomVolumeTransform.position))?.FirstOrDefault()?.transform;
            if (levelPointTransform == null) return;
            for (int i = 0; i < amount; i++) levelGenerator.EnemySpawn(enemySetup, levelPointTransform.position);
        }

        public static bool IsInGame()
        {
            return !SemiFunc.IsMainMenu() && !SemiFunc.RunIsLobby();
        }

        public static bool IsMasterClient()
        {
            return GameObjectManager.LocalPlayer?.Handle()?.IsMasterClient() ?? false;
        }
    }
}
