using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;


public class ComputeToFullScreen : MonoBehaviour
{
    public Camera cam;
    public ComputeShader shader;
    public RawImage fullScreenImage; // Canvas의 RawImage

    private RenderTexture inputTex;
    private RenderTexture resultTex;
    private int kernel;
    private bool captured;
    private float effectBeginTime;

    //디버깅용 렌더러 텍스쳐
    public RenderTexture dbgRenderTexture;

    void Start()
    {
        resultTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        resultTex.enableRandomWrite = true;
        resultTex.filterMode = FilterMode.Point;
        resultTex.wrapMode = TextureWrapMode.Clamp;
        resultTex.useMipMap = false;
        resultTex.Create();

        kernel = shader.FindKernel("CSMain");

        // RawImage에 결과 텍스처 연결
        fullScreenImage.gameObject.SetActive(true);
        fullScreenImage.texture = resultTex;
        SetRawImage(fullScreenImage);
        fullScreenImage.enabled = false;

        // RenderTexture 활성화
        dbgRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

        EventManager.Instance.OnPlayerLivesZero += HandlePlayerLivesZero;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnPlayerLivesZero -= HandlePlayerLivesZero;
    }


    void HandlePlayerLivesZero()
    {
        CaptureInputTexture();
    }


    void SetRawImage(RawImage rawImage)
    {
        if (rawImage == null)
            return;

        RectTransform rt = rawImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;   // 왼쪽 아래
        rt.anchorMax = Vector2.one;    // 오른쪽 위
        rt.offsetMin = Vector2.zero;   // 여백 없음
        rt.offsetMax = Vector2.zero;   // 여백 없음
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;

    }

    void ConectImage()
    {
        fullScreenImage.enabled = false;
        inputTex = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        inputTex.enableRandomWrite = false;
        inputTex.filterMode = FilterMode.Point;
        inputTex.wrapMode = TextureWrapMode.Clamp;
        inputTex.useMipMap = false;
        inputTex.Create();

        cam.targetTexture = inputTex;
        cam.clearFlags = CameraClearFlags.SolidColor;
        //cam.backgroundColor = Color.black;
        cam.Render();
        cam.targetTexture = null;

        fullScreenImage.enabled = true;

        fullScreenImage.texture = inputTex;
    }

    public void SaveRenderTexture(string filePath, Camera camera)
    {
        // 카메라가 RenderTexture에 그리도록 설정
        camera.targetTexture = dbgRenderTexture;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.Render();

        // RenderTexture 활성화
        RenderTexture.active = dbgRenderTexture;

        // Texture2D로 복사
        Texture2D tex = new Texture2D(dbgRenderTexture.width, dbgRenderTexture.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, dbgRenderTexture.width, dbgRenderTexture.height), 0, 0);
        tex.Apply();

        // PNG로 인코딩
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        // 초기화
        cam.targetTexture = null;
        RenderTexture.active = null;

        Debug.Log("RenderTexture saved to: " + filePath);
    }


    void Update()
    {
        if (!captured && Input.GetKeyDown(KeyCode.O))
        {
            string filePath = Application.persistentDataPath + "/screenshot.png";
            SaveRenderTexture(filePath, cam);
        }

        if (!captured && Input.GetKeyDown(KeyCode.K))
        {
            ConectImage();
        }

        if (!captured && Input.GetKeyDown(KeyCode.B))
        {
            CaptureInputTexture();
        }

        if (captured)
        {
            UpdateComputerShader();
        }
    }

    void CaptureInputTexture()
    {
        fullScreenImage.enabled = false;
        inputTex = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);
        inputTex.enableRandomWrite = false;
        inputTex.filterMode = FilterMode.Point;
        inputTex.wrapMode = TextureWrapMode.Clamp;
        inputTex.useMipMap = false;
        inputTex.Create();

        cam.targetTexture = inputTex;
        cam.clearFlags = CameraClearFlags.SolidColor;
        //cam.backgroundColor = Color.black;
        cam.Render();
        cam.targetTexture = null;

        fullScreenImage.enabled = true;
        captured = true;
        effectBeginTime = Time.time;
    }

    void UpdateComputerShader()
    {
        shader.SetTexture(kernel, "InputTex", inputTex);
        shader.SetTexture(kernel, "Result", resultTex);
        shader.SetFloat("_Time", (Time.time - effectBeginTime) * 2.0f);

        const int TX = 8, TY = 8;
        int tgx = Mathf.CeilToInt(resultTex.width / (float)TX);
        int tgy = Mathf.CeilToInt(resultTex.height / (float)TY);

        shader.Dispatch(kernel, tgx, tgy, 1);
    }
}
