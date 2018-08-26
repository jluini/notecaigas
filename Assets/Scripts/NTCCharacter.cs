using JuloUtil;

using UnityEngine;
using UnityEngine.Assertions;

using Random = System.Random;
using System.Collections;

namespace NoTeCaigas
{
    public class NTCCharacter : MonoBehaviour
    {
        [Header("Hooks")]
        public Transform groundDetector;
        public Collider2D coll;
        public SpriteRenderer rend;

        [Header("Attributes")]
        public LayerMask whatIsGround;

        public float shotDelay = 2f;
        public float projectileSpeed = 10f;
        public float speedFactor = 10f;
        public float jumpSpeed   = 1f;

        public float groundDetectionRadius = 0.3f;
        public float maxHorizontalJumpSpeed = 0.1f;

        [HideInInspector]
        public float horizontalAxis = 0f;
        bool horizontalDir = true;

        [HideInInspector]
        public bool  fire = false;
        [HideInInspector]
        public bool  jump = false;
        [HideInInspector]
        public bool suicide = false; // TODO eliminate this


        [HideInInspector]
        public NTCAttack newAttack  = null;

        [HideInInspector]
        public NTCGame game;

        Rigidbody2D rb;
        Animator anim;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            Assert.IsNotNull(rb);
            anim = GetComponent<Animator>();
            Assert.IsNotNull(anim);

            lastShotTimestamp = JuloTime.gameTime() - shotDelay;

            StartCoroutine("Live");
        }

        float lastShotTimestamp;

        IEnumerator Live()
        {
            yield return null; // wait a frame
            
            while(true)
            {
                // is ready
                while(true) //while(newAttack == null)
                {
                    FixPosition();

                    if(suicide)
                    {
                        Random rnd = new Random();
                        int sgn = rnd.Next(2) * 2 - 1;
                        
                        newAttack = new NTCAttack(new Vector3(+200f * sgn, +100f, 0f));
                    }

                    if(newAttack != null)
                        break;

                    bool grounded = IsGrounded();
                    bool walking = false;

                    Vector2 vel = rb.velocity;

                    //if(grounded)
                    //{
                        if(horizontalAxis > 0)
                        {
                            horizontalDir = true;
                            rend.flipX = false;
                            walking = true;
                        } else if(horizontalAxis < 0)
                        {
                            horizontalDir = false;
                            rend.flipX = true;
                            walking = true;
                        }
                        
                        float horizontalSpeed = horizontalAxis * speedFactor;
                        vel.x = horizontalSpeed;

                    //}

                    if(grounded && jump)
                    {
                        vel.y = jumpSpeed;
                        float absx = Mathf.Abs(vel.x);
                        // TODO for what is this?
                        if(absx > maxHorizontalJumpSpeed)
                        {
                            vel.x = maxHorizontalJumpSpeed * Mathf.Sign(vel.x);
                        }
                        anim.SetTrigger("jump");
                    }

                    if(fire)
                    {
                        float now = JuloTime.gameTime();

                        if(now - lastShotTimestamp > shotDelay)
                        {
                            if(CanShot())
                            {
                                // shot
                                Vector3 projectileVel = rb.velocity;
                                projectileVel.x += projectileSpeed * (horizontalDir ? +1f : -1f);
                                anim.SetTrigger("fire");
                                game.NewProjectile(0, transform.position, Quaternion.identity, projectileVel);
                                lastShotTimestamp = now;
                            }
                        }
                    }

                    rb.velocity = vel;

                    anim.SetBool("grounded", grounded);
                    anim.SetBool("walking", grounded && walking);

                    yield return null;
                }

                // is under attack
                anim.SetTrigger("hit");
                rb.AddForce(newAttack.GetForce(), ForceMode2D.Force);
                newAttack = null;

                yield return new WaitForSeconds(2f);
                anim.SetTrigger("back");

                //yield return null;
            }
        }

        void FixPosition()
        {
            Vector3 pos = transform.position;
            if(pos.y < NTCEnvironment.Instance.yMin)
            {
                pos.y = NTCEnvironment.Instance.yMax;
                Vector3 vel = rb.velocity;
                vel.y = 0f;
                rb.velocity = vel;

                transform.position = pos;
            }
        }

        bool CanShot()
        {
            return true;
        }

        bool IsGrounded()
        {
            Vector2 pos = groundDetector.position;
            Collider2D[] grounds = Physics2D.OverlapCircleAll(pos, groundDetectionRadius, whatIsGround);

            foreach(Collider2D c in grounds) {
                if(c.gameObject != coll.gameObject) {
                    return true;
                }
            }
            return false;
        }

        /*
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
            //if(grounded && isReady()) {
            if(isReady()) {
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
        */
    }
}
