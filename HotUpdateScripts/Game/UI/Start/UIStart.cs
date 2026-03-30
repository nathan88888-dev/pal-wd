
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIStart:MonoBehaviour
{

    public Button startBtn;
    public Button loadBtn;
    public Button CGBtn;
    public Button quitBtn;

    public Text info;

    private GameState gState;
    protected void Awake()
    {
        GMemoryCache.Instance.mainCam = Camera.main.GetComponent<MainCam>();
        gState = GMemoryCache.Instance.GetGameState();
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
        GMemoryCache.Instance.UpdateGameState(state =>
        {
            state.MainCharacterIndex = 0;
            state.chaList = new Character[] { NianWan };
        });
    }
}