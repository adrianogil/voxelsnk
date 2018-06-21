using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SphereCubeGeneration : MonoBehaviour {

	public void Generate()
    {
        MeshFilter filter = gameObject.GetComponent< MeshFilter >();

        MeshBuilder meshBuilder = new MeshBuilder();

        // Face 0 - Front
        meshBuilder.Vertices.Add(new Vector3(0,0,0f));
        meshBuilder.Vertices.Add(new Vector3(1f,0f,0f));
        meshBuilder.Vertices.Add(new Vector3(0f,1f,0f));
        meshBuilder.Vertices.Add(new Vector3(1f,1f,0f));
        meshBuilder.AddTriangle(0,1,2);
        meshBuilder.AddTriangle(1,3,2);

        // Face 1 - Up
        meshBuilder.Vertices.Add(new Vector3(0.5f,1f,0.5f));
        meshBuilder.Vertices.Add(new Vector3(0f,1f,1f));
        meshBuilder.Vertices.Add(new Vector3(1f,1f,1f));
        meshBuilder.AddTriangle(2,3,4);
        meshBuilder.AddTriangle(5,2,4);
        meshBuilder.AddTriangle(4,3,6);
        meshBuilder.AddTriangle(4,6,5);

        // Face 2 - Left
        meshBuilder.Vertices.Add(new Vector3(0f,0f,1f));
        meshBuilder.AddTriangle(0,2,5);
        meshBuilder.AddTriangle(7,0,5);

        // Face 3 - Back
        meshBuilder.Vertices.Add(new Vector3(1f,0f,1f));
        meshBuilder.AddTriangle(5,8,7);
        meshBuilder.AddTriangle(6,8,5);

        // Face 4 - Right
        meshBuilder.AddTriangle(3,1,6);
        meshBuilder.AddTriangle(6,1,8);

        // Face 5 - Down
        meshBuilder.Vertices.Add(new Vector3(0.5f,0f,0.5f));
        meshBuilder.AddTriangle(9,1,0);
        meshBuilder.AddTriangle(9,8,1);
        meshBuilder.AddTriangle(9,7,8);
        meshBuilder.AddTriangle(9,0,7);

        MeshProcessing.SubdivideTrianglesIn4(meshBuilder);
        MeshProcessing.SubdivideTrianglesIn4(meshBuilder);
        MeshProcessing.SubdivideTrianglesIn4(meshBuilder);
        MeshProcessing.SubdivideTrianglesIn4(meshBuilder);
        // MeshProcessing.SubdivideTriangles(meshBuilder);
        // MeshProcessing.SubdivideTriangles(meshBuilder);
        // MeshProcessing.SubdivideTriangles(meshBuilder);
        // MeshProcessing.SubdivideTriangles(meshBuilder);

        for (int i = 0; i < meshBuilder.Vertices.Count; i++)
        {
            Vector3 v = (meshBuilder.Vertices[i] - 0.5f * Vector3.one).normalized;

            Vector2 longlat = new Vector2(Mathf.Atan2(v.x, v.z) + Mathf.PI, Mathf.Acos(v.y));
            Vector2 uv = new Vector2(longlat.x / (2f * Mathf.PI), longlat.y / Mathf.PI);
            uv.y = 1f - uv.y;
            uv.x = 1f - uv.x;

            meshBuilder.UVs.Add(uv);
        }

        int[] triangles = meshBuilder.GetTriangles();

        int v1, v2, v3, t1, t2, t3;

        float uv1, uv2, uv3;

        for (int i = 0; i < triangles.Length/3; i++)
        {
            t1 = 3*i;
            t2 = 3*i+1;
            t3 = 3*i+2;

            v1 = triangles[t1];
            v2 = triangles[t2];
            v3 = triangles[t3];

            uv1 = meshBuilder.UVs[v1].x;
            uv2 = meshBuilder.UVs[v2].x;
            uv3 = meshBuilder.UVs[v3].x;

            if (!VerifyWrongTriangleUV(uv1, uv2, uv3, v2, v3, t2, t3, meshBuilder))
            {
                if (!VerifyWrongTriangleUV(uv2, uv1, uv3, v1, v3, t1, t3, meshBuilder))
                {
                    VerifyWrongTriangleUV(uv3, uv2, uv1, v2, v1, t2, t1, meshBuilder);
                }    
            }

        }

        Mesh mesh = meshBuilder.CreateMesh();
        mesh.RecalculateBounds();
        filter.mesh = mesh;
    }

    bool VerifyWrongTriangleUV(float uv1, float uv2, float uv3,
            int v2, int v3, int t2, int t3, MeshBuilder meshBuilder)
    {
        bool fixedUV = false;

        if (uv1 > 0.8 && (uv2 < 0.2 || uv3 < 0.2))
        {
            if (uv2 < 0.2)
            {
                FixTriangleUV(meshBuilder, v2, t2);
                fixedUV = true;
            }
            if (uv3 < 0.2)
            {
                FixTriangleUV(meshBuilder, v3, t3);
                fixedUV = true;
            }
        }

        return fixedUV;
    }

    void FixTriangleUV(MeshBuilder meshBuilder, int v, int t)
    {
        int vIndex = meshBuilder.Vertices.Count;
        meshBuilder.Vertices.Add(meshBuilder.Vertices[v]);

        Vector2 newUv = meshBuilder.UVs[v];
        newUv.x += 1.0f;

        meshBuilder.UVs.Add(newUv);

        meshBuilder.ReplaceTriangleIndex(t, vIndex);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SphereCubeGeneration))]
public class SphereCubeGenerationEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SphereCubeGeneration editorObj = target as SphereCubeGeneration;

        if (editorObj == null) return;

        if (GUILayout.Button("Generate"))
        {
            editorObj.Generate();
        }
    }

}
#endif
