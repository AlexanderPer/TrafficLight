Shader "TrafficLightShader"
{
    Properties
    {
		_ActiveColor("Active Color", Color) = (1, 1, 1, 1)
		_InactiveColor("Inactive Color", Color) = (0.2, 0.2, 0.2, 1)
		_BackColor("Back Color", Color) = (0, 0, 0, 1)
		[Toggle] _IsActive("Is Active", Float) = 0
		[Toggle] _IsBlink("Is Blink", Float) = 0		
		[Toggle] _IsDecor("Is Decor", Float) = 0
		[Toggle] _IsTimer("Is Timer", Float) = 0
		_TimerValue("Timer Value", Float) = 0
		_MainTex("Sprite Texture", 2D) = "black" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
			            
			fixed4 _ActiveColor;
			fixed4 _InactiveColor;
			fixed4 _BackColor;
			float _IsActive;
			float _IsBlink;
			float _IsDecor;
			float _IsTimer;
			float _TimerValue;

			uint D(float2 p, float n) {
				uint i = uint(p.y), b = uint(exp2(floor(30. - p.x - n * 3.)));
				i = (p.x < 0. || p.x > 3. ? 0 :
					i == 5 ? 972980223 : i == 4 ? 690407533 : i == 3 ? 704642687 : i == 2 ? 696556137 : i == 1 ? 972881535 : 0) / b;
				return i - i / 2 * 2;
			}

			float Number(float2 uv, float n)
			{
				uv.x *= 15.;
				uv.y *= 7.;
				
				float n1 = 9.;
				float n2 = 9.;

				if (n < 100.)
				{
					n1 = (n > 9.) ? floor(n / 10.) : 0.;
					n2 = floor(fmod(n, 10.));
				}
				if ((uv.x -= 4.) < 3.) { return D(uv, n1); }
				if ((uv.x -= 4.) < 3.) { return D(uv, n2); }
				return 0.;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = _BackColor;
				float radius = 0.5;
				float center = 0.5;
												
				if (length(i.uv - center) < radius)
				{
					float flashActive = 1.0;
					if (_IsBlink > 0.5)					
						flashActive = fmod(_Time.y, 1.0);

					col = _InactiveColor;					

					if (_IsActive < 0.5)
						return col;

					if (_IsTimer > 0.5)
					{
						if (Number(i.uv, _TimerValue) > 0.5)
							col = _ActiveColor;
						return col;
					}

					if (flashActive > 0.5)
					{
						if (_IsDecor > 0.5)
						{
							float si = 0.707106;
							float co = 0.707106;
							float2x2 m = float2x2(co, si, -si, co);
							float2 rot = abs(mul(m, i.uv));
							float2 rem = fmod(rot, 0.04);
							if ((rem.x < 0.01) || (rem.y < 0.01))
								col = _ActiveColor;
						}
						else
							col = _ActiveColor;
					}
				}
                return col;
            }
            ENDCG
        }
    }
}
