using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Excel2JsonUnity;
using Excel2JsonUnity.Sample;
using Newtonsoft.Json;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Main : MonoBehaviour
{
    public TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        var asset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Sample/Json/SampleXlsx.json");
        var configs = JsonConvert.DeserializeObject<List<SampleXlsx>>(asset.text);
        if (configs == null)
        {
            return;
        }

        var id0 = configs[0];
        text.text = $"Name:{id0.Data.Name} Age:{id0.Data.Age}";
#endif
    }

    // Update is called once per frame
    void Update()
    {
    }
}