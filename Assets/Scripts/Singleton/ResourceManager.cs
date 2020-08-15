using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : SingletonMonoBehaviour<ResourceManager> {

    protected override void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        base.Awake();
    }

    public IEnumerator LoadScriptableObject(string assetPath, UnityAction<ResourceRequest> callback)
    {
        ResourceRequest request = Resources.LoadAsync<StageScriptable>(assetPath);
        yield return new WaitUntil(() => request.isDone);
        if (request.asset != null)
        {
            callback(request);
        }
    }

}
