using System;
using System.Collections.Generic;
using System.IO;
using ResharperToolsLib.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#nullable enable

namespace ResharperToolsLib.Config
{
    public class ConfigLoader
    {
        public const string ConfigJsonLocation = "./appsettings.json";

        public ConfigLoader(ILogger logger)
        {
            Logger = logger;
        }

        private ILogger Logger { get; }

        public ConfigModel? ReadConfig()
        {
            string? configJsonString = null;

            Logger.Info("Reading config");

            if (!ConfigExists())
            {
                Logger.Info("Config does not exist. Creating template config file.");
                File.WriteAllText(ConfigJsonLocation, CreateTemplateString());
                Logger.Info(@"Template config file ""appsettings.json"" created. Please edit it and restart.");
                return null;
            }

            try
            {
                using var file = File.OpenText(ConfigJsonLocation);
                configJsonString = file.ReadToEnd();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to open application settings file.", ex);
                return null;
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
        public static string CreateTemplateString()
        {

            var templateConfig = new ConfigModel()
            {
                RecentSolutions = new string[0]
            };

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(templateConfig, jsonSerializerSettings);
        }

        private ConfigModel? ParseJson(string configJsonString)
        {
            JObject configJson;
            try
            {
                configJson = JObject.Parse(configJsonString);
            }
            catch (JsonReaderException ex)
            {
                Logger.Error("Failed to parse application settings as json.", ex);
                return null;
            }

            try
            {
                var recentSolutions = configJson.GetObjectInsensitive<string[]>("recentSolutions");

                if (recentSolutions != null)
                {
                    var config = new ConfigModel()
                    {
                        RecentSolutions = recentSolutions
                    };

                    return config;
                }

                var logged = false;

                // compulsory properties
                new KeyValuePair<string, object?>[]
                {
                }.ForEach(prop =>
                {
                    if (prop.Value == null)
                    {
                        Logger.Error($"{prop.Key} must be provided in the appsettings.json!");
                        logged = true;
                    }
                });

                if (!logged)
                    throw new Exception(
                        "There is a mismatch in compulsory config properties!! " +
                        "Please let the developer know."
                    );

                return null;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to read values from application settings.", ex);
                return null;
            }
        }
    }
}