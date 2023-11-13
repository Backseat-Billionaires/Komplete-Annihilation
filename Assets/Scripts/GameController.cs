using UnityEngine;

public class GameController : MonoBehaviour
{
    public Map map;
    public GameObject commanderPrefab;

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
            foreach(var unit in activePlayer.UnitList)
                unit.Stop();
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

        // Get spawn point for the commander
        var spawnPoint = map.GetSpawnPoint(spawnOptions.SpawnOrder);

        // Create commander
        var commander = Instantiate(commanderPrefab, spawnPoint, Quaternion.identity);

        // Create player
        var player = new Player(commander.GetComponent<Unit>());
        players[playerIndex] = player;

        for (int i = 0; i < 8; i++)
        {
            var x = Random.Range(8, 184);
            var y = Random.Range(8, -184);
            var location = new Vector3(x, y, 0);
            var unit = Instantiate(commanderPrefab, location, Quaternion.identity);
            player.AddUnit(unit.GetComponent<Unit>());
        }

        if(spawnOptions.IsActivePlayer)
        {
            if (!Camera.main.TryGetComponent<CameraController>(out var cameraController)) throw new System.Exception("Unable to find main camera.");
            if (cameraController.activePlayer != null) throw new System.Exception("Scene already has an active player.");

            // Set active player
            cameraController.activePlayer = player;
            activePlayer = player;

            // Move camera, which will automatically clamp to screen.
            cameraController.transform.position = new Vector3(spawnPoint.x, spawnPoint.y, cameraController.transform.position.z);
        }
    }
}
