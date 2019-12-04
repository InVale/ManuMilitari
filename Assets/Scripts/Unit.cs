using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Order UnitOrder;

    public Vector2 Position;// { get; private set; }
    public float Rotation { get; private set; }
    [NonSerialized] public Player Owner;
    [NonSerialized] public int ID;

    //Specs
    public float MaxHealth = 100;
    public float ViewDistance = 5;

    [SerializeField] List<Ability> _unitAbilities = new List<Ability>();
    public List<Ability> UnitAbilities { get; private set; } = new List<Ability>();

    public bool HasOrder => (UnitOrder.Abilities.Count > 0);
    public float Health { get; private set; }
    public bool IsDead => Health == 0;

    FieldOfView _view;

    public void Init(Player owner, int id, Transform position)
    {
        Position = new Vector2(position.position.x, position.position.z);
        Rotation = position.eulerAngles.y;
        Owner = owner;
        ID = id;

        if (owner.MyType == PlayerType.LOCAL)
        {
            _view = new GameObject().AddComponent<FieldOfView>();
            _view.name = "[RUNTIME - Unit] View";
            _view.Init(position.position, ViewDistance, TempManager.Instance.FogViewMaterial);
        }
    }

    public void Start ()
    {
        UnitOrder = new Order(ID);
        Health = MaxHealth;

        for (int i = 0; i < _unitAbilities.Count; i++)
        {
            Ability instance = Instantiate(_unitAbilities[i]);
            instance.Owner = this;
            instance.ID = i;
            UnitAbilities.Add(instance);
        }
    }

    public bool MoveTo (Vector2 target)
    {
        bool collided = false;

        Vector2 dir = target - Position;
        Position = target;
        Rotation = -Vector2.SignedAngle(Vector2.up, dir);

        UpdatePos();

        return !collided;
    }

    void UpdatePos ()
    {
        Vector3 pos = transform.position;
        pos.x = Position.x;
        pos.z = Position.y;
        transform.position = pos;

        transform.eulerAngles = new Vector3(0, Rotation, 0);

        if (_view)
            _view.Init(transform.position, ViewDistance, TempManager.Instance.FogViewMaterial);
    }

    public void Damage (float damage)
    {
        if (!IsDead && damage > 0)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Health = 0;
                Death();
            }
        }
    }

    public void Heal (float healing)
    {
        if (!IsDead && healing > 0)
        {
            Health += healing;
            if (Health > MaxHealth)
                Health = MaxHealth;
        }
    }

    void Death ()
    {
        gameObject.SetActive(false);
    }
}
