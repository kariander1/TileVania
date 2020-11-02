using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{

    const string RUNNING_STRING_REFERENCE = "IsRunning";
    const string WALKING_STRING_REFERENCE = "IsWalking";
    const string CLIMBING_STRING_REFERENCE = "IsClimbing";
    const string JUMPING_STRING_REFERENCE = "IsJumping";
    const string DOUBLE_JUMPING_STRING_REFERENCE = "IsDoubleJumping";
    const string CLIMBING_STATIC_STRING_REFERENCE = "IsStaticClimb";
    const string DYING_STRING_REFERENCE = "Dying";
    const string WINNING_STRING_REFERENCE = "Winning";

    const string GROUND_LAYER_STRING = "Ground";
    const string CLIMB_LAYER_STRING = "Climbing";
    const string ENEMY_LAYER_STRING = "Enemy";
    const string HAZARDS_LAYER_STRING = "Hazards";
    const string WATER_LAYER_STRING = "Water";

    [Header("Defaults")]
    [SerializeField] float defalutBaseWalkVelocity = 5f;
    [SerializeField] float defaultBaseRunVelocity = 10f;
    [SerializeField] float defalutBaseJumpVelocity = 5f;
    [SerializeField] float defaultBaseDoubleJumpVelocity = 2.5f;

    [Header("Velocity")]
    [SerializeField] float baseWalkVelocity = 5f;
    [SerializeField] float baseRunVelocity = 10f;
    [SerializeField] float baseJumpVelocity = 5f;
    [SerializeField] float baseDoubleJumpVelocity = 2.5f;
    [SerializeField] float baseAscendClimbVelocity = 3f;
    [SerializeField] float baseDescendClimbVelocity = 5f;

    [SerializeField] GameObject VFXdoubleJump;

    [SerializeField] AudioClip SFXjump;
    [SerializeField] AudioClip SFXdoubleJump;
    [SerializeField] AudioClip SFXdeath;
    [SerializeField] AudioClip SFXwin;

    [SerializeField] bool DEBUG = false;
    bool isAlive = true;

    bool isClimbing = false;
    bool jumping = false;
    bool doubleJumping = false;
    bool isSwiming = false;
    bool isWinning = false;

    bool isTouchingGroundThisFrame = false;
    bool isTouchingLadderThisFrame = false;
    bool isTouchingEnemyThisFrame = false;
    bool isTouchingHazardThisFrame = false;
    bool isJumpingThisFrame = false;
    bool isTouchingWaterThisFrame = false;

    Rigidbody2D myBody;
    Animator animator;
    CapsuleCollider2D bodyCollider;
    BoxCollider2D feetCollider;

    [SerializeField] float defaultGravityScale=1f;
    private float currentGravityScale;
    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        feetCollider = GetComponent<BoxCollider2D>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        RestoreDefaults();
    }

    void UpdateFlags()
    {
        isJumpingThisFrame = false; 
        isTouchingGroundThisFrame = isTouchingGround();
        isTouchingLadderThisFrame = isTouchingLadder();
        isTouchingEnemyThisFrame = isTouchingEnemy();
        isTouchingHazardThisFrame = isTouchingHazard();
        isTouchingWaterThisFrame = isTouchingWater();
    }

 

    // Update is called once per frame
    void Update()
    {
        UpdateFlags();
        if (isAlive && !isWinning)
        {
            Jump();
            CheckForRestoreJumps();
            HandleMovement();
            ClimbLadder();
            Swim();
            SnapPlayer();
         
        }
        Die();
        FlipSprite();
        UpdateAnimationState();
        UpdateGravityScale();
    }

    private void UpdateGravityScale()
    {
        myBody.gravityScale = currentGravityScale;
    }
    IEnumerator ReloadWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex );
       
    }
    private void SnapPlayer()
    {
        if(isClimbing)
        {
            
            transform.position = new Vector2(Mathf.Floor(transform.position.x) +0.5f, transform.position.y);
        }
    }
    private void Die()
    {
        if((isTouchingHazardThisFrame || isTouchingEnemyThisFrame )&& isAlive)
        {
            isAlive = false;
            AudioSource.PlayClipAtPoint(this.SFXdeath,myBody.position);
            animator.SetTrigger(DYING_STRING_REFERENCE);
            myBody.velocity = Vector2.zero;
            currentGravityScale = 0;
            StartCoroutine(ReloadWithDelay(1.5f));
        }
    }
    private void SwimCutSpeed()
    {
        currentGravityScale = defaultGravityScale / 2;
        baseWalkVelocity = defalutBaseWalkVelocity / 2;
        baseRunVelocity = defaultBaseRunVelocity / 2;
        baseJumpVelocity = defalutBaseJumpVelocity / 2;
        baseDoubleJumpVelocity = defaultBaseDoubleJumpVelocity / 2;


    }
    private void RestoreDefaults()
    {
        currentGravityScale = defaultGravityScale;
        baseWalkVelocity = defalutBaseWalkVelocity;
        baseRunVelocity = defaultBaseRunVelocity;
        baseJumpVelocity = defalutBaseJumpVelocity ;
        baseDoubleJumpVelocity = defaultBaseDoubleJumpVelocity;

    }
    private void Swim()
    {
        
        if (isTouchingWaterThisFrame)
        {

            isSwiming = true;
            SwimCutSpeed();
        }
        else if(!isTouchingWaterThisFrame &&  isSwiming)
        {
            isSwiming = false;
            RestoreDefaults();
        }
    }
    private void UpdateAnimationState()
    {
        
        if (isTouchingGroundThisFrame)
        {
            animator.SetBool(CLIMBING_STRING_REFERENCE, false);
            animator.SetBool(CLIMBING_STATIC_STRING_REFERENCE, false);
        }
        if(!isTouchingGroundThisFrame)
        {

            animator.SetBool(WALKING_STRING_REFERENCE, false);
            animator.SetBool(RUNNING_STRING_REFERENCE, false);


            
            animator.SetBool(CLIMBING_STRING_REFERENCE, isClimbing);
            if (isClimbing)
                animator.SetBool(CLIMBING_STATIC_STRING_REFERENCE, Mathf.Abs(myBody.velocity.y) <= Mathf.Epsilon);
            else
                animator.SetBool(CLIMBING_STATIC_STRING_REFERENCE, false);



        }
        else if (Mathf.Abs( myBody.velocity.x )>baseWalkVelocity)
        {
            animator.SetBool(RUNNING_STRING_REFERENCE, true);
        }
        else if(Mathf.Abs( myBody.velocity.x) > Mathf.Epsilon )
        {
            animator.SetBool(RUNNING_STRING_REFERENCE, false);
            animator.SetBool(WALKING_STRING_REFERENCE, true);
        }
        else if (Mathf.Abs( myBody.velocity.x) <= Mathf.Epsilon)
        {
            animator.SetBool(WALKING_STRING_REFERENCE, false);
            animator.SetBool(RUNNING_STRING_REFERENCE, false);

        }
        animator.SetBool(JUMPING_STRING_REFERENCE, jumping);
        animator.SetBool(DOUBLE_JUMPING_STRING_REFERENCE, doubleJumping);
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // -1 -> 1
        SetVelocity(new Vector2(controlThrow * baseRunVelocity, myBody.velocity.y));
    }
    
    private void HandleMovement()
    {

        if(!isClimbing)
        {
            float controlThrow = CrossPlatformInputManager.GetAxis("Fire3"); // -1 -> 1
            if (Mathf.Abs(controlThrow) > Mathf.Epsilon)
            {
                Run();
            }
            else
            {
                Walk();
            }
        }
       


    }
    public void TriggerWin()
    {
        isWinning = true;
        AudioSource.PlayClipAtPoint(SFXwin, transform.position);
        animator.SetTrigger(WINNING_STRING_REFERENCE);
        myBody.velocity = Vector2.zero;
        currentGravityScale = 0;
    }
    private bool isTouchingHazard()
    {
        return feetCollider.IsTouchingLayers(LayerMask.GetMask(HAZARDS_LAYER_STRING)) || bodyCollider.IsTouchingLayers(LayerMask.GetMask(HAZARDS_LAYER_STRING));
    }
    bool isTouchingGround()
    {
        return feetCollider.IsTouchingLayers(LayerMask.GetMask(GROUND_LAYER_STRING));
    }
    bool isTouchingLadder()
    {
        return bodyCollider.IsTouchingLayers(LayerMask.GetMask(CLIMB_LAYER_STRING));
    }
    bool isTouchingEnemy()
    {
        return bodyCollider.IsTouchingLayers(LayerMask.GetMask(ENEMY_LAYER_STRING));
    }
    bool isTouchingWater()
    {
        return bodyCollider.IsTouchingLayers(LayerMask.GetMask(WATER_LAYER_STRING));
    }
    private void ClimbLadder()
    {
        
        if (!isTouchingLadderThisFrame)
        {
            if(isClimbing)
               currentGravityScale = defaultGravityScale; //If only player was climbing
            isClimbing = false;
            return;
        }
        if(isTouchingGroundThisFrame || jumping)
        {
            if (isClimbing)
                currentGravityScale = defaultGravityScale; //If only player was climbing
            isClimbing = false;

        
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical"); // -1 -> 1

        Vector2 climbVelocity = (Mathf.Sign(controlThrow) == 1) ?
            new Vector2(myBody.velocity.x, controlThrow * baseAscendClimbVelocity) :
            new Vector2(myBody.velocity.x, controlThrow * baseDescendClimbVelocity);
        if (Mathf.Abs(climbVelocity.y) > Mathf.Epsilon && !(isTouchingGroundThisFrame && Mathf.Sign(climbVelocity.y)==-1)) // Has vertical velocity and not touching ground with negative velocity
        {
            isClimbing = true;

          currentGravityScale = 0;
        }

        if (isClimbing)
           SetVelocity( climbVelocity);


    }
    private void Jump()
    {
        
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {

            if ((isTouchingGroundThisFrame || isTouchingLadderThisFrame) && !jumping)
            {
                LogDebug("Start Jump");
                AudioSource.PlayClipAtPoint(this.SFXjump, myBody.position);
                currentGravityScale = defaultGravityScale;
                jumping = true;
                isJumpingThisFrame = true;
                SetVelocity(new Vector2(myBody.velocity.x, baseJumpVelocity));
            }
            else if (!doubleJumping)
            {
                doubleJumping = true;
                AudioSource.PlayClipAtPoint(this.SFXdoubleJump, myBody.position);
                GameObject vfx = Instantiate(VFXdoubleJump, GetComponent<Collider2D>().bounds.min, Quaternion.identity);
                Destroy(vfx, vfx.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + 0);
                SetVelocity(new Vector2(myBody.velocity.x, baseDoubleJumpVelocity));

            }
            
        }
    }
    bool attemptToRestoreJumps = true;
    private void CheckForRestoreJumps()
    {
        if (isJumpingThisFrame)
        {
            if (isTouchingGroundThisFrame)
                attemptToRestoreJumps = false;
            LogDebug("Jumping this frame, won't check for restoring jumps");
            return;
        }

        if (!attemptToRestoreJumps && !isTouchingGroundThisFrame)
        {
            attemptToRestoreJumps = true;
        }
        if (attemptToRestoreJumps)
        {
            LogDebug("Touching Ground : " + isTouchingGroundThisFrame);
            if (isTouchingGroundThisFrame || isClimbing || isSwiming)
            {
                LogDebug("Restoring jumps");
                jumping = false;
                doubleJumping = false;
                attemptToRestoreJumps = false;
            }
        }
    }
    private void Walk()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // -1 -> 1
        SetVelocity(new Vector2(controlThrow * baseWalkVelocity, myBody.velocity.y));
    }
    private void FlipSprite()
    {
        bool playerHasFlipped = Mathf.Abs(myBody.velocity.x) > Mathf.Epsilon;
        if(playerHasFlipped)
        {
            transform.localScale = new Vector2(Mathf.Sign(myBody.velocity.x), 1f);
        }
    }
    private void SetVelocity(Vector2 newVelocity)
    {
        myBody.velocity = newVelocity;
    }
    public void LogDebug(string text)
    {
        if(DEBUG)
        {
            Debug.Log(text);
        }
    }
}
