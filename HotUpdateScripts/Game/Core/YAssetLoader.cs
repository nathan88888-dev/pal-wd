using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using YooAsset;

public class YAssetLoader: Singleton<YAssetLoader>
{
    public async UniTask LoadScene(string _path, System.Action<float> prog) 
    {

        var sceneMode = LoadSceneMode.Single;
        var physicsMode = LocalPhysicsMode.None;
        bool suspendLoad = false;
        SceneHandle SceneHandle = YooAssets.LoadSceneAsync(_path, sceneMode, physicsMode, suspendLoad);
        while (!SceneHandle.IsDone)
        {
            prog(SceneHandle.Progress * 0.7f);
            await UniTask.Delay(500);
        }
        string SceneName = SceneHandle.SceneName;
        int sceneID = int.Parse(SceneName.Substring(0, SceneName.IndexOf("_")));
        if (sceneID >= 100)
        {
            AssetHandle NMDataHandle = YooAssets.LoadAssetAsync<NavMeshData>($"{SceneHandle.SceneName}_NavMesh-Terrain");
            while (!NMDataHandle.IsDone)
            {
                prog(0.7f+NMDataHandle.Progress * 0.3f);
                await UniTask.Delay(500);
            }
            NavMeshData NMData = (NavMeshData)NMDataHandle.AssetObject;
            NavMesh.AddNavMeshData(NMData);
        }
    }
    public async UniTask<Object> Load<T>(string _path)where T: Object
    {
        AssetHandle handle = YooAssets.LoadAssetAsync<T>(_path);
        await handle.ToUniTask();
        if (handle.Status != EOperationStatus.Succeed || handle.AssetObject == null)
            throw new System.Exception($"加载资源失败：{_path}");

        switch (typeof(T).Name) {
            case "GameObject":
                return handle.InstantiateSync();
            case "TextAsset":
                return handle.AssetObject as TextAsset;
            default:
                break;
        }

        return null;
    }
}
