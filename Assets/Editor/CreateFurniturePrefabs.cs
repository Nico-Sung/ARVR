using UnityEditor;
using UnityEngine;

public static class CreateFurniturePrefabs
{
    [MenuItem("Tools/Create Furniture Prefabs")]
    public static void CreateAll()
    {
        CreateTable();
        CreateChair();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Furniture] Table and Chair prefabs created in Assets/Prefabs/");
    }

    static void CreateTable()
    {
        var root = new GameObject("Table");

        // Tabletop
        var top = GameObject.CreatePrimitive(PrimitiveType.Cube);
        top.name = "Tabletop";
        top.transform.SetParent(root.transform);
        top.transform.localPosition = new Vector3(0, 0.75f, 0);
        top.transform.localScale = new Vector3(1.2f, 0.05f, 0.7f);

        // 4 legs
        Vector3[] legOffsets = {
            new Vector3( 0.55f, 0.375f,  0.3f),
            new Vector3(-0.55f, 0.375f,  0.3f),
            new Vector3( 0.55f, 0.375f, -0.3f),
            new Vector3(-0.55f, 0.375f, -0.3f),
        };
        for (int i = 0; i < 4; i++)
        {
            var leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leg.name = $"Leg{i + 1}";
            leg.transform.SetParent(root.transform);
            leg.transform.localPosition = legOffsets[i];
            leg.transform.localScale = new Vector3(0.06f, 0.75f, 0.06f);
        }

        SavePrefab(root, "Assets/Prefabs/Table.prefab");
    }

    static void CreateChair()
    {
        var root = new GameObject("Chair");

        // Seat
        var seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        seat.name = "Seat";
        seat.transform.SetParent(root.transform);
        seat.transform.localPosition = new Vector3(0, 0.45f, 0);
        seat.transform.localScale = new Vector3(0.5f, 0.05f, 0.5f);

        // Backrest
        var back = GameObject.CreatePrimitive(PrimitiveType.Cube);
        back.name = "Backrest";
        back.transform.SetParent(root.transform);
        back.transform.localPosition = new Vector3(0, 0.7f, -0.225f);
        back.transform.localScale = new Vector3(0.5f, 0.5f, 0.05f);

        // 4 legs
        Vector3[] legOffsets = {
            new Vector3( 0.22f, 0.225f,  0.22f),
            new Vector3(-0.22f, 0.225f,  0.22f),
            new Vector3( 0.22f, 0.225f, -0.22f),
            new Vector3(-0.22f, 0.225f, -0.22f),
        };
        for (int i = 0; i < 4; i++)
        {
            var leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leg.name = $"Leg{i + 1}";
            leg.transform.SetParent(root.transform);
            leg.transform.localPosition = legOffsets[i];
            leg.transform.localScale = new Vector3(0.05f, 0.45f, 0.05f);
        }

        SavePrefab(root, "Assets/Prefabs/Chair.prefab");
    }

    static void SavePrefab(GameObject go, string path)
    {
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
    }
}
