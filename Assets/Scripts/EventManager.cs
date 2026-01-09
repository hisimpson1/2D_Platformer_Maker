using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EventManager : MonoBehaviour
{
    // 1. 싱글톤 설정
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // 게임 전체에서 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public event Action OnPlayerLivesZero;

    public event Action<bool> OnChangePlayMode;

    public event Action<ObjectSelectionType, int> OnTileBaseSelected;

    public event Action<int> OnActorSelected;

    public event Action <ObjectSelectionType> OnObjectSelectionType;

    public event Action<ObjectSelectionType, int, Vector3> OnPlaceDragObject;

    public event Action OnRemoveEndDragObject;

    public void TriggerPlayerLivesZero() => OnPlayerLivesZero?.Invoke();

    public void TriggerChangePlayMode(bool mode) => OnChangePlayMode?.Invoke(mode);

    public void TriggerTileBaseSelected(ObjectSelectionType selectionType, int index) 
    { 
        OnTileBaseSelected?.Invoke(selectionType, index); 
    }

    public void TriggerActorSelected(int index)
    {
        OnActorSelected?.Invoke(index);
    }


    public void TriggerObjectSelection(ObjectSelectionType selectionType)
    {
        OnObjectSelectionType?.Invoke(selectionType);
    }

    public void TriggerPlaceDragObject(ObjectSelectionType selectionType, int index, Vector3 pos)
    {
        OnPlaceDragObject?.Invoke(selectionType, index, pos);
    }

    public void TriggerRemoveEndDragObject()
    {
        OnRemoveEndDragObject?.Invoke();
    }

}