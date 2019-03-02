using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Networking.UnityWebRequest;


public class ModelList : MonoBehaviour {

    [SerializeField]
    ModelBunch modelBunchPrefab;
    [SerializeField]
    ModelPost modelPostPrefab;
    private bool listCreated = false;
    private ScrollRect m_ScrollRect;

    // Use this for initialization
    private void Start() {
        m_ScrollRect = GetComponent<ScrollRect>();
        gameObject.SetActive(false);
    }
    void OnEnable() {
        if(!listCreated){
            StartCoroutine(GetModelList());
        }
    }

    

    IEnumerator GetModelList() {
        Contract.Ensures(Contract.Result<IEnumerator>() != null);

        string typeurl = "http://35.201.249.15/Ikea_place/api.php?m=getType";
        using (var typerequest = Post(typeurl, "")) {
            yield return typerequest.SendWebRequest();
            if (typerequest.error != null) {
                Debug.LogWarning(typerequest.error);
                yield break;
            }
            TypeListTemplate typelist = JsonUtility.FromJson<TypeListTemplate>(typerequest.downloadHandler.text);
            //Debug.Log(typelist.types);

            string url = "http://35.201.249.15/Ikea_place/api.php?m=download";
            using (var request = Post(url, "")) {
                yield return request.SendWebRequest();
                if (request.error != null) {
                    Debug.LogWarning(request.error);
                    yield break;
                }

                ListTemplate list = JsonUtility.FromJson<ListTemplate>(request.downloadHandler.text);
                RectTransform content = GetComponent<ScrollRect>().content;

                var group = from modelData in list.datas group modelData by modelData.type;

                foreach(TypeStruct type in typelist.types){
                    //Debug.Log(type);
                    var bunch = Instantiate(modelBunchPrefab, content);
                    var models = group.SingleOrDefault(g => g.Key == type.id)?.ToList() ?? new List<DataStruct>();
                    bunch.Setup(type.name,models,m_ScrollRect);
                }
                listCreated = true;

                /*
                foreach(DataStruct data in list.datas){
                    ModelPost modelPost = Instantiate(modelPostPrefab, content);
                    modelPost.SetupBy(data);
                }
                */



            }
        }




    }

}
