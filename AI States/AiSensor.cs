using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiSensor : MonoBehaviour
{
    public float detectDist = 10;
    public float detectAngle = 30;
    public float detectHeight = 1.0f;
    public float proximityDist = 4;
    public float proximityAngle = 30;
    public float proimityHeight = 1.0f;
    public float suspectHeight = 1.0f;
    public float suspectAngle = 30f;
    public float suspectDist = 10f;

    public int scanFrequency = 30;
    public LayerMask scanLayers;
    public Color meshColor = Color.red;
    public Color attackMeshColor = Color.red;
    public Color suspectMeshColor = Color.red;
    public LayerMask occlusionLayer;
    public LayerMask coverLayer;
        public LayerMask targettingMask;
    Mesh detectMesh;
    Mesh attackMesh;
    Mesh suspectMesh;

    private List<GameObject> objects = new List<GameObject>();
    private List<AiAgent> agents = new List<AiAgent>();
    Collider[] colliders = new Collider[10];
    int count;
    float scanInterval;
    float scanTimer;

    // Start is called before the first frame update
    void Start()
    {
        scanInterval = 1.0f / scanFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }

       
    }

    void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, detectDist, colliders, scanLayers, QueryTriggerInteraction.Collide);
        //colliders = Physics.OverlapSphere(transform.position, distance, scanLayers, QueryTriggerInteraction.Collide);
        //count = colliders.Length;
        objects.Clear();
        agents.Clear();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if ((IsInSight(obj) || IsInProximity(obj)) && !isAI(obj))
            {
                objects.Add(obj);
               
            }

            if (isAI(obj) && obj != this.gameObject)
            {
                //Debug.Log("added" + obj.name);
                agents.Add(obj.GetComponent<AiAgent>());
            }
        }
    }

    bool isAI(GameObject obj)
    {
        if (obj.GetComponent<AiAgent>() != null)
        {
            return true;
        }
        return false;
    }

    public int Filter(GameObject[] buffer, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        int count = 0;

        foreach(var obj in objects)
        {
            if(obj.layer == layer)
            {
                buffer[count++] = obj;
            }

            if(buffer.Length == count)
            {
                break;//buffer is full
            }
        }

        return count;
    }

    public Collider GetCovers(AiAgent agent)
    {
        Collider[] coversCollided = Physics.OverlapSphere(transform.position, detectDist, coverLayer);
        System.Array.Sort(coversCollided, ColliderArraySortCompare);

        var nearestCover = coversCollided
            .Where(collider => collider.CompareTag("Cover"))
            .Where(collider =>
            {
                if (agent.health.hitDir != null)
                {
                    Vector3 dist = (transform.position - collider.transform.position).normalized;
                    //Debug.Log(Vector3.Dot(dist, agent.health.hitDir));
                    return Vector3.Dot(dist, agent.health.hitDir) <= -0.1f;
                }
                return false;
            })
            .OrderBy(collider => Vector3.Distance(transform.position, collider.transform.position))
            .FirstOrDefault();

            if (nearestCover != null)
            {
                return nearestCover;
            }
            return null;
    }

    public void CallOtherAi()
    {
        if(agents.Count > 0)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                if (agents[i].IsNotInAttackStates)
                {
                   // Debug.Log($"{agents[i].gameObject.name} State has been changed");
                    agents[i].navMeshAgent.speed = agents[i].config.pursueSpeed;
                    agents[i].animSpeed = agents[i].config.pursueSpeed;
                    agents[i].navMeshAgent.SetDestination(this.transform.position);
                }
            }
        }
    }

    public void KeepDistance()
    {
        if(agents.Count > 0)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                Vector3 dir = this.gameObject.transform.position - agents[i].gameObject.transform.position;
                if(dir.magnitude < 4)
                {
                    //agents[i].navMeshAgent.SetDestination(dir);

                }
            }
        }
    }
    private int ColliderArraySortCompare(Collider A, Collider B)
    {
        if (A == null || B != null)
        {
            return 1;
        }
        else if (A != null || B == null)
        {
            return -1;
        }
        else if (A == null || B == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(transform.position, A.transform.position).CompareTo(Vector3.Distance(transform.position, B.transform.position));
        }
    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 dir = dest - origin;
        if (dir.y < 0.0 || dir.y > detectHeight)
        {
            return false;
        }

        /*float dist = Vector3.Distance(origin, dest);
        if(dist > detectDist)
        {
            return false;
        }*/

        if (ObstuctionCheck(obj, detectHeight))
        {
            return false;
        }

        dir.y = 0;
        float deltaAngle = Vector3.Angle(dir, transform.forward);
        if(deltaAngle > detectAngle)
        {
            return false;
        }
        return true;
    }

    public bool IsInProximity(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 dir = dest - origin;
        if (dir.y < 0 || dir.y > proimityHeight)
        {
            return false;
        }

        float dist = Vector3.Distance(origin, dest);
        if (dist > proximityDist)
        {
            return false;
        }



        if (ObstuctionCheck(obj, proimityHeight))
        {
            return false;
        }

        dir.y = 0;
        float deltaAngle = Vector3.Angle(dir, transform.forward);
        if (deltaAngle > proximityAngle)
        {
            return false;
        }
        return true;
    }

    public bool IsSoundClose(Vector3 soundLocation)
    {
        Vector3 origin = transform.position;
        Vector3 dest = soundLocation;

        Vector3 dir = dest - origin;

        if(dir.y < 0 || dir.y > suspectHeight)
        {
            return false;
        }

        float dist = Vector3.Distance(origin, dest);
        if (dist > suspectDist)
        {
            return false;
        }


        dir.y = 0;
        float deltaAngle = Vector3.Angle(dir, -transform.forward);
        if (deltaAngle > suspectAngle)
        {
            return false;            
        }

        return true;
    }
    bool ObstuctionCheck(GameObject obj, float scannerHeight)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;

        origin.y = scannerHeight;
        dest.y = GetColliderHeight(obj.GetComponent<Collider>());
        if (Physics.Linecast(origin, dest, occlusionLayer))
        {
            //Debug.DrawLine(origin, dest, Color.black);
            return true;
        }
      
        return false;
    }

    public float GetColliderHeight(Collider collider)
    {
        if(collider is BoxCollider boxCollider)
            return boxCollider.size.y;

        if (collider is CapsuleCollider capsuleCollider)
            return capsuleCollider.height;

        if(collider is CharacterController characterController)
            return characterController.height;

        return 0;
        
    }
    Mesh CreateWedgeMesh()
       {
            Mesh mesh = new Mesh();

            int segments = 10;
            int numTriangle = (segments * 4) + 2 + 2;
            int numVertices = numTriangle * 3;

            Vector3[] vertices =  new Vector3[numVertices];
            int[] triangles = new int[numVertices];

            Vector3 bottomCenter = Vector3.zero;
            Vector3 bottomLeft = Quaternion.Euler(0, -detectAngle, 0) * Vector3.forward * detectDist; 
            Vector3 bottomRight = Quaternion.Euler(0, detectAngle, 0) * Vector3.forward * detectDist;

            Vector3  topCenter = bottomCenter + Vector3.up * detectHeight;
            Vector3 topLeft = bottomLeft + Vector3.up * detectHeight;
            Vector3 topRight = bottomRight + Vector3.up * detectHeight;

            int vert = 0;

            //left Side

            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = topLeft;

            vertices[vert++] = topLeft;
            vertices[vert++] = topCenter;
            vertices[vert++] = bottomCenter;

            //right Side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = topCenter;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomCenter;

            float currentAngle = -detectAngle;
            float deltaAngle = (detectAngle * 2) / segments;
            for(int i = 0; i < segments; ++i)
            {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * detectDist;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * detectDist;

            topLeft = bottomLeft + Vector3.up * detectHeight;
            topRight = bottomRight + Vector3.up * detectHeight;

                //far side
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = topRight;

                vertices[vert++] = topRight;
                vertices[vert++] = topLeft;
                vertices[vert++] = bottomLeft;

                //top
                vertices[vert++] = topCenter;
                vertices[vert++] = topLeft;
                vertices[vert++] = topRight;

                //bottom
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;

                currentAngle += deltaAngle;
            }


       

            for(int i = 0; i < numVertices; ++i)
            {
                triangles[i] = i;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
       }

    Mesh CreateProximityMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangle = (segments * 4) + 2 + 2;
        int numVertices = numTriangle * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -proximityAngle, 0) * Vector3.forward * proximityDist;
        Vector3 bottomRight = Quaternion.Euler(0, proximityAngle, 0) * Vector3.forward * proximityDist;

        Vector3 topCenter = bottomCenter + Vector3.up * proimityHeight;
        Vector3 topLeft = bottomLeft + Vector3.up * proimityHeight;
        Vector3 topRight = bottomRight + Vector3.up * proimityHeight;

        int vert = 0;

        //left Side

        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //right Side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -proximityAngle;
        float deltaAngle = (proximityAngle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * proximityDist;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * proximityDist;

            topLeft = bottomLeft + Vector3.up * proimityHeight;
            topRight = bottomRight + Vector3.up * proimityHeight;

            //far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }




        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    Mesh CreateSuspectMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangle = (segments * 4) + 2 + 2;
        int numVertices = numTriangle * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -suspectAngle, 0) * Vector3.back * suspectDist;
        Vector3 bottomRight = Quaternion.Euler(0, suspectAngle, 0) * Vector3.back * suspectDist;

        Vector3 topCenter = bottomCenter + Vector3.up * suspectHeight;
        Vector3 topLeft = bottomLeft + Vector3.up * suspectHeight;
        Vector3 topRight = bottomRight + Vector3.up * suspectHeight;

        int vert = 0;

        //left Side

        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //right Side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -suspectAngle;
        float deltaAngle = (suspectAngle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.back * suspectDist;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.back * suspectDist;

            topLeft = bottomLeft + Vector3.up * suspectHeight;
            topRight = bottomRight + Vector3.up * suspectHeight;

            //far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }




        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
    private void OnValidate()
    {
        detectMesh = CreateWedgeMesh();
        attackMesh = CreateProximityMesh();
        suspectMesh = CreateSuspectMesh();
        scanInterval = 1.0f / scanFrequency;
    }

    private void OnDrawGizmos()
    {
        if (detectMesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(detectMesh, transform.position, transform.rotation);
        }

       
        if (attackMesh)
        {
            Gizmos.color = attackMeshColor;
           
            Gizmos.DrawMesh(attackMesh, transform.position, transform.rotation);
        }

        if (suspectMesh)
        {
            Gizmos.color = suspectMeshColor;
            Gizmos.DrawMesh(suspectMesh, transform.position, transform.rotation);
        }
        Gizmos.DrawWireSphere(transform.position, detectDist);
        /*  for (int i = 0; i< count; ++i)
          {

              Gizmos.DrawSphere(colliders[i].transform.position, 0.5f);
          }

         Gizmos.color = Color.red;
          foreach (var obj in objects)
          { 
              Gizmos.DrawSphere(obj.transform.position, 0.5f);
          }*/
    }
}
