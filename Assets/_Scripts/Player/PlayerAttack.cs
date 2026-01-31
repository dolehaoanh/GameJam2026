using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("Key to trigger the attack.")]
    [SerializeField] private KeyCode attackKey = KeyCode.Return;

    void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            Attack();
        }
    }

    private void Attack()
    {
        Debug.Log("Player Attacked!");
        // Future logic for animations, handling damage, etc.
    }
}
