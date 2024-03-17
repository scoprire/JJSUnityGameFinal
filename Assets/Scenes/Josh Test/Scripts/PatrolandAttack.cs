using UnityEngine;


public class PatrolandAttack : MonoBehaviour
{
    public float patrolSpeed = 2.0f;
    public float patrolDistance = 5.0f;
    private Vector3 startingPosition;
    private Animator animator;
    private bool isMovingRight = true;


    public CharacterState currentState;
    



    private void Start()
    {
        startingPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentState == CharacterState.Patrolling)

        {
            MoveToMiddle();
            HandlePatrolMovement();
            
            
        }
        if (currentState == CharacterState.Attacking)
        {
            MoveToMiddle();
            animator.SetTrigger("Attack");
        }
        if (currentState == CharacterState.Idle)
        {
            animator.SetTrigger("Idle");
            MoveToMiddle();
        }
    }

    private void HandlePatrolMovement()
    {
        if (currentState != CharacterState.Patrolling) return;

        float direction = isMovingRight ? 1 : -1;
        transform.position += Vector3.right * direction * patrolSpeed * Time.deltaTime;

        if (isMovingRight && transform.position.x >= startingPosition.x + patrolDistance)
        {
            SwitchDirection(false);
        }
        else if (!isMovingRight && transform.position.x <= startingPosition.x - patrolDistance)
        {
            SwitchDirection(true);
        }
    }

    private void MoveToMiddle()
    {
        

        // Assuming the middle of the screen along the x-axis is at x = 0
        float targetX = 0f;
        
        float duration = 2f; // Duration of 2 seconds to move to the middle

        // Calculate the fraction of the distance to move based on the duration
        float fraction = (Time.time - idleStartTime) / duration;

        // Use Mathf.Lerp to find the new position along the x-axis
        float newX = Mathf.Lerp(transform.position.x, targetX, fraction);
        

        // Update the character's position
        transform.position = new Vector3(newX, transform.position.y, 6);

        // If the character has reached or passed the middle, clamp their position and update the state to Idle
        if (Mathf.Abs(transform.position.x - targetX) < Mathf.Epsilon)
        {
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
            currentState = CharacterState.Idle; // Optionally update the state if needed
            animator.SetTrigger("Idle");
        }
    }

    private float idleStartTime; // Time when the idle state started

    // Call this method when you want to start moving to the idle state
    public void StartIdle()
    {
        idleStartTime = Time.time;
        currentState = CharacterState.Idle;
    }

    private void SwitchDirection(bool moveRight)
    {
        if (currentState != CharacterState.Patrolling) return;

        isMovingRight = moveRight;
        animator.SetBool("WalkRight", moveRight);
        animator.SetBool("WalkLeft", !moveRight);
    }

    
}
