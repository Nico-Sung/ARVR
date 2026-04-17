using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class InventoryManager : MonoBehaviour
{
    [Header("Prefab list (edit in Inspector)")]
    [SerializeField]
    private List<GameObject> prefabs = new();

    [Header("UI References")]
    [SerializeField]
    private Transform slotContainer;

    [SerializeField]
    private Canvas inventoryCanvas;

    [Header("Slot appearance")]
    [SerializeField]
    private float slotSize = 120f;

    [SerializeField]
    private float slotSpacing = 10f;

    [Header("Spawning")]
    [SerializeField]
    private XRBaseInteractor rightInteractor;

    [SerializeField]
    private float maxSpawnDistance = 20f;

    [Header("Audio")]
    [SerializeField]
    private AudioClip placeSound;

    [Header("Input")]
    [SerializeField]
    private InputActionReference toggleInventoryAction;

    [SerializeField]
    private InputActionReference selectAction;

    [SerializeField]
    private InputActionReference spawnAction;

    private readonly List<InventorySlot> _slots = new();
    private InventorySlot _selectedSlot;
    private InventorySlot _hoveredSlot;
    private GameObject _ghostPreview;
    private bool _inventoryOpen;
    private AudioSource _audioSource;

    private void OnEnable()
    {
        EnableAction(toggleInventoryAction, OnToggleInventory);
        EnableAction(selectAction, OnSelect);
        EnableAction(spawnAction, OnSpawn);
    }

    private void OnDisable()
    {
        DisableAction(toggleInventoryAction, OnToggleInventory);
        DisableAction(selectAction, OnSelect);
        DisableAction(spawnAction, OnSpawn);
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        _audioSource.spatialBlend = 0f;
        _audioSource.playOnAwake = false;

        BuildSlots();

        if (inventoryCanvas != null)
        {
            inventoryCanvas.gameObject.SetActive(false);
            _inventoryOpen = false;
        }
    }

    private void Update()
    {
        if (_inventoryOpen)
            UpdateSlotHover();
        else
            UpdateGhostPreview();
    }

    private void BuildSlots()
    {
        foreach (var s in _slots)
            if (s != null)
                Destroy(s.gameObject);
        _slots.Clear();

        if (slotContainer == null)
        {
            Debug.LogWarning("[Inventory] slotContainer is not assigned!");
            return;
        }

        foreach (var prefab in prefabs)
        {
            if (prefab == null)
                continue;

            var slotGo = new GameObject($"Slot_{prefab.name}", typeof(RectTransform));
            slotGo.transform.SetParent(slotContainer, false);

            var rt = slotGo.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(slotSize, slotSize);

            var bg = slotGo.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            bg.raycastTarget = false;

            var col = slotGo.AddComponent<BoxCollider>();
            col.size = new Vector3(slotSize, slotSize, 1f);
            col.center = Vector3.zero;

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(slotGo.transform, false);
            var labelRt = labelGo.GetComponent<RectTransform>();
            labelRt.anchorMin = new Vector2(0, 0);
            labelRt.anchorMax = new Vector2(1, 0.22f);
            labelRt.offsetMin = Vector2.zero;
            labelRt.offsetMax = Vector2.zero;

            var labelBg = labelGo.AddComponent<Image>();
            labelBg.color = new Color(0, 0, 0, 0.6f);
            labelBg.raycastTarget = false;

            var textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(labelGo.transform, false);
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = new Vector2(4, 0);
            textRt.offsetMax = new Vector2(-4, 0);

            var tmp = textGo.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = prefab.name;
            tmp.fontSize = 12;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 8;
            tmp.fontSizeMax = 14;
            tmp.raycastTarget = false;

            var slot = slotGo.AddComponent<InventorySlot>();
            slot.Initialise(prefab, this);
            _slots.Add(slot);
        }

        var grid = slotContainer.GetComponent<GridLayoutGroup>();
        if (grid == null)
            grid = slotContainer.gameObject.AddComponent<GridLayoutGroup>();

        grid.cellSize = new Vector2(slotSize, slotSize);
        grid.spacing = new Vector2(slotSpacing, slotSpacing);
        grid.padding = new RectOffset(10, 10, 10, 10);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 4;
    }

    private void UpdateSlotHover()
    {
        if (rightInteractor == null)
            return;

        Transform rayOrigin = rightInteractor.GetAttachTransform(null);
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        InventorySlot hitSlot = null;

        if (Physics.Raycast(ray, out RaycastHit hit, maxSpawnDistance))
        {
            hitSlot = hit.collider.GetComponent<InventorySlot>();
        }

        if (hitSlot != _hoveredSlot)
        {
            if (_hoveredSlot != null && _hoveredSlot != _selectedSlot)
                _hoveredSlot.SetHighlight(false);

            _hoveredSlot = hitSlot;

            if (_hoveredSlot != null && _hoveredSlot != _selectedSlot)
                _hoveredSlot.SetHighlight(true);
        }
    }

    private void OnSelect(InputAction.CallbackContext ctx)
    {
        if (!_inventoryOpen)
            return;
        if (_hoveredSlot == null)
            return;

        SelectSlot(_hoveredSlot);
    }

    public void SelectSlot(InventorySlot slot)
    {
        if (_selectedSlot == slot)
        {
            _selectedSlot.SetSelected(false);
            _selectedSlot = null;
            if (_ghostPreview != null)
                Destroy(_ghostPreview);
            _ghostPreview = null;
            Debug.Log("[Inventory] Deselected");
            return;
        }

        if (_selectedSlot != null)
            _selectedSlot.SetSelected(false);

        _selectedSlot = slot;
        _selectedSlot.SetSelected(true);

        Debug.Log($"[Inventory] Selected: {slot.prefab.name}");
        CreateGhostPreview();
    }

    private void CreateGhostPreview()
    {
        if (_ghostPreview != null)
            Destroy(_ghostPreview);
        if (_selectedSlot == null)
            return;

        _ghostPreview = Instantiate(_selectedSlot.prefab);
        _ghostPreview.name = "GhostPreview";

        foreach (var col in _ghostPreview.GetComponentsInChildren<Collider>())
            Destroy(col);
        foreach (var rb in _ghostPreview.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);

        foreach (var rend in _ghostPreview.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in rend.materials)
            {
                mat.SetFloat("_Surface", 1);
                mat.SetFloat("_Blend", 0);
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = 3000;
                Color c = mat.color;
                c.a = 0.4f;
                mat.color = c;
            }
        }

        _ghostPreview.SetActive(false);
    }

    private void UpdateGhostPreview()
    {
        if (_ghostPreview == null || _selectedSlot == null)
            return;

        if (TryGetSpawnPoint(out Vector3 point, out Vector3 normal))
        {
            _ghostPreview.SetActive(true);
            _ghostPreview.transform.position = point;
            _ghostPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
        }
        else
        {
            _ghostPreview.SetActive(false);
        }
    }

    private void OnSpawn(InputAction.CallbackContext ctx)
    {
        if (_selectedSlot == null || _inventoryOpen)
            return;

        if (TryGetSpawnPoint(out Vector3 point, out Vector3 normal))
        {
            var spawned = Instantiate(
                _selectedSlot.prefab,
                point,
                Quaternion.FromToRotation(Vector3.up, normal)
            );
            spawned.name = _selectedSlot.prefab.name;

            if (spawned.GetComponent<GrabbableObject>() == null)
                spawned.AddComponent<GrabbableObject>();

            if (placeSound != null)
                _audioSource.PlayOneShot(placeSound);

            Debug.Log($"[Inventory] Spawned {spawned.name} at {point}");
        }
    }

    private bool TryGetSpawnPoint(out Vector3 point, out Vector3 normal)
    {
        point = Vector3.zero;
        normal = Vector3.up;
        if (rightInteractor == null)
            return false;

        Transform rayOrigin = rightInteractor.GetAttachTransform(null);
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxSpawnDistance))
        {
            if (hit.collider.GetComponent<InventorySlot>() != null)
                return false;
            point = hit.point;
            normal = hit.normal;
            return true;
        }
        return false;
    }

    private void OnToggleInventory(InputAction.CallbackContext ctx)
    {
        _inventoryOpen = !_inventoryOpen;

        if (inventoryCanvas != null)
            inventoryCanvas.gameObject.SetActive(_inventoryOpen);

        foreach (var slot in _slots)
            slot.SetPreviewVisible(_inventoryOpen);

        if (!_inventoryOpen && _hoveredSlot != null)
        {
            if (_hoveredSlot != _selectedSlot)
                _hoveredSlot.SetHighlight(false);
            _hoveredSlot = null;
        }

        Debug.Log($"[Inventory] {(_inventoryOpen ? "Opened" : "Closed")}");
    }

    private void EnableAction(
        InputActionReference actionRef,
        System.Action<InputAction.CallbackContext> handler
    )
    {
        if (actionRef != null && actionRef.action != null)
        {
            actionRef.action.Enable();
            actionRef.action.performed += handler;
        }
    }

    private void DisableAction(
        InputActionReference actionRef,
        System.Action<InputAction.CallbackContext> handler
    )
    {
        if (actionRef != null && actionRef.action != null)
            actionRef.action.performed -= handler;
    }
}
