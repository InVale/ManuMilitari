using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AOEAbility", menuName = "Gameplay/Ability/AOE")]
public class PointAbility : Ability
{
    [Header("AOE Ability")]
    public float AOERadius;
    public float AOEMaxRange;
    public float AOEDelay;
    public float AOEDamage;

    LineRenderer _rangeRenderer;
    LineRenderer _aimRenderer;
    LineRenderer _orderRenderer;

    public override void InitAiming()
    {
        if (!_aimRenderer)
        {
            _aimRenderer = new GameObject().AddComponent<LineRenderer>();
            _aimRenderer.name = "[RUNTIME - AOE Ability] Ability aiming";
            _aimRenderer.useWorldSpace = false;
            _aimRenderer.startWidth = 0.05f;
            _aimRenderer.positionCount = 30;
            _aimRenderer.loop = true;

            _rangeRenderer = new GameObject().AddComponent<LineRenderer>();
            _rangeRenderer.name = "[RUNTIME - AOE Ability] Ability range";
            _rangeRenderer.useWorldSpace = false;
            _rangeRenderer.startWidth = 0.05f;
            _rangeRenderer.positionCount = 30;
            _rangeRenderer.loop = true;

            float change = 2 * Mathf.PI / 30;
            float angle = 0;
            for (int i = 0; i < 30; i++)
            {
                angle += change;
                _aimRenderer.SetPosition(i, new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * AOERadius);
                _rangeRenderer.SetPosition(i, new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * AOEMaxRange);
            }
        }
        else
        {
            _aimRenderer.gameObject.SetActive(true);
            _rangeRenderer.gameObject.SetActive(true);
        }

        _rangeRenderer.transform.position = Owner.transform.position;
    }

    public override void UpdateAiming(Vector3 target)
    {
        Vector3 dir = target - Owner.transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > (AOEMaxRange - AOERadius) * (AOEMaxRange - AOERadius))
            _aimRenderer.transform.position = Owner.transform.position + dir.normalized * (AOEMaxRange - AOERadius);
        else
            _aimRenderer.transform.position = target;
    }

    public override void EndAiming()
    {
        if (_aimRenderer)
            _aimRenderer.gameObject.SetActive(false);
        if (_rangeRenderer)
            _rangeRenderer.gameObject.SetActive(false);
    }

    public override bool VisualizeAbility(Vector3 target)
    {
        if (!_orderRenderer)
        {
            _orderRenderer = new GameObject().AddComponent<LineRenderer>();
            _orderRenderer.name = "[RUNTIME - AOE Ability] Ability visualizer";
            _orderRenderer.useWorldSpace = false;
            _orderRenderer.startWidth = 0.05f;
            _orderRenderer.positionCount = 30;
            _orderRenderer.loop = true;

            float change = 2 * Mathf.PI / 30;
            float angle = 0;
            for (int i = 0; i < 30; i++)
            {
                angle += change;
                _orderRenderer.SetPosition(i, new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * AOERadius);
            }
        }
        else
            _orderRenderer.gameObject.SetActive(true);

        Vector3 dir = target - Owner.transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > (AOEMaxRange - AOERadius) * (AOEMaxRange - AOERadius))
            _orderRenderer.transform.position = Owner.transform.position + dir.normalized * (AOEMaxRange - AOERadius);
        else
            _orderRenderer.transform.position = target;

        return true;
    }

    public override void EndVisualization()
    {
        if (_orderRenderer)
            _orderRenderer.gameObject.SetActive(false);
    }

    public override AbilityTickable AbilityLogic(Vector2 target)
    {
        AOEObject ability = new GameObject().AddComponent<AOEObject>();
        ability.Init(Owner, target, AOERadius, AOEDelay, AOEDamage);
        return ability;
    }

    public override void CleanForDestruction()
    {
        if (_aimRenderer)
            Destroy(_aimRenderer.gameObject);
    }
}

public class AOEObject : AbilityTickable
{
    Vector2 _position;
    float _radius;
    float _damage;
    float _delay;
    Unit _owner;

    public void Init(Unit owner, Vector2 position, float radius, float delay, float damage)
    {
        _owner = owner;
        _position = position;
        _radius = radius * radius;
        _delay = SettingManager.Instance.CurrentGameSetting.MovementTimeWindow - delay;
        _damage = damage;

        Init(SettingManager.Instance.CurrentGameSetting.MovementTimeWindow);
    }

    public override bool TickMe(float delta, bool finalTick)
    {
        base.TickMe(delta, finalTick);

        if (_duration <= _delay)
        {
            foreach (Player player in TurnManager.Instance.Players)
            {
                if (_owner.Owner == player)
                    continue;

                foreach (Unit unit in player.Units)
                {
                    if (unit.IsDead)
                        continue;

                    if ((unit.Position - _position).sqrMagnitude <= _radius)
                        if (!Physics.Linecast(_owner.transform.position, new Vector3(unit.Position.x, _owner.transform.position.y, unit.Position.y), SettingManager.Instance.ObstaclesLayer))
                            unit.Damage(_damage);
                }
            }

            return false;
        }

        return true;
    }
}
