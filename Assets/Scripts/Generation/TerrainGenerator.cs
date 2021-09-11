using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public int widthCuts = 5;
    public int heightCuts = 5;
    public float maxHeight = 30;
    public float minHeight = -30;
    public float maxHeightVariation = 4f;
    public Material groundMaterial;
    public LayerMask groundLayer;

    // Water
    public float waterHeight = 2.0f;
    public Material waterMaterial;

    // Plants
    public int plantCount = 2000;
    public List<GameObject> plants;

    // Start is called before the first frame update
    void Start()
    {
        ProBuilderMesh ground = ShapeGenerator.GeneratePlane(PivotLocation.Center, width, height, widthCuts, heightCuts, Axis.Up);
        ground.SetMaterial(ground.faces, groundMaterial);

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

        // Create water.
        GameObject waterPlane  = GameObject.CreatePrimitive(PrimitiveType.Plane);
        waterPlane.transform.position = new Vector3(0,low + waterHeight,0);
        waterPlane.transform.localScale = new Vector3(200, 1, 200);
        MeshRenderer waterRend = waterPlane.GetComponent<MeshRenderer>();
        waterRend.material = waterMaterial;

        Plants();
    }

    void Plants() {
        // for(int i=-width; i<width; i=i+5) {
        //     for(int j=-height; j<height; j=j+5) {
        //         // Ignore activated layer.
        //         int layerMask = 1 << 9;
        //         layerMask = ~layerMask;
        //         RaycastHit hit;
        //         Vector3 raycastOrigin = new Vector3(i,50,j);
        //         if(Physics.Raycast(raycastOrigin, Vector3.down, out hit, 200, layerMask)) {
        //             // if(Physics.Raycast(raycastOrigin, Vector3.down, out hit, 200)) {
        //             // Debug.Log("planting at position: " + hit.point);
        //             // Debug.DrawRay(raycastOrigin, Vector3.down * hit.distance, Color.yellow);
                    
        //             GameObject planted = Instantiate(plant, hit.point, Quaternion.identity);
        //             planted.name = "planted" + i.ToString() + j.ToString();
        //             Debug.Log(planted.name + " hit: " + hit.collider.gameObject.name);
        //         }
        //     }
        // }

        foreach(GameObject plant in plants) {
            for(int i=0; i<plantCount; i++) {
                float x = Random.Range(-width, width);
                float z = Random.Range(-height, height);

                int layerMask = 1 << 9;
                layerMask = ~layerMask;
                RaycastHit hit;
                Vector3 raycastOrigin = new Vector3(x,50,z);
                if(Physics.Raycast(raycastOrigin, Vector3.down, out hit, 200, layerMask)) {
                    GameObject planted = Instantiate(plant, hit.point, Quaternion.identity);
                    // planted.name = "planted" + i.ToString() + j.ToString();
                    // Debug.Log(planted.name + " hit: " + hit.collider.gameObject.name);
                }
            }
        }
        // LightingSettings.
        UnityEditor.Lightmapping.Bake();
        // Lightmapping.Bake();
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
