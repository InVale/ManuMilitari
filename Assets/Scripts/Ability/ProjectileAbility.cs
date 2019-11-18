using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileAbility", menuName = "Gameplay/Ability/Projectile")]
public class ProjectileAbility : Ability
{
    [Header("Projectile Ability")]
    public float ProjectileRange;
    public float ProjectileSpeed;
    public float ProjectileDamage;
    public GameObject ProjectilePrefab;

    LineRenderer _aimRenderer;
    LineRenderer _orderRenderer;

    public override void InitAiming()
    {
        if (!_aimRenderer)
        {
            _aimRenderer = new GameObject().AddComponent<LineRenderer>();
            _aimRenderer.name = "[RUNTIME - Projectile Ability] Ability aiming";
            _aimRenderer.useWorldSpace = true;
            _aimRenderer.startWidth = 0.1f;
        }
        else
            _aimRenderer.gameObject.SetActive(true);
    }

    public override void UpdateAiming(Vector3 target)
    {
        _aimRenderer.SetPosition(0, Owner.transform.position);

        Vector3 dir = (target - Owner.transform.position);
        dir.y = 0;
        dir.Normalize();
        RaycastHit hit;
        if (Physics.Raycast(Owner.transform.position, dir, out hit, ProjectileRange, SettingManager.Instance.ObstaclesLayer))
            _aimRenderer.SetPosition(1, hit.point);
        else
            _aimRenderer.SetPosition(1, Owner.transform.position + dir * ProjectileRange);

    }

    public override void EndAiming()
    {
        if (_aimRenderer)
            _aimRenderer.gameObject.SetActive(false);
    }

    public override bool VisualizeAbility(Vector3 target)
    {
        if (!_orderRenderer)
        {
            _orderRenderer = new GameObject().AddComponent<LineRenderer>();
            _orderRenderer.name = "[RUNTIME - Projectile Ability] Ability visualization";
            _orderRenderer.useWorldSpace = true;
            _orderRenderer.startWidth = 0.1f;
        }
        else
            _orderRenderer.gameObject.SetActive(true);

        _orderRenderer.SetPosition(0, Owner.transform.position);

        Vector3 dir = (target - Owner.transform.position);
        dir.y = 0;
        dir.Normalize();
        RaycastHit hit;
        if (Physics.Raycast(Owner.transform.position, dir, out hit, ProjectileRange, SettingManager.Instance.ObstaclesLayer))
            _orderRenderer.SetPosition(1, hit.point);
        else
            _orderRenderer.SetPosition(1, Owner.transform.position + dir * ProjectileRange);

        return true;
    }

    public override void EndVisualization()
    {
        if (_orderRenderer)
            _orderRenderer.gameObject.SetActive(false);
    }

    public override AbilityTickable AbilityLogic(Vector2 target)
    {
        GameObject proj = Instantiate(ProjectilePrefab);
        proj.transform.position = Owner.transform.position;
        proj.transform.localScale = Vector3.one * 0.25f;
        ProjectileObject projectile = proj.AddComponent<ProjectileObject>();
        projectile.Init(Owner, (ProjectileRange / ProjectileSpeed), (target - Owner.Position).normalized, ProjectileSpeed, ProjectileDamage);
        return projectile;
    }

    public override void CleanForDestruction()
    {
        if (_aimRenderer)
            Destroy(_aimRenderer.gameObject);
    }
}

public class ProjectileObject : AbilityTickable
{
    Vector2 _direction;
    float _speed;
    float _damage;
    bool _done;
    Unit _owner;

    public void Init(Unit owner, float duration, Vector2 direction, float speed, float damage)
    {
        _owner = owner;
        _direction = direction;
        _speed = speed;
        _damage = damage;
        Init(duration);
    }

    public override bool TickMe(float delta, bool finalTick)
    {
        Vector2 mov = _direction * delta * _speed;
        transform.position += new Vector3(mov.x, 0, mov.y);

        if (_done)
            return false;

        return base.TickMe(delta, finalTick);
    }

    void OnTriggerEnter(Collider collider)
    {
        Unit unit = collider.GetComponent<Unit>();
        if (unit)
        {
            if (unit == _owner)
                return;
            unit.Damage(_damage);
        }

        _done = true;
    }
}
