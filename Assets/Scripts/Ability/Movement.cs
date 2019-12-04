using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Movement", menuName = "Gameplay/Ability/Movement")]
public class Movement : Ability
{
    [Header("Movement Ability")]
    public float MovementSpeed;

    [Header("Attack on Arrival")]
    public bool AttackOnArrival;
    [ShowIf(nameof(AttackOnArrival))] public float AttackRange;
    [ShowIf(nameof(AttackOnArrival))] public float AttackDamage;

    bool _init;
    LineRenderer _movementOrderVisualizer;
    LineRenderer _aimRenderer;
    FieldOfView _rangeVisualizer;
    LineRenderer _attackOrderVisualizer;

    float MovementRange => MovementSpeed * SettingManager.Instance.CurrentGameSetting.MovementTimeWindow;

    public override void InitAiming()
    {
        if (!_rangeVisualizer)
        {
            _rangeVisualizer = new GameObject().AddComponent<FieldOfView>();
            _rangeVisualizer.name = "[RUNTIME - Movement] Range visualizer";

            _aimRenderer = new GameObject().AddComponent<LineRenderer>();
            _aimRenderer.name = "[RUNTIME - Movement] Ability radius";
            _aimRenderer.useWorldSpace = false;
            _aimRenderer.startWidth = 0.05f;
            _aimRenderer.positionCount = 30;
            _aimRenderer.loop = true;

            float change = 2 * Mathf.PI / 30;
            float angle = 0;
            for (int i = 0; i < 30; i++)
            {
                angle += change;
                _aimRenderer.SetPosition(i, new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * AttackRange);
            }
        }
        else
            _rangeVisualizer.gameObject.SetActive(true);

        _aimRenderer.gameObject.SetActive(AttackOnArrival);

        _rangeVisualizer.Init(Owner.transform.position, MovementRange, TempManager.Instance.MovementAreaMaterial, SettingManager.Instance.MoveAreaCutawayDistance);
    }

    public override void UpdateAiming(Vector3 target)
    {
        if (AttackOnArrival)
            _aimRenderer.transform.position = target;
    }

    public override void EndAiming()
    {
        if (_rangeVisualizer)
            _rangeVisualizer.gameObject.SetActive(false);

        if (_aimRenderer)
            _aimRenderer.gameObject.SetActive(false);
    }

    public override bool VisualizeAbility(Vector3 target)
    {
        target.y = Owner.transform.position.y;

        if (Vector3.Distance(Owner.transform.position, target) > MovementRange)
            return false;
        if (Physics.Linecast(Owner.transform.position, target, SettingManager.Instance.MovementObstaclesLayer))
            return false;

        if (!_movementOrderVisualizer)
        {
            _movementOrderVisualizer = new GameObject().AddComponent<LineRenderer>();
            _movementOrderVisualizer.name = "[RUNTIME - Movement] Movement Order Visualizer";
            _movementOrderVisualizer.useWorldSpace = false;
            _movementOrderVisualizer.startWidth = 0.05f;
            _movementOrderVisualizer.positionCount = 30;
            _movementOrderVisualizer.loop = true;

            _attackOrderVisualizer = new GameObject().AddComponent<LineRenderer>();
            _attackOrderVisualizer.name = "[RUNTIME - Movement] Movement Attack Order Visualizer";
            _attackOrderVisualizer.useWorldSpace = false;
            _attackOrderVisualizer.startWidth = 0.05f;
            _attackOrderVisualizer.positionCount = 30;
            _attackOrderVisualizer.loop = true;

            float change = 2 * Mathf.PI / 30;
            float angle = 0;
            for (int i = 0; i < 30; i++)
            {
                angle += change;
                _movementOrderVisualizer.SetPosition(i, new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * 0.25f);
                _attackOrderVisualizer.SetPosition(i, new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * AttackRange);
            }

            GameObject bufferObject = new GameObject();
            _movementOrderVisualizer.transform.parent = bufferObject.transform;
            _movementOrderVisualizer = bufferObject.AddComponent<LineRenderer>();
            _movementOrderVisualizer.name = "[RUNTIME - FocusManager] Movement Visualization";
            _movementOrderVisualizer.useWorldSpace = true;
            _movementOrderVisualizer.startWidth = 0.05f;
        }
        else
            _movementOrderVisualizer.gameObject.SetActive(true);

        _attackOrderVisualizer.gameObject.SetActive(AttackOnArrival);
        _attackOrderVisualizer.transform.position = target;

        _movementOrderVisualizer.transform.position = target;
        _movementOrderVisualizer.SetPosition(0, target);
        _movementOrderVisualizer.SetPosition(1, Owner.transform.position);

        return true;
    }

    public override void EndVisualization()
    {
        if (_movementOrderVisualizer)
            _movementOrderVisualizer.gameObject.SetActive(false);

        if (_attackOrderVisualizer)
            _attackOrderVisualizer.gameObject.SetActive(false);
    }

    public override AbilityTickable AbilityLogic(Vector2 target)
    {
        MovementTick movementTick = new GameObject().AddComponent<MovementTick>();
        if (AttackOnArrival)
            movementTick.Init(Owner, target, MovementSpeed, AttackRange, AttackDamage);
        else
            movementTick.Init(Owner, target, MovementSpeed);
        return movementTick;
    }

    public override void CleanForDestruction()
    {
        if (_movementOrderVisualizer)
            Destroy(_movementOrderVisualizer.gameObject);

        if (_rangeVisualizer)
            Destroy(_rangeVisualizer.gameObject);
    }
}

public class MovementTick : AbilityTickable
{
    Unit _unit;
    Vector2 _destination;
    float _movementSpeed;
    bool _attack;
    float _radius;
    float _damage;
    Vector2 _dir;

    public void Init (Unit unit, Vector2 destination, float movementSpeed, float radius, float damage)
    {
        _attack = true;
        _radius = radius;
        _damage = damage;

        Init(unit, destination, movementSpeed);
    }

    public void Init (Unit unit, Vector2 destination, float movementSpeed)
    {
        _unit = unit;
        _destination = destination;
        _movementSpeed = movementSpeed;

        _dir = destination - unit.Position;

        Init(SettingManager.Instance.CurrentGameSetting.MovementTimeWindow);
    }

    public override bool TickMe(float delta, bool finalTick)
    {
        Vector2 newPos = _unit.Position + _dir * _movementSpeed * delta;
        Vector2 newDir = _destination - newPos;
        bool arrived = false;
        if (newDir.x == 0 || Mathf.Sign(newDir.x) != Mathf.Sign(_dir.x))
            if (newDir.y == 0 || Mathf.Sign(newDir.y) != Mathf.Sign(_dir.y))
            {
                newPos = _destination;
                arrived = true;
            }

        arrived = arrived || !_unit.MoveTo(newPos);
        if (arrived)
        {
            Attack();
            return false;
        }

        return base.TickMe(delta, finalTick);
    }

    public void Attack ()
    {
        foreach (Player player in TurnManager.Instance.Players)
        {
            if (_unit.Owner == player)
                continue;

            foreach (Unit unit in player.Units)
            {
                if (unit.IsDead)
                    continue;

                if ((unit.Position - _unit.Position).sqrMagnitude <= _radius)
                    if (!Physics.Linecast(_unit.transform.position, new Vector3(unit.Position.x, _unit.transform.position.y, unit.Position.y), SettingManager.Instance.ObstaclesLayer))
                        unit.Damage(_damage);    
            }
        }
    }
}
