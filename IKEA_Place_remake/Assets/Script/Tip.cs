using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tip : MonoBehaviour {

    [SerializeField]
    Text contentText;

    public void Start() {
        contentText.text = "將相機對準地面至橘色匡線出現";
    }
    public void UpdateTip(string content){
        contentText.text = content;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
   
}
