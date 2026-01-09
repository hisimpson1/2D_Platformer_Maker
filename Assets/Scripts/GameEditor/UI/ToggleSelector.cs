using UnityEngine;
using UnityEngine.UI;

public class ToggleSelector : MonoBehaviour
{
    private ToggleGroup toggleGroup;
    private Toggle[] toggles;
    private ObjectSelectionType lastSelectionType = ObjectSelectionType.TileMap;

    private void Awake()
    {
        toggleGroup = GetComponentInChildren<ToggleGroup>();
        if (toggleGroup)
            toggles = toggleGroup.GetComponentsInChildren<Toggle>();
        else
            return;

            // 모든 Toggle 찾아서 이벤트 연결
            foreach (var toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        SelectionChanged(toggle);
                    }
                });
            }
    }

    private void SelectionChanged(Toggle selectedToggle)
    {
        ObjectSelectionType selectionType = ObjectSelectionType.None;
        string objectName = selectedToggle.gameObject.name;

        if (objectName == "Toggle_Tile")
            selectionType = ObjectSelectionType.TileMap;
        else if (objectName == "Toggle_MapObject")
            selectionType = ObjectSelectionType.MapObject;
        else if (objectName == "Toggle_Actor")
            selectionType = ObjectSelectionType.Actor;

        if (lastSelectionType != selectionType)
        {
            lastSelectionType = selectionType;
            EventManager.Instance.TriggerObjectSelection(selectionType);
        }
    }

    public ObjectSelectionType GetSelectionType()
    {
        return lastSelectionType;
    }
}
