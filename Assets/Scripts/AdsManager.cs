using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    [SerializeField] private GameManager gm;

    private string id;

    void Start()
    {
        #if UNITY_IOS
            id = "4138472";
        #elif UNITY_ANDROID
            id = "4138473";
        #endif

        Advertisement.AddListener(this);
        Advertisement.Initialize(id);
    }

    public void ShowAd()
    {
        gm.Pause(true);
        Advertisement.Show();
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult) { }

    public void OnUnityAdsDidError(string message) { }

    public void OnUnityAdsDidStart(string placementId) { }

    public void OnUnityAdsReady(string placementId) { }

    private void OnDestroy()
        => Advertisement.RemoveListener(this);
}
