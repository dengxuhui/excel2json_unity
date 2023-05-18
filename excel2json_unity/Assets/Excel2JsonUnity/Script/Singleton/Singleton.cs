using System;

namespace Excel2JsonUnity
{
    /// <summary>
    /// 单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : class, new()
    {
        private static T m_instance;

        public static T I
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = Activator.CreateInstance<T>();
                    if (m_instance != null)
                    {
                        (m_instance as Singleton<T>)?.Init();
                    }
                }

                return m_instance;
            }
        }

        public static void Release()
        {
            if (m_instance != null)
            {
                m_instance = (T)((object)null);
            }
        }

        public virtual void Init()
        {
        }

        public void Startup()
        {
        }

        public abstract void Dispose();
    }
}