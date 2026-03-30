
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal enum operType
{
    NULL,
    Animation,
    Operation,
    Attack,
    Skill,
    Item,
    Scape,
    Magic,
    Defend,
    End,
};
public class UIBattle : MonoBehaviour
{
    public Transform sliderp;
    public Transform chStatusp;

    public Transform skillPanel;
    public Transform btnGroup;
    public Transform resultPanel;
    public Material state_Effect;

    public Button attackBtn;
    public Button magicBtn;
    public Button skillBtn;
    public Button scapeBtn;
    public Button itemBtn;
    public Button defendBtn;

    protected void Awake()
    {
        ShaderFactorOscillator outline = gameObject.AddComponent<ShaderFactorOscillator>();
        outline.material = state_Effect;

        GameObject battleRoot = GameObject.Find("battleRoot");
        if (battleRoot == null)
            return;

        self = battleRoot.transform.Find("self");
        enemy = battleRoot.transform.Find("enemy");

        attackBtn.onClick.AddListener(() => {
            setOperationState(operType.Attack);
        });
        skillBtn.onClick.AddListener(() => {
            setOperationState(operType.Skill);
        });
        itemBtn.onClick.AddListener(() => {
            setOperationState(operType.Item);
        });
        scapeBtn.onClick.AddListener(() => {
            setOperationState(operType.Scape);
        });
        magicBtn.onClick.AddListener(() => {
            setOperationState(operType.Magic);
        });
        defendBtn.onClick.AddListener(() => {
            setOperationState(operType.Defend);
        });
        resultPanel.GetComponent<Button>().onClick.AddListener(() => {
            setOperationState(operType.NULL);
            while (creatureList.Count > 0)
            {
                Destroy(creatureList[0]);
                creatureList.RemoveAt(0);
            }
            HotMain.SwitchScene<UIWorld>().Forget();
        });
    }
    private Camera mainCamera;
    private Transform self;
    private Transform enemy;
    private Character[] chaList;
    private List<Character> enemyList;

    private List<KeyValuePair<Character, float>> moveList;
    private List<KeyValuePair<Character, float>> selectList;
    private operType curOperType;
    private Character operCha;
    private List<GameObject> creatureList;

    

    private void OnEnable()
    {
        setOperationState(operType.NULL);
        creatureList = new List<GameObject>();

        mainCamera = Camera.main;
        mainCamera.transform.position = new Vector3(108, 70, 119);
        mainCamera.transform.eulerAngles = new Vector3(45, 150, 0);

        for (int i = 0; i < sliderp.childCount; i++) {
            sliderp.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < chStatusp.childCount; i++)
        {
            chStatusp.GetChild(i).gameObject.SetActive(false);
        }
        skillPanel.localScale = Vector3.zero;
        btnGroup.gameObject.SetActive(false) ;
        resultPanel.localScale = Vector3.zero;

        moveList = new List<KeyValuePair<Character, float>>();
        selectList = new List<KeyValuePair<Character, float>>();

        magicBtn.interactable = false;
        skillBtn.interactable = false;
        scapeBtn.interactable = false;
        itemBtn.interactable = false;
        defendBtn.interactable = false;

        chaList = GMemoryCache.Instance.GetGameState().chaList;
        InitCharacter(chaList).Forget();
        enemyList = new List<Character>(GMemoryCache.Instance.GetCharacters("enemies"));
        InitCharacter(enemyList.ToArray(), chaList.Length, false).Forget();
    }


    private async UniTask InitCharacter(Character[] chlist,int startIndex = 0, bool isCha = true) {

        Debug.Log($"InitCharacter:{chlist.Length}");
        for (int i = 0; i < chlist.Length; i++)
        {
            Character cha = chlist[i];
            GameObject chaobj = await GMemoryCache.Instance.LoadCharacter(
                isCha ? self.GetChild(i) : enemy.GetChild(i % 3),
                cha.Name,
                isCha ? LayerEnum.Character : LayerEnum.Enemy);
            creatureList.Add(chaobj);
            chaobj.transform.GetChild(0).gameObject.SetActive(false);
            cha.CleanUI();
            cha.CharacterObject = chaobj;
            cha.animator = chaobj.transform.GetChild(1).GetComponentInChildren<Animator>();
            while (sliderp.childCount <= i+ startIndex)
            {
                GMemoryCache.Instance.CopyChild(sliderp);
            }
            cha.speedSlider = sliderp.GetChild(i + startIndex).GetComponent<Slider>();
            cha.speedSlider.gameObject.SetActive(true);
            Image sliderTarget = (Image)cha.speedSlider.targetGraphic;
            if (isCha)
            {
                sliderTarget.sprite = cha.speedSlider.spriteState.pressedSprite;
                cha.UpdateStatusUI(chStatusp.GetChild(i).GetComponent<UIStatus>());
                cha.StatusUI.gameObject.SetActive(true);
            }
            else
            {
                sliderTarget.sprite = cha.speedSlider.spriteState.selectedSprite;
            }
        }
    }

