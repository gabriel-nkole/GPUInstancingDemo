using UnityEngine;

public class InstancingGPUWithCS : MonoBehaviour {

    [SerializeField]
    Mesh Mesh;

    [SerializeField]
    Material Mat;

    [SerializeField, Range(1, 300)]
    int Resolution = 11;

    ComputeBuffer matricesBuffer;
    ComputeBuffer matrices2Buffer;
    ComputeBuffer matrices3Buffer;
    ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] {0, 0, 0, 0, 0};


    const int LOCAL_WORK_GROUPS_X = 1;
    const int LOCAL_WORK_GROUPS_Y = 1;

    void OnEnable() {
        int threadGroupsX = Mathf.CeilToInt(Resolution / LOCAL_WORK_GROUPS_X);
        int threadGroupsY = Mathf.CeilToInt(Resolution / LOCAL_WORK_GROUPS_Y);


        matricesBuffer = new ComputeBuffer(Resolution*Resolution, sizeof(float) * 16);
        matrices2Buffer = new ComputeBuffer(Resolution*Resolution, sizeof(float) * 16);
        matrices3Buffer = new ComputeBuffer(Resolution*Resolution, sizeof(float) * 16);
        argsBuffer = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);

        ComputeShader InstancingCompute = Resources.Load<ComputeShader>("InstancingCS");
        InstancingCompute.SetInt("_Resolution", Resolution);
        Matrix4x4 parentToWorld = this.transform.localToWorldMatrix;
        InstancingCompute.SetMatrix("_ParentToWorld", parentToWorld);


        InstancingCompute.SetBuffer(0, "_Matrices", matricesBuffer);
        InstancingCompute.SetFloat("_Angle", 0f);
        InstancingCompute.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        InstancingCompute.SetBuffer(0, "_Matrices", matrices2Buffer);
        InstancingCompute.SetFloat("_Angle", 45f);
        InstancingCompute.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        
        InstancingCompute.SetBuffer(0, "_Matrices", matrices3Buffer);
        InstancingCompute.SetFloat("_Angle", -45f);
        InstancingCompute.Dispatch(0, threadGroupsX, threadGroupsY, 1);


        args[0] = Mesh.GetIndexCount(0);
        args[1] = (uint)(Resolution * Resolution);
        args[2] = Mesh.GetIndexStart(0);
        args[3] = Mesh.GetBaseVertex(0);
        argsBuffer.SetData(args);
    }

    void OnDisable() {
        matricesBuffer.Release();
        matricesBuffer = null;

        matrices2Buffer.Release();
        matrices2Buffer = null;
        
        matrices3Buffer.Release();
        matrices3Buffer = null;

        argsBuffer.Release();
        argsBuffer = null;
    }

    void OnValidate() {
        if (matricesBuffer != null & enabled) {
            OnDisable();
            OnEnable();
        }
    }

    void Update() {
        if (this.transform.hasChanged) {
            OnValidate();
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
