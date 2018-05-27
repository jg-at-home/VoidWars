// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:True,atwp:True,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:35579,y:32629,varname:node_1873,prsc:2|emission-6945-OUT,alpha-8964-A;n:type:ShaderForge.SFN_Tex2d,id:4805,x:32532,y:32259,varname:_MainTex_copy,prsc:2,ntxv:0,isnm:False|UVIN-7607-OUT,TEX-9747-TEX;n:type:ShaderForge.SFN_Multiply,id:1086,x:32945,y:32924,cmnt:RGB,varname:node_1086,prsc:2|A-1925-RGB,B-5983-RGB;n:type:ShaderForge.SFN_Color,id:5983,x:32239,y:32863,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Code,id:5645,x:31319,y:32192,varname:node_5645,prsc:2,code:ZgBsAG8AYQB0ACAAcgBlAHQAIAA9ACAAMAA7AA0ACgBmAGwAbwBhAHQAIABpAHQAZQByAGEAdABpAG8AbgBzACAAPQAgADIAOwANAAoAZgBvAHIAIAAoAGYAbABvAGEAdAAgAGkAIAA9ACAAMAA7ACAAaQAgADwAIABpAHQAZQByAGEAdABpAG8AbgBzADsAIAArACsAaQApAA0ACgB7AA0ACgAJAGYAbABvAGEAdAAyACAAcAAgAD0AIABmAGwAbwBvAHIAKABVAFYAIAAqACAAKABpACsAMQApACkAOwANAAoACQBmAGwAbwBhAHQAMgAgAGYAIAA9ACAAZgByAGEAYwAoAFUAVgAgACoAIAAoAGkAKwAxACkAKQA7AA0ACgAJAGYAIAA9ACAAZgAgACoAIABmACAAKgAgACgAMwAuADAAIAAtACAAMgAuADAAIAAqACAAZgApADsADQAKAAkAZgBsAG8AYQB0ACAAbgAgAD0AIABwAC4AeAAgACsAIABwAC4AeQAgACoAIAA1ADcALgAwADsADQAKAAkAZgBsAG8AYQB0ADQAIABuAG8AaQBzAGUAIAA9ACAAZgBsAG8AYQB0ADQAKABuACwAIABuACAAKwAgADEALAAgAG4AIAArACAANQA3AC4AMAAsACAAbgAgACsAIAA1ADgALgAwACkAOwANAAoACQBuAG8AaQBzAGUAIAA9ACAAZgByAGEAYwAoAHMAaQBuACgAbgBvAGkAcwBlACkAKgA0ADMANwAuADUAOAA1ADQANQAzACkAOwANAAoACQByAGUAdAAgACsAPQAgAGwAZQByAHAAKABsAGUAcgBwACgAbgBvAGkAcwBlAC4AeAAsACAAbgBvAGkAcwBlAC4AeQAsACAAZgAuAHgAKQAsACAAbABlAHIAcAAoAG4AbwBpAHMAZQAuAHoALAAgAG4AbwBpAHMAZQAuAHcALAAgAGYALgB4ACkALAAgAGYALgB5ACkAIAAqACAAKAAgAGkAdABlAHIAYQB0AGkAbwBuAHMAIAAvACAAKABpACsAMQApACkAOwANAAoAfQANAAoAcgBlAHQAdQByAG4AIAByAGUAdAAvAGkAdABlAHIAYQB0AGkAbwBuAHMAOwA=,output:0,fname:noise,width:621,height:316,input:1,input_1_label:UV|A-2431-OUT;n:type:ShaderForge.SFN_ScreenPos,id:4759,x:29947,y:31995,varname:node_4759,prsc:2,sctp:2;n:type:ShaderForge.SFN_ValueProperty,id:7473,x:30459,y:32388,ptovrint:False,ptlb:NoiseTiling,ptin:_NoiseTiling,varname:_NoiseTiling,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_ValueProperty,id:5314,x:30092,y:31862,ptovrint:False,ptlb:UVAspectRatio,ptin:_UVAspectRatio,varname:_UVAspectRatio,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.7;n:type:ShaderForge.SFN_Multiply,id:883,x:30304,y:31921,varname:node_883,prsc:2|A-5314-OUT,B-4759-U;n:type:ShaderForge.SFN_Append,id:5478,x:30665,y:32095,varname:node_5478,prsc:2|A-883-OUT,B-3100-OUT;n:type:ShaderForge.SFN_Multiply,id:2431,x:31116,y:32165,varname:node_2431,prsc:2|A-4572-OUT,B-7473-OUT;n:type:ShaderForge.SFN_RemapRange,id:4572,x:30870,y:31988,varname:node_4572,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-5478-OUT;n:type:ShaderForge.SFN_TexCoord,id:610,x:31909,y:32541,varname:node_610,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:7607,x:32335,y:32176,varname:node_7607,prsc:2|A-9946-OUT,B-610-UVOUT;n:type:ShaderForge.SFN_Add,id:3713,x:34865,y:32575,varname:node_3713,prsc:2|A-1086-OUT,B-3836-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:9747,x:32220,y:32569,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1925,x:32517,y:32634,varname:node_1925,prsc:2,ntxv:0,isnm:False|TEX-9747-TEX;n:type:ShaderForge.SFN_RemapRange,id:9946,x:32064,y:32058,varname:node_9946,prsc:2,frmn:0,frmx:1,tomn:-0.004,tomx:0.004|IN-5645-OUT;n:type:ShaderForge.SFN_Time,id:858,x:29895,y:32295,varname:node_858,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9257,x:30131,y:32295,varname:node_9257,prsc:2|A-858-T,B-5870-OUT;n:type:ShaderForge.SFN_Add,id:3100,x:30494,y:32150,varname:node_3100,prsc:2|A-4759-V,B-9257-OUT;n:type:ShaderForge.SFN_Multiply,id:5728,x:33155,y:32432,varname:node_5728,prsc:2|A-1132-OUT,B-1925-A;n:type:ShaderForge.SFN_Lerp,id:5135,x:34181,y:31969,varname:node_5135,prsc:2|A-8996-OUT,B-2261-RGB,T-766-OUT;n:type:ShaderForge.SFN_Vector3,id:8996,x:33711,y:31792,varname:node_8996,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_Lerp,id:5543,x:34387,y:32185,varname:node_5543,prsc:2|A-5135-OUT,B-5027-OUT,T-1485-OUT;n:type:ShaderForge.SFN_Multiply,id:9086,x:33636,y:32287,varname:node_9086,prsc:2|A-5728-OUT,B-9175-OUT;n:type:ShaderForge.SFN_Vector1,id:9175,x:33421,y:32420,varname:node_9175,prsc:2,v1:5;n:type:ShaderForge.SFN_Clamp01,id:766,x:33872,y:32231,varname:node_766,prsc:2|IN-9086-OUT;n:type:ShaderForge.SFN_Subtract,id:3405,x:33905,y:32415,varname:node_3405,prsc:2|A-9086-OUT,B-7551-OUT;n:type:ShaderForge.SFN_Vector1,id:7551,x:33687,y:32480,varname:node_7551,prsc:2,v1:1;n:type:ShaderForge.SFN_Clamp01,id:1485,x:34107,y:32415,varname:node_1485,prsc:2|IN-3405-OUT;n:type:ShaderForge.SFN_Vector3,id:5027,x:33759,y:32067,varname:node_5027,prsc:2,v1:1,v2:1,v3:1;n:type:ShaderForge.SFN_Color,id:2261,x:33524,y:31961,ptovrint:False,ptlb:GlowColor,ptin:_GlowColor,varname:_GlowColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.7931032,c3:1,c4:1;n:type:ShaderForge.SFN_Code,id:8229,x:31366,y:31718,varname:node_8229,prsc:2,code:ZgBsAG8AYQB0ACAAcgBlAHQAIAA9ACAAMAA7AA0ACgBmAGwAbwBhAHQAIABpAHQAZQByAGEAdABpAG8AbgBzACAAPQAgADIAOwANAAoAZgBvAHIAIAAoAGYAbABvAGEAdAAgAGkAIAA9ACAAMAA7ACAAaQAgADwAIABpAHQAZQByAGEAdABpAG8AbgBzADsAIAArACsAaQApAA0ACgB7AA0ACgAJAGYAbABvAGEAdAAyACAAcAAgAD0AIABmAGwAbwBvAHIAKABVAFYAIAAqACAAKABpACsAMQApACkAOwANAAoACQBmAGwAbwBhAHQAMgAgAGYAIAA9ACAAZgByAGEAYwAoAFUAVgAgACoAIAAoAGkAKwAxACkAKQA7AA0ACgAJAGYAIAA9ACAAZgAgACoAIABmACAAKgAgACgAMwAuADAAIAAtACAAMgAuADAAIAAqACAAZgApADsADQAKAAkAZgBsAG8AYQB0ACAAbgAgAD0AIABwAC4AeAAgACsAIABwAC4AeQAgACoAIAA1ADcALgAwADsADQAKAAkAZgBsAG8AYQB0ADQAIABuAG8AaQBzAGUAIAA9ACAAZgBsAG8AYQB0ADQAKABuACwAIABuACAAKwAgADEALAAgAG4AIAArACAANQA3AC4AMAAsACAAbgAgACsAIAA1ADgALgAwACkAOwANAAoACQBuAG8AaQBzAGUAIAA9ACAAZgByAGEAYwAoAHMAaQBuACgAbgBvAGkAcwBlACkAKgA0ADMANwAuADUAOAA1ADQANQAzACkAOwANAAoACQByAGUAdAAgACsAPQAgAGwAZQByAHAAKABsAGUAcgBwACgAbgBvAGkAcwBlAC4AeAAsACAAbgBvAGkAcwBlAC4AeQAsACAAZgAuAHgAKQAsACAAbABlAHIAcAAoAG4AbwBpAHMAZQAuAHoALAAgAG4AbwBpAHMAZQAuAHcALAAgAGYALgB4ACkALAAgAGYALgB5ACkAIAAqACAAKAAgAGkAdABlAHIAYQB0AGkAbwBuAHMAIAAvACAAKABpACsAMQApACkAOwANAAoAfQANAAoAcgBlAHQAdQByAG4AIAByAGUAdAAvAGkAdABlAHIAYQB0AGkAbwBuAHMAOwA=,output:0,fname:noise2,width:514,height:316,input:1,input_1_label:UV|A-2956-OUT;n:type:ShaderForge.SFN_Multiply,id:2956,x:31129,y:31905,varname:node_2956,prsc:2|A-4572-OUT,B-2086-OUT;n:type:ShaderForge.SFN_Divide,id:2086,x:30805,y:32411,varname:node_2086,prsc:2|A-7473-OUT,B-632-OUT;n:type:ShaderForge.SFN_Vector1,id:632,x:30537,y:32637,varname:node_632,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:1132,x:32846,y:32192,varname:node_1132,prsc:2|A-4031-OUT,B-4805-A;n:type:ShaderForge.SFN_Subtract,id:1236,x:32162,y:31786,varname:node_1236,prsc:2|A-8229-OUT,B-9015-OUT;n:type:ShaderForge.SFN_Vector1,id:9015,x:31960,y:31894,varname:node_9015,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Clamp01,id:4031,x:32343,y:31653,varname:node_4031,prsc:2|IN-1236-OUT;n:type:ShaderForge.SFN_VertexColor,id:8964,x:34179,y:32870,varname:node_8964,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:5870,x:29848,y:32543,ptovrint:False,ptlb:NoiseScrollSpeed,ptin:_NoiseScrollSpeed,varname:_NoiseScrollSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.05;n:type:ShaderForge.SFN_Multiply,id:3836,x:34627,y:32491,varname:node_3836,prsc:2|A-5543-OUT,B-8964-A;n:type:ShaderForge.SFN_Multiply,id:6945,x:35335,y:32699,varname:node_6945,prsc:2|A-3713-OUT,B-8964-RGB;proporder:5983-2261-7473-5870-5314-9747;pass:END;sub:END;*/

