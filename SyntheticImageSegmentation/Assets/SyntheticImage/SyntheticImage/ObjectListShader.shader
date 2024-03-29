﻿Shader "AAI/ObjectListShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			uniform float _ObjectIdR;
			
			/*uniform float _ObjectIdG;
			uniform float _ObjectIdB;
			uniform float _ObjectIdA; */

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(_ObjectIdR, 0, 0, 0);
            }
            ENDCG
        }
    }
}
