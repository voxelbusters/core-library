using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Editor
{
	public class EditorSplitView
	{
        #region Fields

        private		SplitDirection		m_direction;

		private		float				m_normalizedPosition;

		private		bool				m_resize;

		private		Vector2				m_firstContainerScrollPos;

		private		Vector2				m_secondContainerScroller;

		private		Rect				m_availableRect;

        #endregion

        #region Constructors

        private EditorSplitView(SplitDirection direction, float splitRatio)
		{
			m_normalizedPosition	= splitRatio;
			m_direction				= direction;
		}

		#endregion

		#region Static methods

		public static EditorSplitView CreateHorizontalSplitView(float splitRatio = 0.3f)
		{
			return new EditorSplitView(SplitDirection.Horizontal, splitRatio);
		}

		public static EditorSplitView CreateVerticalSplitView(float splitRatio = 0.3f)
		{
			return new EditorSplitView(SplitDirection.Vertical, splitRatio);
		}

        #endregion

        #region Private methods

        public void BeginArea()
		{
			Rect	tempRect;
			if (m_direction == SplitDirection.Horizontal)
			{
				tempRect	= EditorGUILayout.BeginHorizontal();
			}
			else
			{
				tempRect	= EditorGUILayout.BeginVertical();
			}
			if (tempRect.width > 0.0f)
			{
				m_availableRect		= tempRect;
			}

			BeginContainer(ref m_firstContainerScrollPos, isFirst: true);
		}

		public void Split()
		{
			EndContainer();
			ResizeFirstView();
			BeginContainer(ref m_secondContainerScroller, isFirst: false);
		}

		public void EndArea()
		{
			EndContainer();
			if (m_direction == SplitDirection.Horizontal)
			{
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				EditorGUILayout.EndVertical();
			}
		}

		public void ResetFirstContainerScroller()
		{
			m_firstContainerScrollPos	= Vector2.zero;
		}

		public void ResetSecondContainerScroller()
		{
			m_firstContainerScrollPos	= Vector2.zero;
		}

		private void BeginContainer(ref Vector2 scrollPos, bool isFirst)
		{
			float	normalizedSize	= isFirst ? m_normalizedPosition : (1f - m_normalizedPosition);
			if (m_direction == SplitDirection.Horizontal)
			{
				EditorGUILayout.BeginVertical();
				scrollPos	= EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(m_availableRect.width * normalizedSize));
			}
			else if (m_direction == SplitDirection.Vertical)
			{
				EditorGUILayout.BeginHorizontal();
				scrollPos	= EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(m_availableRect.height * normalizedSize));
			}
		}

		private void EndContainer()
		{
			EditorGUILayout.EndScrollView();
			if (m_direction == SplitDirection.Horizontal)
			{
				EditorGUILayout.EndVertical();
			}
			else if (m_direction == SplitDirection.Vertical)
			{
				EditorGUILayout.EndHorizontal();
			}
		}

		private void ResizeFirstView()
		{
			Rect resizeHandleRect;
			if (m_direction == SplitDirection.Horizontal)
			{
				resizeHandleRect	= new Rect(m_availableRect.width * m_normalizedPosition, m_availableRect.y, 1f, m_availableRect.height);
			}
			else
			{
				resizeHandleRect	= new Rect(m_availableRect.x, m_availableRect.height * m_normalizedPosition, m_availableRect.width, 1f);
			}
			EditorGUI.DrawRect(resizeHandleRect, new Color(0.15f, 0.15f, 0.15f, 1f));
			if (m_direction == SplitDirection.Horizontal)
			{
				EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeHorizontal);
			}
			else
			{
				EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeVertical);
			}

			if (Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(Event.current.mousePosition))
			{
				m_resize	= true;
			}
			if (m_resize)
			{
				if (m_direction == SplitDirection.Horizontal)
				{
					m_normalizedPosition	= Event.current.mousePosition.x / m_availableRect.width;
				}
				else
				{
					m_normalizedPosition	= Event.current.mousePosition.y / m_availableRect.height;
				}
			}
			if (Event.current.type == EventType.MouseUp)
			{
				m_resize	= false;
			}
		}

        #endregion

        #region Nested types

		private enum SplitDirection
		{
			Horizontal,

			Vertical
		}

        #endregion
    }
}