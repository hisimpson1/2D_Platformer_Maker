using UnityEngine;

public class CameraBorder : MonoBehaviour
{
    void Start()
    {
        Camera cam = GetComponent<Camera>();
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        Vector3[] corners = new Vector3[5];
        corners[0] = cam.transform.position + new Vector3(-width / 2, -height / 2, 0);
        corners[1] = cam.transform.position + new Vector3(width / 2, -height / 2, 0);
        corners[2] = cam.transform.position + new Vector3(width / 2, height / 2, 0);
        corners[3] = cam.transform.position + new Vector3(-width / 2, height / 2, 0);
        corners[4] = corners[0]; // 닫기

        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.positionCount = corners.Length;
        for (int i = 0; i < corners.Length; i++)
        {
            lr.SetPosition(i, new Vector3(corners[i].x, corners[i].y, -1)); // Z축 앞으로
        }

        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.red;
        lr.endColor = Color.red;

        // 2D에서 보이도록 설정
        lr.sortingLayerName = "Default";
        lr.sortingOrder = 10;
    }
}