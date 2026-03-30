
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SceneEnemyEditor : EditorWindow
{
    List<EnemyPathEntry> enemies =  new List<EnemyPathEntry>();
    List<Color> colors = new List<Color>();
    string jsonPath = "";
    string rawJson = "";
    string sceneName;

    string prefabFolder = "Assets/HotUpdateAssets/03_Models/enemy/";
    string configFolder = "Assets/HotUpdateAssets/95_Config/Enemy_att/";
    List<string> prefabNames;
    List<string> configNames;
    Vector2 scrollPos;

    bool isDirty = false;
    [MenuItem("Tools/Scene Enemy Editor")]
    static void OpenWindow()
    {
        SceneEnemyEditor seEditor = GetWindow<SceneEnemyEditor>("Scene Enemy Editor");
    }

    void OnGUI()
    {
        if (sceneName != SceneManager.GetActiveScene().name) {
            enemies.Clear();
        }


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load", GUILayout.Width(60))) LoadData();
        if (isDirty)
        {
            if (GUILayout.Button("Save", GUILayout.Width(60))) SaveData();
        }
        else
        {
            if (GUILayout.Button("play", GUILayout.Width(60))) playData();
        }

                EditorGUILayout.EndHorizontal();

        if (enemies.Count == 0)
        {
            EditorGUILayout.HelpBox("No enemy data loaded.", MessageType.Info);
            return;
        }

        bool ismodify = false;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < enemies.Count; i++)
        {
            var epe = enemies[i];
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Enemy {i + 1}", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.Width(50));
            epe.enemyName = EditorGUILayout.TextField(epe.enemyName, GUILayout.Width(100));
            EditorGUILayout.LabelField("Scale", GUILayout.Width(50));
            epe.scale = EditorGUILayout.FloatField(epe.scale, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            int currentIndex = Mathf.Max(0, prefabNames.IndexOf(epe.monsterPrefab));
            EditorGUILayout.LabelField("Prefab", GUILayout.Width(50));
            int selectedIndex = EditorGUILayout.Popup( currentIndex, prefabNames.ToArray(), GUILayout.Width(100));
            if (selectedIndex != currentIndex && selectedIndex >= 0 && selectedIndex < prefabNames.Count)
            {
                epe.monsterPrefab = prefabNames[selectedIndex];
            }
            EditorGUILayout.LabelField("Speed", GUILayout.Width(50));
            epe.moveSpeed = EditorGUILayout.FloatField( epe.moveSpeed, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Path Points:");
            for (int p = 0; p < epe.pathMove.Count; p++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    // 删除路径点
                    List<Vector3> tempList = new List<Vector3>(epe.pathMove);
                    tempList.RemoveAt(p);
                    epe.pathMove = tempList;
                    ismodify = true;
                }
                EditorGUILayout.LabelField("x:", GUILayout.Width(10));
                EditorGUILayout.FloatField(epe.pathMove[p].x, GUILayout.Width(50));
                EditorGUILayout.LabelField("y:", GUILayout.Width(10));
                EditorGUILayout.FloatField(epe.pathMove[p].y, GUILayout.Width(50));
                EditorGUILayout.LabelField("z:", GUILayout.Width(10));
                EditorGUILayout.FloatField(epe.pathMove[p].z, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                if (ismodify)
                {
                    ismodify = false;
                    break;
                }
            }

            if (GUILayout.Button("Add Path Point", GUILayout.Width(150)))
                epe.pathMove.Add(epe.pathMove[epe.pathMove.Count-1]);

            EditorGUILayout.EndVertical();
            EditorGUILayout.LabelField("Enemy In Battle:", EditorStyles.boldLabel);

            for (int d = 0; d < epe.enemies.Count; d++)
            {
                var def = epe.enemies[d];
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    epe.enemies.RemoveAt(d);
                    ismodify = true;
                }
                EditorGUILayout.LabelField("Monster ID", GUILayout.Width(50));
                int currentMIndex = Mathf.Max(0, configNames.IndexOf(def.monsterID));
                int selectedMIndex = EditorGUILayout.Popup(currentMIndex, configNames.ToArray(), 
                    GUILayout.Width(100));
                if (selectedMIndex != currentMIndex && selectedMIndex >= 0 && 
                    selectedMIndex < configNames.Count)
                {
                    def.monsterID = configNames[selectedMIndex];
                }
                EditorGUILayout.LabelField("Level", GUILayout.Width(50));
                def.level = EditorGUILayout.IntField(def.level, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                if (ismodify)
                {
                    ismodify = false;
                    break;
                }
            }

            if (GUILayout.Button("Add Enemy In Battle", GUILayout.Width(150)))
            {
                epe.enemies.Add(epe.enemies[epe.enemies.Count-1].Copy());
            }
        }
        EditorGUILayout.EndScrollView();
        string jsont =JsonUtilityWrapper.ToJsonList(enemies);
        isDirty = rawJson != jsont;
        GameObject go = GameObject.Find("playp");
        if (isDirty && go != null) {
            GameObject.DestroyImmediate(go);
        }
    }
    int activeEnemyIndex = -1; // -1 表示没选中任何路径点
    int activePathPointIndex = -1;
    void OnSceneGUI(SceneView sceneView)
    {
        if (enemies == null) return;

        for (int ei = 0; ei < enemies.Count; ei++)
        {
            var e = enemies[ei];

            for (int pi = 0; pi < e.pathMove.Count; pi++)
            {
                Vector3 pos = e.pathMove[pi];

                EditorGUI.BeginChangeCheck();

                Vector3 newPos = Handles.PositionHandle(pos, Quaternion.identity);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this, "Move Path Point");
                    e.pathMove[pi] = newPos;

                    activeEnemyIndex = ei;
                    activePathPointIndex = pi;
                }
            }
        }

        for (int ei = 0; ei < enemies.Count; ei++)
        {
            var e = enemies[ei];

            if (ei == activeEnemyIndex)
                Handles.color = Color.green; // 高亮颜色
            else
                Handles.color = colors[ei]; 

            for (int i = 0; i < e.pathMove.Count; i++)
            {
                Handles.SphereHandleCap(0, e.pathMove[i], Quaternion.identity, 0.1f, EventType.Repaint);

                if (i < e.pathMove.Count - 1)
                    Handles.DrawAAPolyLine(ei == activeEnemyIndex?30:10, 
                        new Vector3[] { e.pathMove[i], e.pathMove[i + 1] });
            }
        }
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void LoadData()
    {
        // 这里改成你想用的固定路径，必须是Assets开头相对路径
        // 查找所有prefab文件
        string[] guidspre = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolder });
        prefabNames = new List<string>();
        for(int i = 0;i < guidspre.Length;i ++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guidspre[i]);
            string name = Path.GetFileNameWithoutExtension(path);
            prefabNames.Add(name);
        }
        var listall = new List<string>(AssetDatabase.FindAssets("", new[] { configFolder }));
        configNames = new List<string>();
        for (int i = 0; i < listall.Count; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(listall[i]);
            string name = Path.GetFileNameWithoutExtension(path);
            if (prefabNames.IndexOf(name)>-1)
                configNames.Add(name);
        }


        Scene sNew = SceneManager.GetActiveScene();
        sceneName = sNew.name;
        jsonPath = Path.GetDirectoryName(sNew.path) + "/" + sceneName + "/sceneEnemy.json";
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("File not found: " + jsonPath);
            return;
        }
        rawJson = File.ReadAllText(jsonPath);
        enemies = new EnemyList(rawJson).entries;

        colors.Clear();
        for (int i = 0; i < enemies.Count; i++)
        {
            colors.Add(new Color(Random.value, Random.value, Random.value));
        }
        Debug.Log($"Loaded enemy:{enemies.Count},prefab:{prefabNames.Count},data:{configNames.Count}");
    }

    void SaveData()
    {
        string json = JsonUtilityWrapper.ToJsonList(enemies);
        File.WriteAllText(jsonPath, json);
        Debug.Log("Saved enemy data to: " + jsonPath);
        LoadData();
    }
    void playData()
    {
        GameObject go = GameObject.Find("playp");
        if (go != null) {
            GameObject.DestroyImmediate(go);
        }
        go = new GameObject("playp");
        for (int i = 0; i < enemies.Count; i++) {
            EnemyPathEntry epe = enemies[i];
            GameObject enego = new GameObject("ene:" + i);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = enego.transform;
            cube.transform.localPosition = Vector3.up;
            Transform tran = enego.transform;
            tran.parent = go.transform;
            tran.position = epe.pathMove[0];
            PatrolWithList pwl = enego.AddComponent<PatrolWithList>();
            pwl.enabled = false;
            pwl.pathPoints = epe.pathMove.ToArray();
            pwl.speed = epe.moveSpeed;
            pwl.enabled = true;
        }
    }
}
public static class JsonUtilityWrapper
{
    public static string ToJsonList(List<EnemyPathEntry> list)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.Converters.Add(new Vector3Converter());
        string json = JsonConvert.SerializeObject(new EnemyList(list), Formatting.Indented, settings);
        return json.Replace("\n", "").Replace(" ", "");
    }
}
