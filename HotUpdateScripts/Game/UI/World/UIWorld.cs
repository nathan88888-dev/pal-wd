

using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIWorld:MonoBehaviour
{
    public Button SettingBtn;

    public Transform totalPanel;

    public Text allGameTime;
    public Text curGameTime;

    public Button systemBtn;
    public UIStatus[] status;
    public Button loadGameBtn;
    public Button saveGameBtn;
    public Button quitGameBtn;
    public Button continueBtn;
    public GameObject sysPanel;
    public GameObject loadPanel;
    public GameObject savePanel;


    protected async UniTask OnEnable()
    {
        Debug.Log("OnEnable");
        for (int i = 0; i < status.Length; i++)
        {
            status[i].gameObject.SetActive(false);
        }
        gState = GMemoryCache.Instance.GetGameState();
        maincam = Camera.main;
        totalPanel.localScale = Vector3.zero;

        if (player == null)
        {
            Transform _playerp = GameObject.Find("ChaBorn").transform;
            GameObject _player = await GMemoryCache.Instance.LoadCharacter(
                _playerp, gState.chaList[0].Name, LayerEnum.Character
                );
            NavMeshHit hit;
            if (NavMesh.SamplePosition(_player.transform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                _player.transform.position = hit.position;
            }
            else
            {
                Debug.LogError("无法找到有效的 NavMesh 位置");
            }
        }
        player = GameObject.Find("ChaBorn").GetComponentInChildren<NavMeshAgent>();
        GMemoryCache.Instance.mainCam.SetFollowUp(player.transform);

        if (worldEnemies == null)
        {
            worldEnemies = new GameObject("EnemiesBorn").transform;
            await GMemoryCache.Instance.LoadEnemy(worldEnemies);
            SetListeners();
        }
        childpath = worldEnemies.GetComponentsInChildren<PatrolWithList>();
        Debug.Log("Enemies:" + childpath.Length);
    }

    protected void OnDisable()
    {
        GMemoryCache.Instance.mainCam.SetFollowUp(null);
    }

    private GameState gState;
    private NavMeshAgent player;
    private Transform worldEnemies;
    private PatrolWithList[] childpath;
    Camera maincam;
    protected void SetListeners()
    {
        SettingBtn.onClick.AddListener(() =>
        {
            totalPanel.DOScale(Vector3.one, 0.5f);
            systemBtn.onClick.Invoke();
            systemBtn.Select();
        });
        continueBtn.onClick.AddListener(()=> {
            totalPanel.DOScale(Vector3.zero, 0.5f);
        });
        systemBtn.onClick.AddListener(() =>
        {
            sysPanel.SetActive(true);
            savePanel.SetActive(false);
            loadPanel.SetActive(false);
            for (int i = 0; i < gState.chaList.Length; i++) {
                status[i].gameObject.SetActive(true);
            }
        });
        Button[] loadPages = loadPanel.transform.Find("loadPages").GetComponentsInChildren<Button>();
        loadGameBtn.onClick.AddListener(() =>
        {
            Debug.Log("loadGameBtn");
            sysPanel.SetActive(false);
            savePanel.SetActive(false);
            loadPanel.SetActive(true);
            loadPages[0].onClick.Invoke();
            loadPages[0].Select();
        });
        DataOperation.Instance.showGameDate(
            loadPages,
            loadPanel.transform.Find("loadPanel").GetComponentsInChildren<UIGameDate>(),
            (data, state) => {
                Debug.Log(data.name);
            });
        Button[] savepages = savePanel.transform.Find("savePages").GetComponentsInChildren<Button>();
        Debug.Log("init saveGameBtn");
        saveGameBtn.onClick.AddListener(() =>
        {
            Debug.Log("saveGameBtn");
            sysPanel.SetActive(false);
            savePanel.SetActive(true);
            loadPanel.SetActive(false);
            savepages[0].onClick.Invoke();
            savepages[0].Select();
        });
        DataOperation.Instance.showGameDate(
            savepages,
            savePanel.transform.Find("savePanel").GetComponentsInChildren<UIGameDate>(),
            (data, state) => {
                UIManager.Instance.ShowMsg($"确定要存储到{data.dataID.text}号挡位吗", () => {
                    int dataID = int.Parse(data.dataID.text) - 1;
                    gState.SetScreenshot(GMemoryCache.Instance.mainCam.CaptureCameraScreenshot());
                    DataOperation.Instance.SaveGame(dataID, gState);
                }, null);
            });
        quitGameBtn.onClick.AddListener(() =>
        {
            HotMain.SwitchScene<UIStart>("01_Start").Forget();
        });
    }

    void Update() {
        if (totalPanel.localScale == Vector3.one ||
            (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()))
        {
            allGameTime.text = gState.allGameTime;
            curGameTime.text = gState.curGameTime;
            return;
        }
            
        if (Input.GetMouseButton(0))
        {
            Ray ray = maincam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                player.SetDestination(hit.point);
            }
        }
        if (childpath == null) return;
        for (int i = 0; i < childpath.Length; i++) {
            if (Vector3.Distance(childpath[i].transform.position, player.transform.position) < 2f) {
                Character[] enemyList = new Character[childpath[i].enemies.Length];
                for (int j = 0; j < childpath[i].enemies.Length; j++)
                {
                    enemyList[j] = new Character(childpath[i].enemies[j].monsterID,
                        childpath[i].enemies[j].level, LayerEnum.Enemy);
                }
                GMemoryCache.Instance.ClearCache("enemies");
                GMemoryCache.Instance.StoreCharacters("enemies", enemyList);
                childpath[i].gameObject.SetActive(false);
                GMemoryCache.Instance.GetGameState().enemyData.RemoveAt(i);
                HotMain.SwitchScene<UIBattle>().Forget();
            }
        }
    }

}