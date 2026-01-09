using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceTileCommand : IEditorCommand
{
    private Tilemap tilemap;
    private Vector3Int position;
    private TileBase newTile;
    private TileBase oldTile;

    public PlaceTileCommand(Tilemap tilemap, Vector3Int position, TileBase newTile)
    {
        this.tilemap = tilemap;
        this.position = position;
        this.newTile = newTile;
        this.oldTile = tilemap.GetTile(position); // 기존 타일 저장
    }

    public void Execute()
    {
        tilemap.SetTile(position, newTile);
    }

    public void Undo()
    {
        tilemap.SetTile(position, oldTile);
    }
}
