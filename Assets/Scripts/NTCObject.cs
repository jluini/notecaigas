using UnityEngine;

using JuloUtil;

namespace NoTeCaigas {
    public class NTCObject : MonoBehaviour {
        /*
        protected Rigidbody2D rb;
        protected Collider2D coll;
        
        
        void Start() {
            rb = GetComponent<Rigidbody2D>();
            coll = JuloFind.oneDescendant<Collider2D>(this);
            onStart();
        }
        public virtual void onStart() {}
        
        void Update() {
            respawn();
            onUpdate();
        }
        public virtual void onUpdate() {}
        void respawn() {
            if(transform.position.y < -3f) {
                Vector3 pos = transform.position;
                transform.position = new Vector3(pos.x, 20f, pos.z);
                
                rb.velocity = new Vector2();
            }
        }
        
        public void hit(Vector2 force) {
            rb.AddForce(force);
            onHit();
        }
        public virtual void onHit() {}
        */
    }
}
