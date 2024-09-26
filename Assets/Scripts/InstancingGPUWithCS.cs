using UnityEngine;

public class InstancingGPUWithCS : MonoBehaviour {
    [SerializeField]
    Mesh Mesh;

    [SerializeField]
    Material Mat;

    [SerializeField, Range(1, 300)]
    int Resolution = 11;

    ComputeBuffer argsBuffer;
    ComputeBuffer matricesBuffer;
    ComputeBuffer matrices2Buffer;
    ComputeBuffer matrices3Buffer;
    private uint[] args = new uint[5] {0, 0, 0, 0, 0};


    const int LOCAL_WORK_GROUPS_X = 8;
    const int LOCAL_WORK_GROUPS_Y = 8;

    void OnEnable() {
        int threadGroupsX = Mathf.CeilToInt((float)Resolution / (float)LOCAL_WORK_GROUPS_X);
        int threadGroupsY = Mathf.CeilToInt((float)Resolution / (float)LOCAL_WORK_GROUPS_Y);

        argsBuffer = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
        matricesBuffer = new ComputeBuffer(Resolution*Resolution, sizeof(float) * 16);
        matrices2Buffer = new ComputeBuffer(Resolution*Resolution, sizeof(float) * 16);
        matrices3Buffer = new ComputeBuffer(Resolution*Resolution, sizeof(float) * 16);

        args[0] = Mesh.GetIndexCount(0);
        args[1] = (uint)(Resolution * Resolution);
        args[2] = Mesh.GetIndexStart(0);
        args[3] = Mesh.GetBaseVertex(0);
        argsBuffer.SetData(args);



        ComputeShader Grass_CS = Resources.Load<ComputeShader>("Grass");
        Grass_CS.SetInt("_Resolution", Resolution);
        Matrix4x4 parentToWorld = this.transform.localToWorldMatrix;
        Grass_CS.SetMatrix("_ParentToWorld", parentToWorld);


        Grass_CS.SetBuffer(0, "_Matrices", matricesBuffer);
        Grass_CS.SetFloat("_Angle", 0f);
        Grass_CS.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Grass_CS.SetBuffer(0, "_Matrices", matrices2Buffer);
        Grass_CS.SetFloat("_Angle", 45f);
        Grass_CS.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        
        Grass_CS.SetBuffer(0, "_Matrices", matrices3Buffer);
        Grass_CS.SetFloat("_Angle", -45f);
        Grass_CS.Dispatch(0, threadGroupsX, threadGroupsY, 1);
    }

    void OnDisable() {
        matrices3Buffer.Release();
        matrices3Buffer = null;

        matrices2Buffer.Release();
        matrices2Buffer = null;
        
        matricesBuffer.Release();
        matricesBuffer = null;

        argsBuffer.Release();
        argsBuffer = null;
    }

    void OnValidate() {
        if (argsBuffer != null & enabled) {
            OnDisable();
            OnEnable();
        }
    }

    void Update() {
        if (this.transform.hasChanged) {
            OnValidate();
            this.transform.hasChanged = false;
        }

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        
        mpb.SetBuffer("_Matrices", matricesBuffer);
        Graphics.DrawMeshInstancedIndirect(Mesh, 0, Mat, new Bounds(Vector3.zero, new Vector3(300.0f, 200.0f, 300.0f)), argsBuffer, 0, mpb);

        mpb.SetBuffer("_Matrices", matrices2Buffer);
        Graphics.DrawMeshInstancedIndirect(Mesh, 0, Mat, new Bounds(Vector3.zero, new Vector3(300.0f, 200.0f, 300.0f)), argsBuffer, 0, mpb);
        
        mpb.SetBuffer("_Matrices", matrices3Buffer);
        Graphics.DrawMeshInstancedIndirect(Mesh, 0, Mat, new Bounds(Vector3.zero, new Vector3(300.0f, 200.0f, 300.0f)), argsBuffer, 0, mpb);
    }
}