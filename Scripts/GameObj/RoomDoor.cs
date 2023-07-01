using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomDoor : MonoBehaviour
{
    public BoxCollider2D Colli { get; set; }
    public SpriteRenderer Pic { get; set; }
}
