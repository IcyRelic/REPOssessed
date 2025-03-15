﻿using Photon.Pun;
using REPOssessed.Extensions;
using REPOssessed.Handler;
using REPOssessed.Language;
using REPOssessed.Manager;
using REPOssessed.Menu.Core;
using REPOssessed.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace REPOssessed.Menu.Tab
{
    internal class EnemyTab : MenuTab
    {
        public EnemyTab() : base("EnemyTab.Title") { }

        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;
        private Vector2 scrollPos3 = Vector2.zero;
        private int selectedTab = 0;
        private readonly string[] tabs = ["EnemyTab.EnemyList", "EnemyTab.SpawnEnemies"];
        private static int selectedEnemy = -1;
        private static int selectedEnemySetup = -1;
        private PlayerAvatar selectedPlayer;
        private string s_spawnAmount = "1";
        private int damage = 3;
        private int heal = 3;
        private float freeze = 3;

        public override void Draw()
        {
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth - HackMenu.Instance.spaceFromLeft));
            selectedTab = GUILayout.Toolbar(selectedTab, tabs.LocalizeArray());
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.3f - HackMenu.Instance.spaceFromLeft));
            EnemyList();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.7f - HackMenu.Instance.spaceFromLeft));
            UI.VerticalSpace(ref scrollPos, () =>
            {
                switch (selectedTab)
                {
                    case 0:
                        GeneralActions();
                        EnemyActions();
                        break;
                    case 1:
                        EnemySpawnerContent();
                        break;
                }
            });
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void EnemyList()
        {
            switch (selectedTab)
            {
                case 0:
                    if (!GameObjectManager.enemies.Exists(e => e.GetInstanceID() == selectedEnemy)) selectedEnemy = -1;
                    DrawList<Enemy>("EnemyTab.EnemyList", GameObjectManager.enemies.Where(e => !e.Handle().IsDead() && !e.Handle().IsDisabled()).OrderBy(e => e.Handle().GetName()).ToList(), e => e.Handle().GetName(), ref scrollPos2, ref selectedEnemy);
                    break;
                case 1:
                    if (!GetEnemies().Exists(e => e.GetInstanceID() == selectedEnemySetup)) selectedEnemySetup = -1;
                    DrawList<EnemySetup>("EnemyTab.EnemyType", GetEnemies().OrderBy(e => e.GetName()).ToList(), e => e.GetName(), ref scrollPos3, ref selectedEnemySetup);
                   break;
            }
        }

        private void DrawList<T>(string title, IEnumerable<T> objects, Func<T, string> label, ref Vector2 scroll, ref int instanceID) where T : Object
        {
            float width = HackMenu.Instance.contentWidth * 0.3f - HackMenu.Instance.spaceFromLeft * 2;
            float height = HackMenu.Instance.contentHeight - 45;

            Rect rect = new Rect(0, 30, width, height);
            GUI.Box(rect, title.Localize());

            GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));
            GUILayout.Space(25);
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            foreach (T item in objects)
            {
                if (instanceID == -1) instanceID = item.GetInstanceID();

                if (instanceID == item.GetInstanceID()) GUI.contentColor = Settings.c_success.GetColor();

                if (GUILayout.Button(label(item), GUI.skin.label)) instanceID = item.GetInstanceID();

                GUI.contentColor = Settings.c_menuText.GetColor();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void GeneralActions()
        {
            selectedPlayer = PlayersTab.selectedPlayer;

            string s_target = selectedPlayer == null ? "General.None".Localize() : selectedPlayer.Handle().GetName();

            UI.Header("EnemyTab.GeneralActions");

            UI.Button("EnemyTab.KillAll", () => GameObjectManager.enemies.Where(e => e != null && !e.Handle().IsDead()).ToList().ForEach(e => e.Handle().Kill()));
            UI.Button("EnemyTab.LureAll", () => GameObjectManager.enemies.Where(e => e != null && !e.Handle().IsDead()).ToList().ForEach(e => e.Handle().Lure(selectedPlayer)));

            UI.Label("EnemyTab.SelectedPlayer", s_target);
        }

        private void EnemyActions()
        {
            Enemy enemy = GetSelectedEnemy();
            if (enemy == null || enemy.Handle() == null || enemy.Handle().IsDead() || enemy.Handle().enemyHealth == null || enemy.transform == null)
            {
                enemy = null;
                return;
            }

            UI.Header("EnemyTab.MonsterActions");

            UI.Button("EnemyTab.Kill", () => enemy.Handle().Kill());
            UI.Button("EnemyTab.Lure", () => enemy.Handle().Lure(selectedPlayer));
            UI.Button("EnemyTab.TeleportToEnemy", () => PlayerAvatar.instance.GetLocalPlayer().Handle().Teleport(enemy.transform.position, enemy.transform.rotation));

            UI.TextboxAction("EnemyTab.Damage", ref damage, 3,
                new UIButton("General.Set", () => enemy.Handle().Hurt(damage))
            );
            UI.TextboxAction("EnemyTab.Heal", ref heal, 3,
                new UIButton("General.Set", () => enemy.Handle().Heal(heal))
            );
            UI.TextboxAction("EnemyTab.Freeze", ref freeze, 3,
                new UIButton("General.Set", () => enemy.Freeze(freeze))
            );
        }

        private void EnemySpawnerContent()
        {
            if (selectedEnemySetup == -1) return;
            EnemySetup enemySetup = GetEnemies().Find(x => x.GetInstanceID() == selectedEnemySetup);
            if (enemySetup == null) return;

            if (!SemiFunc.IsMasterClientOrSingleplayer())
            {
                UI.Label("General.HostRequired", Settings.c_error);
                return;
            }

            UI.Header("EnemyTab.EnemySpawnerContent");

            UI.Label("EnemyTab.SelectedEnemy", enemySetup.GetName());
            UI.Textbox("EnemyTab.SpawnAmount", ref s_spawnAmount, @"[^0-9]");

            UI.Button("EnemyTab.Spawn", () => SpawnEnemy(enemySetup, int.Parse(s_spawnAmount)));
        }

        private Enemy GetSelectedEnemy()
        {
            return GameObjectManager.enemies.FirstOrDefault(x => x.GetInstanceID() == selectedEnemy);
        }

        private List<EnemySetup> GetEnemies()
        {
            List<EnemySetup> enemies = new List<EnemySetup>();
            enemies.AddRange(EnemyDirector.instance.enemiesDifficulty1);
            enemies.AddRange(EnemyDirector.instance.enemiesDifficulty2);
            enemies.AddRange(EnemyDirector.instance.enemiesDifficulty3);
            return enemies.Where(e => !e.name.Contains("Enemy Group")).ToList();
        }

        private void SpawnEnemy(EnemySetup enemy, int amount)
        {
            if (LevelGenerator.Instance == null || enemy == null) return;
            RoomVolume roomVolume = Object.FindObjectsOfType<RoomVolume>().FirstOrDefault(i => i.Truck);
            if (roomVolume == null || roomVolume.transform == null) return;
            LevelPoint levelPoint = LevelGenerator.Instance.LevelPathPoints.OrderByDescending(p => Vector3.Distance(p.transform.position, roomVolume.transform.position)).FirstOrDefault();
            if (levelPoint == null || levelPoint.transform == null) return;
            GameObject gameObject = null;
            for (int i = 0; i < amount; i++)
            {
                enemy.spawnObjects.ToList().ForEach(e =>
                {
                    if (SemiFunc.IsMultiplayer()) gameObject = $"{LevelGenerator.Instance.Reflect().GetValue<string>("ResourceEnemies")}/{e.name}".Spawn(SemiFunc.MainCamera().transform.position);
                    else gameObject = e.Spawn(levelPoint.transform.position);
                    EnemyParent enemyParent = gameObject.GetComponent<EnemyParent>();
                    if (enemyParent)
                    {
                        enemyParent.Reflect().SetValue("SetupDone", true);
                        gameObject.GetComponentInChildren<Enemy>().EnemyTeleported(levelPoint.transform.position);
                        enemyParent.DespawnedTimerSet(0f);
                    }
                });
            }
        }
    }
}
