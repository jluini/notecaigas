using UnityEngine;
using System.Collections.Generic;

using JuloUtil;

namespace NoTeCaigas {
	public class NoTeCaigasGame : MonoBehaviour {
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
	}
}
