using JuloUtil;

using UnityEngine;
using UnityEngine.Assertions;

using String = System.String;
using System.Collections;

namespace NoTeCaigas {
    public class NTCGame : MonoBehaviour {

        public NTCProjectile[] projectiles;

        NTCPlayer[] players;

        [HideInInspector]
        public int numPlayers = 0;

        public void Init(NTCPlayer[] players)
        {
            this.players = players;
            this.numPlayers = players.Length;
        }


        void Start()
        {
            Debug.Log(String.Format("Arranca un partido con {0} jugadores!", numPlayers));

            for(int i = 0; i < numPlayers; i++)
            {
                players[i].Init(this);
            }

            //StartCoroutine("Prueba");
        }

        IEnumerator Prueba()
        {
            while(true)
            {
                yield return new WaitForSeconds(7f);

                //Debug.Log("Holitas");

                players[0].currentCharacter.newAttack = new NTCAttack(new Vector3(+200f, +100f, 0f));
            }
        }

        public NTCPlayer GetPlayer(int numPlayer)
        {
            return players[numPlayer - 1];
        }

        public void NewProjectile(int projectileType, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            if(projectileType < 0 || projectileType >= projectiles.Length)
            {
                Debug.Log(String.Format("Invalid projectile number: {0}", projectileType));
            }

            NTCProjectile projectilePrefab = projectiles[projectileType];

            Assert.IsNotNull(projectilePrefab, "Projectile prefab number " + projectileType + " is null");

            GameObject newObj = (GameObject)Instantiate(projectilePrefab.gameObject, position, rotation, this.transform);
            NTCProjectile newProjectile = newObj.GetComponent<NTCProjectile>();
            newProjectile.velocity = velocity;

            //newProjectile.gameObject.SetActive(true);
        }

        /*
        public float force = 5f;
        public LayerMask whatIsGround;
        
        Player p1, p2;
        
        List<Player> allPlayers;
        List<Obj> allObjects;
        
        void Start() {
            p1 = JuloFind.byName<Player>("Player1");
            p2 = JuloFind.byName<Player>("Player2");
            
            p1.numPlayer = 0;
            p2.numPlayer = 1;
            
            allPlayers = new List<Player>();
            allPlayers.Add(p1);
            allPlayers.Add(p2);
            
            allObjects = JuloFind.allWithComponent<Obj>(); // new List<Rigidbody2D>();
        }
        
        public void fire(Vector3 position) {
            ForceWeighting weighting = new DistanceWeighting(position, 0.1f, 10f);
            float threshold = 0.01f;
            
            foreach(Obj p in allObjects) {
                if(Vector3.Distance(p.transform.position, position) > threshold) {
                    Vector2 f = weighting.weight(p.transform);
                    float magnitude = f.magnitude;
                    
                    if(magnitude > 0f) {
                        p.hit(f * force);
                    }
                }
            }
        }
        */
    }
}
