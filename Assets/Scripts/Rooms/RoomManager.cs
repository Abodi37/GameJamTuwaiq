using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    public Transform[] roomSpawnPoints;

    public int currentRoom = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void SwitchRoom(Transform player)
    {
         Debug.Log("SwitchRoom called");

        currentRoom++;

        if (currentRoom >= roomSpawnPoints.Length)
            currentRoom = 0;

             Debug.Log("Current room: " + currentRoom);

        Debug.Log("Target position: " + roomSpawnPoints[currentRoom].position);

        CharacterController cc = player.GetComponent<CharacterController>();

        cc.enabled = false;
        player.position = roomSpawnPoints[currentRoom].position;
        cc.enabled = true;

        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();

        foreach (EnemyAI enemy in enemies)
        {
            enemy.ResetEnemy();
        }
    }
}
