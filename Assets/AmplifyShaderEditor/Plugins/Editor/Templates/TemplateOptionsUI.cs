// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	// UI STRUCTURES
	[Serializable]
	public class TemplateOptionUIItem
	{
		public delegate void OnActionPerformed( bool isRefreshing, bool invertAction, TemplateOptionUIItem uiItem, params TemplateActionItem[] validActions );
		public event OnActionPerformed OnActionPerformedEvt;

		[SerializeField]
		private bool m_isVisible = true;

		[SerializeField]
		private int m_currentOption = 0;

		[SerializeField]
		private TemplateOptionsItem m_options;

		[SerializeField]
		private bool m_checkOnExecute = false;

		[SerializeField]
		private bool m_invertActionOnDeselection = false;

		public TemplateOptionUIItem( TemplateOptionsItem options )
		{
			m_options = options;
			m_currentOption = m_options.DefaultOptionIndex;
			m_invertActionOnDeselection = options.Setup == AseOptionItemSetup.InvertActionOnDeselection;
		}

		public void Draw( UndoParentNode owner )
		{
			if( m_isVisible )
			{
				int lastOption = m_currentOption;
				EditorGUI.BeginChangeCheck();
				switch( m_options.UIWidget )
				{
					case AseOptionsUIWidget.Dropdown:
					{
						m_currentOption = owner.EditorGUILayoutPopup( m_options.Name, m_currentOption, m_options.Options );
					}
					break;
					case AseOptionsUIWidget.Toggle:
					{
						m_currentOption = owner.EditorGUILayoutToggle( m_options.Name, m_currentOption == 1 ) ? 1 : 0;
					}
					break;
				}
				if( EditorGUI.EndChangeCheck() )
				{
					if( OnActionPerformedEvt != null )
					{
						if( m_invertActionOnDeselection )
							OnActionPerformedEvt( false, true, this, m_options.ActionsPerOption[ lastOption ] );

						OnActionPerformedEvt( false, false, this, m_options.ActionsPerOption[ m_currentOption ] );
					}
				}
			}
		}

		public void FillDataCollector( ref MasterNodeDataCollector dataCollector )
		{
			if( m_isVisible && m_checkOnExecute )
			{
				for( int i = 0; i < m_options.ActionsPerOption[ m_currentOption ].Length; i++ )
				{
					switch( m_options.ActionsPerOption[ m_currentOption ][ i ].ActionType )
					{
						case AseOptionsActionType.SetDefine:
						{
							dataCollector.AddToDefines( -1, m_options.ActionsPerOption[ m_currentOption ][ i ].ActionData );
						}
						break;
						case AseOptionsActionType.SetUndefine:
						{
							dataCollector.AddToDefines( -1, m_options.ActionsPerOption[ m_currentOption ][ i ].ActionData, false );
						}
						break;
					}
				}
			}
		}

		public void Refresh()
		{
			if( OnActionPerformedEvt != null )
			{
				if( m_invertActionOnDeselection )
				{
					for( int i = 0; i < m_options.Count; i++ )
					{
						if( i != m_currentOption )
						{
							OnActionPerformedEvt( true, true, this, m_options.ActionsPerOption[ i ] );
						}
					}
				}

				OnActionPerformedEvt( true, false, this, m_options.ActionsPerOption[ m_currentOption ] );
			}
		}

		public TemplateOptionsItem Options { get { return m_options; } }

		public void Destroy()
		{
			OnActionPerformedEvt = null;
		}

		public bool IsVisible
		{
			get { return m_isVisible; }
			set { m_isVisible = value; }
		}

		public bool CheckOnExecute
		{
			get { return m_checkOnExecute; }
			set { m_checkOnExecute = value; }
		}

		public int CurrentOption
		{
			get { return m_currentOption; }
			set
			{
				m_currentOption = Mathf.Clamp( value, 0, m_options.Options.Length - 1 );
				Refresh();
			}
		}
		public bool EmptyEvent { get { return OnActionPerformedEvt == null; } }
	}
}
