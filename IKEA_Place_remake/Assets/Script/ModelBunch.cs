using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ModelBunch : MonoBehaviour ,IDragHandler{

    [SerializeField]
    ModelPost modelPostPrefab;
    [SerializeField]
    Text typename;
    [SerializeField]
    ScrollRect parentScrollRect;

    private float speed = 40f;

    public void Setup(string typename,List<DataStruct>modelDatas,ScrollRect parentScrollRect){
        this.parentScrollRect = parentScrollRect;
        RectTransform content = GetComponent<ScrollRect>().content;
        this.typename.text = typename;
        if(modelDatas!=null){
            foreach (var modelData in modelDatas) {
                var modelPost = Instantiate(modelPostPrefab, content);
                modelPost.SetupBy(modelData);

            }
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (Mathf.Abs(eventData.delta.y) >= Mathf.Abs(eventData.delta.x))
            parentScrollRect.velocity = new Vector2(0f,eventData.delta.y*speed);
            //parentScrollRect.verticalNormalizedPosition -= eventData.delta.y * speed * Time.deltaTime;
        //else
            //GetComponent<ScrollRect>().horizontalNormalizedPosition -= eventData.delta.x * speed * Time.deltaTime;

    }

}
