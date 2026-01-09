using UnityEngine;
using UnityEngine.UI;

public class ModeManager : MonoBehaviour
{
    public GameObject playUICanvas;
    public GameObject editorUICanvas;
    public Camera playCamera;
    public Camera editorCamera;
    public GameObject UI_ModeSwitchBtn;
    
    private bool isPlayMode = false;

    private void Start()
    {
        if (UI_ModeSwitchBtn)
        {
            SetButtonText();
            Button button = UI_ModeSwitchBtn.GetComponentInChildren<Button>();  //GetComponentInChildren은 본인먼저 찾고 자식을 찾는다.
            button.onClick.AddListener(HandleSwitchModeClick);

            ChangeMode();
        }
    }

    public void ChangeMode()
    {
        if(playUICanvas)
            playUICanvas.SetActive(isPlayMode);
        if(editorUICanvas)
            editorUICanvas.SetActive(!isPlayMode);

        if(playCamera)
            playCamera.gameObject.SetActive(isPlayMode);
        if(editorCamera)
            editorCamera.gameObject.SetActive(!isPlayMode);

        SetButtonText();
        GameReferences.Instance.ChangeMode(isPlayMode);
        EventManager.Instance.TriggerChangePlayMode(isPlayMode);
    }

    public bool IsEditorMode()
    {
        return (!isPlayMode);
    }

    private void HandleSwitchModeClick()
    {
        isPlayMode = !isPlayMode;

        ChangeMode();
        Debug.Log("버튼이 클릭되었습니다!");
    }

    void SetButtonText()
    {
        if (UI_ModeSwitchBtn == null)
            return;

        Text btnText = UI_ModeSwitchBtn.GetComponentInChildren<Text>();
        if (IsEditorMode())
            btnText.text = "To Play";
        else
            btnText.text = "To Edit";
    }
}