using JuloUtil;

using UnityEngine;
using UnityEngine.Assertions;

using String = System.String;
using System.Collections;

namespace NoTeCaigas {
    public class NTCPlayer : MonoBehaviour {

        public int playerNumber = 0;
        public string playerName = "player";

        public uint numLives;

        public NTCCharacter characterPrefab;

        [HideInInspector]
        public NTCCharacter currentCharacter = null;

        NTCGame game;

        public void Init(NTCGame game)
        {
            Assert.IsNotNull(game);
            this.game = game;

            Assert.IsNotNull(characterPrefab);

            StartCoroutine("Live");
        }

        IEnumerator Live()
        {
            yield return null;
            InputManager im = InputManager.Instance;

            while(numLives > 0)
            {
                currentCharacter = SpawnCharacter();

                // TODO ...

                while(true) {
                    currentCharacter.horizontalAxis = im.getAxis("Horizontal", playerNumber - 1);
                    currentCharacter.fire = im.isDown("Fire", playerNumber - 1);
                    currentCharacter.jump = im.isDown("Jump", playerNumber - 1);
                    currentCharacter.suicide = im.isDown("Weapon", playerNumber - 1);

                    //yield return new WaitForSeconds(1f);
                    yield return null;
                }

            }

            Debug.Log(String.Format("I'm dead!! {0}"));

            yield return null;
        }

        NTCCharacter SpawnCharacter()
        {
            GameObject newCharObj = Instantiate(characterPrefab.gameObject);
            NTCCharacter newCharacter = newCharObj.GetComponent<NTCCharacter>();
            newCharacter.game = this.game;
            newCharacter.transform.SetParent(this.transform);

            newCharacter.transform.position = new Vector3((playerNumber - 2) * 3f, 5f, 0f);

            return newCharacter;
        }
    }
}
