using UnityEngine;

using JuloUtil;


namespace NoTeCaigas {
	public class Player : Obj {
		public float speedFactor = 1f;
		public float jumpSpeed   = 1f;
		
		public float shotDelay = 2f;
		public float recoverDelay = 1f;
		
		public float groundDetectionRadius = 0.3f;
		public float maxHorizontalJumpSpeed = 0.1f;
		
		bool recover;
		float lastHitTimestamp;
		
		[HideInInspector]
		public int numPlayer;
		
		NoTeCaigasGame game;
		InputManager inputManager;
		
		Animator anim;
		SpriteRenderer rend;
		
		bool charged; // make private
		float lastShotTimestamp;
		
		Transform groundDetector;
		
		bool grounded = false;
		bool walking = false;
		
		public override void onStart() {
			game = JuloFind.singleton<NoTeCaigasGame>();
			inputManager = JuloFind.singleton<InputManager>();
			anim = GetComponent<Animator>();
			rend = JuloFind.oneDescendant<SpriteRenderer>(this);
			
			groundDetector = JuloFind.byName<Transform>("GroundDetector", this);
			
			charged = true;
			lastShotTimestamp = JuloTime.gameTime();
			
			recover = false;
			lastHitTimestamp = JuloTime.gameTime();
		}
		
		public override void onUpdate() {
			grounded = isGrounded();
			anim.SetBool("grounded", grounded);
			
			bool fireAxis = inputManager.isDown("Fire", numPlayer);
			if(fireAxis) {
				tryToFire();
			}
			walking = false;
			if(/*grounded && */isReady()) {
				float horizontalAxis = inputManager.getAxis("Horizontal", numPlayer);
				if(horizontalAxis > 0) {
					rend.flipX = false;
					walking = true;
				} else if(horizontalAxis < 0) {
					rend.flipX = true;
					walking = true;
				}
				
				float horizontalSpeed = horizontalAxis * speedFactor;
				
				Vector2 vel = rb.velocity;
				vel.x = horizontalSpeed;
				
				bool jumpAxis = inputManager.isDown("Jump", numPlayer);
				if(jumpAxis) {
					if(grounded) {
						vel.y = jumpSpeed;
						float absx = Mathf.Abs(vel.x);
						if(absx > maxHorizontalJumpSpeed) {
							vel.x = maxHorizontalJumpSpeed * Mathf.Sign(vel.x);
						}
						anim.SetTrigger("jump");
					}
				}
				
				rb.velocity = vel;
			}
			anim.SetBool("grounded", grounded);
			anim.SetBool("walking", walking);
		}
		
		public override void onHit() {
			recover = true;
			lastHitTimestamp = JuloTime.gameTime();
		}
		
		bool isReady() {
			if(recover && JuloTime.gameTimeSince(lastHitTimestamp) > recoverDelay) {
				recover = false;
			}
			return !recover;
		}
		
		bool isGrounded() {
			Vector2 pos = groundDetector.position;
			Collider2D[] grounds = Physics2D.OverlapCircleAll(pos, groundDetectionRadius, game.whatIsGround);
			
			foreach(Collider2D c in grounds) {
				if(c.gameObject != coll.gameObject) {
					return true;
				}
			}
			return false;
		}
		
		bool isCharged() {
			if(!charged && JuloTime.gameTimeSince(lastShotTimestamp) > shotDelay) {
				charged = true;
			}
			return charged;
		}
		
		void tryToFire() {
			if(isCharged() && isReady()) {
				fire();
			}
		}
		void fire() {
			game.fire(transform.position);
			// ...
			charged = false;
			lastShotTimestamp = JuloTime.gameTime();
			anim.SetTrigger("fire");
		}
	}
}
