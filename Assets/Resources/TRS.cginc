#define PI 3.14159265359
#define Deg2Rad PI/180

float4x4 Scale(float3 scale) {
    return float4x4(scale.x,    0,         0,       0,
                    0,          scale.y,   0,       0,
                    0,          0,         scale.z, 0,
                    0,          0,         0,       1);
}

float4x4 Rotate(float yDeg){
    float yRad = Deg2Rad * yDeg;

    return float4x4( cos(yRad), 0, sin(yRad),       0,
                     0,         1,         0,       0,
                    -sin(yRad), 0, cos(yRad),       0,
                     0,         0,         0,       1);
}

float4x4 Translate(float3 position) {
    return float4x4(1,          0,         0, position.x,
                    0,          1,         0, position.y,
                    0,          0,         1, position.z,
                    0,          0,         0, 1);
} 