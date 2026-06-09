using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public int damage = 1;

    public void InflictDamage()
    {
        Transform player = GetComponent<Enemy_Movement>().player;
        if( player != null)
        {
            player.GetComponent<PlayerHealth>().ChangeHealth(-damage);
        }
            
    }
}
