// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Detail Buildings"
{
	Properties
	{
		_WallA("Wall A", 2D) = "white" {}
		_WallB("Wall B", 2D) = "white" {}
		_RoofA("Roof A", 2D) = "white" {}
		_RoofB("Roof B", 2D) = "white" {}
		_Windows("Windows", 2D) = "black" {}
		[HDR]_HouseLightColors("House Light Colors", Color) = (1,0.8590416,0.4292453,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _WallA;
		uniform float4 _WallA_ST;
		uniform sampler2D _WallB;
		uniform float4 _WallB_ST;
		uniform sampler2D _RoofA;
		uniform float4 _RoofA_ST;
		uniform sampler2D _RoofB;
		uniform float4 _RoofB_ST;
		uniform sampler2D _Windows;
		uniform float4 _HouseLightColors;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_WallA = i.uv_texcoord * _WallA_ST.xy + _WallA_ST.zw;
			float2 uv_WallB = i.uv_texcoord * _WallB_ST.xy + _WallB_ST.zw;
			float3 objToWorld5 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float simplePerlin2D6 = snoise( objToWorld5.xy );
			float clampResult10 = clamp( simplePerlin2D6 , 0.0 , 1.0 );
			float4 lerpResult9 = lerp( tex2D( _WallA, uv_WallA ) , tex2D( _WallB, uv_WallB ) , clampResult10);
			float2 uv_RoofA = i.uv_texcoord * _RoofA_ST.xy + _RoofA_ST.zw;
			float2 uv_RoofB = i.uv_texcoord * _RoofB_ST.xy + _RoofB_ST.zw;
			float4 lerpResult11 = lerp( tex2D( _RoofA, uv_RoofA ) , tex2D( _RoofB, uv_RoofB ) , clampResult10);
			float4 lerpResult3 = lerp( lerpResult9 , lerpResult11 , i.vertexColor);
			float simplePerlin2D15 = snoise( ( 1.0 - objToWorld5 ).xy );
			float clampResult16 = clamp( simplePerlin2D15 , 0.0 , 1.0 );
			float clampResult28 = clamp( ( clampResult16 * 15.0 ) , 3.0 , 6.0 );
			float2 temp_cast_2 = (clampResult28).xx;
			float2 uv_TexCoord20 = i.uv_texcoord * temp_cast_2;
			float4 temp_output_18_0 = ( tex2D( _Windows, uv_TexCoord20 ) * ( 1.0 - i.vertexColor ) );
			float4 temp_output_31_0 = ( lerpResult3 - ( temp_output_18_0 * float4( 0.490566,0.490566,0.490566,0 ) ) );
			o.Albedo = temp_output_31_0.rgb;
			o.Emission = ( ( clampResult10 * temp_output_18_0 ) * _HouseLightColors ).rgb;
			o.Smoothness = temp_output_31_0.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
319;650;1165;531;568.3925;-80.55852;1;True;True
Node;AmplifyShaderEditor.TransformPositionNode;5;-1489.571,200.8549;Float;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;14;-1233.653,348.82;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;15;-1027.522,332.2885;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;16;-798.2008,326.8549;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-459.6772,330.5967;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;15;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;28;-56.15485,345.8548;Float;False;3;0;FLOAT;0;False;1;FLOAT;3;False;2;FLOAT;6;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;4;-312.9063,737.8687;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;6;-1046.514,194.1632;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;20;312.3316,298.8215;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;10;-818.0361,191.507;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-829.4943,464.5364;Float;True;Property;_RoofA;Roof A;2;0;Create;True;0;0;False;0;None;d2edc357e27a1a64492fb60a3b810a72;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;17;611.3208,296.4475;Float;True;Property;_Windows;Windows;4;0;Create;True;0;0;False;0;None;3ba7305ebfc8fae4fa4e177e6aaf0780;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;12;-839.0237,680.7699;Float;True;Property;_RoofB;Roof B;3;0;Create;True;0;0;False;0;None;a97c458f3a1305d49af2e5c957d66fda;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;8;-783.3444,-32.83757;Float;True;Property;_WallB;Wall B;1;0;Create;True;0;0;False;0;None;3afdf5d5477aa2b42a40af41c5cd0652;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-783.8378,-224.0543;Float;True;Property;_WallA;Wall A;0;0;Create;True;0;0;False;0;None;4117f8ac6ad764041a9a2a0e82af0583;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;19;604.3132,654.9037;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;11;-454.6669,584.2567;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;961.8666,365.6511;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;9;-401.5734,-115.1787;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;3;-173.7543,86.68995;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;1116.704,269.622;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.490566,0.490566,0.490566,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;1156.161,450.767;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;35;938.006,653.6609;Float;False;Property;_HouseLightColors;House Light Colors;5;1;[HDR];Create;True;0;0;False;0;1,0.8590416,0.4292453,0;1.498039,0.5882353,0.1411765,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;31;1230.657,127.98;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;1298.154,535.8262;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1518.397,109.4364;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Detail Buildings;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;5;0
WireConnection;15;0;14;0
WireConnection;16;0;15;0
WireConnection;21;0;16;0
WireConnection;28;0;21;0
WireConnection;6;0;5;0
WireConnection;20;0;28;0
WireConnection;10;0;6;0
WireConnection;17;1;20;0
WireConnection;19;0;4;0
WireConnection;11;0;2;0
WireConnection;11;1;12;0
WireConnection;11;2;10;0
WireConnection;18;0;17;0
WireConnection;18;1;19;0
WireConnection;9;0;1;0
WireConnection;9;1;8;0
WireConnection;9;2;10;0
WireConnection;3;0;9;0
WireConnection;3;1;11;0
WireConnection;3;2;4;0
WireConnection;32;0;18;0
WireConnection;29;0;10;0
WireConnection;29;1;18;0
WireConnection;31;0;3;0
WireConnection;31;1;32;0
WireConnection;33;0;29;0
WireConnection;33;1;35;0
WireConnection;0;0;31;0
WireConnection;0;2;33;0
WireConnection;0;4;31;0
ASEEND*/
//CHKSM=3A1026F3E954082650D3498A0EEDE14FDA4B4AA8