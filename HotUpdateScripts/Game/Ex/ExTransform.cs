using UnityEngine;

public static class ExTransform
{

}
public static class ExGameObject
{
    public static T ExGetCom<T>(this GameObject trans) where T : MonoBehaviour
    {
        Debug.LogError($"find type:{typeof(T).FullName}");
        MonoBehaviour[] monos = trans.GetComponents<MonoBehaviour>();
        for (int i = 0; i < monos.Length; i++)
        {
            Debug.LogError($"{trans.name} find type:{monos[i].GetType().FullName}");
            if (monos[i].GetType().Name == typeof(T).Name)
            {
                return (T)monos[i];
            }
        }
        Debug.LogError($"Can't find type:{typeof(T).FullName}");
        return null;
    }
}
