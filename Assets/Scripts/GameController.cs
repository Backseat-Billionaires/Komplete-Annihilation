using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Map map;
    public GameObject commanderPrefab;
    public GameObject playerPrefab; 

    private Player[] players = null;
    private Player activePlayer;
    private bool isInitialized;



    public GameObject minePrefab; // Assign in Inspector
    private GameObject selectedMetalDeposit;



    private List<IGameSelectable> selectedObjects = new List<IGameSelectable>();


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

        if (Input.GetKeyDown(KeyCode.M) && selectedMetalDeposit != null)
        {
            TryPlaceMine();
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



    public void SelectObject(GameObject obj)
    {
        IGameSelectable selectable = obj.GetComponent<IGameSelectable>();
        if (selectable != null)
        {
            if (selectable.IsSelected())
            {
                // Already selected, so deselect it
                selectable.Deselect();
                selectedObjects.Remove(selectable);
            }
            else
            {
                // Select it and add it to the list
                selectable.Select();
                selectedObjects.Add(selectable);
            }
        }

        // Enforce rules for selection
        EnforceSelectionRules();
    }

    private void EnforceSelectionRules()
    {
        // If a mine is selected, deselect any other selectable objects
        bool mineSelected = selectedObjects.Any(obj => obj is Mine);
        if (mineSelected)
        {
            var othersToDeselect = selectedObjects.Where(obj => !(obj is Mine)).ToList();
            foreach (var selectable in othersToDeselect)
            {
                selectable.Deselect();
                selectedObjects.Remove(selectable);
            }
        }
        // If a metal deposit is selected, make sure only one can be selected at a time
        var selectedDeposits = selectedObjects.OfType<MetalDeposit>().ToList();
        if (selectedDeposits.Count > 1)
        {
            // Deselect all but the most recently selected metal deposit
            for (int i = 0; i < selectedDeposits.Count - 1; i++)
            {
                selectedDeposits[i].Deselect();
                selectedObjects.Remove(selectedDeposits[i]);
            }
        }
    }



    private void TryPlaceMine()
    {
        var resourceManagement = activePlayer.GetComponent<ResourceManagement>();
        if (resourceManagement.CanAffordMineCost())
        {
            var mineScript = selectedMetalDeposit.GetComponent<MetalDeposit>();
            if (mineScript && !mineScript.HasMine)
            {
                resourceManagement.SpendResourcesForMine(); // Deduct the resources
                mineScript.PlaceMine(minePrefab, activePlayer); // Place the mine and assign the owner
                selectedMetalDeposit = null; // Clear the selection
            }
            else
            {
                Debug.Log("A mine is already placed here or the metal deposit script is missing.");
            }
        }
        else
        {
            Debug.Log("Not enough resources to place a mine.");
        }
    }



}
