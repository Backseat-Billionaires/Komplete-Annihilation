using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Map map;
    public GameObject commanderPrefab;
    public GameObject playerPrefab;
    public GameObject minePrefab; // Assign in Inspector
    private GameObject selectedMetalDeposit;

    private Player[] players = null;
    private Player activePlayer;
    private bool isInitialized;
    private List<IGameSelectable> selectedObjects = new List<IGameSelectable>();

    void Start()
    {
        Initialize(1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            var command = new Command(CommandType.Stop);
            activePlayer.SendCommandToSelectedUnits(command);
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        if (Input.GetKeyDown(KeyCode.M) && selectedMetalDeposit != null)
        {
            TryPlaceMine();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectAll();
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

    private void HandleClick()
    {
        var obj = GetObjectAtCursor();
        if (obj != null)
        {
            SelectObject(obj);
            // Update selectedMetalDeposit if a metal deposit is clicked
            if (obj.GetComponent<MetalDeposit>() != null)
            {
                selectedMetalDeposit = obj;
            }
        }
    }

    public void SelectObject(GameObject obj)
    {
        IGameSelectable selectable = obj.GetComponent<IGameSelectable>();
        if (selectable != null)
        {
            if (selectable.IsSelected())
            {
                selectable.Deselect();
                selectedObjects.Remove(selectable);
            }
            else
            {
                selectable.Select();
                selectedObjects.Add(selectable);
            }
        }

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

    public void DeselectAll()
    {
        foreach (var selectable in selectedObjects)
        {
            selectable.Deselect();
        }
        selectedObjects.Clear();
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

    public void AddToSelectedObjects(IGameSelectable selectable)
    {
        if (!selectedObjects.Contains(selectable))
        {
            selectedObjects.Add(selectable);
        }
    }

    public void RemoveFromSelectedObjects(IGameSelectable selectable)
    {
        selectedObjects.Remove(selectable);
    }

    private GameObject GetObjectAtCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }

        return null;
    }
}
