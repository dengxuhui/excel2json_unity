using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Excel2JsonUnity.Sample;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var type = typeof(SampleBaseConfig);
        var fullName = type.FullName;
        Debug.Log(fullName);
        var ab = Assembly.GetAssembly(type);
        Debug.Log(ab.FullName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
