using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// Original creation of lordconstant - all credits go to him
/// (<see href="https://github.com/lordconstant/SubclassPropertyDrawer"/>)
/// This piece of code is NOT covered by the overall MIT license!
/// </summary>
public class SubclassSelector
{
    List<System.Type> m_subClasses;
    string[] m_subClassNames;

    int m_selectedType;

    public SubclassSelector(System.Type InType, object InDefault = null)
    {
        m_selectedType = -1;

        m_subClasses = new List<System.Type>();

        System.Type[] foundClasses = System.AppDomain.CurrentDomain.GetAssemblies()
         .SelectMany(assembly => assembly.GetTypes())
         .Where(type => type.IsSubclassOf(InType)).ToArray();

        for (int i = 0; i < foundClasses.Length; i++)
        {
            if (foundClasses[i].ContainsGenericParameters)
                continue;

            if (InDefault != null)
            {
                if (InDefault.GetType() == foundClasses[i])
                    m_selectedType = m_subClasses.Count;
            }

            m_subClasses.Add(foundClasses[i]);
        }

        m_subClassNames = new string[m_subClasses.Count];

        for (int i = 0; i < m_subClassNames.Length; i++)
        {
            m_subClassNames[i] = m_subClasses[i].Name;
        }
    }

    public void RefreshSelection(object InDefault)
    {
        m_selectedType = -1;
        if (InDefault == null)
            return;

        for (int i = 0; i < m_subClasses.Count; i++)
            if (InDefault.GetType() == m_subClasses[i])
            {
                m_selectedType = i;
                break;
            }
    }

    public bool Draw()
    {
        int selectedIdx = m_selectedType < 0 ? 0 : m_selectedType;

        if (selectedIdx >= 0)
        {
            selectedIdx = EditorGUILayout.Popup(selectedIdx, m_subClassNames);
        }

        if (selectedIdx != m_selectedType)
        {
            m_selectedType = selectedIdx;

            return true;
        }

        return false;
    }

    public bool Draw(Rect InPosition)
    {
        int selectedIdx = m_selectedType < 0 ? 0 : m_selectedType;

        if (selectedIdx >= 0)
        {
            selectedIdx = EditorGUI.Popup(InPosition, selectedIdx, m_subClassNames);
        }

        if (selectedIdx != m_selectedType)
        {
            m_selectedType = selectedIdx;

            return true;
        }

        return false;
    }

    public object CreateSelected()
    {
        return System.Activator.CreateInstance(m_subClasses[m_selectedType]);
    }
}
/*
public class SubclassSelector<T> : SubclassSelector where T : class
{
    public SubclassSelector(T InDefault) : base(typeof(T), InDefault)
    {

    }

    public T CreateSelectedFromType()
    {
        return CreateSelected() as T;
    }
}*/