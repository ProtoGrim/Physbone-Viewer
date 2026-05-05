using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Dynamics.PhysBone.Components;
using System.Linq;
using System.Drawing.Printing;

public class PhysboneViewer : EditorWindow
{
    UnityEngine.Object avatarObject;
    UnityEngine.Object oldAvatarObject;
    Vector2 scrollPos = Vector2.zero;
    Dictionary<VRCPhysBone, int> physboneTransformCountList = new Dictionary<VRCPhysBone, int>();
    

    [MenuItem("Tools/ProtoGrim/Physbone Viewer")]

    public static void ShowWindow()
    {
        GetWindow<PhysboneViewer>("Physbone Viewer");
    }

    void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        avatarObject = EditorGUILayout.ObjectField("Avatar", avatarObject, typeof(UnityEngine.Object), true);
        bool refreshClicked = GUILayout.Button("Refresh");
        bool showList = avatarObject != null;

        if (avatarObject != oldAvatarObject || refreshClicked)
        {
            oldAvatarObject = avatarObject;
            if (showList)
            {
                Debug.Log("Entered If statement");
                GameObject avatar = (GameObject)avatarObject;

                HashSet<VRCPhysBone> tempHash = new HashSet<VRCPhysBone>(avatar.GetComponentsInChildren<VRCPhysBone>(true));
                tempHash.Remove(avatar.GetComponent<VRCPhysBone>());
                VRCPhysBone[] physboneList = tempHash.ToArray<VRCPhysBone>();

                physboneTransformCountList = new Dictionary<VRCPhysBone, int>();

                foreach (VRCPhysBone physbone in physboneList)
                {
                    Debug.Log(" --------- " + physbone.name);
                    int transformCount = GetAffectedTransforms(physbone.GetRootTransform(), physbone.ignoreTransforms, physbone.ignoreOtherPhysBones);
                    physboneTransformCountList.Add(physbone, transformCount);
                }
                
                foreach (KeyValuePair<VRCPhysBone, int> entry in physboneTransformCountList)
                {
                    Debug.Log(entry.Key + ", " + entry.Value);
                }
                
            }
        }

        if (showList)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.LabelField($"Total physbone count: {physboneTransformCountList.Count}");
            EditorGUILayout.LabelField($"Total physbone transform count: {physboneTransformCountList.Values.Sum()}");
            foreach (KeyValuePair<VRCPhysBone, int> pair in physboneTransformCountList)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(pair.Key, typeof(VRCPhysBone), false, GUILayout.ExpandWidth(true));
                EditorGUILayout.LabelField($"Transform count: {pair.Value}", GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndFadeGroup();
        }
    }

    private int GetAffectedTransforms(Transform trns, List<Transform> ignoreList, bool ignorePhysbones, int count = 0)
    {
        foreach (Transform child in trns)
        {
            Debug.Log(child.name);
            if (ignoreList.Contains(child)) { continue; }
            if (ignorePhysbones && child.GetComponent<VRCPhysBone>() != null) { continue; }

            count = GetAffectedTransforms(child, ignoreList, ignorePhysbones, count);
        }

        return count + 1;
    }

}
