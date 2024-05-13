using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour, IuseAbility
{
    public static UIManager Instance { get; private set;}

    [SerializeField] private InputReader _inputReader = default;

    [SerializeField, ReadOnly] public PlayerUI playerUI;

    // 從玩家身上抓取System
    [SerializeField, ReadOnly] public SkillSystem SkillSystem;
    [SerializeField, ReadOnly] public StatusSystem StatusSystem;

    [SerializeField] public GameObject PauseMenu;
    [SerializeField] public KeyBindingFootBarHelper KeyBindingFootBar;

    //public List<GameObject> Skills;
    // all skills cooldown scripts
    //private List<AbilityCooldown> _skillsCooldown = new List<AbilityCooldown>();
    // current style skills cooldown scripts
    //private List<AbilityCooldown> _currentSkillsCoolDown = new List<AbilityCooldown>();

    private void OnEnable()
    {
        _inputReader.pauseEvent += OpenPauseMenu;
        _inputReader.menuUnpauseEvent += ClosePauseMenu;
        _inputReader.menuBackViewEvent += BackTrackView;
    }

    private void OnDisable()
    {
        _inputReader.pauseEvent -= OpenPauseMenu;
        _inputReader.menuUnpauseEvent -= ClosePauseMenu;
        _inputReader.menuBackViewEvent -= BackTrackView;
    }

    private void Awake()
    {
        Instance = GetComponent<UIManager>();

        playerUI = GetComponentInChildren<PlayerUI>();

        SkillSystem = GameObject.FindWithTag("Player").GetComponent<SkillSystem>();
        StatusSystem = GameObject.FindWithTag("Player").GetComponent<StatusSystem>();

    }

    private void Start()
    {
        //StatusChanging += HealthChanging;

        //healthBar.Setup(StatusSystem.healthSystem);

        KeyBindingFootBar.Initialized(_inputReader);
    }

    // 打開遊戲菜單並且把Action map設為menu action, 同時也打開footbar
    private void OpenPauseMenu()
    {
        PauseMenu.SetActive(true);
        KeyBindingFootBar.gameObject.SetActive(true);
        _inputReader.EnableMenuInput();
    }

    // 關閉遊戲菜單並且把Action map設為gameplay action, 同時也關閉footbar
    private void ClosePauseMenu()
    {
        PauseMenu.SetActive(false);
        KeyBindingFootBar.gameObject.SetActive(false);
        _inputReader.EnableGameplayInput();
    }

    private void BackTrackView()
    {
        ViewManager.ShowLast();
    }

    public void InitSkillListOnUI(List<GameObject> skillList, int ListIndex)
    {
        playerUI.ChangeSkillList(ListIndex);
        //for (int i = styleIndex * 4, j = 0; i < styleIndex * 4 + 4; i++, j++)
        //{
        //    _currentSkillsCoolDown[j] = _skillsCooldown[i];
        //    //_currentSkillsCoolDown[j].abilityObject = skillList[i].GetComponent<Spell>().spellObj;
        //    //_currentSkillsCoolDown[j].Initialize(skillList[i].GetComponent<Spell>().spellObj);
        //    _currentSkillsCoolDown[j].Initialize(_currentSkillsCoolDown[i].gameObject.GetComponent<Spell>().spellObj);

        //}
    }


    public void OnSkillButtonPressed(int index)
    {
        // call cool down script

        switch (index)
        {
            case 1:
                Debug.Log("Pressed Ability_1 button.");
                //playerUI.currentSkillsCoolDownScript[0].ButtonTriggered();
                playerUI.SkillBtnTriggered(0);
                break;

            case 2:
                Debug.Log("Pressed Ability_2 button.");
                //_skillsCooldown[1].ButtonTriggered();
                //_currentSkillsCoolDown[0].ButtonTriggered();
                //playerUI.currentSkillsCoolDownScript[1].ButtonTriggered();
                playerUI.SkillBtnTriggered(1);
                break;
            case 3:
                Debug.Log("Pressed Ability_3 button.");
                //_skillsCooldown[2].ButtonTriggered();
                //_currentSkillsCoolDown[0].ButtonTriggered();
                //playerUI.currentSkillsCoolDownScript[2].ButtonTriggered();
                playerUI.SkillBtnTriggered(2);
                break;
            case 4:
                Debug.Log("Pressed Ability_4 button.");
                //_skillsCooldown[3].ButtonTriggered();
                //_currentSkillsCoolDown[0].ButtonTriggered();
                //playerUI.currentSkillsCoolDownScript[3].ButtonTriggered();
                playerUI.SkillBtnTriggered(3);
                break;
            default:
                Debug.Log("Pressed invalid button.");
                break;
        }

    }

    public void HealthChanging(object sender, StatusEventArgs e)
    {
        if (e.target == "UI")
        {
            if (e.actType == StatusEventArgs.ActType.Damage)
            {
                StatusSystem.healthSystem.Damage(e.value);
            }
            else if (e.actType == StatusEventArgs.ActType.Heal)
            {
                StatusSystem.healthSystem.Heal(e.value);
            }
        }
        
    }

}