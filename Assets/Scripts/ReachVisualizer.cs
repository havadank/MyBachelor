using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Ball
{
    Vector3 position;
    float radius;
    //Vector3 albedo;
    //Vector3 specular;

    public Ball(Vector3 position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }
}

public class ReachVisualizer : MonoBehaviour
{
    public GameObject Center;
    public GameObject Edge;

    public GameObject Target;
    public GameObject Placement;

    [SerializeField]
    GameObject Robot;

    private Vector3 centerPosition;
    private Vector3 edgePosition;
    private float radius;

    public ComputeShader ReachShader;
    [HideInInspector]
    public Texture SkyTexture;
    [HideInInspector]
    public RenderTexture renderTexture;
    private Camera camera;

    private Ball sphere;



    private void Awake()
    {
        camera = GetComponent<Camera>();
        centerPosition = Center.transform.position;
        edgePosition = Edge.transform.position;
        radius = Vector3.Distance(centerPosition, edgePosition);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        SetShaderParameters();
        SetGeometry();
        Render(destination);
    }

    private void SetGeometry()
    {
        //Ball sphere = new Ball (centerPosition, radius);

        // Pass the ball to the shader
        //ComputeBuffer sphereBuffer = new ComputeBuffer(1, 16);
        //Ball[] sphereArr = new Ball[1];
        //sphereArr[0] = sphere;
        //sphereBuffer.SetData(sphereArr);
        ReachShader.SetFloat("_Radius", radius);
        ReachShader.SetVector("_SphereCenter", centerPosition);
        Vector4 SphereVec4 = new Vector4(centerPosition.x, centerPosition.y, centerPosition.z, radius);
        ReachShader.SetVector("_SphereVec4", SphereVec4);

        Vector4 TargetVec4 = new Vector4(Target.transform.position.x, Target.transform.position.y, Target.transform.position.z, 0f);
        Vector4 PlacementVec4 = new Vector4(Placement.transform.position.x, Placement.transform.position.y, Placement.transform.position.z, 0f);
        ReachShader.SetVector("_TargetVec4", TargetVec4);
        ReachShader.SetVector("_PlacementVec4", PlacementVec4);


        // Pass the Target and Placement to the shader
        ComputeBuffer interactionBuffer = new ComputeBuffer(2, 24);
        Vector3[] vector3s = new Vector3[2];
        vector3s[0] = Target.transform.position;
        vector3s[1] = Placement.transform.position;
        interactionBuffer.SetData(vector3s);

        //sphereBuffer.Release();
        interactionBuffer.Release();
    }

    private void Render(RenderTexture destination)
    {
        InitRenderTexture();

        ReachShader.SetTexture(0, "_Result", renderTexture);
        ReachShader.Dispatch(0, renderTexture.width / 32, renderTexture.height / 32, 1);

        //sphereBuffer.Release();

        Graphics.Blit(renderTexture, destination);
    }

    private void InitRenderTexture()
    {
        if (renderTexture == null || renderTexture.width != Screen.width || renderTexture.height != Screen.height)
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
            }

            renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    private void SetShaderParameters()
    {
        Texture2D tx2d = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        tx2d.SetPixel(0, 0, Color.clear);
        tx2d.Apply();
        SkyTexture = tx2d;

        ReachShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
        ReachShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
        ReachShader.SetTexture(0, "_SkyTexture", SkyTexture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
