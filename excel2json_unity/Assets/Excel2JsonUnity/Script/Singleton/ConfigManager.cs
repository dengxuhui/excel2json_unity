using System.Collections.Generic;

namespace Excel2JsonUnity
{
    public class ConfigManager : Excel2JsonUnity.Singleton<ConfigManager>
    {
        private Dictionary<string, IConfig> _configDic;

        public override void Init()
        {
            base.Init();
            //初始化配置
            _configDic = new Dictionary<string, IConfig>
            {
                // { nameof(excel), new TConfig<ResNodes>().Load() }
            };
        }

        public override void Dispose()
        {
            _configDic = null;
        }

        /// <summary>
        /// 获取配置对象集
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TConfig<T> GetGroup<T>() where T : BaseConfig
        {
            var name = typeof(T).Name;
            if (I._configDic.TryGetValue(name, out IConfig data))
            {
                return (data as TConfig<T>);
            }

            return null;
        }

        /// <summary>
        /// 根据id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>(int id) where T : BaseConfig
        {
            var name = typeof(T).Name;
            if (I._configDic.TryGetValue(name, out IConfig data))
            {
                return (data as TConfig<T>)?.Get(id);
            }

            return null;
        }
    }
}