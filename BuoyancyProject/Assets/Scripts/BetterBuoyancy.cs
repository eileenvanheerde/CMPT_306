using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (MeshFilter))]
public class BetterBuoyancy : MonoBehaviour
{
    // what to divisor for each obj shape
    private Dictionary<string, float> objDiv;

    // water constants
    private const float waterLvl = 4f;    

    // some buoyancy force
    private Vector3 verticalForce = new Vector3(0,10,0);    

    private class VertexData
    {
        public Vector3 globalPos;
        public float distToWater;

        public VertexData(Vector3 p, float dist)
        {
            this.globalPos = p;
            this.distToWater = dist;
        }
    }

    // Object mesh (will get vertices from this)
    private Transform objTransform;
    private Collider objCollider;
    private Vector3[] localVertices;

    // list of vertices currently underwater
    private List<VertexData> UWVertices;

    private Rigidbody rbody;

    private void GenerateVertexData()
    {
        // clear list
        UWVertices.Clear();

        for (int i = 0; i < localVertices.Length; i++)
        {
            // get global pos of coord
            Vector3 globalPos = objTransform.TransformPoint(localVertices[i]);
            float dist = waterLvl - globalPos.y;

            if (globalPos.y < waterLvl)
            {
                // Is below water, Add to list of UWVertices
                UWVertices.Add(new VertexData(globalPos, dist));
            }
        }
    }

    // Not being used :(
    /// <summary>
    /// Finds the centermost point between all the underwater points
    /// </summary>
    /// <returns> VertexData of centermost point </returns>
    private VertexData GenerateUWCenter()
    {
        Vector3 UWCenter = Vector3.zero;

        // sum UWVertices
        for (int j = 0; j < UWVertices.Count; j++)
        {
            UWCenter += UWVertices[j].globalPos;
        }

        // calculate avg
        UWCenter = UWCenter / UWVertices.Count;

        // calc dist from center point to surface of water
        float dist = waterLvl - UWCenter.y;

        return new VertexData(UWCenter, dist);
    }

    // draws curr velocity vector of obj
    void OnDrawGizmos()
    {
        Vector3 pointOutFront = rbody.position + (rbody.velocity * 10);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rbody.position, pointOutFront);
    }


    // Start is called before the first frame update
    void Start()
    {
        // Obj lookup
        // diff shapes have diff # of vertices, this helps me normalize
        //   how the force is applied to each of them
        objDiv = new Dictionary<string, float>();
        objDiv.Add("Sphere", 5f);
        objDiv.Add("Cube", 3f);
        objDiv.Add("Cylinder", 4f);
        objDiv.Add("Rectangle", 4f);

        // get obj rigidbody
        rbody = gameObject.GetComponent<Rigidbody>();
        // get local vertices from mesh
        localVertices = gameObject.GetComponent<MeshFilter>().mesh.vertices;
        objTransform = gameObject.transform;
        objCollider = gameObject.GetComponent<Collider>();

        print("ObjCollider" + objCollider.GetType());

        // instantiate List of under/above water vertices
        UWVertices = new List<VertexData>();
    }

    // Update is called once per frame
    void Update()
    {
        float div = objDiv[gameObject.tag];
        this.GenerateVertexData();
        
        // cube will float on angle
        // this makes it fall over (still a little delayed)
        foreach (VertexData p in UWVertices)
        {
            // apply buoyancy force
            rbody.AddForceAtPosition(((p.distToWater/rbody.mass)*2f*(verticalForce)/(UWVertices.Count/div)), p.globalPos);
            // apply drag from water
            rbody.AddForceAtPosition((rbody.velocity * (-0.5f)) / (UWVertices.Count / div), p.globalPos);
        }
        if (UWVertices.Count > 0)
        {
            // stabilizes objects so they don't rotate too much
            rbody.AddTorque(rbody.angularVelocity * (-0.7f) / rbody.mass);   
        }
    }
}
