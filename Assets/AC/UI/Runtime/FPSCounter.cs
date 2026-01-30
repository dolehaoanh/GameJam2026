using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AC.GameTool.UI
{
    public class FPSCounter : MonoBehaviour
    {

        [SerializeField] TMP_Text m_fpsText;
        [SerializeField] float m_fpsUpdateInterval = 1f;
        private float deltaTime;

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            float fps = m_fpsUpdateInterval / deltaTime;
            //fps = fps > Application.targetFrameRate ? Application.targetFrameRate + fps-Mathf.Floor(fps) : fps;
            m_fpsText.text = string.Format("{0:0.0} FPS", fps);
        }
    }
}