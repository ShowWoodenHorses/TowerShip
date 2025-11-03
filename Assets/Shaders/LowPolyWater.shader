Shader "Custom/LowPolyWaterFoamShoreline"
{
    Properties
    {
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
        _FoamThickness ("Foam Width", Range(0.01, 2)) = 0.3
        _FoamNoiseScale ("Foam Noise Scale", Range(0.1, 5)) = 1.5
        _FoamNoiseStrength ("Foam Noise Strength", Range(0,1)) = 0.4
    }

    SubShader
    {
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

        // --- FOAM ARRAYS ---
        #define MAX_FOAM_ARRAY 32
        float4 _FoamPosArray[MAX_FOAM_ARRAY];
        float4 _FoamNormalArray[MAX_FOAM_ARRAY];
        int _FoamCount;

        fixed4 _FoamColor;
        float _FoamGlobalIntensity;
        float _FoamThickness;
        float _FoamNoiseScale;
        float _FoamNoiseStrength;

        struct Input
        {
            float3 worldPos;
            float waveSlope;
        };

        // === SIMPLE NOISE ===
        float hash(float2 p)
        {
            return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
        }

        float noise(float2 p)
        {
            float2 i = floor(p);
            float2 f = frac(p);
            float a = hash(i);
            float b = hash(i + float2(1, 0));
            float c = hash(i + float2(0, 1));
            float d = hash(i + float2(1, 1));
            float2 u = f * f * (3.0 - 2.0 * f);
            return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
        }

        // === VERTEX WAVE DISPLACEMENT ===
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

            float waveX = sin(worldPos.x * _WaveFrequency + _Time.y * _WaveSpeed);
            float waveZ = cos(worldPos.z * _WaveFrequency * 0.7 + _Time.y * _WaveSpeed * 0.9);
            float combined = (waveX + waveZ) * 0.5;

            combined = pow(combined * 0.5 + 0.5, _Sharpness) * 2.0 - 1.0;

            v.vertex.y += combined * _WaveHeight;

            o.waveSlope = abs(waveX - waveZ) * 0.5;
            o.worldPos = worldPos;
        }

        // === SURFACE FUNCTION ===
        void surf(Input IN, inout SurfaceOutput o)
        {
            // Базовый цвет воды
            fixed4 waterColor = lerp(_BaseColor, _WaveColor, IN.waveSlope);

            // --- FOAM CALCULATION ---
            float foamMask = 0;

            for (int i = 0; i < _FoamCount; i++)
            {
                float3 foamPos = _FoamPosArray[i].xyz;
                float radius = _FoamPosArray[i].w;
                float3 normal = normalize(_FoamNormalArray[i].xyz);

                // Вектор от берега к текущей точке
                float3 dir = IN.worldPos - foamPos;

                // Расстояние вдоль нормали (вглубь воды)
                float distForward = dot(dir, normal);

                // Поперечное расстояние вдоль линии берега
                float lateralDist = length(dir - normal * distForward);

                // Только со стороны воды (в направлении нормали)
                if (distForward > 0)
                {
                    // Базовая маска толщины пены
                    float foamBand = smoothstep(radius, 0.0, distForward);

                    // Мягкий спад вдоль берега
                    float width = saturate(1.0 - (lateralDist / (radius * 2.0)));

                    // Шумовое искажение края
                    float n = noise(IN.worldPos.xz * _FoamNoiseScale + i * 15.3 + _Time.y * 0.5);
                    foamBand *= (0.7 + n * _FoamNoiseStrength);
                    foamBand *= width;

                    foamMask = max(foamMask, foamBand);
                }
            }

            // Итоговый цвет
            float foamIntensity = saturate(foamMask * _FoamGlobalIntensity);
            waterColor.rgb = lerp(waterColor.rgb, _FoamColor.rgb, foamIntensity);

            o.Albedo = waterColor.rgb;
            o.Alpha = 1.0;
            o.Specular = _SpecularPower;
            o.Gloss = IN.waveSlope * _SpecularPower;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
