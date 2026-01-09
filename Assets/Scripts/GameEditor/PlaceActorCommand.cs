using UnityEngine;

public class PlaceActorCommand : IEditorCommand
{
    private GameObject prefab;
    private int index;
    private Vector3 position;
    private Transform parent;

    private GameObject spawnedActor;

    public PlaceActorCommand(GameObject prefab, int index, Vector3 position, Transform parent)
    {
        this.prefab = prefab;
        this.index = index;
        this.position = position;
        this.parent = parent;
    }

    public void Execute()
    {
        if (prefab)
        {
            spawnedActor = Object.Instantiate(prefab, position, Quaternion.identity, parent);
            var holder = spawnedActor.GetComponent<IndexHolder>();
            if (holder == null) 
                holder = spawnedActor.AddComponent<IndexHolder>();
            holder.index = index;

            var sr = spawnedActor.GetComponent<SpriteRenderer>();
            if (sr) 
                sr.sortingOrder = 1;
        }
    }

    public void Undo()
    {
        if (spawnedActor)
        {
            Object.Destroy(spawnedActor);
        }
    }
}