Shader "VFX/UI/PlayerHandArea" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _GlowColor ("GlowColor", Color) = (0,0.7931032,1,1)
        _NoiseTiling ("NoiseTiling", Float ) = 10
        _NoiseScrollSpeed ("NoiseScrollSpeed", Float ) = 0.05
        _UVAspectRatio ("UVAspectRatio", Float ) = 1.7
        _MainTex ("MainTex", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        _Stencil ("Stencil ID", Float) = 0
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilOpFail ("Stencil Fail Operation", Float) = 0
        _StencilOpZFail ("Stencil Z-Fail Operation", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            Stencil {
                Ref [_Stencil]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilOp]
                Fail [_StencilOpFail]
                ZFail [_StencilOpZFail]
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float4 _Color;
            float noise( float2 UV ){
            float ret = 0;
            float iterations = 2;
            for (float i = 0; i < iterations; ++i)
            {
            	float2 p = floor(UV * (i+1));
            	float2 f = frac(UV * (i+1));
            	f = f * f * (3.0 - 2.0 * f);
            	float n = p.x + p.y * 57.0;
            	float4 noise = float4(n, n + 1, n + 57.0, n + 58.0);
            	noise = frac(sin(noise)*437.585453);
            	ret += lerp(lerp(noise.x, noise.y, f.x), lerp(noise.z, noise.w, f.x), f.y) * ( iterations / (i+1));
            }
            return ret/iterations;
            }
            
            uniform float _NoiseTiling;
            uniform float _UVAspectRatio;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _GlowColor;
            float noise2( float2 UV ){
            float ret = 0;
            float iterations = 2;
            for (float i = 0; i < iterations; ++i)
            {
            	float2 p = floor(UV * (i+1));
            	float2 f = frac(UV * (i+1));
            	f = f * f * (3.0 - 2.0 * f);
            	float n = p.x + p.y * 57.0;
            	float4 noise = float4(n, n + 1, n + 57.0, n + 58.0);
            	noise = frac(sin(noise)*437.585453);
            	ret += lerp(lerp(noise.x, noise.y, f.x), lerp(noise.z, noise.w, f.x), f.y) * ( iterations / (i+1));
            }
            return ret/iterations;
            }
            
            uniform float _NoiseScrollSpeed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
////// Lighting:
////// Emissive:
                float4 node_1925 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_858 = _Time;
                float2 node_4572 = (float2((_UVAspectRatio*sceneUVs.r),(sceneUVs.g+(node_858.g*_NoiseScrollSpeed)))*2.0+-1.0);
                float2 node_7607 = ((noise( (node_4572*_NoiseTiling) )*0.008+-0.004)+i.uv0);
                float4 _MainTex_copy = tex2D(_MainTex,TRANSFORM_TEX(node_7607, _MainTex));
                float node_9086 = (((saturate((noise2( (node_4572*(_NoiseTiling/2.0)) )-0.5))*_MainTex_copy.a)*node_1925.a)*5.0);
                float3 node_3713 = ((node_1925.rgb*_Color.rgb)+(lerp(lerp(float3(0,0,0),_GlowColor.rgb,saturate(node_9086)),float3(1,1,1),saturate((node_9086-1.0)))*i.vertexColor.a));
                float3 emissive = (node_3713*i.vertexColor.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,i.vertexColor.a);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
