using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace AmplifyShaderEditor
{
	[CustomEditor( typeof( AmplifyShaderFunction ) )]
	public class AmplifyShaderFunctionEditor : Editor
	{
		AmplifyShaderFunction m_target;

		void OnEnable()
		{
			m_target = ( target as AmplifyShaderFunction );
		}

		public override void OnInspectorGUI()
		{
			//base.OnInspectorGUI();
			//base.serializedObject.Update();
			if( GUILayout.Button( "Open in Shader Editor" ) )
			{
#if UNITY_2018_3_OR_NEWER
				ASEPackageManagerHelper.SetupLateShaderFunction( m_target );
#else
				AmplifyShaderEditorWindow.LoadShaderFunctionToASE( m_target, false );
#endif
			}
			//EditorGUILayout.Separator();
			//m_target.FunctionInfo = EditorGUILayout.TextArea( m_target.FunctionInfo );

			if( m_target.Description.Length > 0 )
			{
				EditorGUILayout.HelpBox( m_target.Description, MessageType.Info );
			}
		}
	}
}
