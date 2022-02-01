using System;
using System.IO;
using ResharperToolsLib.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#nullable enable

namespace ResharperToolsLib.Config
{
    public class ConfigLoader<T>
    {
        public const string ConfigJsonLocation = "./appsettings.json";

        public ConfigLoader(ILogger logger)
        {
            Logger = logger;
        }

        private ILogger Logger { get; }

        public T? ReadConfig(T templateConfig)
        {
            string? configJsonString = null;

            Logger.Info("Reading config");

            if (!ConfigExists())
            {
                Logger.Info("Config does not exist. Creating template config file.");
                File.WriteAllText(ConfigJsonLocation, CreateTemplateString(templateConfig));
                Logger.Info(@"Template config file ""appsettings.json"" created.");
                return templateConfig;
            }

            try
            {
                using var file = File.OpenText(ConfigJsonLocation);
                configJsonString = file.ReadToEnd();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to open application settings file.", ex);
                return default;
            }

            return ParseJson(configJsonString);
        }

        public static bool ConfigExists()
        {
            return File.Exists(ConfigJsonLocation);
        }

        /// <summary>
        /// Create a template config string
        /// </summary>
        /// <returns></returns>
        public static string CreateTemplateString(T templateConfig)
        {

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(templateConfig, jsonSerializerSettings);
        }

        private T? ParseJson(string configJsonString)
        {
            JObject configJson;
            try
            {
                configJson = JObject.Parse(configJsonString);
            }
            catch (JsonReaderException ex)
            {
                Logger.Error("Failed to parse application settings as json.", ex);
                return default;
            }

            try
            {
                return configJson.ToObject<T>();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to read values from application settings.", ex);
                return default;
            }
        }
    }
}