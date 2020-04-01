using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataVisualizer : MonoBehaviour {
    public Material PointMaterial;//PointMaterial assigned as a Material type
    public Gradient Colors;//Colors is of type Gradient
    public GameObject Earth;//Earth is type GameObject..
    public GameObject PointPrefab;//PointPrefab is type GameObject
    public float ValueScaleMultiplier = 1;//setting ValueScaleMultiplier to 1
    GameObject[] seriesObjects;//creating an array in GameObject called seriesObjects
    int currentSeries = 0;//setting currentSeries to 0 
    public void CreateMeshes(SeriesData[] allSeries)//method for point array
    {
        seriesObjects = new GameObject[allSeries.Length];//getting number of array objects and putting into seriesObjects
        GameObject p = Instantiate<GameObject>(PointPrefab);//make instances of PointPrefab and put into p
        Vector3[] verts = p.GetComponent<MeshFilter>().mesh.vertices;//getting a copy of the vertex positions of p and put into vector3 array
        int[] indices = p.GetComponent<MeshFilter>().mesh.triangles;//getting a copy of the indicies positions of p and put into integer array

        List<Vector3> meshVertices = new List<Vector3>(65000);
        List<int> meshIndices = new List<int>(117000);
        List<Color> meshColors = new List<Color>(65000);


        //scatter loop
        for (int i = 0; i < allSeries.Length; i++)//starting loop
        {
            GameObject seriesObj = new GameObject(allSeries[i].Name);//making an array inside of GameObject called allSeries
            seriesObj.transform.parent = Earth.transform;//taking Earth's transform and putting it into parent transform of seriesObj
            seriesObjects[i] = seriesObj;//putting the seriesObj array into seriesObjects
            SeriesData seriesData = allSeries[i];
            for (int j = 0; j < seriesData.Data.Length; j+=3)//loop for lat, long, value, and color vertex positions.
            {
                float lat = seriesData.Data[j];//storing latitute 
                float lng = seriesData.Data[j + 1];//storing longitute
                float value = seriesData.Data[j + 2];//storing value
                AppendPointVertices(p, verts, indices, lng, lat, value, meshVertices, meshIndices, meshColors);//method that adds point vertices
                if (meshVertices.Count + verts.Length > 65000)//if meshVertices count and vert length is greater than 6500, execute
                {
                    CreateObject(meshVertices, meshIndices, meshColors, seriesObj);//builds the point cloud of vertex points
                    meshVertices.Clear();//remove meshVertices from list...
                    meshIndices.Clear();
                    meshColors.Clear();
                }
            }
            CreateObject(meshVertices, meshIndices, meshColors, seriesObj);//builds the point cloud of vertex points
            meshVertices.Clear();
            meshIndices.Clear();
            meshColors.Clear();
            seriesObjects[i].SetActive(false); 
        }


        seriesObjects[currentSeries].SetActive(true);
        Destroy(p);
    }
    private void AppendPointVertices(GameObject p, Vector3[] verts, int[] indices, float lng,float lat,float value, List<Vector3> meshVertices,
    List<int> meshIndices,
    List<Color> meshColors)
    {
        Color valueColor = Colors.Evaluate(value);
        Vector3 pos;
        pos.x = 0.5f * Mathf.Cos((lng) * Mathf.Deg2Rad) * Mathf.Cos(lat * Mathf.Deg2Rad);
        pos.y = 0.5f * Mathf.Sin(lat * Mathf.Deg2Rad);
        pos.z = 0.5f * Mathf.Sin((lng) * Mathf.Deg2Rad) * Mathf.Cos(lat * Mathf.Deg2Rad);
        p.transform.parent = Earth.transform;
        p.transform.position = pos;
        p.transform.localScale = new Vector3(1, 1, Mathf.Max(0.001f, value * ValueScaleMultiplier));
        p.transform.LookAt(pos * 2);

        int prevVertCount = meshVertices.Count;

        for (int k = 0; k < verts.Length; k++)
        {
            meshVertices.Add(p.transform.TransformPoint(verts[k]));
            meshColors.Add(valueColor);
        }
        for (int k = 0; k < indices.Length; k++)
        {
            meshIndices.Add(prevVertCount + indices[k]);
        }
    }
    private void CreateObject(List<Vector3> meshvertices, List<int> meshindecies, List<Color> meshColors, GameObject seriesObj)//copies prefab to each vertex point and reads the data for each point
    {
        Mesh mesh = new Mesh();//creating the box for the new Mesh
        mesh.vertices = meshvertices.ToArray();//copying array into mesh
        mesh.triangles = meshindecies.ToArray();
        mesh.colors = meshColors.ToArray();
        GameObject obj = new GameObject();//creating a new game object
        obj.transform.parent = Earth.transform;//Earth transform is now the parent transform of new game object
        obj.AddComponent<MeshFilter>().mesh = mesh;
        obj.AddComponent<MeshRenderer>().material = PointMaterial;
        obj.transform.parent = seriesObj.transform;
    }
    public void ActivateSeries(int seriesIndex)
    {
        if (seriesIndex >= 0 && seriesIndex < seriesObjects.Length)
        {
            seriesObjects[currentSeries].SetActive(false);
            currentSeries = seriesIndex;
            seriesObjects[currentSeries].SetActive(true);

        }
    }
}
[System.Serializable]
public class SeriesData
{
    public string Name;
    public float[] Data;
}
