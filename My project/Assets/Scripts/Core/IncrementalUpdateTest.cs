using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

// 增量更新测试
public class IncrementalUpdateTest : MonoBehaviour
{
    /// <summary>
    /// 下载目录
    /// </summary>
    public Button startCheckCatalogDownloadBtn;

    /// <summary>
    /// 需要下载资源大小
    /// </summary>
    public Button checkDownloadSizeBtn;

    /// <summary>
    /// 下载资源
    /// </summary>
    public Button downloadResBtn;

    /// <summary>
    /// 加载资源
    /// </summary>
    public Button loadAssetsBtn;

    public Button clearCacheBtn;

    public Image downloadProgress;

    private AsyncOperationHandle _downloadOperationHandle;

    // Start is called before the first frame update
    void Start()
    {
        Addressables.InitializeAsync().WaitForCompletion();
        Debug.Log("可寻址初始化结束");
        startCheckCatalogDownloadBtn.onClick.AddListener(StartCheckCatalogAndDownload);
        checkDownloadSizeBtn.onClick.AddListener(StartCheckTotalDownloadSize);
        downloadResBtn.onClick.AddListener(StartDownloadRes);
        loadAssetsBtn.onClick.AddListener(StartLoadAssets);
        clearCacheBtn.onClick.AddListener(CacheClear);
        downloadProgress.fillAmount = 0;
    }

    private void Update()
    {
        if (_downloadOperationHandle.IsValid())
        {
            var percent = _downloadOperationHandle.GetDownloadStatus().Percent;
            if (percent < 1)
            {
                // Debug.Log($"下载进度:{percent}");
                downloadProgress.fillAmount = percent;
            }
            else if (_downloadOperationHandle.IsDone)
            {
                Addressables.Release(_downloadOperationHandle);
                Debug.Log($"下载结束");
            }
        }
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
    /// 仅仅检测资源目录并下载
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckCatalogAndDownload()
    {
        Debug.Log("检查目录");
        var checkForCatalogUpdates = Addressables.CheckForCatalogUpdates(false);
        yield return checkForCatalogUpdates;
        if (checkForCatalogUpdates.Result.Count > 0)
        {
            Debug.Log("目录开始更新");
            var operationHandle = Addressables.UpdateCatalogs(true, checkForCatalogUpdates.Result, false);
            yield return operationHandle;

            if (operationHandle.Status != AsyncOperationStatus.Succeeded)
            {
                yield break;
            }

            Addressables.Release(checkForCatalogUpdates);
            Addressables.Release(operationHandle);
            Debug.Log("目录更新完毕");
        }
        else
        {
            Debug.Log("目录已经是最新");
        }
    }

    public string remoteResLabel = "RemoteRes";

    public void StartCheckTotalDownloadSize()
    {
        StartCoroutine(CheckTotalDownloadSize());
    }

    /// <summary>
    /// 总共要下载多少资源
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckTotalDownloadSize()
    {
        // 所有要远程下载的资源都加上该标签
        var downloadSizeAsync = Addressables.GetDownloadSizeAsync(remoteResLabel);
        var byteSize = downloadSizeAsync.Result;
        if (byteSize > 0)
        {
            var sizeValue = ByteTransferHelper.GetMB(byteSize);
            Debug.Log($"需要下载 [{sizeValue}]MB的内容");
        }
        else
        {
            Debug.Log($"没有资源需要下载");
        }

        yield break;
    }

    public void StartDownloadRes()
    {
        StartCoroutine(DownloadRes());
    }

    /// <summary>
    /// 下载还未下载的资源
    /// </summary>
    public IEnumerator DownloadRes()
    {
        _downloadOperationHandle = Addressables.DownloadDependenciesAsync(remoteResLabel);
        yield return _downloadOperationHandle;
    }
}