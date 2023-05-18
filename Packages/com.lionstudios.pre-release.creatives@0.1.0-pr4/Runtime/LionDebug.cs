using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using IngameDebugConsole;
using UnityEngine.Networking;
using YamlDotNet.Serialization;
using Object = UnityEngine.Object;
using System.IO;
using UnityEngine.SceneManagement;

namespace LionStudios.Suite.Creatives
{

    [Serializable]
    public class CommandsYml
    {
        public string version;
        public CommandChain[] commandChains;
    }

    [Serializable]
    public class CommandChain
    {
        public string name;
        public string[] parameters;
        public string[] commands;
    }

    public static partial class LionDebug
    {
        private const int VERSION = 1;

        private const string COMP_BY_PATH_PREFIX = "find:";

        private const string CMD_PREFIX = "lion";

        private const BindingFlags BINDING_FLAGS_INSTANCE =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        static string GetCmdName(string suffix) => $"{CMD_PREFIX}.{suffix}";

        static Dictionary<string, List<string>> onSceneLoadCommands = new Dictionary<string, List<string>>();

        #region SceneLoaded

        static LionDebug()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            DebugLogManager.Instance.StartCoroutine(OnSceneLoadedCoroutine(scene, mode));
        }

        static IEnumerator OnSceneLoadedCoroutine(Scene scene, LoadSceneMode mode)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            if (onSceneLoadCommands.ContainsKey(scene.name))
            {
                foreach (string command in onSceneLoadCommands[scene.name])
                {
                    string[] parameters = command.Split(' ');
                    InvokeCommandChain(parameters[0], parameters.Skip(1).ToArray());
                    //DebugLogConsole.ExecuteCommand(command);
                }
            }
        }

        static void ClearOnSceneLoadedCommands()
        {
            onSceneLoadCommands.Clear();
        }

        static void ClearOnSceneLoadedCommands(string sceneName)
        {
            if(onSceneLoadCommands.ContainsKey(sceneName))
            {
                onSceneLoadCommands.Remove(sceneName);
            }
        }

        static void AddOnSceneLoadedCommand(string sceneName, string command)
        {
            string[] parameters = new string[0];
            AddOnSceneLoadedCommand(sceneName, command, parameters);
        }

        static void AddOnSceneLoadedCommand(string sceneName, string command, string parameter1)
        {
            string[] parameters = { parameter1 };
            AddOnSceneLoadedCommand(sceneName, command, parameters);
        }

        static void AddOnSceneLoadedCommand(string sceneName, string command, string parameter1, string parameter2)
        {
            string[] parameters = { parameter1, parameter2 };
            AddOnSceneLoadedCommand(sceneName, command, parameters);
        }

        static void AddOnSceneLoadedCommand(string sceneName, string command, string[] parameters)
        {
            string combinedParameters = String.Join(" ", parameters);
            string combinedCommand = command + (!string.IsNullOrEmpty(combinedParameters) ? (" " + combinedParameters) : "");
            if(!onSceneLoadCommands.ContainsKey(sceneName))
            {
                onSceneLoadCommands.Add(sceneName, new List<string>());
            }
            onSceneLoadCommands[sceneName].Add(combinedCommand);
        }
        #endregion

        #region AddCommand Overrides

        static void AddCommand(string command, string description, Action method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        static void AddCommand<T1>(string command, string description, Action<T1> method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        static void AddCommand<T1>(string command, string description, Func<T1> method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        static void AddCommand<T1, T2>(string command, string description, Action<T1, T2> method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        static void AddCommand<T1, T2>(string command, string description, Func<T1, T2> method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        static void AddCommand<T1, T2, T3>(string command, string description, Action<T1, T2, T3> method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        static void AddCommand<T1, T2, T3>(string command, string description, Func<T1, T2, T3> method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        static void AddCommand<T1, T2, T3, T4>(string command, string description, Action<T1, T2, T3, T4> method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        static void AddCommand<T1, T2, T3, T4>(string command, string description, Func<T1, T2, T3, T4> method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        static void AddCommand<T1, T2, T3, T4, T5>(string command, string description, Action<T1, T2, T3, T4, T5> method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        static void AddCommand<T1, T2, T3, T4, T5>(string command, string description, Func<T1, T2, T3, T4, T5> method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        static void AddCommand<T1, T2, T3, T4, T5, T6>(string command, string description, Action<T1, T2, T3, T4, T5, T6> method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        static void AddCommand<T1, T2, T3, T4, T5, T6>(string command, string description, Func<T1, T2, T3, T4, T5, T6> method)
        {
            DebugLogConsole.AddCommand(GetCmdName(command), description, method);
        }

        #endregion

        #region Variable Storage System

        private static Dictionary<string, object> storedObjects = new Dictionary<string, object>();

        private static void Store(string key, object value)
        {
            storedObjects[key] = value;
            PrintStored(key);
        }

        private static void PrintStored(string key)
        {
            object value = GetStored(key);
            string valueStr = value != null ? $"{value} -> {value.GetType()}" : "<null>";
            Debug.Log($"Stored value for key: {key} ; value: {valueStr}");
        }


        private static object GetStored(string key)
        {
            return storedObjects[key];
        }
        private static T GetStored<T>(string key)
        {
            return (T)storedObjects[key];
        }
        private static T GetStoredComp<T>(string key) where T : Component
        {
            object obj;
            if (key.StartsWith(COMP_BY_PATH_PREFIX))
                obj = GameObject.Find(key.Substring(COMP_BY_PATH_PREFIX.Length));
            else
                obj = storedObjects[key];
            if (obj is T t)
                return t;
            if (obj is GameObject go && go.GetComponent<T>() != null)
                return go.GetComponent<T>();
            throw new ArgumentException($"Wrong type. Need {typeof(T).Name}, but {obj.GetType()} is provided.");
        }
        private static object GetStoredComp(string key, params Type[] types)
        {
            object obj;
            if (key.StartsWith(COMP_BY_PATH_PREFIX))
                obj = GameObject.Find(key.Substring(COMP_BY_PATH_PREFIX.Length));
            else
                obj = storedObjects[key];
            if (obj is GameObject go)
            {
                foreach (Type type in types)
                {
                    Component comp = go.GetComponent(type);
                    if (comp != null)
                        return comp;
                }
            }
            else if (obj is Component)
            {
                foreach (Type type in types)
                {
                    if (obj.GetType().IsAssignableFrom(type))
                        return obj;
                }
            }
            throw new ArgumentException($"Wrong type. Need [{string.Join(",", types.Select(t => t.Name))}], but {obj.GetType()} is provided.");
        }
        
        #endregion

        #region Remote Command Chains System
        
        private static CommandChain[] GetCommandChains()
        {
            bool useRemoteCommands = LionCreativesSettings.GetUseRemote();
            if(useRemoteCommands)
            {
                string applicationPath = Application.identifier.Replace('.', '/');
                string url = $"{BUNDLES_BASE_URL}{applicationPath}/commands.yml";
                var request = UnityWebRequest.Get(url);
                request.SendWebRequest();
                while (!request.isDone) { }
                string chainsTxt = request.downloadHandler.text;
                var deserializer = new DeserializerBuilder().Build();
                CommandsYml commandsYml = deserializer.Deserialize<CommandsYml>(chainsTxt);

                if (!string.IsNullOrEmpty(commandsYml.version) && int.Parse(commandsYml.version) > VERSION)
                {
                    Debug.LogWarning($"Command version in app is older than commands file. App version: {VERSION}. File version: {commandsYml.version}");
                }

                return commandsYml.commandChains;
            }
            else
            {
                string text = File.ReadAllText(LionCreativesSettings.GetLocalCommandsPath());
                var deserializer = new DeserializerBuilder().Build();
                CommandsYml commandsYml = deserializer.Deserialize<CommandsYml>(text);

                if (!string.IsNullOrEmpty(commandsYml.version) && int.Parse(commandsYml.version) > VERSION)
                {
                    Debug.LogWarning($"Command version in app is older than commands file. App version: {VERSION}. File version: {commandsYml.version}");
                }

                return commandsYml.commandChains;
            }
        }

        private static void ListCommandChains()
        {
            CommandChain[] chainList = GetCommandChains();
            Debug.Log(string.Join("\n", chainList.Select(cc => cc.name)));
        }

        private static void InvokeCommandChain(string chainKey)
        {
            string[] parameters = new string[0];
            InvokeCommandChain(chainKey, parameters);
        }

        private static void InvokeCommandChain(string chainKey, string parameter1)
        {
            string[] parameters = { parameter1 };
            InvokeCommandChain(chainKey, parameters);
        }

        private static void InvokeCommandChain(string chainKey, string parameter1, string parameter2)
        {
            string[] parameters = { parameter1, parameter2 };
            InvokeCommandChain(chainKey, parameters);
        }

        private static void InvokeCommandChain(string chainKey, string parameter1, string parameter2, string parameter3)
        {
            string[] parameters = { parameter1, parameter2, parameter3 };
            InvokeCommandChain(chainKey, parameters);
        }

        private static void InvokeCommandChain(string chainKey, string parameter1, string parameter2, string parameter3, string parameter4)
        {
            string[] parameters = { parameter1, parameter2, parameter3, parameter4 };
            InvokeCommandChain(chainKey, parameters);
        }

        private static void InvokeCommandChain(string chainKey, string parameter1, string parameter2, string parameter3, string parameter4, string parameter5)
        {
            string[] parameters = { parameter1, parameter2, parameter3, parameter4, parameter5 };
            InvokeCommandChain(chainKey, parameters);
        }

        private static void InvokeCommandChain(string chainKey, string[] parameters)
        {
            CommandChain[] chainList = GetCommandChains();
            CommandChain chain = chainList.FirstOrDefault(c => c.name == chainKey);
            if (chain != null)
            {
                if (chain.parameters == null) chain.parameters = new string[0];
                if(chain.parameters.Length != parameters.Length)
                {
                    Debug.LogError($"Parameter count mismatch for command {chain.name}. Expecting {chain.parameters.Length}, got {parameters.Length}");
                    return;
                }
                foreach (string command in chain.commands)
                {
                    string commandWithParameters = GetCommandWithChainParameters(command, chain, parameters);
                    int missingParameterIndex = commandWithParameters.IndexOf('{');
                    if (missingParameterIndex >= 0)
                    {
                        int missingParameterEndIndex = commandWithParameters.IndexOf('}', missingParameterIndex);
                        string missingParameter = commandWithParameters.Substring(missingParameterIndex, missingParameterEndIndex - missingParameterIndex + 1);
                        Debug.LogError($"Unknown parameter {missingParameter}. Command: {command}");
                        return;
                    }
                    DebugLogConsole.ExecuteCommand(commandWithParameters);
                }
            }
            else
            {
                Debug.LogError($"Could not find command-chain: {chainKey} for {Application.identifier}");
            }
        }

        private static string GetCommandWithChainParameters(string command, CommandChain chain, string[] parameters)
        {
            string result = command;

            if(chain.parameters != null)
            {
                for(int i = 0; i < chain.parameters.Length; i++)
                {
                    string parameterToReplace = "{" + chain.parameters[i] + "}";
                    result = result.Replace(parameterToReplace, parameters[i]);
                }
            }

            return result;
        }
        
        #endregion

        [RuntimeInitializeOnLoadMethod]
        public static void AddBaseCommands()
        {

            void AddStoreValueCommand<T>() =>
                AddCommand<T, string>(
                    "storevalue",
                    "Store the given primitive type value in the storedObjects for access by key by other commands",
                    (value, outKey) => Store(outKey, value));

            AddStoreValueCommand<bool>();
            AddStoreValueCommand<int>();
            AddStoreValueCommand<float>();
            AddStoreValueCommand<Vector2>();
            AddStoreValueCommand<Vector3>();
            AddStoreValueCommand<string>();

            AddCommand<string>(
                "printstored",
                "Prints the stored value at the given key",
                PrintStored);

            AddCommand<string>(
                "cmdchain",
                "Executes a chain command from the S3 bucket",
                InvokeCommandChain);

            AddCommand<string, string>(
                "cmdchain",
                "Executes a chain command from the S3 bucket",
                InvokeCommandChain);

            AddCommand<string, string, string>(
                "cmdchain",
                "Executes a chain command from the S3 bucket",
                InvokeCommandChain);

            AddCommand<string, string, string, string>(
                "cmdchain",
                "Executes a chain command from the S3 bucket",
                InvokeCommandChain);

            AddCommand<string, string, string, string, string>(
                "cmdchain",
                "Executes a chain command from the S3 bucket",
                InvokeCommandChain);

            AddCommand<string, string, string, string, string, string>(
                "cmdchain",
                "Executes a chain command from the S3 bucket",
                InvokeCommandChain);

            AddCommand(
                "cmdchainlist", 
                "Lists chain commands from the S3 bucket", 
                ListCommandChains);

            AddCommand<string, string>(
                "onsceneload",
                "Adds a command to execute on scene load",
                AddOnSceneLoadedCommand);

            AddCommand<string, string, string>(
                "onsceneload",
                "Adds a command to execute on scene load",
                AddOnSceneLoadedCommand);

            AddCommand<string, string, string, string>(
                "onsceneload",
                "Adds a command to execute on scene load",
                AddOnSceneLoadedCommand);

            AddCommand(
                "clearonsceneloadcommands",
                "Clears all on scene load commands",
                ClearOnSceneLoadedCommands);

            AddCommand<string>(
                "clearonsceneloadcommands",
                "Clears on scene load commands for given scene",
                ClearOnSceneLoadedCommands);

        }

    }

}
