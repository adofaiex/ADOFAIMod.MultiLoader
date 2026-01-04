using System;
using System.IO;
using Newtonsoft.Json;

namespace AdofaiMod.MultiLoader.Loaders
{
    public class DoorstopHandler : IHandler
    {
        private readonly string _modPath;
        private readonly string _settingsPath;

        public DoorstopHandler()
        {
            _modPath = Path.GetDirectoryName(GetType().Assembly.Location) ?? ".";
            _settingsPath = Path.Combine(_modPath, "Config", $"{ModId}.json");
        }

        public string ModId => "AdofaiMod.MultiLoader";
        public string ModVersion => "1.0.0";
        public string ModPath => _modPath;

        public void Log(string message) => UnityEngine.Debug.Log($"[{ModId}] {message}");
        public void Warning(string message) => UnityEngine.Debug.LogWarning($"[{ModId}] {message}");
        public void Error(string message) => UnityEngine.Debug.LogError($"[{ModId}] {message}");

        public T LoadSettings<T>() where T : class, new()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    return JsonConvert.DeserializeObject<T>(json) ?? new T();
                }
            }
            catch (Exception ex)
            {
                Error($"Failed to load settings: {ex}");
            }
            return new T();
        }

        public void SaveSettings<T>(T settings) where T : class
        {
            try
            {
                var dir = Path.GetDirectoryName(_settingsPath)!;
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                Error($"Failed to save settings: {ex}");
            }
        }

        public void TriggerToggle(bool value) => OnToggle?.Invoke(value);
        public void TriggerUpdate(float dt) => OnUpdate?.Invoke(dt);
        public void TriggerGUI() => OnGUI?.Invoke();

        public event Action<float>? OnUpdate;
        public event Action<bool>? OnToggle;
        public event Action? OnGUI;
        public event Action? OnSaveGUI;
    }
}
