// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/CrafterSeries/ArmoryCrafterTexture"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
		
		_BlendMode("Blend Mode", Int) = 0

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha, One One
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
			int _BlendMode;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

			float Lum(float4 C)
			{
				float lum = 0.3*C.r + 0.59*C.g + 0.11*C.b;
				
				return lum;
			}
			
			
			
			float4 ClipColor(float4 C)
			{
				float l = Lum(C);
				float n = min(C.r, min(C.g, C.b));
				float x = max(C.r, max(C.g, C.b));
				
				if(n < 0.0)
				{
					C.r = l + (((C.r-l)*l)/(l-n));
					C.g = l + (((C.g-l)*l)/(l-n));
					C.b = l + (((C.b-l)*l)/(l-n));
				}
				
				if(x > 1.0)
				{
					C.r = l + (((C.r-l)*(1-l))/(x-l));
					C.g = l + (((C.g-l)*(1-l))/(x-l));
					C.b = l + (((C.b-l)*(1-l))/(x-l));
				}
				
				
				return C;
			}	
			
			float4 SetLum(float4 C, float l)
			{
				float d = l - Lum(C);
				C.r = C.r + d;
				C.g = C.g + d;
				C.b = C.b + d;
				
				
				return ClipColor(C);
			}
			
			
			float4 Multiply(float4 cB, float4 cS)
			{
				float4 newColor = cB * cS;
				return newColor;
			}
			
			float4 Screen(float4 cB, float4 cS)
			{
				float4 newColor = 1-((1-cB)*(1-cS));
				newColor.a = cB.a;
				return newColor;
			}
			
			float4 HardLight(float4 cB, float4 cS)
			{
				float maxValue = max(cS.r,max(cS.g, cS.b));
				float4 newColor;
				if(maxValue <= 0.5)
				{
					newColor = Multiply(cB, 2*cS);
				
				}else{
					
					newColor = Screen(cB, 2*cS-1);
				}
				
				return newColor;
			}
			
            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color;
				float aA;
				
				switch(_BlendMode)
				{
					case 0: //Multiply
						//color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
						color = Multiply((tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd), IN.color);
						break;
					case 1: //Screen
						aA = ((tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd)*IN.color).a;
						
						color = Screen((tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd), IN.color);
						
						color.a = aA;
						
						break;
					case 2: //Overlay
					
					
						aA = ((tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd)*IN.color).a;
						
						IN.worldPosition /= IN.worldPosition.w;
						half4 base = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);
						half gray = (base.r + base.g + base.b) / 3;
						half4 overlay = IN.color;
		 
						float4 effect = lerp(1 - (2 * (1 - base)) * (1 - overlay), (2 * base) * overlay, step(base, 0.5f));
				   
						color = lerp(base, effect, (overlay.w * 0.66));
						
						color.a = aA;
					/*
						aA = ((tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd)*IN.color).a;
					
						color = HardLight(IN.color, (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd));
						
						color.a = aA;
					*/
						break;
						
					case 3: //Color
	
						aA = ((tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd)*IN.color).a;
						
						color = SetLum(IN.color, Lum((tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd)));
						
						color.a = aA;
						
						break;
					default:
						aA = ((tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd)*IN.color).a;
						color = IN.color;
						color.a = aA;
					break;
					/*case X: //Luminosity
				
						color = SetLum((tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd), Lum(IN.color));
						
						break;
					*/
					
				}
				
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}