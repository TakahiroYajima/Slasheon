using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashCollider : MonoBehaviour {

    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;
    Mesh mesh;

    private Vector3[] vertices;
    int[] triangles;

    public delegate void OnColliderEnterCallback(Collider collider);
    public OnColliderEnterCallback colliderCallback;

    // Use this for initialization
    void Start () {
        vertices = new Vector3[3]{
            new Vector3(0,0,0),
            new Vector3(3f,0f,10f),
            new Vector3(-3f,0f,10f)
            };

        triangles = new int[3];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        mesh = new Mesh();
    }

    public void SetCollisionEnterCallback(OnColliderEnterCallback callback)
    {
        colliderCallback = callback;
    }
	
	public void SetPoint(Vector3[] point)
    {
        vertices = point;
    }
    //public void MakeTriangles(int size)
    //{
    //    for(int i = 0; i < size; i++)
    //    {
    //        if(triangles.Count < size)
    //        {
    //            triangles.Add(i);
    //        }
    //        else
    //        {
    //            triangles[i] = i;
    //        }
    //    }
    //}

    public void CreateCollider()
    {
        //Mesh mesh = new Mesh();
        mesh.name = "ColliderMesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;

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
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    colliderCallback(other);
    //}
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("CollisionHit");
        colliderCallback(collision.collider);
    }
}
