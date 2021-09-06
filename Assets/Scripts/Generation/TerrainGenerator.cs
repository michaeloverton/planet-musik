using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class TerrainGenerator : MonoBehaviour
{
    public Material defaultMaterial;
    public int width = 20;
    public int height = 20;
    public int widthCuts = 5;
    public int heightCuts = 5;
    public float maxHeight = 30;
    public float minHeight = -30;
    public float maxHeightVariation = 4f;
    public LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        ProBuilderMesh ground = ShapeGenerator.GeneratePlane(PivotLocation.Center, width, height, widthCuts, heightCuts, Axis.Up);
        ground.SetMaterial(ground.faces, defaultMaterial);

        // Collection information about vertex to face associations.
        Dictionary<int, Face> vertexIndexToFace = new Dictionary<int, Face>();
        foreach(Face f in ground.faces) {
            foreach(int vertexIndex in f.distinctIndexes) {
                vertexIndexToFace[vertexIndex] = f;
            }
        }

        Dictionary<int, List<Face>> vertexToNeighboringFaces = new Dictionary<int, List<Face>>();
        foreach(SharedVertex sv in ground.sharedVertices) {
            List<Face> faces = new List<Face>();
            foreach(int vertexIndex in sv) {
                faces.Add(vertexIndexToFace[vertexIndex]);
            }
            // This means that all shared indexes will have the same set of neighboring faces.
            foreach(int vertexIndex in sv) {
                vertexToNeighboringFaces[vertexIndex] = faces;
            }
        }

        Vertex[] originalVertices = ground.GetVertices();
        Vector3[] randomizedVertices = new Vector3[originalVertices.Length];
        // Initialize randomizedVertices to be the same as original.
        for(int i=0;i<originalVertices.Length; i++) {
            randomizedVertices[i] = originalVertices[i].position;
        }

        // Each sharedVertex is the set of vertices at the same position. We change the positions of these sets of vertices.
        foreach(SharedVertex sv in ground.sharedVertices) {

            float lowestNearPoint = 0;
            float highestNearPoint = 0;
            List<Face> neighboringFaces = vertexToNeighboringFaces[sv[0]];
            foreach(Face f in neighboringFaces) {
                foreach(int vertexIndex in f.distinctIndexes) {
                    if(randomizedVertices[vertexIndex].y < lowestNearPoint) {
                        lowestNearPoint = randomizedVertices[vertexIndex].y;
                    }
                    if(randomizedVertices[vertexIndex].y > highestNearPoint) {
                        highestNearPoint = randomizedVertices[vertexIndex].y;
                    }
                }
            }

            Vector3 randomPositionChange = new Vector3(0, Random.Range(-maxHeightVariation + lowestNearPoint, maxHeightVariation + highestNearPoint), 0);
            foreach(int vertexIndex in sv) {
                randomizedVertices[vertexIndex] = originalVertices[vertexIndex].position + randomPositionChange ;
            }

        }

        ground.RebuildWithPositionsAndFaces(randomizedVertices, ground.faces);
        ground.Refresh();

        float high = 0;
        float low = 0;
        foreach(Vertex v in ground.GetVertices()) {
            if(v.position.y < low) {
                low = v.position.y;
            }
            if(v.position.y > high) {
                high = v.position.y;
            }
        }
        Debug.Log("high: " + high);
        Debug.Log("low: " + low);

        ground.gameObject.AddComponent<MeshCollider>();

        ground.gameObject.layer = groundLayer;
    }

    /*
    void Start()
    {
        ProBuilderMesh ground = ShapeGenerator.GeneratePlane(PivotLocation.Center, width, height, widthCuts, heightCuts, Axis.Up);
        ground.SetMaterial(ground.faces, defaultMaterial);

        Vertex[] originalVertices = ground.GetVertices();
        Vector3[] randomizedVertices = new Vector3[originalVertices.Length];

        // Each sharedVertex is the set of vertices at the same position. We change the positions of these sets of vertices.
        foreach(SharedVertex sv in ground.sharedVertices) {
            Vector3 randomPositionChange = new Vector3(0, Random.Range(-2f,2f), 0);
            foreach(int vertexIndex in sv) {
                randomizedVertices[vertexIndex] = originalVertices[vertexIndex].position + randomPositionChange;
            }
        }

        ground.RebuildWithPositionsAndFaces(randomizedVertices, ground.faces);
        ground.Refresh();

        ground.gameObject.AddComponent<MeshCollider>();
    }
    */
}
