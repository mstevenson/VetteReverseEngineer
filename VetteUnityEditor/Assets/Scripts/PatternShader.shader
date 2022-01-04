// https://www.ronja-tutorials.com/post/039-screenspace-texture/
// https://github.com/ronja-tutorials/ShaderTutorials/blob/master/Assets/039_Screenspace_Textures/UnlitScreenspaceTextures.shader

Shader "Vette/Pattern Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TexelSize ("Texel Size", Int) = 4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Cull off
        
        Pass
        {
            CGPROGRAM
            
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            int _TexelSize;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float4 screenPosition : TEXCOORD0;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                //convert the vertex positions from object space to clip space so they can be rendered
                o.position = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.position);
                return o;
            }

            //the fragment shader
            fixed4 frag(v2f i) : SV_TARGET
            {
                float2 textureCoordinate = i.screenPosition.xy / i.screenPosition.w;
                textureCoordinate /= (_MainTex_TexelSize.zw / _ScreenParams.xy) * _TexelSize;
                //float aspect = _ScreenParams.x / _ScreenParams.y;
                //textureCoordinate.x = textureCoordinate.x * aspect;
                textureCoordinate = TRANSFORM_TEX(textureCoordinate, _MainTex);
                fixed4 col = tex2D(_MainTex, textureCoordinate);
                return col;
            }
            
            ENDCG
        }
    }
}