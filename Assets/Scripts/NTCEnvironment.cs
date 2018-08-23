using JuloUtil;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using System.Collections;


namespace NoTeCaigas {
    /**
     * Entorno simple que crea un juego con dos jugadores locales.
     */
    public class NTCEnvironment : Singleton<NTCEnvironment> {
        [Header("Fixing position")]
        public float yMin = -5f;
        public float yMax = 10f;

        public NTCGame gamePrefab;
        public NTCPlayer playerPrefab;

        public NTCProjectile [] projectiles;

        public NTCLevel [] levels;


        NTCGame currentGame = null;

        void Start()
        {
            //CreateGame();
        }

        public void Play(SceneField scene)
        {
            SceneManager.LoadScene(scene);
            StartGame();
        }

        public void StartGame()
        {
            Assert.IsNotNull(gamePrefab, "Game prefab is empty");
            Assert.IsNotNull(playerPrefab, "Player prefab is empty");

            NTCPlayer p1 = NewPlayer(1, "p1");
            NTCPlayer p2 = NewPlayer(2, "p2");

            currentGame = NewGame();
            currentGame.projectiles = this.projectiles;

            NTCPlayer[] players = new NTCPlayer[2];
            players[0] = p1;
            players[1] = p2;

            currentGame.Init(players);
        }

        NTCPlayer NewPlayer(int playerNumber, string playerName)
        {
            GameObject newPlayerObj = Instantiate(playerPrefab.gameObject);
            NTCPlayer newPlayer = newPlayerObj.GetComponent<NTCPlayer>();

            // TODO right?
            newPlayer.transform.SetParent(this.transform);

            newPlayer.playerNumber = playerNumber;
            newPlayer.playerName = playerName;

            return newPlayer;
        }

        NTCGame NewGame()
        {
            GameObject newGameObj = Instantiate(gamePrefab.gameObject);
            NTCGame newGame = newGameObj.GetComponent<NTCGame>();

            // TODO right?
            newGame.transform.SetParent(this.transform);

            return newGame;
        }

    }
}
