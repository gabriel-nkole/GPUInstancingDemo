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


    const int LOCAL_WORK_GROUPS_X = 8;
    const int LOCAL_WORK_GROUPS_Y = 8;

    void OnEnable() {
        int threadGroupsX = Mathf.CeilToInt((float)Resolution / (float)LOCAL_WORK_GROUPS_X);
        int threadGroupsY = Mathf.CeilToInt((float)Resolution / (float)LOCAL_WORK_GROUPS_Y);

        matrices = new Matrix4x4[Resolution*Resolution];
        matrices2 = new Matrix4x4[Resolution*Resolution];
        matrices3 = new Matrix4x4[Resolution*Resolution];
        matricesBuffer = new ComputeBuffer(Resolution*Resolution, sizeof(float) * 16);



        ComputeShader Grass_CS = Resources.Load<ComputeShader>("Grass");
        Grass_CS.SetFloat("_Resolution", Resolution);
        Grass_CS.SetBuffer(0, "_Matrices", matricesBuffer);
        Grass_CS.SetMatrix("_ParentToWorld", this.transform.localToWorldMatrix);


        Grass_CS.SetFloat("_Angle", 0f);
        Grass_CS.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        matricesBuffer.GetData(matrices);

        Grass_CS.SetFloat("_Angle", 45f);
        Grass_CS.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        matricesBuffer.GetData(matrices2);
        
        Grass_CS.SetFloat("_Angle", -45f);
        Grass_CS.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        matricesBuffer.GetData(matrices3);
    }

    void OnDisable() {
        matricesBuffer.Release();
        matricesBuffer = null;

        matrices3 = null;
        matrices2 = null;
        matrices = null;
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
            this.transform.hasChanged = false;
        }

        Graphics.DrawMeshInstanced(Mesh, 0, Mat, matrices);
        Graphics.DrawMeshInstanced(Mesh, 0, Mat, matrices2);
        Graphics.DrawMeshInstanced(Mesh, 0, Mat, matrices3);
    }
}