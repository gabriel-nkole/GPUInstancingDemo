using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public class InstancingCPUWithJobs : MonoBehaviour {

    [SerializeField]
    Mesh Mesh;

    [SerializeField]
    Material Mat;

    [SerializeField, Range(1, 300)]
    int Resolution = 11;

    Matrix4x4[] matrices;  
    Matrix4x4[] matrices2; 
    Matrix4x4[] matrices3;


    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    struct GrassTransformJob : IJobParallelFor {
        public int Resolution;
        public Quaternion rotation0;
        public Quaternion rotation1;
        public Quaternion rotation2;
        public Vector3 scale;
        public Matrix4x4 parentToWorld;

        [WriteOnly]
        public NativeArray<Matrix4x4> matricesBuffer;

        [WriteOnly]
        public NativeArray<Matrix4x4> matrices2Buffer;

        [WriteOnly]
        public NativeArray<Matrix4x4> matrices3Buffer;
        

        public void Execute(int i) {
            int x = i % Resolution;
            int z = i / Resolution;

            Vector3 translation = new Vector3(x - 0.5f * (Resolution - 1f), 0.5f, z - 0.5f * (Resolution - 1f));

            matricesBuffer[i] = parentToWorld * Matrix4x4.TRS(translation, rotation0, scale);
            matrices2Buffer[i] = parentToWorld * Matrix4x4.TRS(translation, rotation1, scale);
            matrices3Buffer[i] = parentToWorld * Matrix4x4.TRS(translation, rotation2, scale);
        }
    }


    void OnEnable() {
        matrices = new Matrix4x4[Resolution * Resolution];
        matrices2 = new Matrix4x4[Resolution * Resolution];
        matrices3 = new Matrix4x4[Resolution * Resolution];


        Quaternion rotation0 = Quaternion.identity;
        Quaternion rotation1 = Quaternion.Euler(0, 45, 0);
        Quaternion rotation2 = Quaternion.Euler(0, -45, 0);
        Vector3 scale = Vector3.one;
        Matrix4x4 parentToWorld = this.transform.localToWorldMatrix;

        NativeArray<Matrix4x4> matricesBuffer = new NativeArray<Matrix4x4>(Resolution*Resolution, Allocator.TempJob);
        NativeArray<Matrix4x4> matrices2Buffer = new NativeArray<Matrix4x4>(Resolution*Resolution, Allocator.TempJob);
        NativeArray<Matrix4x4> matrices3Buffer = new NativeArray<Matrix4x4>(Resolution*Resolution, Allocator.TempJob);

        JobHandle jobHandle = new GrassTransformJob { Resolution = Resolution, rotation0 = rotation0, rotation1 = rotation1, rotation2 = rotation2,
                                                      scale = scale, parentToWorld = parentToWorld,
                                                      matricesBuffer = matricesBuffer, matrices2Buffer = matrices2Buffer, matrices3Buffer = matrices3Buffer}
                              .Schedule(Resolution * Resolution, 1);
        jobHandle.Complete();

        
        matricesBuffer.CopyTo(matrices);
        matrices2Buffer.CopyTo(matrices2);
        matrices3Buffer.CopyTo(matrices3);

        matricesBuffer.Dispose();
        matrices2Buffer.Dispose();
        matrices3Buffer.Dispose();
    }

    void OnDisable() {
        matrices = null;
        matrices2 = null;
        matrices3 = null;
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
        }

        Graphics.DrawMeshInstanced(Mesh, 0, Mat, matrices);
        Graphics.DrawMeshInstanced(Mesh, 0, Mat, matrices2);
        Graphics.DrawMeshInstanced(Mesh, 0, Mat, matrices3);
    }
}
