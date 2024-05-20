using UnityEngine;

public class InstancingCPUWithCS: MonoBehaviour {

    [SerializeField]
    Mesh Mesh;

    [SerializeField]
    Material Mat;

    [SerializeField, Range(1, 300)]
    int Resolution = 11;

    Matrix4x4[] matrices;
    ComputeBuffer matricesBuffer;
    Matrix4x4[] matrices2;
    Matrix4x4[] matrices3;


    const int LOCAL_WORK_GROUPS_X = 1;
    const int LOCAL_WORK_GROUPS_Y = 1;

    void OnEnable() {
        int threadGroupsX = Mathf.CeilToInt(Resolution / LOCAL_WORK_GROUPS_X);
        int threadGroupsY = Mathf.CeilToInt(Resolution / LOCAL_WORK_GROUPS_Y);


        matrices = new Matrix4x4[Resolution*Resolution];
        matrices2 = new Matrix4x4[Resolution*Resolution];
        matrices3 = new Matrix4x4[Resolution*Resolution];
        matricesBuffer = new ComputeBuffer(Resolution*Resolution, sizeof(float) * 16);

        ComputeShader InstancingCompute = Resources.Load<ComputeShader>("InstancingCS");
        InstancingCompute.SetFloat("_Resolution", Resolution);
        InstancingCompute.SetBuffer(0, "_Matrices", matricesBuffer);
        Matrix4x4 parentToWorld = this.transform.localToWorldMatrix;
        InstancingCompute.SetMatrix("_ParentToWorld", parentToWorld);


        InstancingCompute.SetFloat("_Angle", 0f);
        InstancingCompute.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        matricesBuffer.GetData(matrices);

        InstancingCompute.SetFloat("_Angle", 45f);
        InstancingCompute.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        matricesBuffer.GetData(matrices2);
        
        InstancingCompute.SetFloat("_Angle", -45f);
        InstancingCompute.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        matricesBuffer.GetData(matrices3);
    }

    void OnDisable() {
        matricesBuffer.Release();
        matricesBuffer = null;

        matrices = null;
        matrices2 = null;
        matrices3 = null;
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

        Graphics.DrawMeshInstanced(Mesh, 0, Mat, matrices);
        Graphics.DrawMeshInstanced(Mesh, 0, Mat, matrices2);
        Graphics.DrawMeshInstanced(Mesh, 0, Mat, matrices3);
    }
}
