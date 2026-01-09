using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class EditorManager : MonoBehaviour
{
    public static EditorManager Instance { get; private set; }

    private Stack<IEditorCommand> undoStack = new Stack<IEditorCommand>();
    private Stack<IEditorCommand> redoStack = new Stack<IEditorCommand>();

    private void Awake()
    {
        Instance = this;
    }

    public void ExecuteCommand(IEditorCommand command)
    {
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear(); // 새로운 작업이 생기면 Redo 초기화
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            var command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            var command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
        }
    }

    public void PlaceTile(Tilemap tilemap, Vector3Int pos, TileBase tile)
    {
        var command = new PlaceTileCommand(tilemap, pos, tile);
        ExecuteCommand(command);
    }

    public void RemoveTile(Tilemap tilemap, Vector3Int pos)
    {
        TileBase tile = tilemap.GetTile(pos);
        if (tile == null)
            return;

        var command = new RemoveTileCommand(tilemap, pos);
        ExecuteCommand(command);
    }
    public void PlaceActor(GameObject prefab, int index, Vector3 pos, Transform parent)
    {
        var command = new PlaceActorCommand(prefab, index, pos, parent);
        ExecuteCommand(command);
    }

    public void RemoveActor(GameObject actor)
    {
        if (actor == null) return;

        var command = new RemoveActorCommand(actor);
        ExecuteCommand(command);
    }
}
