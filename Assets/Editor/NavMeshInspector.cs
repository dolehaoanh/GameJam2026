using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System.Collections.Generic;

public class NavMeshInspector : EditorWindow
{
    Vector2 scrollPos;

    [MenuItem("Mad Tools/NavMesh Inspector (Soi Chỉ Số)")]
    public static void ShowWindow()
    {
        GetWindow<NavMeshInspector>("NavMesh Soi");
    }

    void OnGUI()
    {
        GUILayout.Label("DANH SÁCH TOÀN BỘ AGENT TRONG SCENE", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🔄 Quét lại Scene"))
        {
            // Chỉ để refresh GUI
        }

        GUILayout.Space(10);
        
        // TIÊU ĐỀ CỘT
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Label("Name", EditorStyles.boldLabel, GUILayout.Width(120));
        GUILayout.Label("Speed", EditorStyles.boldLabel, GUILayout.Width(50));
        GUILayout.Label("Radius", EditorStyles.boldLabel, GUILayout.Width(50));
        GUILayout.Label("Height", EditorStyles.boldLabel, GUILayout.Width(50));
        GUILayout.Label("StopDist", EditorStyles.boldLabel, GUILayout.Width(60));
        GUILayout.Label("Status", EditorStyles.boldLabel, GUILayout.Width(80));
        EditorGUILayout.EndHorizontal();

        // DANH SÁCH
        NavMeshAgent[] agents = FindObjectsOfType<NavMeshAgent>();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (var agent in agents)
        {
            if (agent == null) continue;

            EditorGUILayout.BeginHorizontal("box");

            // Cột 1: Tên (Bấm vào để chọn object)
            if (GUILayout.Button(agent.name, GUILayout.Width(120)))
            {
                Selection.activeGameObject = agent.gameObject;
                EditorGUIUtility.PingObject(agent.gameObject);
            }

            // Cột 2: Speed
            ChangeColorIfZero(agent.speed);
            agent.speed = EditorGUILayout.FloatField(agent.speed, GUILayout.Width(50));
            GUI.color = Color.white;

            // Cột 3: Radius (Cái này quá to sẽ gây kẹt tường)
            // Nếu Radius > 0.5 dễ bị kẹt, cảnh báo màu vàng
            if (agent.radius > 0.4f) GUI.color = Color.yellow;
            agent.radius = EditorGUILayout.FloatField(agent.radius, GUILayout.Width(50));
            GUI.color = Color.white;

            // Cột 4: Height (Chiều cao)
            agent.height = EditorGUILayout.FloatField(agent.height, GUILayout.Width(50));

            // Cột 5: Stopping Distance
            agent.stoppingDistance = EditorGUILayout.FloatField(agent.stoppingDistance, GUILayout.Width(60));

            // Cột 6: Trạng thái (Đang kẹt hay đang chạy?)
            string status = "Idle";
            if (agent.hasPath) status = "Moving";
            if (agent.isStopped) status = "Stopped";
            if (!agent.isOnNavMesh) status = "OFF MESH!";

            // Tô đỏ nếu rớt khỏi NavMesh
            if (!agent.isOnNavMesh) GUI.backgroundColor = Color.red;
            GUILayout.Label(status, GUILayout.Width(80));
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        
        GUILayout.Label($"Tổng cộng: {agents.Length} agents", EditorStyles.miniLabel);
    }

    void ChangeColorIfZero(float val)
    {
        if (val <= 0.01f) GUI.color = Color.red;
        else GUI.color = Color.white;
    }
}