using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class EngineUtil
{
    public static void ClearAllChildren(GameObject gameObject)
    {       
        for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(gameObject.transform.GetChild(i).gameObject);
        }
    }
}