

using Cysharp.Threading.Tasks;

public class Singleton<T> where T : Singleton<T>, new()
    {

        private static T m_instance;

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new T();
                    m_instance.Initialize().Forget();
                }
                return m_instance;
            }
        }

        protected virtual async UniTask Initialize()
        {}
    }