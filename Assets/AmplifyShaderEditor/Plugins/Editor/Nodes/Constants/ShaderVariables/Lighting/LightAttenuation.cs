// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Light Attenuation", "Light", "Contains light attenuation for all types of light", NodeAvailabilityFlags = (int)( NodeAvailability.CustomLighting | NodeAvailability.TemplateShader ) )]
	public sealed class LightAttenuation : ParentNode
	{
		static readonly string SurfaceError = "This node only returns correct information using a custom light model, otherwise returns 1";
		static readonly string TemplateError = "This node will only produce proper attenuation if the template contains a shadow caster pass";

		private const string ASEAttenVarName = "ase_lightAtten";

		private readonly string[] LightweightPragmaMultiCompiles =
		{
			"multi_compile _ _MAIN_LIGHT_SHADOWS",
			"multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE",
			"multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS",
			"multi_compile _ _ADDITIONAL_LIGHT_SHADOWS",
			"multi_compile _ _SHADOWS_SOFT"
		};

		private readonly string[] LightweightVertexInstructions =
		{
			/*local vertex position*/"VertexPositionInputs ase_vertexInput = GetVertexPositionInputs ({0});",
			"#ifdef _MAIN_LIGHT_SHADOWS//ase_lightAtten_vert",
			/*available interpolator*/"{0} = GetShadowCoord( ase_vertexInput );",
			"#endif//ase_lightAtten_vert"
		};
		private const string LightweightLightAttenDecl = "float ase_lightAtten = 0;";
		private readonly string[] LightweightFragmentInstructions =
		{
			/*shadow coords*/"Light ase_lightAtten_mainLight = GetMainLight( {0} );",
			"ase_lightAtten = ase_lightAtten_mainLight.distanceAttenuation * ase_lightAtten_mainLight.shadowAttenuation;",
			"#ifdef _ADDITIONAL_LIGHTS//ase_lightAtten_frag",
			"int ase_lightAtten_pixelLightCount = GetAdditionalLightsCount();",
			"for (int i = 0; i < ase_lightAtten_pixelLightCount; ++i)",
			"{//ase_lightAtten_frag",
			/*world pos*/"\tLight ase_lightAtten_pointLight = GetAdditionalLight( i, {0} );",
			"\tase_lightAtten += ase_lightAtten_pointLight.distanceAttenuation * ase_lightAtten_pointLight.shadowAttenuation;",
			"}//ase_lightAtten_frag",
			"#endif//ase_lightAtten_frag",
			"ase_lightAtten = saturate( ase_lightAtten );"
		};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_errorMessageTypeIsError = NodeMessageType.Warning;
			m_errorMessageTooltip = SurfaceError;
			m_previewShaderGUID = "4b12227498a5c8d46b6c44ea018e5b56";
			m_drawPreviewAsSphere = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsTemplate  )
			{
				if( !dataCollector.IsSRP )
				{
					return dataCollector.TemplateDataCollectorInstance.GetLightAtten( UniqueId ); ;
				}
				else
				{
					if( dataCollector.CurrentSRPType == TemplateSRPType.Lightweight )
					{
						if( dataCollector.HasLocalVariable( LightweightLightAttenDecl ))
							return ASEAttenVarName;

						// Pragmas
						for( int i = 0; i < LightweightPragmaMultiCompiles.Length; i++ )
							dataCollector.AddToPragmas( UniqueId, LightweightPragmaMultiCompiles[ i ] );

						// Vertex Instructions
						TemplateVertexData shadowCoordsData = dataCollector.TemplateDataCollectorInstance.RequestNewInterpolator( WirePortDataType.FLOAT4, false );
						string vertexInterpName = dataCollector.TemplateDataCollectorInstance.CurrentTemplateData.VertexFunctionData.OutVarName;
						string vertexShadowCoords = vertexInterpName + "." + shadowCoordsData.VarNameWithSwizzle;
						string vertexPos = dataCollector.TemplateDataCollectorInstance.GetVertexPosition( WirePortDataType.FLOAT3, PrecisionType.Float ,false,MasterNodePortCategory.Vertex );

						dataCollector.AddToVertexLocalVariables( UniqueId, string.Format( LightweightVertexInstructions[ 0 ], vertexPos ));
						dataCollector.AddToVertexLocalVariables( UniqueId, LightweightVertexInstructions[ 1 ]);
						dataCollector.AddToVertexLocalVariables( UniqueId, string.Format( LightweightVertexInstructions[ 2 ], vertexShadowCoords ) );
						dataCollector.AddToVertexLocalVariables( UniqueId, LightweightVertexInstructions[ 3 ]);

						// Fragment Instructions
						string worldPos = dataCollector.TemplateDataCollectorInstance.GetWorldPos();
						string fragmentInterpName = dataCollector.TemplateDataCollectorInstance.CurrentTemplateData.FragmentFunctionData.InVarName;
						string fragmentShadowCoords = fragmentInterpName + "." + shadowCoordsData.VarNameWithSwizzle;

						dataCollector.AddLocalVariable( UniqueId, LightweightLightAttenDecl );
						dataCollector.AddLocalVariable( UniqueId, string.Format( LightweightFragmentInstructions[ 0 ], fragmentShadowCoords ) );
						dataCollector.AddLocalVariable( UniqueId, LightweightFragmentInstructions[ 1 ] );
						dataCollector.AddLocalVariable( UniqueId, LightweightFragmentInstructions[ 2 ] );
						dataCollector.AddLocalVariable( UniqueId, LightweightFragmentInstructions[ 3 ] );
						dataCollector.AddLocalVariable( UniqueId, LightweightFragmentInstructions[ 4 ] );
						dataCollector.AddLocalVariable( UniqueId, LightweightFragmentInstructions[ 5 ] );
						dataCollector.AddLocalVariable( UniqueId, string.Format( LightweightFragmentInstructions[ 6 ], worldPos ) );
						dataCollector.AddLocalVariable( UniqueId, LightweightFragmentInstructions[ 7 ] );
						dataCollector.AddLocalVariable( UniqueId, LightweightFragmentInstructions[ 8 ] );
						dataCollector.AddLocalVariable( UniqueId, LightweightFragmentInstructions[ 9 ] );
						dataCollector.AddLocalVariable( UniqueId, LightweightFragmentInstructions[ 10 ] );
						return ASEAttenVarName;
					}
				}
			}

			if ( dataCollector.GenType == PortGenType.NonCustomLighting || dataCollector.CurrentCanvasMode != NodeAvailability.CustomLighting )
                return "1";

			dataCollector.UsingLightAttenuation = true;
			return ASEAttenVarName;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if( ContainerGraph.CurrentCanvasMode == NodeAvailability.TemplateShader )
			{
				m_showErrorMessage = true;
				m_errorMessageTypeIsError = NodeMessageType.Warning;
				m_errorMessageTooltip = TemplateError;
			} else
			{
				m_errorMessageTypeIsError = NodeMessageType.Error;
				m_errorMessageTooltip = SurfaceError;
				if ( ( ContainerGraph.CurrentStandardSurface != null && ContainerGraph.CurrentStandardSurface.CurrentLightingModel != StandardShaderLightModel.CustomLighting ) )
					m_showErrorMessage = true;
				else
					m_showErrorMessage = false;
			}


		}
	}
}
