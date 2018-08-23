using JuloUtil;

using UnityEngine;

using String = System.String;
using System.Collections;
using Type = System.Type;

namespace NoTeCaigas
{
    public class NTCLoader : MonoBehaviour
    {
        public bool startGameAutomatically;

        [Header("Singletons")]
        public NTCEnvironment environmentPrefab;
        public InputManager inputManager;

        void Start()
        {
            if(CheckSingleton(typeof(NTCEnvironment), environmentPrefab.gameObject))
            {
                // TODO check some level is loaded?
                if(startGameAutomatically)
                {
                    NTCEnvironment.Instance.StartGame();
                }
            }
        }

        bool CheckSingleton(Type singletonClass, GameObject prefab)
        {
            Object[] objs = FindObjectsOfType(singletonClass);
            int num = objs.Length;
            if(num > 1)
            {
                throw new NTCException(String.Format("More than one {0}", singletonClass));
            }
            else if(num == 1)
            {
                //Debug.Log(String.Format("Instance of {0} already created, doing nothing", singletonClass));
                return false;
            }
            else
            {
                //Debug.Log(String.Format("Creating {0} instance", singletonClass));
                Instantiate(prefab);
                return true;
            }
        }
    }
}