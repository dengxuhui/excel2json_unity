using System.Collections.Generic;
using Excel2JsonUnity;
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
        Debug.Log($"Country::{id0.EnumField}");
#endif
    }

    // Update is called once per frame
    void Update()
    {
    }
}