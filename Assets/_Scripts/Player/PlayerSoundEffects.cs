using UnityEngine;

public class PlayerSoundEffects : MonoBehaviour
{
    [Header("Audio Clips")]
    [Tooltip("Sound to play for footsteps.")]
    [SerializeField] private AudioClip footstepSound;
    [Tooltip("Sound to play for attacks.")]
    [SerializeField] private AudioClip attackSound;

    [Header("Settings")]
    [Tooltip("Time in seconds between footstep sounds.")]
    [SerializeField] private float footstepInterval = 0.5f;

    private AudioSource audioSource;
    private PlayerControllerFSM playerController;
    private float footstepTimer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerControllerFSM>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        HandleMovementSound();
        HandleAttackSound();
    }

    private void HandleMovementSound()
    {
        bool isMoving = false;

        // Check movement based on FSM state if available, otherwise fallback to input/velocity
        if (playerController != null)
        {
            isMoving = playerController.currentState == PlayerControllerFSM.State.Move;
        }
        else
        {
            // Fallback: Check WASD input
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            isMoving = (h != 0 || v != 0);
        }

        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlaySound(footstepSound);
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            // Reset timer so sound plays immediately when starting to move again
            footstepTimer = 0f;
        }
    }

    private void HandleAttackSound()
    {
        // Check for Attack Inputs: Ctrl (Fire1/Custom), Enter/Return, or Left Click
        if (Input.GetKeyDown(KeyCode.LeftControl) || 
            Input.GetKeyDown(KeyCode.Return) || 
            Input.GetKeyDown(KeyCode.KeypadEnter) || 
            Input.GetMouseButtonDown(0))
        {
            PlaySound(attackSound);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            // Randomize pitch slightly for variety (optional but good feel)
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }
    }
}
