using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using IngameDebugConsole;
using Object = UnityEngine.Object;

namespace LionStudios.Suite.Creatives
{

    public static partial class LionDebug
    {

        private const string SEND_MESSAGE_CMD = "sendmessage";
        private const string SEND_MESSAGE_DESC = "Find and stores an object of given component type";

        private static void GetFieldValue(string objKey, string memberName, string outKey)
        {
            object obj = GetStored(objKey);
            Type type = obj.GetType();
            FieldInfo fieldInfo = type.GetField(memberName, BINDING_FLAGS_INSTANCE);
            Store(outKey, fieldInfo.GetValue(obj));
        }

        private static void GetPropertyValue(string objKey, string memberName, string outKey)
        {
            object obj = GetStored(objKey);
            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty(memberName, BINDING_FLAGS_INSTANCE);
            Store(outKey, propertyInfo.GetValue(obj));
        }

        private static void SetFieldValue(string objKey, string memberName, string valueKey)
        {
            object obj = GetStored(objKey);
            object val = GetStored(valueKey);
            Type type = obj.GetType();
            FieldInfo fieldInfo = type.GetField(memberName, BINDING_FLAGS_INSTANCE);
            fieldInfo.SetValue(obj, val);
        }

        private static void SetPropertyValue(string objKey, string memberName, string valueKey)
        {
            object obj = GetStored(objKey);
            object val = GetStored(valueKey);
            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty(memberName, BINDING_FLAGS_INSTANCE);
            propertyInfo.SetValue(obj, val);
        }

        private static void New(string className, string outKey)
        {
            New(className, new string[] { }, outKey);
        }

        private static void New(string className, string[] argKeys, string outKey)
        {
            object[] args = argKeys.Select(k => GetStored(k)).ToArray();
            Type[] argTypes = args.Select(a => a.GetType()).ToArray();
            Type type = GetType(className);
            if (type == null)
            {
                Debug.LogError($"Could not find type: {className}");
                return;
            }

            ConstructorInfo constructor =
                type.GetConstructor(BINDING_FLAGS_INSTANCE, null, argTypes, new ParameterModifier[] { });
            if (constructor == null)
            {
                Debug.LogError($"Could not find a constructor with 1 arguments for type: {className}");
                return;
            }

            Store(outKey, constructor.Invoke(args));
        }

        private static void FindObjectOfType(string componentName, string outKey)
        {
            Store(outKey, Object.FindObjectOfType(GetType(componentName)));
        }

        private static void Find(string path, string outKey)
        {
            Store(outKey, GameObject.Find(path));
        }

        private static void Find(string goKey, string path, string outKey)
        {
            GameObject go = GetStored(goKey) as GameObject;
            Store(outKey, go.transform.Find(path));
        }

        private static void Child(string goKey, int index, string outKey)
        {
            GameObject go = GetStored(goKey) as GameObject;
            Store(outKey, go.transform.GetChild(index).gameObject);
        }

        private static void FindWithTag(string tag, string outKey)
        {
            Store(outKey, GameObject.FindWithTag(tag));
        }
        
        private static void GetComponent(string goKey, string type, string outKey)
        {
            GameObject go = GetStored(goKey) as GameObject;
            Store(outKey, go.GetComponent(type));
        }
        
        private static object InvokeMethod(string objKey, string methodName, string[] argKeys)
        {
            object[] args = argKeys.Select(k => GetStored(k)).ToArray();
            Type[] argTypes = args.Select(a => a.GetType()).ToArray();
            object obj = GetStored(objKey);
            Type type = obj.GetType();
            MethodInfo method =
                type.GetMethod(methodName, BINDING_FLAGS_INSTANCE, null, argTypes, new ParameterModifier[] { });
            if (method == null)
            {
                Debug.LogError($"Could not find a method named: {methodName} with 1 arguments for type: {obj.GetType()}");
                return null;
            }
            return method.Invoke(obj, args);
        }

        private static object InvokeMethod(string objKey, string methodName)
        {
            return InvokeMethod(objKey, methodName, new string[]{});
        }
        
        private static void InvokeMethod(string objKey, string methodName, string[] argKeys, string outKey)
        {
            Store(outKey, InvokeMethod(objKey, methodName, argKeys));
        }

        private static void InvokeMethod(string objKey, string methodName, string outKey)
        {
            InvokeMethod(objKey, methodName, new string[]{}, outKey);
        }

        private static void SendMessage(string componentKey, string message)
        {
            if (GetStored(componentKey) is Component comp)
                comp.SendMessage(message);
        }

        private static void SendMessage(string componentKey, string message, object value)
        {
            if (GetStored(componentKey) is Component comp)
                comp.SendMessage(message, value);
        }

        [RuntimeInitializeOnLoadMethod]
        public static void AddReflectionCommands()
        {
            AddCommand<string, string>(SEND_MESSAGE_CMD, SEND_MESSAGE_DESC, SendMessage);

            void AddSendMessageCommand<T>() =>
                AddCommand<string, string, T>(
                    SEND_MESSAGE_CMD,
                    SEND_MESSAGE_DESC,
                    (componentKey, msg, v) => SendMessage(componentKey, msg, v));

            AddSendMessageCommand<bool>();
            AddSendMessageCommand<int>();
            AddSendMessageCommand<float>();
            AddSendMessageCommand<Vector2>();
            AddSendMessageCommand<Vector3>();
            AddSendMessageCommand<string>();

            AddCommand<string, string>("new", "Uses the constructor for that class", New);
            AddCommand<string, string[], string>("new", "Uses the constructor for that class", New);

            AddCommand<string, string, string>("getfieldvalue", "", GetFieldValue);
            AddCommand<string, string, string>("setfieldvalue", "", SetFieldValue);
            AddCommand<string, string, string>("getpropertyvalue", "", GetPropertyValue);
            AddCommand<string, string, string>("setpropertyvalue", "", SetPropertyValue);
            AddCommand<string, string>("findobjectoftype", "", FindObjectOfType);
            AddCommand<string, string>("find", "", Find);
            AddCommand<string, string, string>("find", "", Find);
            AddCommand<string, int, string>("child", "", Child);
            AddCommand<string, string>("findwithtag", "", FindWithTag);
            AddCommand<string, string, string>("getcomponent", "", GetComponent);
            AddCommand<string, string, string[], string>("invokemethod", "", InvokeMethod);
            AddCommand<string, string, string>("invokemethod", "", InvokeMethod);
            AddCommand<string, string, string[], object>("invokemethod", "", InvokeMethod);
            AddCommand<string, string, object>("invokemethod", "", InvokeMethod);

        }

        private static Type GetType(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type == null)
                type = Type.GetType($"{typeName}, Assembly-CSharp");
            if (type == null)
                type = Type.GetType($"{typeName}, UnityEngine");
            if (type == null)
                type = Type.GetType($"UnityEngine.{typeName}, UnityEngine");
            return type;
        }

    }

}
