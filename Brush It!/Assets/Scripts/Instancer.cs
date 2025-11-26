using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Instancer : MonoBehaviour
{
    [Header("Surface")]
    public Mesh surfaceMesh;
    public Transform surfaceTransform;

    [Header("Instances")]
    public Mesh instanceMesh;
    public Material instanceMaterial;

    [Header("Placement Settings")]
    public int instancesPerTriangle = 1;
    public float grassScale = 0.25f;
    public float randomScaleVariation = 0.1f;

    public float randomRotation = 180f;
    public float normalAlignment = 1f;
    public float offsetFromSurface = 0.01f;

    [Header("Regeneration")]
    public bool autoUpdate = false;
    public int seed = 1234;
    
    [Header("Paint Mask")]
    public Texture2D maskTexture;
    public bool useMask = false;

    private const int batchSize = 1023;
    private List<Matrix4x4[]> batches = new List<Matrix4x4[]>();

    void OnValidate()
    {
        if (autoUpdate)
        {
            Generate();
        }
    }

    void Update()
    {
        if (instanceMesh != null && instanceMaterial != null)
        {
            foreach (var b in batches)
                Graphics.DrawMeshInstanced(instanceMesh, 0, instanceMaterial, b);
        }
    }

    public void Generate()
    {
        if (!surfaceMesh || !surfaceTransform)
        {
            Debug.LogWarning("GrassInstancer: falta asignar mesh o transform del suelo.");
            return;
        }

        if (useMask && maskTexture == null)
        {
            Debug.LogWarning("useMask está activado pero maskTexture es null.");
        }
        
    #if UNITY_EDITOR
        if (useMask && maskTexture != null && !maskTexture.isReadable)
        {
            Debug.LogWarning("La maskTexture debe tener Read/Write Enabled en sus import settings.");
        }
    #endif

        Random.InitState(seed);
        batches.Clear();

        Vector3[] verts = surfaceMesh.vertices;
        int[] tris = surfaceMesh.triangles;
        Vector3[] normals = surfaceMesh.normals;
        Vector2[] uvs = surfaceMesh.uv;

        bool hasUVs = (uvs != null && uvs.Length == verts.Length && uvs.Length > 0);
        
        Vector3 localMin = Vector3.zero;
        Vector3 localSize = Vector3.one;
        if (!hasUVs)
        {
            Bounds b = new Bounds(verts[0], Vector3.zero);
            for (int i = 1; i < verts.Length; i++) b.Encapsulate(verts[i]);
            localMin = b.min;
            localSize = b.size;
           
            if (localSize.x <= 0.0001f) localSize.x = 1f;
            if (localSize.z <= 0.0001f) localSize.z = 1f;
        }

        int triCount = tris.Length / 3;
        List<Matrix4x4> buffer = new List<Matrix4x4>();

        for (int t = 0; t < triCount; t++)
        {
            int i0 = tris[t * 3 + 0];
            int i1 = tris[t * 3 + 1];
            int i2 = tris[t * 3 + 2];

            Vector3 v0_local = verts[i0];
            Vector3 v1_local = verts[i1];
            Vector3 v2_local = verts[i2];
            
            Vector3 v0 = surfaceTransform.TransformPoint(v0_local);
            Vector3 v1 = surfaceTransform.TransformPoint(v1_local);
            Vector3 v2 = surfaceTransform.TransformPoint(v2_local);

            Vector3 n0 = (normals != null && normals.Length == verts.Length) ? surfaceTransform.TransformDirection(normals[i0]) : Vector3.up;
            Vector3 n1 = (normals != null && normals.Length == verts.Length) ? surfaceTransform.TransformDirection(normals[i1]) : Vector3.up;
            Vector3 n2 = (normals != null && normals.Length == verts.Length) ? surfaceTransform.TransformDirection(normals[i2]) : Vector3.up;

            for (int k = 0; k < instancesPerTriangle; k++)
            {
                float r1 = Mathf.Sqrt(Random.value);
                float r2 = Random.value;
                
                Vector3 pos = v0 * (1 - r1) + v1 * (r1 * (1 - r2)) + v2 * (r1 * r2);
                
                Vector3 normal = (n0 * (1 - r1) + n1 * (r1 * (1 - r2)) + n2 * (r1 * r2)).normalized;

                pos += normal * offsetFromSurface;
                
                if (useMask && maskTexture != null)
                {
                    Vector2 uvSample;

                    if (hasUVs)
                    {
                        Vector2 uv0 = uvs[i0];
                        Vector2 uv1 = uvs[i1];
                        Vector2 uv2 = uvs[i2];

                        uvSample = uv0 * (1 - r1) + uv1 * (r1 * (1 - r2)) + uv2 * (r1 * r2);
                    }
                    else
                    {
                        Vector3 localPos =
                            v0_local * (1 - r1) + v1_local * (r1 * (1 - r2)) + v2_local * (r1 * r2);

                        float u = (localPos.x - localMin.x) / localSize.x;
                        float v = (localPos.z - localMin.z) / localSize.z;
                        uvSample = new Vector2(u, v);
                    }
                    
                    uvSample.x = Mathf.Repeat(uvSample.x, 1f);
                    uvSample.y = Mathf.Repeat(uvSample.y, 1f);

                    Color maskCol = maskTexture.GetPixelBilinear(uvSample.x, uvSample.y);
                    
                    if (maskCol.r < 0.5f)
                        continue;
                }
                
                Quaternion rot = Quaternion.identity;
                if (normalAlignment > 0f)
                {
                    rot = Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation(Vector3.up, normal), normalAlignment);
                }
                if (randomRotation > 0f)
                    rot *= Quaternion.Euler(0f, Random.Range(-randomRotation, randomRotation), 0f);

                float finalScale = grassScale + Random.Range(-randomScaleVariation, randomScaleVariation);

                Matrix4x4 m = Matrix4x4.TRS(pos, rot, new Vector3(finalScale, finalScale, finalScale));
                buffer.Add(m);

                if (buffer.Count >= batchSize)
                {
                    batches.Add(buffer.ToArray());
                    buffer.Clear();
                }
            }
        }

        if (buffer.Count > 0)
            batches.Add(buffer.ToArray());

        Debug.Log($"GrassInstancer: Generadas {batches.Count} batches (instancias aproximadas: {batches.Count * batchSize}).");
    }

}