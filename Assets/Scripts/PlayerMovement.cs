using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    public float velocita = 5f;
    private Rigidbody2D player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
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


        player.linearVelocity = new Vector2(x, y).normalized * velocita;
    }
}
