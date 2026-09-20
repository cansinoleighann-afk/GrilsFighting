using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AwCon.Fight
{
    public sealed class FightSession : MonoBehaviour
    {
        public FightLevelSO level;
        public Text statusText;
        public Text playerHealthText;
        public Text enemyHealthText;
        private FightActor player;
        private readonly List<FightActor> enemies = new List<FightActor>();
        private int wave;
        private void Start()
        {
            if (level == null)
            {
                Debug.LogError("FightSession requires a FightLevelSO.", this);
                enabled = false;
                return;
            }

            if (level.environmentPrefab != null) Instantiate(level.environmentPrefab);
            SpawnWave();
        }
        private void Update()
        {
            if (player == null || !player.Alive) { if (statusText != null) statusText.text = "战斗失败 - 按 M 返回菜单"; if (Input.GetKeyDown(KeyCode.M)) SceneManager.LoadScene("Menu"); return; }
            enemies.RemoveAll(x => x == null || !x.Alive);
            if (enemies.Count == 0 && wave < level.waves.Length) SpawnWave();
            if (statusText != null) statusText.text = wave >= level.waves.Length && enemies.Count == 0 ? "战斗胜利 - 按 M 返回菜单" : "第 " + Mathf.Min(wave, level.waves.Length) + " / " + level.waves.Length + " 波";
            if (playerHealthText != null) playerHealthText.text = "玩家 " + Mathf.CeilToInt(player.Health);
            if (enemyHealthText != null) enemyHealthText.text = enemies.Count == 0 ? "" : "敌人 " + enemies.Count;
            if (Input.GetKeyDown(KeyCode.M)) SceneManager.LoadScene("Menu");
        }
        private void SpawnWave()
        {
            if (level == null || wave >= level.waves.Length) return;
            var current = level.waves[wave++];
            if (player == null) { player = Spawn(level.player, level.playerSpawnPosition, true); player.gameObject.AddComponent<FightPlayerInput>(); }
            int count = Mathf.Max(1, current.enemies);
            for (int i = 0; i < count; i++) enemies.Add(Spawn(current.boss ? level.boss : level.enemy, level.enemySpawnPosition + new Vector3(i * 1.6f, 0, i), false));
        }
        private FightActor Spawn(FighterDefinitionSO fighter, Vector3 position, bool isPlayer)
        {
            var go = new GameObject(fighter.displayName); go.transform.position = position;
            go.AddComponent<CapsuleCollider>(); var motor = go.AddComponent<CharacterController>(); motor.height = 1.8f; motor.radius = 0.35f; motor.center = new Vector3(0, .9f, 0);
            var actor = go.AddComponent<FightActor>(); actor.definition = fighter; actor.player = isPlayer; actor.aiEnabled = !isPlayer; return actor;
        }
    }
}
