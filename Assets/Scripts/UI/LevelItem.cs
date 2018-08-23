using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace NoTeCaigas
{
    public class LevelItem : MonoBehaviour {

        NTCLevel attachedLevel = null;

        public Text nameDisplay;
        public Button playButton;

        public void Draw(NTCLevel level)
        {
            this.attachedLevel = level;

            nameDisplay.text = level.levelName;

            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(delegate{

                NTCEnvironment.Instance.Play(attachedLevel.scene);

            });

        }

    }
}
