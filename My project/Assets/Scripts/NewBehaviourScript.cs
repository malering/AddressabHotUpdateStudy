using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public string sceneAddressToLoad;

    public Button startCheckCatalogBtn;
    public Button preLoadBtn;
    public Button getUpdateBtn;
    public Button getUpdate1Btn;
    public Button loadSceneBtn;
    public Button clearCacheBtn;
    public Text debugText;

    private List<string> logs = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        // Addressables.InitializeAsync();
        startCheckCatalogBtn.onClick.AddListener(StartCheckCatalog);
        preLoadBtn.onClick.AddListener(StartPreload);
        loadSceneBtn.onClick.AddListener(LoadRemoveScene);
        clearCacheBtn.onClick.AddListener(CacheClear);
    }

    private void CacheClear()
    {
        var clearCache = Caching.ClearCache();
        if (clearCache)
        {
            Debug.Log("清除缓存");
        }

        else
        {
            Debug.Log("没有清除缓存");
        }
    }

    public void StartCheckCatalog()
    {
        StartCoroutine(CheckCatalog());
    }

    public void StartPreload()
    {
        StartCoroutine(GetPreload());
    }

    public IEnumerator GetPreload()
    {
        var preLabel = "Pre";
        var updateLabel = "Update";
        var labels = new List<string>();
        labels.Add(preLabel);
        labels.Add(updateLabel);

        foreach (var label in labels)
        {
            var asyncOperationHandle = Addressables.LoadAssetsAsync<GameObject>(label, null);
            yield return asyncOperationHandle;
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                var gameObjects = asyncOperationHandle.Result;
                foreach (var o in gameObjects)
                {
                    Instantiate(o);
                }
            }
        }

        Addressables.InstantiateAsync("Capsule 7");
        Addressables.InstantiateAsync("Cube (4)");
    }

    public IEnumerator CheckCatalog()
    {
        var asyncOperationHandle = Addressables.InitializeAsync();
        yield return asyncOperationHandle;

        var checkForCatalogUpdates = Addressables.CheckForCatalogUpdates(false);
        yield return checkForCatalogUpdates;
        if (checkForCatalogUpdates.Result.Count > 0)
        {
            var operationHandle = Addressables.UpdateCatalogs(checkForCatalogUpdates.Result, false);
            yield return operationHandle;

            if (operationHandle.Status != AsyncOperationStatus.Succeeded)
            {
                yield break;
            }

            var resourceLocators = operationHandle.Result;
            if (resourceLocators.Count < 1)
            {
                yield break;
            }

            foreach (var resourceLocator in resourceLocators)
            {
                foreach (var resourceLocatorKey in resourceLocator.Keys)
                {
                    var downloadSizeAsync = Addressables.GetDownloadSizeAsync(resourceLocatorKey);
                    yield return downloadSizeAsync;
                    var downloadSize = downloadSizeAsync.Result;

                    // 下载
                    // 下载大小等于0，可能该资源资源已经下载完在设备中
                    if (downloadSize > 0)
                    {
                        var downloadDependenciesAsync = Addressables.DownloadDependenciesAsync(resourceLocatorKey);
                        while (!downloadDependenciesAsync.IsDone)
                        {
                            if (downloadDependenciesAsync.Status == AsyncOperationStatus.Failed)
                            {
                                Debug.LogError("DownloadDependenciesAsync error");
                                yield break;
                            }

                            yield return null;
                        }

                        // if (downloadDependenciesAsync.Status == AsyncOperationStatus.Succeeded)
                        {
                            Debug.Log(
                                $"{resourceLocator.LocatorId} 下载完毕 {downloadDependenciesAsync.Status} {resourceLocatorKey} downloadSize:{downloadSize}");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("没有检测到更新");
        }
    }

    public void LoadRemoveScene()
    {
        Addressables.LoadSceneAsync(sceneAddressToLoad);
    }

    public void AddLog(string log)
    {
        logs.Add(log);
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var s in logs)
        {
            stringBuilder.Append(s);
            stringBuilder.AppendLine();
        }

        debugText.text = stringBuilder.ToString();
    }
}