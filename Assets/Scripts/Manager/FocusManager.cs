using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FocusManager : MonoBehaviour
{
    public static FocusManager Instance;

    public Unit FocusedUnit { get; private set; }

    Ability _focusedAbility;
    GameObject _abilityHolder;
    List<Button> _abilityButtons = new List<Button>();

    void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    public void Start ()
    {
        _abilityHolder = Instantiate(AssetManager.Instance.UIAbilityHolder);
        _abilityHolder.name = "[RUNTIME - Focus Manager] UI - Abilities Holder";
        _abilityHolder.SetActive(false);
    }

    void Update()
    {
        if (TurnManager.Instance.State == TurnState.WAITING && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (FocusedUnit)
                {
                    if (_focusedAbility)
                    {
                        UnfocusAbility();
                        DrawAbilities(FocusedUnit);
                    }
                    else
                        UnfocusUnit();
                }
                return;
            }

            if (!_focusedAbility && Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, SettingManager.Instance.SelectionLayer))
                {
                    Selectable touched = hit.collider.GetComponent<Selectable>();
                    if (touched)
                    {
                        FocusUnit(touched.MyUnit);
                    }
                    return;
                }
            }

            if (FocusedUnit && _focusedAbility)
            {
                RaycastHit hit;
                Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, SettingManager.Instance.GroundLayer))
                {
                    _focusedAbility.UpdateAiming(hit.point);
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (_focusedAbility.VisualizeAbility(hit.point))
                        {
                            FocusedUnit.UnitOrder.Abilities.Add(new Tuple<int, Vector2>(_focusedAbility.ID, new Vector2(hit.point.x, hit.point.z)));
                            UnfocusUnit();
                        }
                    }
                }
            }
        }
    }

    public void FocusUnit (Unit unit)
    {
        if((!TempManager.Instance.Build && unit.Owner.MyType == PlayerType.DEBUG) || unit.Owner == TurnManager.Instance.LocalPlayers[TurnManager.Instance.currentLocalPlayer])
        {
            if (FocusedUnit != unit)
            {
                UnfocusUnit();

                FocusedUnit = unit;
                DrawAbilities(unit);
            }
        }
    }

    public void FocusAbility (int abilityIndex)
    {
        if (FocusedUnit)
        {
            HideAbilities();
            UnfocusAbility();

            foreach (Ability ability in FocusedUnit.UnitAbilities)
                ability.EndVisualization();

            if (abilityIndex >= 0 && FocusedUnit.UnitAbilities.Count > abilityIndex)
            {
                _focusedAbility = FocusedUnit.UnitAbilities[abilityIndex];
                _focusedAbility.InitAiming();
            }
        }
    }

    public void UnfocusUnit()
    {
        UnfocusAbility();
        FocusedUnit = null;

        HideAbilities();
    }

    public void UnfocusAbility ()
    {
        if (FocusedUnit)
        {
            if (_focusedAbility)
            {
                _focusedAbility.EndAiming();
                _focusedAbility = null;
            }
        }
    }

    public void DrawAbilities (Unit unit)
    {
        _abilityHolder.SetActive(true);
        _abilityHolder.transform.position = unit.transform.position + Vector3.up * 1.1f;
        _abilityHolder.transform.rotation = CameraManager.Instance.transform.rotation;

        for (int i = 0; i < unit.UnitAbilities.Count; i++)
        {
            if (i < _abilityButtons.Count)
            {
                _abilityButtons[i].image.sprite = unit.UnitAbilities[i].AbilityIcon;
                _abilityButtons[i].gameObject.SetActive(true);
            }
            else
            {
                Button button = Instantiate(AssetManager.Instance.UIAbilityButton, _abilityHolder.transform).GetComponent<Button>();
                button.image.sprite = unit.UnitAbilities[i].AbilityIcon;
                int index = i;
                button.onClick.AddListener(() => FocusAbility(index));
                _abilityButtons.Add(button);
            }
        }

        for (int i = unit.UnitAbilities.Count; i < _abilityButtons.Count; i++)
            _abilityButtons[i].gameObject.SetActive(false);
    }

    public void HideAbilities()
    {
        _abilityHolder.SetActive(false);
    }
}
