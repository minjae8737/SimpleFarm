// Assets/Editor/TilemapBaker.cs
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapBaker
{
    [MenuItem("Tools/Tilemap/Export Tilemap to Sprite & Prefab")]
    public static void ExportSelectedTilemap()
    {
        var go = Selection.activeGameObject;
        if (!go)
        {
            EditorUtility.DisplayDialog("Tilemap Baker", "먼저 내보낼 Tilemap 오브젝트를 선택하세요.", "OK");
            return;
        }

        var tilemap = go.GetComponent<Tilemap>();
        var renderer = go.GetComponent<TilemapRenderer>();
        if (!tilemap || !renderer)
        {
            EditorUtility.DisplayDialog("Tilemap Baker", "선택된 오브젝트에 Tilemap / TilemapRenderer가 없습니다.", "OK");
            return;
        }

        // 실제 타일이 있는 영역만으로 경계 압축
        tilemap.CompressBounds();

        // 사용 중인 스프라이트에서 PPU 추정 (없으면 16 기본)
        int ppu = GuessPPU(tilemap, defaultPPU: 16);

        // 로컬/월드 경계 계산
        Bounds local = tilemap.localBounds;
        Vector3 worldMin = tilemap.transform.TransformPoint(local.min);
        Vector3 worldMax = tilemap.transform.TransformPoint(local.max);
        Vector3 worldSize = worldMax - worldMin;

        // 출력 해상도(픽셀) 계산: 월드 단위 * PPU
        int widthPx  = Mathf.Max(1, Mathf.RoundToInt(worldSize.x * ppu));
        int heightPx = Mathf.Max(1, Mathf.RoundToInt(worldSize.y * ppu));

        // 내보내기 폴더
        string exportDir = "Assets/Exports";
        if (!Directory.Exists(exportDir)) Directory.CreateDirectory(exportDir);

        // 파일 이름
        string baseName = go.name + "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string pngPath = $"{exportDir}/{baseName}.png";
        string prefabPath = $"{exportDir}/{baseName}.prefab";

        // 임시 카메라 생성 (이 타일맵만 렌더)
        var prevCameras = Camera.allCameras;
        foreach (var camera in prevCameras) camera.enabled = false;

        var camGO = new GameObject("TilemapBake_Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0); // 투명
        cam.allowHDR = false;

        // 카메라가 해당 타일맵만 보도록 같은 레이어만 렌더
        int originalLayer = tilemap.gameObject.layer;
        int isolatedLayer = 2; // Ignore Raycast(기본 프로젝트에 항상 존재)
        tilemap.gameObject.layer = isolatedLayer;
        cam.cullingMask = 1 << isolatedLayer;

        // 카메라 프레이밍 (정확히 경계에 맞춤)
        Vector3 worldCenter = (worldMin + worldMax) * 0.5f;
        cam.transform.position = new Vector3(worldCenter.x, worldCenter.y, -10f);
        cam.orthographicSize = worldSize.y * 0.5f;

        // 렌더 텍스처 만들고 렌더
        var rt = new RenderTexture(widthPx, heightPx, 24, RenderTextureFormat.ARGB32);
        rt.useMipMap = false;
        rt.antiAliasing = 1;
        cam.targetTexture = rt;

        // 타일맵 외 타일맵은 잠시 숨김 (같은 레이어 충돌 방지)
        bool prevEnabled = renderer.enabled;
        renderer.enabled = true;

        try
        {
            cam.Render();

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;

            var tex = new Texture2D(widthPx, heightPx, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, widthPx, heightPx), 0, 0);
            tex.Apply(false, false);

            RenderTexture.active = prev;
            cam.targetTexture = null;
            rt.Release();

            // PNG 저장
            byte[] png = tex.EncodeToPNG();
            File.WriteAllBytes(pngPath, png);
            AssetDatabase.ImportAsset(pngPath);

            // 임포터 설정 (스프라이트)
            var importer = (TextureImporter)AssetImporter.GetAtPath(pngPath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = ppu;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePivot = new Vector2(0f, 0f); // 피벗: 좌하단(그리드에 맞추기 쉬움)
            importer.spriteBorder = Vector4.zero;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            // 스프라이트 로드
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);

            // 프리팹 만들기 (SpriteRenderer)
            var prefabGO = new GameObject(baseName);
            var sr = prefabGO.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingLayerID = renderer.sortingLayerID;
            sr.sortingOrder = renderer.sortingOrder;

            // 프리팹의 월드 위치 = 타일맵 경계의 좌하단 (피벗을 좌하단으로 맞췄기 때문)
            prefabGO.transform.position = new Vector3(worldMin.x, worldMin.y, go.transform.position.z);

            // 필요시 기본 콜라이더 추가 (단순 사각이라면 BoxCollider2D가 깔끔)
            // var col = prefabGO.AddComponent<BoxCollider2D>();
            // col.size = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y);
            // col.offset = new Vector2(sprite.bounds.extents.x, sprite.bounds.extents.y);

            // 저장
            PrefabUtility.SaveAsPrefabAsset(prefabGO, prefabPath);
            Object.DestroyImmediate(prefabGO);

            EditorUtility.DisplayDialog("Tilemap Baker", $"Export 완료!\nPNG: {pngPath}\nPrefab: {prefabPath}", "OK");
        }
        finally
        {
            // 상태 복구
            tilemap.gameObject.layer = originalLayer;
            foreach (var camPrev in prevCameras) if (camPrev) camPrev.enabled = true;
            if (cam) Object.DestroyImmediate(cam.gameObject);
            renderer.enabled = prevEnabled;
        }
    }

    private static int GuessPPU(Tilemap tilemap, int defaultPPU)
    {
        BoundsInt b = tilemap.cellBounds;
        foreach (var pos in b.allPositionsWithin)
        {
            var sp = tilemap.GetSprite(pos);
            if (sp != null) return Mathf.RoundToInt(sp.pixelsPerUnit);
        }
        return defaultPPU;
    }
}
#endif
