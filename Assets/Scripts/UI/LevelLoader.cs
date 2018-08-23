using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace NoTeCaigas
{
    
    public class LevelLoader : MonoBehaviour {

        public LevelItem itemPrefab;
        public Transform levelContainer;

        void Start()
        {
            ListLevels();
        }

        void ListLevels()
        {
            NTCEnvironment env = NTCEnvironment.Instance;

            foreach(NTCLevel level in env.levels)
            {
                Assert.IsNotNull(level, "Null level");

                LevelItem newItem = Instantiate(itemPrefab.gameObject).GetComponent<LevelItem>();
                Assert.IsNotNull(newItem);

                newItem.Draw(level);

                newItem.transform.SetParent(levelContainer);
            }
        }
    }
}
