
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PatrolWithList : MonoBehaviour
{
    public Vector3[] pathPoints;  // 巡逻点列表
    public float speed = 2f;               // 移动速度
    public EnemyDefinition[] enemies;
    private float reachThreshold = 0.5f;   // 到达点的误差距离

    private int currentPointIndex = 0;
    private bool forward = true;           // 是否正向巡逻
    private NavMeshAgent agent;
    protected void Start()
    {
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.speed = speed;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(agent.transform.position, out hit, 20f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }
    void Update()
    {
        if (agent == null || pathPoints == null || pathPoints.Length == 0)
            return;

        Vector3 targetPos = pathPoints[currentPointIndex];

        // 判断是否到达目标点
        if (Vector3.Distance(transform.position, targetPos) < reachThreshold)
        {
            if (forward)
            {
                currentPointIndex++;
                if (currentPointIndex >= pathPoints.Length)
                {
                    currentPointIndex = pathPoints.Length - 2;
                    forward = false;  // 反向
                }
            }
            else
            {
                currentPointIndex--;
                if (currentPointIndex < 0)
                {
                    currentPointIndex = 1;
                    forward = true;  // 正向
                }
            }
        }
        else {
            agent.SetDestination(targetPos);
        }
    }
}