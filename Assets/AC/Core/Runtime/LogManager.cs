#if UNITY_EDITOR
#define DEBUG
#define ASSERT
#endif
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AC.Core
{
    /// <summary>
    /// Hệ thống log tối ưu: Tự động loại bỏ hoàn toàn khỏi Release build.
    /// - DEBUG logs biến mất trong Production
    /// - ASSERT checks chỉ chạy trong Editor
    /// - Zero performance overhead khi ship game
    /// </summary>
    public static class LogManager
    {
        //-----------------------------------
        //--------------------- Log , warning, 

        [Conditional("DEBUG")]
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        [Conditional("DEBUG")]
        public static void Log(string format, params object[] args)
        {
            Debug.Log(string.Format(format, args));
        }

        /// <summary>Log với màu sắc (chỉ hiển thị trong Editor)</summary>
        [Conditional("DEBUG")]
        public static void LogColored(string message, string hexColor = "#00FF00")
        {
            Debug.Log($"<color={hexColor}>{message}</color>");
        }

        /// <summary>Log hệ thống (màu xanh dương)</summary>
        [Conditional("DEBUG")]
        public static void LogSystem(string message)
        {
            Debug.Log($"<color=#00BFFF>[SYSTEM]</color> {message}");
        }

        /// <summary>Log gameplay (màu xanh lá)</summary>
        [Conditional("DEBUG")]
        public static void LogGameplay(string message)
        {
            Debug.Log($"<color=#00FF00>[GAMEPLAY]</color> {message}");
        }

        [Conditional("DEBUG")]
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        [Conditional("DEBUG")]
        public static void LogWarning(Object context, string format, params object[] args)
        {
            Debug.LogWarning(string.Format(format, args), context);
        }

        [Conditional("DEBUG")]
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        [Conditional("DEBUG")]
        public static void LogError(string format, params object[] args)
        {
            Debug.LogError(string.Format(format, args));
        }

        [Conditional("DEBUG")]
        public static void LogError(Object context, string format, params object[] args)
        {
            Debug.LogError(string.Format(format, args), context);
        }



        [Conditional("DEBUG")]
        public static void Warning(bool condition, object message)
        {
            if (!condition) Debug.LogWarning(message);
        }

        [Conditional("DEBUG")]
        public static void Warning(bool condition, object message, Object context)
        {
            if (!condition) Debug.LogWarning(message, context);
        }

        [Conditional("DEBUG")]
        public static void Warning(bool condition, Object context, string format, params object[] args)
        {
            if (!condition) Debug.LogWarning(string.Format(format, args), context);
        }


        //---------------------------------------------
        //------------- Assert ------------------------

        /// Thown an exception if condition = false
        [Conditional("ASSERT")]
        public static void Assert(bool condition)
        {
            if (!condition) throw new UnityException();
        }

        /// Thown an exception if condition = false, show message on console's log
        [Conditional("ASSERT")]
        public static void Assert(bool condition, string message)
        {
            if (!condition) throw new UnityException(message);
        }

        /// Thown an exception if condition = false, show message on console's log
        [Conditional("ASSERT")]
        public static void Assert(bool condition, string format, params object[] args)
        {
            if (!condition) throw new UnityException(string.Format(format, args));
        }
    }
}
