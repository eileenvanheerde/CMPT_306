using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class BuoyancyScript : MonoBehaviour
{
    // Water Plane info
    private const float waterLvl = 4f;
    private const float waterDensity = 2f;

    // Forces applied to Object
    private Vector3 verticalForce;
    private Vector3 horizontalForce;

    // Object's RigidRody
    private Rigidbody rbody;
    private BoxCollider collider;

    // this is to get global pos of obj
    private Transform boatTransport;

    // coords of all vertices in boat
    private Vector3[] localBoatVertices;
    private Vector3[] globalBoatVertices;

    // Distance of all vertices to water
    private List<float> distToSurface;

    // bounding box coords of boxCollider (if used)
    private Vector3[] globalBox = new Vector3[8];
    private List<Vector3> UWVertices = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        rbody = gameObject.GetComponent<Rigidbody>();
        if (gameObject.GetComponent<BoxCollider>() != null)
        {
            collider = gameObject.GetComponent<BoxCollider>();
        }

        verticalForce = new Vector3(0, 25, 0);
        horizontalForce = Vector3.zero;
        distToSurface = new List<float>();
        globalBox = GetColliderVertexPositions(gameObject);
    }

    private Vector3[] GetColliderVertexPositions(GameObject obj)
    {
        Vector3[] vertices = new Vector3[8];
        Matrix4x4 matrix = obj.transform.localToWorldMatrix;
        Quaternion storedRotation = obj.transform.rotation;

        obj.transform.rotation = Quaternion.identity;

        Vector3 extents = obj.GetComponent<BoxCollider>().bounds.extents;
        vertices[0] = matrix.MultiplyPoint3x4(extents);
        vertices[1] = matrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, extents.z));
        vertices[2] = matrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, -extents.z));
        vertices[3] = matrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, -extents.z));
        vertices[4] = matrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, extents.z));
        vertices[5] = matrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, extents.z));
        vertices[6] = matrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, -extents.z));
        vertices[7] = matrix.MultiplyPoint3x4(-extents);

        obj.transform.rotation = storedRotation;

        return vertices;
    }

    private void CircleBuoyancy()
    {
        if (rbody.worldCenterOfMass.y < waterLvl)
        {
            float dist = waterLvl - rbody.worldCenterOfMass.y;
            rbody.AddForce(verticalForce * 2f * (dist/1.5f));
            rbody.AddForce(rbody.velocity * (-1f) * waterDensity);
            horizontalForce.x = rbody.velocity.x;
            horizontalForce.z = rbody.velocity.z;
        } 
        if ((horizontalForce.x > 0 || horizontalForce.z > 0) && rbody.worldCenterOfMass.y > waterLvl) {
            rbody.AddForce(horizontalForce);
        }
    }

    private void GetDistFromSurface(List<Vector3> vlist)
    {
        distToSurface.Clear();
        foreach (Vector3 p in vlist)
        {
            distToSurface.Add(waterLvl - p.y);
        }
    }

    /// <summary>
    /// Calculates centermost point between all underwater vertices
    /// </summary>
    /// <param name="vlist">List of bounding box vertices that are underwater</param>
    /// <returns>underwater center</returns>
    private Vector3 GetUnderWaterCenter(List<Vector3> vlist)
    {
        int lsize = vlist.Count;
        float xAvg = 0f;
        float yAvg = 0f;
        float zAvg = 0f;

        for (int i = 0; i < lsize; i++)
        {
            xAvg += vlist[i].x;
            yAvg += vlist[i].y;
            zAvg += vlist[i].z;
            // print("UWVertices[" + i + "]: " + vlist[i].ToString());
        }

        xAvg = xAvg / lsize;
        yAvg = yAvg / lsize;
        zAvg = zAvg / lsize;

        return new Vector3(xAvg, yAvg, zAvg);
    }

    // Update is called once per frame
    void Update()
    {
        UWVertices.Clear();
        globalBox = GetColliderVertexPositions(gameObject);
        Vector3 UWCenter;

        if (gameObject.tag == "Sphere")
        {
            this.CircleBuoyancy();
        }

        /*if (rbody.worldCenterOfMass.y < waterLvl)
        {
            for (int i = 0; i < 8; i++)
            {
                if (globalBox[i].y < waterLvl)
                {
                    UWVertices.Add(globalBox[i]);
                }
            }

            UWCenter = GetUnderWaterCenter(UWVertices);
            float dist = waterLvl - UWCenter.y;
            print("UWCenter dist: " + dist);

            if (UWVertices.Count > 0)
            {               
                foreach (Vector3 p in UWVertices)
                {
                    rbody.AddForceAtPosition((verticalForce/6) * dist, p);
                }
                rbody.AddForce(verticalForce * dist);
                // Debug.DrawLine(rbody.worldCenterOfMass, rbody.worldCenterOfMass + verticalForce, Color.red, 10);
            }
            else
            {
                rbody.AddForceAtPosition(verticalForce*dist, UWCenter);
                rbody.AddForce(verticalForce*dist);

                // Debug.DrawLine(UWCenter, UWCenter + verticalForce, Color.red, 10);
                // Debug.Log("UWCENTER: " + UWCenter);
            }
            rbody.AddForce(rbody.velocity * (-1f) * waterDensity);
            // print(gameObject.name + " ANGULAR: " + rbody.angularVelocity);
            rbody.AddTorque(rbody.angularVelocity * -1);
        }*/


        // This was before trying to do the UW points
        /*
        if (horizontalForce.x > 0)
        {
            rbody.AddForce(horizontalForce);
        } 
        if (rbody.worldCenterOfMass.y < waterLvl)
        {
            rbody.AddForce(verticalForce);
            rbody.AddForce(rbody.velocity * (-1f) * waterDensity);
            horizontalForce.x = rbody.velocity.x;
        }*/
        // Debug.Log("velocityAbove: " + rbody.velocity.x);
    }
}
