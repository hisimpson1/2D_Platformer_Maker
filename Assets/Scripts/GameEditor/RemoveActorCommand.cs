using UnityEngine;

public class RemoveActorCommand : IEditorCommand
{
    private GameObject actor;
    private Vector3 position;
    private Transform parent;
    private GameObject backupPrefab;

    public RemoveActorCommand(GameObject actor)
    {
        this.actor = actor;
        this.position = actor.transform.position;
        this.parent = actor.transform.parent;

        // Prefab 원본을 저장하기 위해
        var prefabRef = actor.GetComponent<ActorPrefabReference>();
        if (prefabRef != null)
        {
            backupPrefab = prefabRef.prefab; // 원래 프리팹 참조
        }
    }

    public void Execute()
    {
        if (actor)
        {
            Object.Destroy(actor);
        }
    }

    public void Undo()
    {
        if (backupPrefab)
        {
            var restored = Object.Instantiate(backupPrefab, position, Quaternion.identity, parent);
            var sr = restored.GetComponent<SpriteRenderer>();
            if (sr) sr.sortingOrder = 1;
        }
    }
}