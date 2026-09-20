using System;
using UnityEngine;

namespace AwCon.Fight
{
    [CreateAssetMenu(menuName = "AwCon/Fight/Level")]
    public sealed class FightLevelSO : ScriptableObject
    {
        [Serializable] public class Wave { public int enemies = 2; public bool boss; }
        public FighterDefinitionSO player, enemy, boss;
        [Tooltip("原项目的关卡地图预制体；战斗开始时加载。")]
        public GameObject environmentPrefab;
        public Vector3 playerSpawnPosition = new Vector3(-2, 0, 0);
        public Vector3 enemySpawnPosition = new Vector3(2, 0, -1);
        public Wave[] waves = { new Wave(), new Wave { enemies = 2, boss = true } };
        public Vector2 xBounds = new Vector2(-9, 9);
        public Vector2 zBounds = new Vector2(-3, 3);
    }
}
