using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [HideInInspector]
    public GameObject prefab;

    [HideInInspector]
    public InventoryManager manager;

    private GameObject _previewInstance;
    private Image _border;
    private bool _isSelected;

    [Header("Preview settings")]
    [SerializeField]
    private float previewScale = 0.06f;

    private static readonly Color ColorDefault = new(0.15f, 0.15f, 0.15f, 1f);
    private static readonly Color ColorHover = new(0.35f, 0.35f, 0.35f, 1f);
    private static readonly Color ColorSelected = new(0.2f, 0.8f, 1f, 1f);

    public void Initialise(GameObject prefab, InventoryManager manager)
    {
        this.prefab = prefab;
        this.manager = manager;
        _border = GetComponent<Image>();

        CreatePreview();
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        if (_border != null)
            _border.color = selected ? ColorSelected : ColorDefault;
    }

    public void SetHighlight(bool highlighted)
    {
        if (_isSelected)
            return;
        if (_border != null)
            _border.color = highlighted ? ColorHover : ColorDefault;
    }

    public void SetPreviewVisible(bool visible)
    {
        if (_previewInstance != null)
            _previewInstance.SetActive(visible);
    }

    private void CreatePreview()
    {
        _previewInstance = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        _previewInstance.name = $"Preview_{prefab.name}";

        SetLayerRecursive(_previewInstance, 2);

        foreach (var col in _previewInstance.GetComponentsInChildren<Collider>())
            Destroy(col);
        foreach (var rb in _previewInstance.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);
        foreach (var mb in _previewInstance.GetComponentsInChildren<MonoBehaviour>())
        {
            if (mb != null)
                mb.enabled = false;
        }

        Bounds bounds = CalculateBounds(_previewInstance);
        float maxDim = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        if (maxDim < 0.001f)
            maxDim = 1f;

        float uniformScale = previewScale / maxDim;
        _previewInstance.transform.localScale = Vector3.one * uniformScale;

        Vector3 scaledOffset = (bounds.center - _previewInstance.transform.position) * uniformScale;
        _previewInstance.transform.localPosition = -scaledOffset;
        _previewInstance.transform.localPosition += Vector3.back * 0.01f;

        _previewInstance.SetActive(false);
    }

    private Bounds CalculateBounds(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one * 0.5f);

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);
        return b;
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursive(child.gameObject, layer);
    }

    private void Update()
    {
        if (_previewInstance != null && _previewInstance.activeSelf)
            _previewInstance.transform.Rotate(Vector3.forward, 30f * Time.deltaTime, Space.Self);
    }

    private void OnDestroy()
    {
        if (_previewInstance != null)
            Destroy(_previewInstance);
    }
}
