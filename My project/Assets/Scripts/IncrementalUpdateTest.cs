﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

// 增量更新测试
public class IncrementalUpdateTest : MonoBehaviour
{
    public string sceneAddressToLoad;

    public Button startCheckCatalogDownloadBtn;
    public Button loadAssetsBtn;
    public Button loadSceneBtn;
    public Button clearCacheBtn;

    // Start is called before the first frame update
    void Start()
    {
        // Addressables.InitializeAsync();
        startCheckCatalogDownloadBtn.onClick.AddListener(StartCheckCatalogAndDownload);
        loadAssetsBtn.onClick.AddListener(StartLoadAssets);
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

    public void StartCheckCatalogAndDownload()
    {
        StartCoroutine(CheckCatalogAndDownload());
    }

    public void StartLoadAssets()
    {
        StartCoroutine(LoadAssets());
    }

    public IEnumerator LoadAssets()
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

    /// <summary>
    /// 检测资源目录和资源目录下载
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckCatalogAndDownload()
    {
        var asyncOperationHandle = Addressables.InitializeAsync();
        yield return asyncOperationHandle;

        // var versionCode = "";
        // var kCacheDataFolder =
        //     $"{UnityEngine.Application.persistentDataPath}/com.unity.addressables/catalog_{versionCode}.json";
        // var kCacheDataFolder1 =
        //     $"{UnityEngine.Application.persistentDataPath}/com.unity.addressables/catalog_{versionCode}.hash";
        // File.Delete(kCacheDataFolder);
        // File.Delete(kCacheDataFolder1);

        // var loadContentCatalogAsync = Addressables.LoadContentCatalogAsync(kCacheDataFolder);
        // yield return loadContentCatalogAsync;
        // var resourceLocator = loadContentCatalogAsync.Result;

        var checkForCatalogUpdates = Addressables.CheckForCatalogUpdates(false);
        yield return checkForCatalogUpdates;
        if (checkForCatalogUpdates.Result.Count > 0)
        {
            var operationHandle = Addressables.UpdateCatalogs(true, checkForCatalogUpdates.Result, false);
            yield return operationHandle;

            if (operationHandle.Status != AsyncOperationStatus.Succeeded)
            {
                yield break;
            }

            var resourceLocators = operationHandle.Result;
            yield return DownloadNewRes(resourceLocators);
            Addressables.Release(checkForCatalogUpdates);
            Addressables.Release(operationHandle);
        }
        else
        {
            Debug.Log("没有检测到更新");
        }
    }

    public IEnumerator DownloadNewRes(List<IResourceLocator> resourceLocators)
    {
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

    public void LoadRemoveScene()
    {
        Addressables.LoadSceneAsync(sceneAddressToLoad);
    }
}