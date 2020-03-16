using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ChangePosition;
using static ForceAttack;
using static Summon;

public class BattleView : View
{
    const float TURN_TIME = 20f;

    [SerializeField] Button retreatbutton;

    [Header("Timeline")]
    [SerializeField] TimeLineController tlController;
    [SerializeField] TextMeshProUGUI turnTimer;

    [Header("Skill")]
    [SerializeField] TextMeshProUGUI skillDescription;
    [SerializeField] SkillEntry[] skillEntries;
    [SerializeField] UseSkillButton useSkillbutton;

    [Header("Characters")]
    [SerializeField] GameObject battleCharPrefab;
    [SerializeField] BattleSlot[] heroSlots = new BattleSlot[9];
    [SerializeField] BattleSlot[] monsterSlots = new BattleSlot[9];
    [SerializeField] CharacterChangeDisplayCollection displayCollection;

    public static BattleController controller;

    bool _battleActive = false;
    bool _nextTurn = false;
    float _currentTurnTime = 0f;
    Coroutine _battleCycle;
    Character _currentChar;
    public Character CurrentChar
    {
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
    public static SkillStatus CurrentSkillStatus
    {
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

    static BattleSlot _selectedSlot;
    static Character _selectedChar;
    public static Character SelectedChar
    {
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

    protected override void Awake()
    {
        retreatbutton.onClick.AddListener(() => { controller.Retreat(); });
    }

    public void SetData()
    {
        tlController.SetData(controller.Heroes.ToArray(), controller.Monsters.ToArray());
        useSkillbutton.SetData(UseSkill);
    }

    public static Character GetCharacter(int id)
    {
        return controller.allCharacters[id];
    }

    void UseSkill()
    {
        if (CurrentSkillStatus != SkillStatus.useSkill) return;

        if (_selectedSkill.targetType != TargetType.Position)
        {
            var targets = new List<Character>();
            switch (_selectedSkill.targetCount)
            {
                case TargetAmount.Single:
                    targets.Add(_selectedTarget);
                    break;
                case TargetAmount.Multiple:
                    if (_selectedSkill.targetSite == TargetSite.Enemy)
                    {
                        foreach (var chars in controller.allCharacters)
                        {
                            if (chars.Key < 0 && _selectedSkill.CheckForPossibleTarget(chars.Value.position))
                                targets.Add(chars.Value);
                        }
                    } else if (_selectedSkill.targetSite == TargetSite.Team)
                    {
                        foreach (var chars in controller.allCharacters)
                        {
                            if (chars.Key > 0 && _selectedSkill.CheckForPossibleTarget(chars.Value.position))
                                targets.Add(chars.Value);
                        }
                    }
                    break;
                case TargetAmount.All:
                    if (_selectedSkill.targetSite == TargetSite.Enemy)
                    {
                        foreach (var chars in controller.allCharacters)
                        {
                            if (chars.Key < 0)
                                targets.Add(chars.Value);
                        }
                    } else if (_selectedSkill.targetSite == TargetSite.Team)
                    {
                        foreach (var chars in controller.allCharacters)
                        {
                            if (chars.Key > 0)
                                targets.Add(chars.Value);
                        }
                    }
                    break;
                case TargetAmount.GlobalAll:
                    foreach (var chars in controller.allCharacters)
                    {
                        targets.Add(chars.Value);
                    }
                    break;
            }
            _selectedSkill.UseSkill(_currentChar, targets.ToArray(), this);
        } else
        {
            // _selectedSkill.UseSkill(_currentChar, this);
        }
        EndTurn();
    }

    public void SetController(Controller newController)
    {
        controller = (BattleController)newController;
        SetData();
        SetPositions();
        controller.battleEnded.AddListener(EndBattle);
    }

    public override void AfterShow()
    {
        StartBattle();
    }

    void StartBattle()
    {
        _battleActive = true;
        if (_battleCycle == null)
        {
            _battleCycle = StartCoroutine(BattleCycle());
        }
    }

    void EndBattle()
    {
        _battleActive = false;
        _battleCycle = null;
    }

    void FirstTurn()
    {
        var firstEntryId = TimeLineController.controller.FirstEntry();
        CurrentChar = controller.allCharacters[firstEntryId];
        TurnSetup();
    }

    public void EndTurn()
    {
        if (!_battleActive) return;

        _currentTurnTime = 0f;
        _nextTurn = true;
        var nextId = TimeLineController.controller.NextTurn();
        CurrentChar = controller.allCharacters[nextId];
        TurnSetup();
    }

    void TurnSetup()
    {
        _selectedTarget = null;
        CurrentSkillStatus = SkillStatus.selectSkill;

        if (CurrentChar.Skills != null && CurrentChar.Skills.Length > 0) CurrentChar.Skills.ToList().ForEach(skill => skill.ReduceCooldown(1));
        if (CurrentChar.id > 0)
        {
            SetSkillInfo(CurrentChar);
        } else
        {
            CurrentSkillStatus = SkillStatus.enemyTurn;
            SetSkillPlaceHolders();
        }
        UnhighlightEverything();
        HightLightActivePlayer();

        CurrentChar.ApplyRegen();
        CurrentChar.ReduceTurnCounts();
    }

    private IEnumerator BattleCycle()
    {
        Coroutine turnTimerCoroutine = null;
        FirstTurn();
        while (_battleActive)
        {
            _currentTurnTime = TURN_TIME;
            _nextTurn = false;

            if (turnTimerCoroutine != null)
                StopCoroutine(turnTimerCoroutine);
            turnTimerCoroutine = StartCoroutine(TurnTimerCycle());
            yield return new WaitWhile(() => !_nextTurn);
        }
    }

    private IEnumerator TurnTimerCycle()
    {
        while (!_nextTurn)
        {
            if (_currentTurnTime - Time.deltaTime > 0)
                _currentTurnTime -= Time.deltaTime;
            else
            {
                EndTurn();
                turnTimer.text = Mathf.CeilToInt(_currentTurnTime).ToString("00");
                yield return new WaitForSeconds(1f);
                break;
            }
            turnTimer.text = Mathf.CeilToInt(_currentTurnTime).ToString("00");
            yield return new WaitForEndOfFrame();
        }
    }

    void SetPositions()
    {
        positionedChars.Clear();
        ClearSlots();

        foreach (var hero in controller.Heroes)
        {
            var s = Instantiate(battleCharPrefab).transform;
            s.SetParent(heroSlots[(int)hero.position].transform);
            s.localPosition = Vector3.zero;
            s.localScale = Vector3.one;
            s.GetComponent<BattleChar>().SetData(hero, DataHolder._data.raceImages[(int)hero.race], displayCollection);
            heroSlots[hero.position].SetData(CharClicked, TargetClicked);
            positionedChars.Add(hero.id, hero);
        }

        foreach (var monster in controller.positionedMonsters)
        {
            var s = Instantiate(battleCharPrefab).transform;
            s.SetParent(monsterSlots[(int)monster.Value.position].transform);
            s.localPosition = Vector3.zero;
            s.localScale = Vector3.one;
            s.GetComponent<BattleChar>().SetData(monster.Value, monster.Value.icon, displayCollection);
            monsterSlots[monster.Key].SetData(CharClicked, TargetClicked);
            positionedChars.Add(monster.Value.id, monster.Value);
        }
    }

    void ClearSlots()
    {
        foreach (var slot in heroSlots)
        {
            var trans = slot.GetBattlecharTransform();
            if (trans == null) continue;
            Destroy(trans.gameObject);
        }

        foreach (var slot in monsterSlots)
        {
            var trans = slot.GetBattlecharTransform();
            if (trans == null) continue;
            Destroy(trans.gameObject);
        }
    }

    void CharClicked(int id)
    {
        SelectedChar = controller.allCharacters[id];
    }

    void TargetClicked(BattleSlot slot, int id)
    {
        if (_selectedSkill?.targetCount != TargetAmount.Single)
            return;
        if (controller.allCharacters.ContainsKey(id))
        {
            _selectedTarget = controller.allCharacters[id];
        }
        _selectedSlot = slot;

        CurrentSkillStatus = SkillStatus.useSkill;
        UnhighlightEverything();
        HighLightPlayingField();
        HighlightTargetslot();
    }

    void HighlightTargetslot()
    {
        _selectedSlot.SetColor(Color.gray);
    }

    void UnhighlightEverything()
    {
        foreach (BattleSlot trans in heroSlots)
        {
            trans.SetTarget(false);
        }
        foreach (BattleSlot trans in monsterSlots)
        {
            trans.SetTarget(false);
        }
        HightLightActivePlayer();
    }

    void HightLightActivePlayer()
    {
        var slots = CurrentChar.id > 0 ? heroSlots : monsterSlots;

        var pos = CurrentChar.position;

        slots[pos].SetColor(Colors.BATTLE_ACTIVE);
    }

    void HighLightPlayingField()
    {
        var slots = _selectedSkill.targetSite == TargetSite.Enemy ? monsterSlots : heroSlots;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetTarget(_selectedSkill.possibleTargets[i]);
        }
    }

    public void SetSkillPlaceHolders()
    {
        ResetSkillEntryColors();
        for (int i = 0; i < skillEntries.Length; i++)
        {
            skillEntries[i].EnablePlaceHolder();
        }
    }

    public void SetSkillInfo(Character character)
    {
        ResetSkillEntryColors();
        for (int i = 0; i < character.Skills.Length; i++)
        {
            skillEntries[i].SetData(character, character.Skills[i], i, e => SelectSkill(e));
        }
    }

    void ResetSkillEntryColors()
    {
        foreach (var entry in skillEntries)
        {
            entry.SetColor(Color.white);
        }
    }

    void SelectSkill(int selectedSkill)
    {
        _selectedTarget = null;
        _selectedSlot = null;

        UnhighlightEverything();

        _selectedSkill = CurrentChar.Skills[selectedSkill];

        if (_selectedSkill.targetSite == TargetSite.Self)
        {
            CurrentSkillStatus = SkillStatus.useSkill;
        } else
        {
            if (_selectedSkill.targetCount == TargetAmount.Single)
            {
                switch (_selectedSkill.targetType)
                {
                    case TargetType.Character:
                        CurrentSkillStatus = SkillStatus.selectCharacter;
                        break;
                    case TargetType.Position:
                        CurrentSkillStatus = SkillStatus.selectPosition;
                        break;
                    case TargetType.CharacterAndPosition:
                        CurrentSkillStatus = SkillStatus.selectCharAndPosition;
                        break;
                }
            } else
            {
                CurrentSkillStatus = SkillStatus.useSkill;
            }
        }

        for (int i = 0; i < skillEntries.Length; i++)
        {
            skillEntries[i].SetColor(i == selectedSkill && _selectedSkill.IsUsable(_currentChar) ? Colors.BATTLE_ACTIVE : Color.white);
        }
        HighLightPlayingField();
        ChangeSkillDescription();
    }

    void ChangeSkillDescription()
    {
        var skill = _selectedSkill;
        var desc = skill.GetDescription(_currentChar);
        skillDescription.text = desc;
    }

    public void ForceAttack(Character target, float damagePercentage, DamageType type, AttackMode mode)
    {
        var posCharacters = target.GetType() == typeof(Hero) ? heroSlots : monsterSlots;
        var targets = new List<Character>();
        var targetPos = target.position;
        var posVictims = new int[4] { targetPos - 3, targetPos - 1, targetPos + 1, targetPos + 3 };
        switch (mode)
        {
            case AttackMode.AllClose:
                foreach (var posPosition in posVictims)
                {
                    if (posPosition < 0 || posPosition > 8)
                        continue;

                    var id = posCharacters[posPosition].AppliedId;
                    if (!controller.allCharacters.TryGetValue(id, out var victim))
                        continue;

                    targets.Add(victim);
                }
                break;

            case AttackMode.Random:
                if (controller.positionedMonsters.Count <= 1) break;

                var pool = controller.positionedMonsters;
                pool.Remove(target.position);

                var randTarget = pool[UnityEngine.Random.Range(0, pool.Count)];
                targets.Add(randTarget);
                break;

            case AttackMode.RandomClose:
                var randTargets = new List<Character>();
                foreach (var posPosition in posVictims)
                {
                    if (posPosition < 0 || posPosition > 8)
                        continue;

                    var id = posCharacters[posPosition].AppliedId;
                    if (!controller.allCharacters.TryGetValue(id, out var victim))
                        continue;

                    randTargets.Add(victim);
                }
                var randTarget1 = randTargets[UnityEngine.Random.Range(0, randTargets.Count)];
                targets.Add(randTarget1);

                break;
        }

        foreach (var t in targets)
        {
            DealDamage(t);
        }

        void DealDamage(Character victim)
        {
            var amount = 0f;
            if (target.GetType() == typeof(Hero))
            {
                var hero = target as Hero;
                amount = UnityEngine.Random.Range(hero.MinPhysicalDamage, hero.MaxPhysicalDamage + 1);
            } else
            {
                var monster = target as Monster;
                amount = UnityEngine.Random.Range(monster.minPhysicalDamage, monster.maxPhysicalDamage + 1);
            }

            amount = amount * damagePercentage;
            var appliedDamage = victim.ApplyDamage(target, type, amount, 0);
        }
    }

    public void Summon(Character caster, Monster monster, SummonPositions positions, int turnCounts)
    {
        var casterPos = caster.position;
        var posCharacters = caster.GetType() == typeof(Hero) ? heroSlots : monsterSlots;
        var newPos = 0;
        switch (positions)
        {
            case SummonPositions.Target:
                newPos = Array.IndexOf(posCharacters, _selectedSlot);
                break;
            case SummonPositions.Back:
                newPos = GetNewSlot(new int[] { 2, 5, 8 });
                break;
            case SummonPositions.Front:
                newPos = GetNewSlot(new int[] { 0, 3, 6 });
                break;
            case SummonPositions.Middle:
                newPos = GetNewSlot(new int[] { 1, 4, 7 });
                break;
            case SummonPositions.Close:
                newPos = GetNewSlot(new int[] { casterPos - 3, casterPos - 1, casterPos + 1, casterPos + 3 });
                break;
            case SummonPositions.Random:
                var randList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                newPos = GetNewSlot(randList.ToArray());
                break;
        }

        if (newPos < 0) return;

        monster.position = newPos;
        var s = Instantiate(battleCharPrefab).transform;
        s.SetParent(monsterSlots[(int)monster.position].transform);
        s.localPosition = Vector3.zero;
        s.localScale = Vector3.one;
        s.GetComponent<BattleChar>().SetData(monster, monster.icon, displayCollection);
        monsterSlots[monster.position].SetData(CharClicked, TargetClicked);
        positionedChars.Add(monster.id, monster);
        tlController.AddEntry(monster);

        int GetNewSlot(int[] posSlots)
        {
            var posPool = new List<int>();
            foreach (var posSlot in posSlots)
            {
                if (posSlot < 0 || posSlot > 8 || posCharacters[posSlot].AppliedId != 0)
                    continue;
                posPool.Add(posSlot);
            }

            if (posPool.Count == 0)
            {
                return -1;
            } else
            {
                return posPool[UnityEngine.Random.Range(0, posPool.Count)];
            }
        }
    }

    public void ChangePosition(Character target, MoveDirection direction)
    {
        var posCharacters = target.GetType() == typeof(Hero) ? heroSlots : monsterSlots;

        posCharacters[target.position].RemoveChar();
        var newSlot = posCharacters[0];
        var movedChar = newSlot.GetBattlecharTransform();
        var pos = target.position;
        var newPos = 0;
        switch (direction)
        {
            case MoveDirection.Target:
                newPos = Array.IndexOf(posCharacters, _selectedSlot);
                break;
            case MoveDirection.Back:
                if (pos == 0 || pos == 3 || pos == 6) return;

                newPos = GetNewSlot(new int[] { pos - 4, pos - 1, pos + 2 });
                break;
            case MoveDirection.Front:
                if (pos == 2 || pos == 5 || pos == 8) return;

                newPos = GetNewSlot(new int[] { pos - 2, pos + 1, pos + 4 });
                break;
            case MoveDirection.Side:
                newPos = GetNewSlot(new int[] { pos - 3, pos + 3 });
                break;
            case MoveDirection.Random:
                var randList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                randList.Remove(target.position);
                newPos = GetNewSlot(randList.ToArray());
                break;

        }
        newSlot = posCharacters[newPos];
        target.position = newPos;
        var portrait = target.GetType() == typeof(Hero) ? DataHolder._data.raceImages[(int)(((Hero)target).race)] : ((Monster)target).icon;

        movedChar.SetParent(posCharacters[(int)target.position].transform);
        movedChar.localPosition = Vector3.zero;
        movedChar.localScale = Vector3.one;
        movedChar.GetComponent<BattleChar>().SetData(target, portrait, displayCollection);
        posCharacters[target.position].SetData(CharClicked, TargetClicked);
        positionedChars.Add(target.id, target);

        int GetNewSlot(int[] posSlots)
        {
            var posPool = new List<int>();
            foreach (var posSlot in posSlots)
            {
                if (posSlot < 0 || posSlot > 8 || posCharacters[posSlot].AppliedId != 0)
                    continue;
                posPool.Add(posSlot);
            }

            if (posPool.Count == 0)
            {
                return target.position;
            } else
            {
                return posPool[UnityEngine.Random.Range(0, posPool.Count)];
            }
        }

    }
}
public enum SkillStatus
{
    selectCharacter,
    noTargets,
    useSkill,
    selectSkill,
    enemyTurn,
    selectPosition,
    selectCharAndPosition
}
