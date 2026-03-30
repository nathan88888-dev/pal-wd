using UnityEngine;

    [System.Serializable]
    public class MainCam : MonoBehaviour
    {
        public Transform followTarget { private set; get; }
        private float distance = 15f;    // 与目标之间的直线距离
        private float angle = 60f;      // 俯视角度（单位为度）

        void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            followTarget = null;
        }

        public void SetFollowUp(Transform follow) {
            followTarget = follow;
        }

        void LateUpdate()
        {
            if (followTarget == null) return;

            float radians = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(0, Mathf.Sin(radians), -Mathf.Cos(radians)) * distance;
            transform.position = followTarget.position + offset;
            transform.LookAt(followTarget);
        }

    }
