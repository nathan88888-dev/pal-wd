
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPathEntry
{
    [Tooltip("怪物行走的路径，请拖入一个场景中的 Spline Container 对象")]
    public List<Vector3> pathMove;

    [Tooltip("要生成的怪物预制件")]
    public string monsterPrefab;

    [Tooltip("怪物的缩放比例")]
    public float scale = 1f;

    [Tooltip("怪物的移动速度")]
    public float moveSpeed = 2f;

    [Tooltip("这一波怪物的组名或标识")]
    public string enemyName = "Wave 1";

    public List<EnemyDefinition> enemies;
}
[System.Serializable]
public class EnemyDefinition
{
    [Tooltip("怪物ID，从敌人配置中选择")]
    public string monsterID;
    [Tooltip("怪物等级")]
    public int level = 1;

    public EnemyDefinition Copy()
    {
        return (EnemyDefinition)this.MemberwiseClone();
    }
}

