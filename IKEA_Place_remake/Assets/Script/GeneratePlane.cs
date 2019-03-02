using System;
using System.Collections.Generic;
using Collections.Hybrid.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;


public class GeneratePlane : MonoBehaviour {


    private Plane groundPlane;
    private DateTime planeDisableTimer;

    public bool PlaneCreated = false;
    public Plane GroundPlane{
        get {
            return groundPlane;
        }
    }
    private LinkedListDictionary<string, ARPlaneAnchorGameObject> planeAnchorMap;


    // Use this for initialization
    public GeneratePlane(GameObject planePrefab) {

        planeAnchorMap = new LinkedListDictionary<string, ARPlaneAnchorGameObject>();
        UnityARUtility.InitializePlanePrefab(planePrefab);
        UnityARSessionNativeInterface.ARAnchorAddedEvent += AddAnchor;
        UnityARSessionNativeInterface.ARAnchorUpdatedEvent += UpdateAnchor;

    }

    

    public void AddAnchor(ARPlaneAnchor arPlaneAnchor) {
        planeDisableTimer = DateTime.Now;
        Vector3 position = UnityARMatrixOps.GetPosition(arPlaneAnchor.transform);
        groundPlane = new Plane(Vector3.up, position);
        GameObject go = UnityARUtility.CreatePlaneInScene(arPlaneAnchor);
        go.AddComponent<DontDestroyOnLoad>();  //this is so these GOs persist across scene loads
        ARPlaneAnchorGameObject arpag = new ARPlaneAnchorGameObject();
        arpag.planeAnchor = arPlaneAnchor;
        arpag.gameObject = go;
        planeAnchorMap.Add(arPlaneAnchor.identifier, arpag);
        UnityARSessionNativeInterface.ARAnchorAddedEvent -= AddAnchor;
        PlaneCreated = true;

    }

    public void UpdateAnchor(ARPlaneAnchor arPlaneAnchor) {
        if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier)) {
            ARPlaneAnchorGameObject arpag = planeAnchorMap[arPlaneAnchor.identifier];
            UnityARUtility.UpdatePlaneWithAnchorTransform(arpag.gameObject, arPlaneAnchor);
            arpag.planeAnchor = arPlaneAnchor;
            planeAnchorMap[arPlaneAnchor.identifier] = arpag;
        }
        TimeSpan interval = DateTime.Now-planeDisableTimer;
        if(interval.TotalSeconds>3f){
            PlaneVisualDisable(arPlaneAnchor);
        }
    }


    public void Destroy() {
        foreach (ARPlaneAnchorGameObject arpag in GetCurrentPlaneAnchors()) {
            GameObject.Destroy(arpag.gameObject);
        }

        planeAnchorMap.Clear();
       
    }

    public LinkedList<ARPlaneAnchorGameObject> GetCurrentPlaneAnchors() {
        return planeAnchorMap.Values;
    }

    public void PlaneVisualDisable(ARPlaneAnchor arPlaneAnchor) {
        UnityARSessionNativeInterface.ARAnchorUpdatedEvent -= UpdateAnchor;
        if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier)) {

            ARPlaneAnchorGameObject arpag = planeAnchorMap[arPlaneAnchor.identifier];
            arpag.gameObject.SetActive(false);

        }
    }
}


