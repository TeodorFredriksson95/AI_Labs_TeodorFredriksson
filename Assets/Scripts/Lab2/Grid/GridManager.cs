using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour
{
    [Header(("Grid Settings"))]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1f;


    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Material walkableMaterial;
    [SerializeField] private Material wallMaterial;

    [Header("Special Nodes")]
    [SerializeField] public Node StartNode { get; private set; }
    [SerializeField] public Node TargetNode { get; private set; }

    private Node[,] nodes;
    private Dictionary<GameObject, Node> tileToNode = new();

    private InputAction clickAction;
    private InputAction rightClickAction;

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;

    private void Awake()
    {
        GenerateGrid();
    }

    private void OnEnable()
    {
        clickAction = new InputAction(
            name: "Click",
            type: InputActionType.Button,
            binding: "<Mouse/leftButton>"
            );
        clickAction.performed += OnClickPerformed;
        clickAction.Enable();

        rightClickAction = new InputAction(
            name: "RightClick",
            type: InputActionType.Button,
            binding: "<Mouse/rightButton>"
            );
        rightClickAction.performed += OnRightClickPerformed;
        rightClickAction.Enable();
    }

    private void OnDisable()
    {
        if (clickAction != null)
        {
            clickAction.performed -= OnClickPerformed;
            clickAction.Disable();
        }

        if (rightClickAction != null)
        {
            rightClickAction.performed += OnRightClickPerformed;
            rightClickAction.Disable();
        }
    }

    private void Update()
    {
        if (Mouse.current == null) Debug.Log("Mouse.current is NULL");
    }

    public IEnumerable<Node> AllNodes
    {
        get
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    yield return nodes[x, y];
                }
            }
        }
    }

    public void ResetNodes()
    {
        foreach (Node node in AllNodes)
        {
            node.gCost = float.PositiveInfinity;
            node.hCost = 0;
            node.parent = null;

            SetTileMaterial(node, walkableMaterial);
        }
    }

    private void GenerateGrid()
    {
        nodes = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = new Vector3(x * cellSize + 0.5f, 0f, y * cellSize + 0.5f);
                GameObject tileGO = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
                tileGO.name = $"Tile_{x}_{y}";

                Node node = new Node(x, y, true, tileGO);
                nodes[x, y] = node;

                tileToNode[tileGO] = node;

                SetTileMaterial(node, walkableMaterial);
            }
        }
    }



    public void OnRightClickPerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("OnRightClickPerformed");
        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clicked = hit.collider.gameObject;
            if (tileToNode.TryGetValue(clicked, out Node node))
            {
                TargetNode = node;
            }
        }
    }

    public void OnClickPerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("OnClickPerformed");
        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clicked = hit.collider.gameObject;
            if (tileToNode.TryGetValue(clicked, out Node node))
            {
                bool newWalkable = !node.walkable;
                SetWalkable(node, newWalkable);
                StartNode = node;
            }
        }

    }

    private void SetWalkable(Node node, bool walkable)
    {
        node.walkable = walkable;
        SetTileMaterial(node, walkable ? walkableMaterial : wallMaterial);
    }

    public void SetTileMaterial(Node node, Material mat)
    {
        MeshRenderer renderer = node.tile.GetComponent<MeshRenderer>();
        if (renderer != null && mat != null)
            renderer.material = mat;
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.RoundToInt(worldPos.z / cellSize);

        return GetNode(x, y);
    }

    public IEnumerable<Node> GetNeighbours(Node node, bool allowDiagonals = false)
    {
        int x = node.x;
        int y = node.y;

        // yield 4 neighbours (cardinal diretions)
        yield return GetNode(x + 1, y);
        yield return GetNode(x - 1, y);
        yield return GetNode(x, y + 1);
        yield return GetNode(x, y - 1);

        // Octagonal
        if (allowDiagonals)
        {
            yield return GetNode(x + 1, y + 1);
            yield return GetNode(x - 1, y + 1);
            yield return GetNode(x + 1, y - 1);
            yield return GetNode(x - 1, y - 1);

        }
    }

    public Node GetNode(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return null;

        return nodes[x, y];
    }


}
