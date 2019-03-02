using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.Networking;
using System.Collections;

public class ModelCreator : Singleton<ModelCreator> {

    public GameObject modelPrefab;
    public float maxRayDistance = 30.0f;
    public LayerMask collisionLayer = 1 << 10;  //ARKitPlane layer
    public AssetBundle bundle;
    public bool isDetecting;
    public GeneratePlane generatePlane;
    private Plane groundPlane;
    public GameObject planePrefab;
    public Tip tip;

    private void Start() {
        generatePlane = new GeneratePlane(planePrefab);
    }

    public void GetAssetBundle(string modelurl, string modelhash, int modelcrc) {
        string url = "http://35.201.249.15/Ikea_place/model/iOS/" + modelurl;
        Hash128 hash = Hash128.Parse(modelhash);
        
        StartCoroutine(LoadAssetBundle(url, hash));
        tip.UpdateTip("點選地面放置傢俱");
    }

    IEnumerator LoadAssetBundle(string url, Hash128 hash, uint crc = 0) {
        // Wait for caching ready
        while (!Caching.ready) {
            yield return null;
        }
        using (var request = UnityWebRequestAssetBundle.GetAssetBundle(url, hash, crc)) {
            yield return request.SendWebRequest();
            if (request.error != null) {
                Debug.LogWarning(request.error);
                yield break;
            }
            bundle = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
        }
    }

    bool HitTestWithPlane(Ray ray, Plane m_Plane) {
        float enter = 0.0f;
        if (m_Plane.Raycast(ray, out enter)) {
            //Get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);
            GameObject parent = Instantiate(modelPrefab, hitPoint, Quaternion.identity);
            var model = Instantiate(bundle.LoadAllAssets<GameObject>()[0], hitPoint,Quaternion.identity);
            model.transform.SetParent(parent.transform);
            isDetecting = false;
            tip.UpdateTip("單指移動傢俱\n雙指旋轉傢俱\n雙擊刪除傢俱");
            bundle.Unload(false);
            parent.GetComponent<Model>().model = model;
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update() {

        if (Input.touchCount > 0 && isDetecting) {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved) {

                if (generatePlane.PlaneCreated) {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    groundPlane = generatePlane.GroundPlane;
                    if (HitTestWithPlane(ray, groundPlane)){
                        return;
                    }
                }
            }
        }

        if (generatePlane.PlaneCreated && FindObjectOfType<Model>() == null) {
            if(!isDetecting)
                tip.UpdateTip("按+添加傢俱");
        }
    }


}
