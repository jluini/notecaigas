using JuloUtil;

using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
#endif

using String = System.String;
using Random = System.Random;
using System.Collections;

namespace NoTeCaigas
{
    public class NTCCharacter : MonoBehaviour
    {
        public int playerNumber = 0;

        [Header("Animatable")]
        public bool pushing = false;
        bool pushTaken = false;

        [Header("Hooks")]
        public LayerMask whatIsGround;

        public Transform groundDetector;
        public float groundDetectionRadius = 0.3f;
        public Transform pushDetector;
        public float pushDetectionRadius = 0.3f;

        public Collider2D coll;
        public SpriteRenderer rend;

        [Header("Attributes")]
        public float shotDelay = 2f;

        public float pushForce = 10f;

        public float recoverDelay = 1f;


        public float speedFactor = 10f;
        public float jumpSpeed   = 1f;
        public float maxHorizontalJumpSpeed = 0.1f;

        [HideInInspector]
        public float horizontalAxis = 0f;
        [HideInInspector]
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

                    if(horizontalAxis > 0)
                    {
                        walking = true;
                        if(!horizontalDir)
                        {
                            horizontalDir = true;
                            rend.flipX = false;
                            Vector3 ppos = pushDetector.transform.localPosition;
                            ppos.x = -ppos.x;
                            pushDetector.transform.localPosition = ppos;
                        }
                    } else if(horizontalAxis < 0)
                    {
                        walking = true;
                        if(horizontalDir)
                        {
                            horizontalDir = false;
                            rend.flipX = true;
                            Vector3 ppos = pushDetector.transform.localPosition;
                            ppos.x = -ppos.x;
                            pushDetector.transform.localPosition = ppos;
                        }
                    }
                    
                    float horizontalSpeed = horizontalAxis * speedFactor;
                    vel.x = horizontalSpeed;


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
                                //Vector3 projectileVel = rb.velocity;
                                //projectileVel.x += projectileSpeed * (horizontalDir ? +1f : -1f);
                                //game.NewProjectile(0, transform.position, Quaternion.identity, projectileVel);


                                anim.SetTrigger("fire");
                                pushTaken = false;
                                lastShotTimestamp = now;
                            }
                        }
                    }

                    rb.velocity = vel;

                    anim.SetBool("grounded", grounded);
                    anim.SetBool("walking", grounded && walking);

                    yield return null;
                }

                // restore pushing, maybe I were pushing while being attacked
                pushing = false;

                // is under attack
                anim.SetTrigger("hit");
                rb.AddForce(newAttack.GetForce(), ForceMode2D.Force);
                newAttack = null;

                yield return new WaitForSeconds(recoverDelay);
                anim.SetTrigger("back");

                //yield return null;
            }
        }

        void LateUpdate()
        {
            if(pushing && !pushTaken)
            {
                for(int i = 1; i <= game.numPlayers; i++)
                {
                    if(i != playerNumber)
                    {
                        //Debug.Log("Checking " + Time.frameCount);

                        Vector3 pushPos = pushDetector.position;
                        NTCCharacter otherCharacter = game.GetPlayer(i).currentCharacter;
                        Vector3 otherPos = otherCharacter.transform.position;

                        float dist = Vector3.Distance(pushPos, otherPos);

                        if(dist <= pushDetectionRadius)
                        {
                            //Debug.Log("Hitting");

                            otherCharacter.newAttack = new NTCAttack(new Vector3(pushForce * (horizontalDir ? 1f : -1f), 0f, 0f));

                            //pushing = false;
                            pushTaken = true;
                        }
                    }
                }
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

        #if UNITY_EDITOR

        void OnDrawGizmos()
        {
            if(groundDetector)
            {
                DrawGizmoCircle(Color.green, groundDetector.position, groundDetectionRadius);
            }

            if(pushDetector)
            {
                DrawGizmoCircle(Color.red, pushDetector.position, pushDetectionRadius);
                if(pushing)
                {
                    for(float rad = 0.9f; rad > 0f; rad -= 0.1f)
                    {
                        DrawGizmoCircle(Color.yellow, pushDetector.position, pushDetectionRadius * rad);
                    }
                }
            }
        }

        void DrawGizmoCircle(Color color, Vector3 position, float radius)
        {
            //Gizmos.color = color;
            //Gizmos.DrawSphere(position, radius);
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawWireDisc(position , Vector3.back, radius);
        }

        #endif
    }
}
