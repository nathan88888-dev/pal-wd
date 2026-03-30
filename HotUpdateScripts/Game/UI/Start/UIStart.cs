
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIStart:MonoBehaviour
{

    public Button startBtn;
    public Button loadBtn;
    public Button CGBtn;
    public Button quitBtn;

    public Text info;
    public Transform loadPanel;
    protected async UniTask Awake()
    {
        await UniTask.WaitUntil(() => DataOperation.Instance !=null);
    }

    protected void Start()
    {
        startBtn.onClick.AddListener(() => {
            InitializeGame();
            HotMain.SwitchScene<UIWorld>("100_Wild").Forget();
        });
        quitBtn.onClick.AddListener(() => {
            Application.Quit();
        });
        Button[] togs = loadPanel.transform.GetChild(0).Find("loadPages").GetComponentsInChildren<Button>();
        loadBtn.onClick.AddListener(() =>
        {
            loadPanel.gameObject.SetActive(true);
            togs[0].onClick.Invoke();
            togs[0].Select();
        });
        DataOperation.Instance.showGameDate(
            togs,
            loadPanel.transform.GetChild(0).Find("loadPanel").GetComponentsInChildren<UIGameDate>(),
            (data, state) => {
                Debug.Log(data.name);
            });
        loadPanel.GetComponent<Button>().onClick.AddListener(() =>
        {
            loadPanel.gameObject.SetActive(false);
        });
    }


    protected void onDestory()
    {
    }

    private void InitializeGame()
    {
        // 创建角色
        var NianWan = new Character("NianWan", 7);
        NianWan.ChangeFavor(100);

        // 初始化一些物品
        var items = new Item[]
        {
            new Item { Id = "item_001", Name = "止血草", Description = "恢复少量HP", CanStack = true },
            new Item { Id = "item_002", Name = "还魂香", Description = "复活队友", CanStack = false }
        };

        // 设置游戏状态
        GMemoryCache.Instance.setGameState(state =>
        {
            state.MainCharacterIndex = 0;
            state.chaList = new Character[] { NianWan };
        });
    }
}