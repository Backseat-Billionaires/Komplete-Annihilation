using UnityEngine;

public class GameController : MonoBehaviour
{
    public Map map;
    public GameObject commanderPrefab;
    public GameObject playerPrefab; 

    private Player[] players = null;
    private Player activePlayer;
    private bool isInitialized;


    // Start is called before the first frame update
    void Start()
    {
        Initialize(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            var command = new Command(CommandType.Stop);
            activePlayer.SendCommandToSelectedUnits(command);
        }
    }

    public void Initialize(int playerCount)
    {
        isInitialized = true;
        players = new Player[playerCount];

        // Spawn player
        int rand = Random.Range(0, 4);
        var spawnOptions = new PlayerSpawnOptions(rand, PlayerColor.Blue) { IsActivePlayer = true };
        SpawnPlayer(0, spawnOptions);
    }

    public void SpawnPlayer(int playerIndex, PlayerSpawnOptions spawnOptions)
    {
        if (!isInitialized) throw new System.InvalidOperationException("Game is not initialized.");

        var spawnPoint = map.GetSpawnPoint(spawnOptions.SpawnOrder);

        // Instantiate the player prefab
        var playerObject = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
        var player = playerObject.GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError("Player component not found on player prefab");
            return;
        }

        // Create and assign the commander
        var commander = Instantiate(commanderPrefab, spawnPoint, Quaternion.identity);
        player.Initialize(commander.GetComponent<Unit>());

        players[playerIndex] = player;

        if (spawnOptions.IsActivePlayer)
        {
            SetActivePlayer(player, commander);
        }
    }

    private void SetActivePlayer(Player player, GameObject commander)
    {
        activePlayer = player;

        if (Camera.main.TryGetComponent<CameraController>(out var cameraController))
        {
            cameraController.activePlayer = player;
            cameraController.characterToFollow = commander.transform;
            cameraController.transform.position = new Vector3(commander.transform.position.x, commander.transform.position.y, cameraController.transform.position.z);
        }
        else
        {
            Debug.LogError("CameraController not found on the main camera.");
        }
    }


}
