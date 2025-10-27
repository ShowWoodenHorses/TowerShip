Shader "Custom/LowPolyWaterFoamRing" {
    Properties {
        _BaseColor ("Base Color", Color) = (0.2, 0.5, 0.8, 1)
        _WaveColor ("Wave Color", Color) = (0.1, 0.3, 0.6, 1)
        _WaveHeight ("Wave Height", Range(0, 2)) = 0.5
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 1.0
        _WaveFrequency ("Wave Frequency", Range(0, 10)) = 1.5
        _Sharpness ("Wave Sharpness", Range(0.1, 5)) = 2.0
        _SpecularPower ("Specular Power", Range(0, 1)) = 0.5

        // --- FOAM SETTINGS ---
        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _FoamGlobalIntensity ("Foam Intensity", Range(0,1)) = 1.0
        _FoamThickness ("Foam Ring Thickness", Range(0.01, 1)) = 0.25
        _FoamSmoothness ("Foam Edge Smoothness", Range(0.01, 1)) = 0.2
        _FoamNoiseScale ("Foam Noise Scale", Range(0.1, 5)) = 1.5
        _FoamNoiseStrength ("Foam Noise Strength", Range(0,1)) = 0.3
    }
    
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert addshadow
        #pragma target 3.0
        
        fixed4 _BaseColor;
        fixed4 _WaveColor;
        float _WaveHeight;
        float _WaveSpeed;
        float _WaveFrequency;
        float _Sharpness;
        float _SpecularPower;

        // --- FOAM DATA ---
        #define MAX_FOAM_ARRAY 32
        float4 _FoamPosArray[MAX_FOAM_ARRAY];
        int _FoamCount;
        fixed4 _FoamColor;
        float _FoamGlobalIntensity;
        float _FoamThickness;
        float _FoamSmoothness;
        float _FoamNoiseScale;
        float _FoamNoiseStrength;
        
        struct Input {
            float3 worldPos;
            float waveSlope;
            float3 viewDir;
        };

        // Простой 2D-шум
        float hash(float2 p) { return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453); }
        float noise(float2 p) {
            float2 i = floor(p);
            float2 f = frac(p);
            float a = hash(i);
            float b = hash(i + float2(1, 0));
            float c = hash(i + float2(0, 1));
            float d = hash(i + float2(1, 1));
            float2 u = f * f * (3.0 - 2.0 * f);
            return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
        }
        
        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            
            float waveX = sin(worldPos.x * _WaveFrequency + _Time.y * _WaveSpeed);
            float waveZ = cos(worldPos.z * _WaveFrequency * 0.7 + _Time.y * _WaveSpeed * 0.9);
            float combinedWave = (waveX + waveZ) * 0.5;
            combinedWave = pow(combinedWave * 0.5 + 0.5, _Sharpness) * 2.0 - 1.0;
            
            v.vertex.y += combinedWave * _WaveHeight;
            
            o.waveSlope = length(float2(waveX, waveZ)) * 0.5;
            o.worldPos = worldPos;
        }
        
        void surf(Input IN, inout SurfaceOutput o) {
        fixed4 color = lerp(_BaseColor, _WaveColor, IN.waveSlope);

        float foamMask = 0;
        for (int i = 0; i < _FoamCount; i++) {
            float3 foamPos = _FoamPosArray[i].xyz;
            float radius = _FoamPosArray[i].w;

        // "Дышащая" анимация радиуса
        float timeShift = sin(_Time.y * 0.7 + i * 1.3) * 0.3;
        float animatedRadius = radius * (1.0 + timeShift * 0.05);

        float dist = distance(IN.worldPos.xz, foamPos.xz);
        float edgeDist = abs(dist - animatedRadius);

        // Движущийся шум
        float n = noise(IN.worldPos.xz * _FoamNoiseScale + i * 10.0 + _Time.y * 0.5);
        edgeDist += (n - 0.5) * _FoamNoiseStrength * radius;

        // Толстое кольцо пены
        float ring = smoothstep(_FoamThickness, 0.0, edgeDist / radius);
        ring *= smoothstep(1.0, 0.0, edgeDist / (radius * _FoamSmoothness));

        foamMask = max(foamMask, ring);
    }

    color.rgb = lerp(color.rgb, _FoamColor.rgb, foamMask * _FoamGlobalIntensity);

    o.Albedo = color.rgb;
    o.Alpha = color.a;
    o.Specular = _SpecularPower;
    o.Gloss = IN.waveSlope * _SpecularPower;
}
        ENDCG
    }
    FallBack "Diffuse"
}