    private async UniTask DecideToAttackCha()
    {
        setOperationState(operType.Animation);
        int randomAttack = Random.Range(0, chaList.Length - 1);
        if (await operCha.AttackMove(chaList[randomAttack])) {
            //chaList.Remove(chaList[randomAttack]);
        }
        setOperationState(operType.NULL);
    }

    private async UniTask DetectNearestEnemy()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        selectList.Clear();
        if(Physics.Raycast(ray, out RaycastHit hit, 20, (int)LayerEnum.Ground))
        {
            Vector3 hitPoint = hit.point;
            for (int i = 0; i < enemyList.Count; i++)
            {
                Character cha = enemyList[i];
                if (cha.CharacterObject.layer != (int)LayerEnum.Enemy) continue;
                cha.CharacterObject.transform.GetChild(0).gameObject.SetActive(false);
                selectList.Add(new KeyValuePair<Character, float>(cha,
                    Vector3.Distance(hitPoint, cha.CharacterObject.transform.position)));
            }
        }

        selectList.Sort((a, b) => a.Value > b.Value ? 1 : 0);
        if (selectList.Count > 0)
            selectList[0].Key.CharacterObject.transform.GetChild(0).gameObject.SetActive(true);

        if (Input.GetMouseButton(0) && selectList.Count>0)
        {
            selectList[0].Key.CharacterObject.transform.GetChild(0).gameObject.SetActive(false);
            operCha.StatusUI.gameObject.GetComponent<Image>().material = null;
            Debug.Log(operCha.StatusUI.gameObject.name);
            btnGroup.gameObject.SetActive(false);
            setOperationState(operType.Animation);
            if(await operCha.AttackMove(selectList[0].Key))
            {
                enemyList.Remove(selectList[0].Key);
            }
            setOperationState(operType.NULL);
        }
        if (Input.GetMouseButton(1))
        {
            setOperationState(operType.NULL);
        }
    }

    void Update()
    {
        if (enemyList == null) return;
        switch (curOperType)
        {
            case operType.NULL:
                if (enemyList.Count == 0)
                {
                    Debug.Log("Battle End");
                    setOperationState(operType.End);
                    resultPanel.DOScale(1, 1f);
                }
                if (moveList.Count == 0)
                {
                    MoveFoward(chaList);
                    MoveFoward(enemyList.ToArray());
                    moveList.Sort((a, b) => a.Value < b.Value ? 1 : 0);
                }
                else
                {
                    operCha = moveList[0].Key;
                    if (operCha.Favor > 10)
                    {
                        btnGroup.gameObject.SetActive(true);
                    }
                    else
                    {
                        DecideToAttackCha().Forget();
                    }
                    setOperationState(operType.Operation);
                }
                break;
            case operType.Animation:
                break;
            case operType.Operation:
                break;
            case operType.Attack:
                DetectNearestEnemy().Forget();
                break;
            default:break;
        }
    }

    private void MoveFoward(Character[] listCha)
    {
        for (int i = 0; i < listCha.Length; i++)
        {
            if (listCha[i].UpdateCurrentMove())
            {
                moveList.Add(new KeyValuePair<Character, float>(listCha[i], listCha[i].currentMove));
            }
        }
    }

    void setOperationState(operType type) {
        Debug.Log("cha:"+ (operCha ==null?"NULL": operCha.Name) + ",OperationState:" + type );
        curOperType = type;
        switch (curOperType)
        {
            case operType.NULL:
                operCha = null;
                break;
            case operType.Animation:
                if (operCha != null && operCha == moveList[0].Key)
                {
                    moveList.RemoveAt(0);
                    if(operCha.Favor>10)
                    operCha.StatusUI.gameObject.GetComponent<Image>().material = null;
                }
                break;
            case operType.Operation:
            case operType.Attack:
            case operType.Magic:
            case operType.Skill:
            case operType.Scape:
            case operType.Item:
            case operType.Defend:
                if (operCha.Favor <= 10) return;
                operCha.StatusUI.gameObject.GetComponent<Image>().material = state_Effect;
                attackBtn.interactable = curOperType == operType.Operation || curOperType == operType.Attack;
                magicBtn.interactable = curOperType == operType.Operation || curOperType == operType.Magic;
                skillBtn.interactable = curOperType == operType.Operation || curOperType == operType.Skill;
                scapeBtn.interactable = curOperType == operType.Operation || curOperType == operType.Scape;
                itemBtn.interactable = curOperType == operType.Operation || curOperType == operType.Item;
                defendBtn.interactable = curOperType == operType.Operation || curOperType == operType.Defend;
                //skillPanel.SetOperType(curOperType);
                break;
            default:break;
        }
    }
}