using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NavyUIHelper : EditorWindow {

    // 	[MenuItem("Window/NavyUIHelper  %r")]  
    // 	private static void GenerateUI()
    // 	{
    // 		foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject))) {
    // 			Debug.Log (obj.name);
    // 			UIHelper helper = obj.GetComponent<UIHelper> ();
    // 			if (null != helper)
    // 				helper.GenerateUI ();
    // 		}
    // 	}

    

    [MenuItem(@"Tool/CopySceneOrPrefabTest #&s")]  
	private static void GenerateCopySceneOrPrefabTest()
	{
        string outPut = "";
        YAML.Node node = YAML.Load(Application.dataPath + "/prefabs/Canvas.prefab");


        //test 1
        //         List<YAML.Node> lstAttributeNode = node.AttributeForGenerateCode();//将要产生在代码内的对象，未筛选
        //         List<YAML.Node> ScriptNodes = new List<YAML.Node>();
        //         node.Find("MonoBehaviour", ScriptNodes);
        //         if (ScriptNodes.Count == 0)
        //             return;
        //         foreach (var sn in ScriptNodes)
        //         {
        //             foreach (var n in sn.Children)
        //             {
        //                 if (n.Value != null)
        //                 {
        //                     Dictionary<string, YAML.Node>  valueNode = YAML.DeserializeValue(n.Value);
        //                     foreach (KeyValuePair<string, YAML.Node> kvp in valueNode)
        //                     {
        //                         Debug.Log(string.Format("Key:{0}; Value:{1}", kvp.Value.Name, kvp.Value.Value));
        //                     }
        //                 }
        //             }
        // 
        //         }


        //test 2
        /*
     *
--- !u!114 &360487655
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_PrefabParentObject: {fileID: 0}
  m_PrefabInternal: {fileID: 0}
  m_GameObject: {fileID: 360487649}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 1c5fb6a982960fd4285cfe1773af15d7, type: 3}
  m_Name: 
  m_EditorClassIdentifier: 
  TestButton: {fileID: 0}
  hhhhText: {fileID: 0}
     */
        YAML.Node nodeNew = new YAML.Node();
        nodeNew.Name = "--- !u!114 &114437434465559884";
        nodeNew.Value = null;
        node.Children.Add(nodeNew);

        YAML.Node nodeScriptNew = new YAML.Node();
        nodeScriptNew.Name = "MonoBehaviour";
        nodeScriptNew.Value = " ";

        nodeScriptNew.AddNode("m_ObjectHideFlags", " 1", 2, "114437434465559884", "");//fileID在非脚本属性里是随机的，只要保持唯一即可,格式是定死的。
        nodeScriptNew.AddNode("m_PrefabParentObject", " {fileID: 0}", 2, "114437434465559884", "");
        nodeScriptNew.AddNode("m_PrefabInternal", " {fileID: 100100000}", 2, "114437434465559884", "");
        nodeScriptNew.AddNode("m_GameObject", " {fileID: 1659991782575188}", 2, "114437434465559884", "");  //此处是挂载对象 预设走m_RootGameObject属性。此外，需要在m_RootGameObject fileid归属上加上component，component的fileid与这个脚本的fileid保持一致
        nodeScriptNew.AddNode("m_Enabled", " 1", 2, "114437434465559884", "");
        nodeScriptNew.AddNode("m_EditorHideFlags", " 0", 2, "114437434465559884", "");
        nodeScriptNew.AddNode("m_Script", " {fileID: 11500000, guid: 1c5fb6a982960fd4285cfe1773af15d7, type: 3}", 2, "114437434465559884", "");
        nodeScriptNew.AddNode("m_Name", " ", 2, "114437434465559884", "");
        nodeScriptNew.AddNode("m_EditorClassIdentifier", " ", 2, "114437434465559884", "");
        nodeScriptNew.AddNode("TestButton", " {fileID: 0}", 2, "114437434465559884", "");
        nodeScriptNew.AddNode("hhhhText", " {fileID: 0}", 2, "114437434465559884", "");
        node.Children.Add(nodeScriptNew);

        foreach (var sn in node.Children)
        {
            if(sn.RootFileID == "1659991782575188")
            {

            }
        }

        outPut = YAML.Instance.Deserialize(node);
        YAML.Instance.Write(Application.dataPath + "/prefabs/CanvasCopy.prefab", outPut);
    }
    //添加菜单
    [MenuItem(@"Tool/NavyUIHelper #&r")]

    public static void GetTransforms()
    {
        Dictionary<string, Vector3> dic = new Dictionary<string, Vector3>();
        //transforms是Selection类的静态字段，其返回的是选中的对象的Transform
        Transform[] transforms = Selection.transforms;

        if(transforms.Length == 0)
        {
            Debug.Log("请选择需要生成UI类的预设或者UI根节点(在Hierachy窗口上选取)");
        }
        //将选中的对象的postion保存在字典中
        for (int i = 0; i < transforms.Length; i++)
        {
            dic.Add(transforms[i].name, transforms[i].position);
        }

        //将字典中的信息打印出来
//         foreach (Transform item in transforms)
//         {
//             Debug.Log(item.name + ":" + item.position);
//             UIHelper helper = item.gameObject.AddComponent<UIHelper>();
//             if (null != helper)
//             {
//                 helper.NGUI = true;
//                 helper.className = item.name + "Base";
//                 helper.parentName = "WindowBase";
//                 helper.GenerateUI();
//             }
//             DestroyImmediate(item.gameObject.GetComponent<UIHelper>());
//         }
    }
}
