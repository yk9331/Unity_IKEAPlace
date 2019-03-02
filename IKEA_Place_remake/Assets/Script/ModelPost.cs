using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.Networking;

public class ModelPost : MonoBehaviour {
    public DataStruct data { get; private set; }

    public Text nameLbl, colorLbl, priceLbl;

    public Button modelBtn;
    public string modelUrl,modelHash;
    public int modelCrc;


    public RawImage modelImg;
    public string imageName;
    public UnityWebRequest request { get; private set; }


    public void SetupBy(DataStruct data) {

        this.data = data;
      
        nameLbl.text = data.name;
        colorLbl.text = data.color;
        priceLbl.text = data.price.ToString("C", CultureInfo.CurrentCulture);
       
        imageName = data.img_name;
        string url = "http://35.201.249.15/Ikea_place/img/" + imageName;
        request = UnityWebRequestTexture.GetTexture(url);
        request.SendWebRequest();

        modelBtn.gameObject.SetActive(data.url != "");
        modelUrl = data.url;
        modelHash = data.hash;
        modelCrc = data.crc;

    }

    private void Update() {
        if (request == null)
            return;
        if (request.isDone) {
            modelImg.texture = DownloadHandlerTexture.GetContent(request);
            request.Dispose();
            request = null;
        }
    }

    public void OnModelBtnClick() {
        ModelCreator.instance.GetAssetBundle(modelUrl,modelHash,modelCrc);
        ModelCreator.instance.isDetecting = true;
        FindObjectOfType<ModelList>().gameObject.SetActive(false);
    }


}

[System.Serializable]
public struct DataStruct{
    public int id,price,type,crc;
    public string name, color, img_name, url, hash, create_at;
}

[System.Serializable]
public struct TypeStruct{
    public int id;
    public string name;
}

[System.Serializable]
public struct ListTemplate {
    public DataStruct[] datas;
}

[System.Serializable]
public struct TypeListTemplate{
    public TypeStruct[] types;
}

public enum ModelType{
    bed,
    carpet,
    chair,
    decoration,
    light,
    shelf,
    storage,
    table
}