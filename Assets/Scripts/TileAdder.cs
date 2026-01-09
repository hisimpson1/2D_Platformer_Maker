using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAdder : MonoBehaviour
{
    public Tilemap tilemap;       // 연결할 Tilemap
    public TileBase tileToPlace;  // 추가할 타일
    public Transform player;      // 플레이어 Transform 참조

    void Update()
    {
        // 스페이스바를 누르면 플레이어 위치에 타일 추가
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 플레이어의 월드 좌표 → 타일맵 좌표로 변환
            Vector3Int cellPos = tilemap.WorldToCell(player.position);

            // 해당 위치에 타일 배치
            tilemap.SetTile(cellPos, tileToPlace);
        }

        // R 키를 누르면 플레이어 위치의 타일 제거
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3Int cellPos = tilemap.WorldToCell(player.position);
            tilemap.SetTile(cellPos, null); // null → 타일 제거
        }
    }
}

