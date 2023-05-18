using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Excel2JsonUnity
{
    /// <summary>
    /// 配置接口
    /// </summary>
    public interface IConfig
    {
        public void Dispose();
    }

    /// <summary>
    /// 配置基类
    /// </summary>
    public class BaseConfig
    {
        public int ID;
    }

    public class TConfig<T> : Excel2JsonUnity.IConfig where T : Excel2JsonUnity.BaseConfig
    {
        //配置列表
        private List<T> _configs;

        //配置字典 key：ID，value：配置
        private Dictionary<int, T> _configDic;

        //配置名字
        private string _name;

        //加载完成回调
        private event Action<Excel2JsonUnity.IConfig> _loadedCallback;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TConfig()
        {
            _name = typeof(T).Name;
        }

        public void Dispose()
        {
            _name = null;
            if (_configs != null)
            {
                _configs.Clear();
                _configs = null;
            }

            if (_configDic != null)
            {
                _configDic.Clear();
                _configDic = null;
            }
        }

        /// <summary>
        /// 获取配置列表
        /// </summary>
        public List<T> Configs => _configs;

        /// <summary>
        /// 获取指定id配置数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get(int id)
        {
            if (_configDic.TryGetValue(id, out T config))
            {
                return config;
            }

            return null;
        }

        /// <summary>
        /// 是否加载成功
        /// </summary>
        /// <returns></returns>
        public bool IsLoaded()
        {
            return _configs != null;
        }

        #region 资源加载相关

        private void DoLoadedCallBack()
        {
            var cb = _loadedCallback;
            _loadedCallback?.Invoke(this);
            _loadedCallback -= cb;
        }

        public TConfig<T> Load(Action<IConfig> callback = null)
        {
            //todo修改为异步加载
            _loadedCallback += callback;
            if (IsLoaded())
            {
                DoLoadedCallBack();
            }
            else
            {
                var textAsset =
                    AssetDatabase.LoadAssetAtPath<TextAsset>(Path.Combine(Excel2JsonUnity.ConfigDefine.ConfigRootPath,
                        $"{_name}.json"));
                Serialize(textAsset.text);
                DoLoadedCallBack();
            }

            return this;
        }

        /// <summary>
        /// 序列化数据
        /// </summary>
        /// <param name="json"></param>
        public void Serialize(string json)
        {
            _configs = JsonConvert.DeserializeObject<List<T>>(json);
            _configDic = new Dictionary<int, T>();
            foreach (var data in _configs)
            {
                _configDic.Add(data.ID, data);
            }
        }

        #endregion
    }
}