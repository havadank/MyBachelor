using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//struct Sphere
//{
//    Vector3 position;
//    float radius;
//    //Vector3 albedo;
//    //Vector3 specular;

//    public Sphere(Vector3 position, float radius)
//    {
//        this.position = position;
//        this.radius = radius;
//    }
//}

[ExecuteAlways]
public class RobotReachVisual : MonoBehaviour
{
    [SerializeField]
    float Radius = 1.0f;
    [SerializeField]
    GameObject Boundry;
    [SerializeField]
    GameObject RobotPosition;
    [SerializeField]
    GameObject Robot;

    [SerializeField]
    GameObject Target;
    [SerializeField]
    GameObject Placement;

    [SerializeField]
    bool drawBoundry;

    public ComputeShader sphereShader;
    [HideInInspector]
    public RenderTexture renderTexture;
    //public RenderTexture destination;

    Sphere sphere;

    private int geometryArraySize = 3;
    private Camera camera;
    public Texture SkyTexture;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void SetShaderParameters()
    {
        sphereShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
        sphereShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
        sphereShader.SetTexture(0, "_SkyTexture", SkyTexture);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject[] makeGameObjectArray()
    {
        GameObject[] gameObjectArray = new GameObject[geometryArraySize];
        gameObjectArray[0] = GameObject.Find("ur5e");
        gameObjectArray[1] = Target;
        gameObjectArray[2] = Placement;
        return gameObjectArray;
    }

    Sphere[] makeSphereArray()
    {
        Sphere[] sphereArr = new Sphere[1];
        sphereArr[0] = makeSphere();
        return sphereArr;
    }

    Sphere makeSphere()
    {
        return new Sphere(RobotPosition.transform.position, Radius);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParameters();
        Render(destination);
    }

    private void Render(RenderTexture destination)
    {
        InitRenderTexture();

        // Taken from RunSphereShader method
        // Create sphereBuffer to house the sphere to compute
        Sphere[] sphereArr = makeSphereArray();
        var sphereBuffer = new ComputeBuffer(1, 1);
        sphereBuffer.SetData(sphereArr);

        // Taken from RunSphereShader method
        // Create buffer to house scene geometry
        GameObject[] gameObjects = makeGameObjectArray();
        var geometryBuffer = new ComputeBuffer(geometryArraySize, 1);
        geometryBuffer.SetData(gameObjects);

        // Taken from RunSphereShader method
        // Assign buffers to compute shader
        sphereShader.SetBuffer(0, "_sphereBuffer", sphereBuffer);
        sphereShader.SetBuffer(0, "_geometryBuffer", geometryBuffer);

        sphereShader.SetTexture(0, "_Result", renderTexture);
        sphereShader.Dispatch(0, renderTexture.width / 32, renderTexture.height / 32, 1);

        sphereBuffer.Release();

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

    public void DrawBoundryInWorld(bool draw)
    {
        //drawBoundry = draw;
    }

    
}