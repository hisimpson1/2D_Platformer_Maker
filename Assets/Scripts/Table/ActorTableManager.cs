using System.Linq;
using UnityEngine;

public class ActorTableManager : MonoBehaviour
{
    public static ActorTableManager Instance { get; private set; }

    public ActorSpriteTable actorTable; // 인스펙터에서 드래그 앤 드롭
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 전체 배열을 반환
    public ActorSpriteData[] GetAllActors()
    {
        return actorTable.actors;
    }

}