Shader "Custom/LowPolyWaterSimple" {
     Properties {
        _BaseColor ("Base Color", Color) = (0.2, 0.5, 0.8, 1)
        _WaveColor ("Wave Color", Color) = (0.1, 0.3, 0.6, 1)
        _WaveHeight ("Wave Height", Range(0, 2)) = 0.5
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 1.0
        _WaveFrequency ("Wave Frequency", Range(0, 10)) = 1.5
        _Sharpness ("Wave Sharpness", Range(0.1, 5)) = 2.0
        _SpecularPower ("Specular Power", Range(0, 1)) = 0.5
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
        
        struct Input {
            float3 worldPos;
            float waveSlope;
            float3 viewDir;
        };
        
        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            
            // Создаем острые low-poly волны
            float waveX = sin(worldPos.x * _WaveFrequency + _Time.y * _WaveSpeed);
            float waveZ = cos(worldPos.z * _WaveFrequency * 0.7 + _Time.y * _WaveSpeed * 0.9);
            
            // Комбинируем волны для создания ромбовидной структуры
            float combinedWave = (waveX + waveZ) * 0.5;
            
            // Увеличиваем резкость волн
            combinedWave = pow(combinedWave * 0.5 + 0.5, _Sharpness) * 2.0 - 1.0;
            
            v.vertex.y += combinedWave * _WaveHeight;
            
            // Рассчитываем наклон для отблесков
            o.waveSlope = length(float2(waveX, waveZ)) * 0.5;
            o.worldPos = worldPos;
        }
        
        void surf(Input IN, inout SurfaceOutput o) {
            // Цвет основывается на наклоне волны
            fixed4 color = lerp(_BaseColor, _WaveColor, IN.waveSlope);
            
            o.Albedo = color.rgb;
            o.Alpha = color.a;
            o.Specular = _SpecularPower;
            o.Gloss = IN.waveSlope * _SpecularPower;
        }
        ENDCG
    }
    FallBack "Diffuse"
}