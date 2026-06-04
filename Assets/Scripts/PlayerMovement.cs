using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float velocita = 5f;
    private Rigidbody2D player;
    private Animator anim;
    private int facingDirection = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void FixedUpdate() // Agisce con un tempo fisso in linea con il motore fisico di RigidBody2D. Quindi la velocitá di lettura input non dipende dalla velocitá di generazione dei frame.
    {
        float x = 0f;
        float y = 0f;

        if (Keyboard.current.dKey.isPressed) x = 1f;
        if (Keyboard.current.aKey.isPressed) x = -1f;
        if (Keyboard.current.wKey.isPressed) y = 1f;
        if (Keyboard.current.sKey.isPressed) y = -1f;


        if(x > 0 && transform.localScale.x < 0 || x < 0 && transform.localScale.x > 0)
        {
            Flip();
        }

        anim.SetFloat("horizontal", Mathf.Abs(x));
        anim.SetFloat("vertical", Mathf.Abs(y));

        player.linearVelocity = new Vector2(x, y).normalized * velocita;
       
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    

  
}
