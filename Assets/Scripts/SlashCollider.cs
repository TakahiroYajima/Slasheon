using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashCollider : MonoBehaviour {

    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;
    Mesh mesh;

    private List<Vector3> vertices = new List<Vector3>();
    List<int> triangles;

    public delegate void OnColliderEnterCallback(Collider collider);
    public OnColliderEnterCallback colliderCallback;

    // Use this for initialization
    void Start () {
        vertices.AddRange(new List<Vector3>() {
            new Vector3(0,0,0),
            new Vector3(3f,0f,10f),
            new Vector3(-3f,0f,10f) }
            );

        triangles = new List<int>();
        //triangles[0] = 0;
        //triangles[1] = 1;
        //triangles[2] = 2;
        mesh = new Mesh();
    }

    public void SetCollisionEnterCallback(OnColliderEnterCallback callback)
    {
        colliderCallback = callback;
    }
	/// <summary>
    /// 頂点座標をリストで追加
    /// </summary>
    /// <param name="point"></param>
	public void SetPointsList(List<Vector3> point)
    {
        vertices = point;
    }
    public void SetPoint(Vector3 point)
    {
        vertices.Add(point);
    }
    public void MakeTriangles(int size)
    {
        triangles.Clear();
        for (int i = 0; i < size; i++)
        {
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(i + 2);
            //if (triangles.Count < size)
            //{
            //    triangles.Add(i);

            //}
            //else
            //{
            //    triangles[i] = i;
            //}
        }
    }

    public void CreateCollider()
    {
        //Mesh mesh = new Mesh();
        if(vertices.Count <= 2 || triangles.Count < 3)
        {
            Debug.Log("ret:: " + vertices.Count + " :: " + triangles.Count);
            return;
        }
        Debug.Log("col:: " + vertices.Count + " :: " + triangles.Count);

        mesh.Clear();
        mesh.name = "ColliderMesh";
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        //meshCollider.enabled = false;
        meshCollider.enabled = true;
    }

    public void CreateCollider(List<Vector3> points)
    {
        int pointsCount = points.Count;
        if(pointsCount >= 3)
        {
            pointsCount -= 3;
        }
        else
        {
            return;
        }
        int[] trianglesArray = new int[pointsCount * 3 + 3];
        //Debug.Log("pointsCount : " + points.Count + " :: triangleCount : " + trianglesArray.Length);
        for(int i = 0; i < pointsCount + 1; i++)
        {
            //Debug.Log("i : " + i);
            trianglesArray[i * 3 + 0] = 0;
            trianglesArray[i * 3 + 1] = i + 1;
            trianglesArray[i * 3 + 2] = i + 2;
            //Debug.Log("point :: " + points[i] + " :: " + "triangle" + i + " :: " + trianglesArray[i * 3 + 0] + " : " + trianglesArray[i * 3 + 1] + " : " + trianglesArray[i * 3 + 2]);
        }

        mesh.Clear();
        mesh.name = "ColliderMesh";
        mesh.vertices = points.ToArray();
        mesh.triangles = trianglesArray;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        //meshCollider.enabled = false;
        meshCollider.enabled = true;
    }

    public void RemoveCollider()
    {
        meshCollider.enabled = false;
        meshCollider.sharedMesh = null;
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        colliderCallback(other);
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("CollisionHit");
    //    colliderCallback(collision.collider);
    //}
}
