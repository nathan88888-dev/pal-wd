

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIWorld:MonoBehaviour
{
    public Button SettingBtn;

    public Transform totalPanel;
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
        for (int i = 0; i < status.Length; i++)
        {
            status[i].gameObject.SetActive(false);
        }
        _GState = GMemoryCache.Instance.GetGameState();
        maincam = Camera.main;
        totalPanel.localScale = Vector3.zero;

        if (player == null)
        {
            Transform _playerp = GameObject.Find("ChaBorn").transform;
            GameObject _player = await GMemoryCache.Instance.LoadCharacter(
                _playerp, _GState.chaList[0].Name, LayerEnum.Character
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

        if (WorldEnemies == null)
        {
            WorldEnemies = new GameObject("EnemiesBorn").transform;
            await GMemoryCache.Instance.LoadEnemy(WorldEnemies);
            SetListeners();
        }
        childpath = WorldEnemies.GetComponentsInChildren<PatrolWithList>();
        Debug.Log("Enemies:" + childpath.Length);
    }

    protected void OnDisable()
    {
        GMemoryCache.Instance.mainCam.SetFollowUp(null);
    }

    private GameState _GState;
    private NavMeshAgent player;
    private Transform WorldEnemies;
    private PatrolWithList[] childpath;
    Camera maincam;
    protected void SetListeners()
    {
        SettingBtn.onClick.AddListener(() =>
        {
            totalPanel.DOScale(Vector3.one, 0.5f);
            systemBtn.onClick.Invoke();
        });
        continueBtn.onClick.AddListener(()=> {
            totalPanel.DOScale(Vector3.zero, 0.5f);
        });
        systemBtn.onClick.AddListener(() =>
        {
            sysPanel.SetActive(true);
            savePanel.SetActive(false);
            loadPanel.SetActive(false);
            for (int i = 0; i < _GState.chaList.Length; i++) {
                status[i].gameObject.SetActive(true);
            }
        });
        loadGameBtn.onClick.AddListener(() =>
        {
            sysPanel.SetActive(false);
            savePanel.SetActive(false);
            loadPanel.SetActive(true);
        });
        saveGameBtn.onClick.AddListener(() =>
        {
            sysPanel.SetActive(false);
            savePanel.SetActive(true);
            loadPanel.SetActive(false);
        });
        quitGameBtn.onClick.AddListener(() =>
        {
            HotMain.SwitchScene<UIStart>("01_Start").Forget();
        });
    }

    void Update() {
        if (totalPanel.localScale == Vector3.one||
            (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())) return;
            
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
            if (Vector3.Distance(childpath[i].transform.position, player.transform.position) < 1.2f) {
                Character[] enemyList = new Character[childpath[i].enemies.Length];
                for (int j = 0; j < childpath[i].enemies.Length; j++)
                {
                    enemyList[j] = new Character(childpath[i].enemies[j].monsterID,
                        childpath[i].enemies[j].level);
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