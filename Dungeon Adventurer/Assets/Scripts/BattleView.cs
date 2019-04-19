using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : View {
    [SerializeField] Button retreatbutton;
    [SerializeField] TimeLineController tlController;
    [SerializeField] GameObject preFab;
    [SerializeField] TextMeshProUGUI turnTimer;
    [SerializeField] SkillEntry[] skillEntries;
    [SerializeField] UseSkillButton useSkillbutton;
    [SerializeField] BattleSlot[] heroSlots = new BattleSlot[9];
    [SerializeField] BattleSlot[] monsterSlots = new BattleSlot[9];
    [SerializeField] GameObject finishFeedback;

    static BattleController controller;

    const float TURN_TIME = 20f;

    bool _battleActive = false;
    bool _nextTurn = false;
    float _currentTurnTime = 0f;
    Coroutine _battleCycle;
    Character _currentChar;
    public Character CurrentChar {
        get { return _currentChar; }
        set
        {
            SelectedChar = value;
            _currentChar = value;
        }
    }
    Character _selectedTarget;
    Skill _selectedSkill;

    Dictionary<int, Character> positionedChars = new Dictionary<int, Character>();

    static SkillStatus _currentSkillStatus;
    public static SkillStatus CurrentSkillStatus {
        get
        {
            return _currentSkillStatus;
        }
        set
        {
            _currentSkillStatus = value;
            StatusChanged();
        }
    }

    public delegate void StatusEvent();
    public static event StatusEvent StatusChanged;

    static Character _selectedChar;
    public static Character SelectedChar {
        get
        {
            return _selectedChar;
        }
        set
        {
            _selectedChar = value;
            SelectedCharChanged();
        }
    }
    public static event StatusEvent SelectedCharChanged;

    public void SetData() {
        tlController.SetData(controller.heroes.ToArray(), controller.Monsters.ToArray());
        retreatbutton.onClick.AddListener(() => { CloseScene(); });
        useSkillbutton.SetData(UseSkill);
    }

    public void CloseScene() {
        UIManager._instance.RemoveLastScene();
    }

    public static Character GetCharacter(int id) {
        return controller.allCharacters[id];
    }

    void UseSkill() {
        var targets = new List<Character>();
        switch (_selectedSkill.targetCount) {
            case TargetAmount.Single:
                targets.Add(_selectedTarget);
                break;
            case TargetAmount.Multiple:
                if (_selectedSkill.targetType == TargetType.Enemy) {
                    foreach (var chars in controller.allCharacters) {
                        if (chars.Key < 0 && _selectedSkill.CheckForPossibleTarget(chars.Value.position))
                            targets.Add(chars.Value);
                    }
                } else if (_selectedSkill.targetType == TargetType.Team) {
                    foreach (var chars in controller.allCharacters) {
                        if (chars.Key > 0 && _selectedSkill.CheckForPossibleTarget(chars.Value.position))
                            targets.Add(chars.Value);
                    }
                }
                break;
            case TargetAmount.All:
                if (_selectedSkill.targetType == TargetType.Enemy) {
                    foreach (var chars in controller.allCharacters) {
                        if (chars.Key < 0)
                            targets.Add(chars.Value);
                    }
                } else if (_selectedSkill.targetType == TargetType.Team) {
                    foreach (var chars in controller.allCharacters) {
                        if (chars.Key > 0)
                            targets.Add(chars.Value);
                    }
                }
                break;
            case TargetAmount.GlobalAll:
                foreach (var chars in controller.allCharacters) {
                    targets.Add(chars.Value);
                }
                break;
        }
        _selectedSkill.UseSkill(targets.ToArray());
        EndTurn();
    }

    public override void OnControllerChanged(Controller newController) {
        controller = (BattleController)newController;
        SetData();
        SetPositions();
        controller.battleEnded.AddListener(EndBattle);
    }

    public override void AfterShow() {
        StartBattle();
    }

    void StartBattle() {
        _battleActive = true;
        if (_battleCycle == null) {
            _battleCycle = StartCoroutine(BattleCycle());
        }
    }

    void FirstTurn() {
        var firstEntryId = TimeLineController.controller.FirstEntry();
        CurrentChar = controller.allCharacters[firstEntryId];
        TurnSetup();
    }

    public void EndTurn() {
        _currentTurnTime = 0f;
        _nextTurn = true;
        var nextId = TimeLineController.controller.NextTurn();
        CurrentChar = controller.allCharacters[nextId];
        TurnSetup();
    }

    void TurnSetup() {
        _selectedTarget = null;
        CurrentSkillStatus = SkillStatus.selectSkill;
        if (CurrentChar.id > 0) {
            SetSkillInfo(CurrentChar);
        } else {
            CurrentSkillStatus = SkillStatus.enemyTurn;
            SetSkillPlaceHolders();
        }
        UnhighlightEverything();
        HightLightActivePlayer();
    }

    private IEnumerator BattleCycle() {
        Coroutine turnTimerCoroutine = null;
        FirstTurn();
        while (_battleActive) {
            _currentTurnTime = TURN_TIME;
            _nextTurn = false;

            if (turnTimerCoroutine != null)
                StopCoroutine(turnTimerCoroutine);
            turnTimerCoroutine = StartCoroutine(TurnTimerCycle());
            yield return new WaitWhile(() => !_nextTurn);
        }
    }

    private IEnumerator TurnTimerCycle() {
        while (!_nextTurn) {
            if (_currentTurnTime - Time.deltaTime > 0)
                _currentTurnTime -= Time.deltaTime;
            else {
                EndTurn();
                turnTimer.text = Mathf.CeilToInt(_currentTurnTime).ToString("00");
                yield return new WaitForSeconds(1f);
                break;
            }
            turnTimer.text = Mathf.CeilToInt(_currentTurnTime).ToString("00");
            yield return new WaitForEndOfFrame();
        }
    }

    void SetPositions() {
        foreach (var hero in controller.heroes) {
            var s = Instantiate(preFab).transform;
            s.SetParent(heroSlots[(int)hero.position].transform);
            s.localPosition = new Vector3(0, 0, 0);
            s.localScale = new Vector3(14, 14, 0);
            s.GetComponent<BattleChar>().SetData(hero, DataHolder._data.raceImages[(int)hero.race]);
            heroSlots[hero.position].SetData(CharClicked, TargetClicked);
            positionedChars.Add(hero.id, hero);
        }

        foreach (var monster in controller.positionedMonsters) {
            var s = Instantiate(preFab).transform;
            s.SetParent(monsterSlots[(int)monster.Value.position].transform);
            s.localPosition = new Vector3(0, 0, 0);
            s.localScale = new Vector3(105, 105, 0);
            s.GetComponent<BattleChar>().SetData(monster.Value, monster.Value.icon);
            monsterSlots[monster.Key].SetData(CharClicked, TargetClicked);
            positionedChars.Add(monster.Value.id, monster.Value);
        }
    }

    void EndBattle() {
        finishFeedback.SetActive(true);
        _battleActive = false;
    }

    void CharClicked(int id) {
        SelectedChar = controller.allCharacters[id];
    }

    void TargetClicked(int id) {
        if (_selectedSkill?.targetCount != TargetAmount.Single)
            return;
        _selectedTarget = controller.allCharacters[id];
        CurrentSkillStatus = SkillStatus.useSkill;
    }

    void UnhighlightEverything() {
        foreach (BattleSlot trans in heroSlots) {
            trans.SetTarget(false);
        }
        foreach (BattleSlot trans in monsterSlots) {
            trans.SetTarget(false);
        }
    }

    void HightLightActivePlayer() {
        var slots = CurrentChar.id > 0 ? heroSlots : monsterSlots;

        var pos = CurrentChar.position;

        slots[pos].SetColor(Colors.BATTLE_ACTIVE);
    }

    void HighLightPlayingField(int skillID) {
        var slots = _selectedSkill.targetType == TargetType.Enemy ? monsterSlots : heroSlots;

        for (int i = 0; i < slots.Length; i++) {
            var highlight = _selectedSkill.possibleTargets[i];
            slots[i].SetTarget(highlight);
            if (highlight && slots[i].transform.childCount > 0) {
                if (_selectedSkill.targetCount == TargetAmount.Single) {
                    CurrentSkillStatus = SkillStatus.selectTarget;
                } else if (_selectedSkill.targetCount == TargetAmount.Multiple) {
                    CurrentSkillStatus = SkillStatus.useSkill;
                }
            }
        }
    }

    public void SetSkillPlaceHolders() {
        ResetSkillEntryColors();
        for (int i = 0; i < skillEntries.Length; i++) {
            skillEntries[i].EnablePlaceHolder();
        }
    }

    public void SetSkillInfo(Character character) {
        ResetSkillEntryColors();
        for (int i = 0; i < character.skills.Length; i++) {
            skillEntries[i].SetData(character.skills[i], i, e => SelectSkill(e));
        }
    }

    void ResetSkillEntryColors() {
        foreach (var entry in skillEntries) {
            entry.SetColor(Color.white);
        }
    }

    void SelectSkill(int selectedSkill) {
        _selectedSkill = CurrentChar.skills[selectedSkill];
        CurrentSkillStatus = SkillStatus.selectTarget;
        for (int i = 0; i < skillEntries.Length; i++) {
            skillEntries[i].SetColor(i != selectedSkill ? Color.white : Colors.BATTLE_ACTIVE);
        }
        HighLightPlayingField(selectedSkill);
    }
}
public enum SkillStatus {
    selectTarget,
    noTargets,
    useSkill,
    selectSkill,
    enemyTurn
}
