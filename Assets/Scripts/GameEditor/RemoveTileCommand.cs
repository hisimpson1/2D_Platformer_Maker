using UnityEngine;
using UnityEngine.Tilemaps;

public class RemoveTileCommand : IEditorCommand
{
    private Tilemap tilemap;
    private Vector3Int position;
    private TileBase oldTile;

    public RemoveTileCommand(Tilemap tilemap, Vector3Int position)
    {
        this.tilemap = tilemap;
        this.position = position;
        this.oldTile = tilemap.GetTile(position);
    }

    public void Execute()
    {
        tilemap.SetTile(position, null); // 타일 제거
    }

    public void Undo()
    {
        tilemap.SetTile(position, oldTile); // 원래 타일 복원
    }
}
