using UnityEngine;

[System.Serializable]
public class ActorSpriteData
{
    public string name;  // 이름
    public ActorType actorType; // Npc, Enemy, Coin
    public Sprite sprite;  // 게임 내 표시용 스프라이트
}

[CreateAssetMenu(fileName = "ActorSpriteTable", menuName = "GameTable/ActorSpriteTable")]
public class ActorSpriteTable : ScriptableObject
{
    public ActorSpriteData[] actors;
}