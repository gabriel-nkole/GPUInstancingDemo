using UnityEngine;

public class InstancingCPU : MonoBehaviour {
    [SerializeField]
    Mesh Mesh;

    [SerializeField]
    Material Mat;

    [SerializeField, Range(1, 300)]
    int Resolution = 11;

    Matrix4x4[] matrices;
    Matrix4x4[] matrices2;
    Matrix4x4[] matrices3;


    void OnEnable() { 
        matrices = new Matrix4x4[Resolution*Resolution];
        matrices2 = new Matrix4x4[Resolution*Resolution];
        matrices3 = new Matrix4x4[Resolution*Resolution];

        Matrix4x4 parentToWorld = this.transform.localToWorldMatrix;
        Vector3 scale = Vector3.one;
        Quaternion[] rotations = { Quaternion.identity, Quaternion.Euler(0, 45, 0), Quaternion.Euler(0, -45, 0)};
        for (int i = 0; i < Resolution*Resolution; i++) {
            float x = i % Resolution;
            float z = i / Resolution;

            Vector3 translation = new Vector3(x - 0.5f * (Resolution - 1f), 0.5f, z - 0.5f * (Resolution - 1f));

            matrices[i] = parentToWorld * Matrix4x4.TRS(translation, rotations[0], scale);
            matrices2[i] = parentToWorld * Matrix4x4.TRS(translation, rotations[1], scale);
            matrices3[i] = parentToWorld * Matrix4x4.TRS(translation, rotations[2], scale);
        }
    }

    void OnDisable() {
        matrices3 = null;
        matrices2 = null;
        matrices = null;
    }

    void OnValidate() {
        if (matrices != null & enabled) {
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