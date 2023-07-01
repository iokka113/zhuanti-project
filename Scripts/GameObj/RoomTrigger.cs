using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTrigger : MonoBehaviour
{
    public Room Room { get; set; }
    public BoxCollider2D Colli { get; set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Room.OnPlayerEnter();
        }
    }
}
