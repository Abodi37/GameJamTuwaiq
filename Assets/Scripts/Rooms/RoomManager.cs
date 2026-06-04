using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("Linear Rooms")]
    public Transform[] roomSpawnPoints;
    public string[] roomNames = { "Garage", "Alchemist Room", "Angel Hallway" };
    public int currentRoom = 0;

    [Header("Optional Companion")]
    public HeIsRightBehindMeIsntHe companion;

    private int[] roomVisitCounts;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (roomSpawnPoints != null)
            roomVisitCounts = new int[roomSpawnPoints.Length];

        RegisterRoomVisit(currentRoom);
        UpdateUI();
    }

    public void SwitchRoom(Transform player)
    {
        if (roomSpawnPoints == null || roomSpawnPoints.Length == 0) return;
        if (player == null) return;

        currentRoom++;

        if (currentRoom >= roomSpawnPoints.Length)
            currentRoom = 0;

        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        player.position = roomSpawnPoints[currentRoom].position + Vector3.up * 0.3f;
        player.rotation = roomSpawnPoints[currentRoom].rotation;

        if (cc != null)
            cc.enabled = true;

        RegisterRoomVisit(currentRoom);
        UpdateUI();

        if (companion != null)
            companion.WarpToPlayer();
    }

    void RegisterRoomVisit(int roomIndex)
    {
        if (roomVisitCounts == null || roomIndex < 0 || roomIndex >= roomVisitCounts.Length)
            return;

        roomVisitCounts[roomIndex]++;

        if (GameManager.Instance != null)
            GameManager.Instance.SetCurrentRoom(roomIndex);

        Debug.Log("Entered Room: " + GetCurrentRoomName() + " Visit: " + roomVisitCounts[roomIndex]);
    }

    public int GetRoomVisitCount(int roomIndex)
    {
        if (roomVisitCounts == null || roomIndex < 0 || roomIndex >= roomVisitCounts.Length)
            return 0;

        return roomVisitCounts[roomIndex];
    }

    public string GetCurrentRoomName()
    {
        if (roomNames != null && currentRoom >= 0 && currentRoom < roomNames.Length)
            return roomNames[currentRoom];

        return "Room " + currentRoom;
    }

    void UpdateUI()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.SetRoom(GetCurrentRoomName());
    }
}