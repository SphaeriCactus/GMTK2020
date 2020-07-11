using UnityEngine;

public class Edward : MonoBehaviour
{
    public Transform[] endPoints;
    public Transform player;

    public void Clip(int index)
    {
        player.transform.position = endPoints[index].position;
    }
}
