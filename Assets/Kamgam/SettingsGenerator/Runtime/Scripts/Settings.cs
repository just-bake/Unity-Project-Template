using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Settings are a scriptable object and they are loaded
    /// immediately (synchronously) upon first access through
    /// the SettingsProvider.
    /// 
    /// If a new setting is added via code (GetOrCreate(..))
    /// then Apply() should be called on it to push the default
    /// value to the connection.
    /// </summary>
    [CreateAssetMenu(fileName = "Settings", menuName = "SettingsGenerator/Settings", order = 2)]
    public partial class Settings : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Triggered whenever a setting is changed.
        /// </summary>
        public event System.Action<ISetting> OnSettingChanged;

        /// <summary>
        /// Is true while the settings are loaded and post processed.
        /// </summary>
        protected bool _isLoading = false;

        protected List<ISetting> _settingsCache = new List<ISetting>();

        [SerializeField]
        protected List<SettingBool> _bools = new List<SettingBool>();

        [SerializeField]
        protected List<SettingOption> _options = new List<SettingOption>();

        [SerializeField]
        protected List<SettingInt> _integers = new List<SettingInt>();

        [SerializeField]
        protected List<SettingFloat> _floats = new List<SettingFloat>();

        [SerializeField]
        protected List<SettingString> _strings = new List<SettingString>();

        [SerializeField]
        protected List<SettingColor> _colors = new List<SettingColor>();

        [SerializeField]
        protected List<SettingColorOption> _colorOptions = new List<SettingColorOption>();

        [SerializeField]
        protected List<SettingKeyCombination> _keyCombinations = new List<SettingKeyCombination>();

        /// <summary>
        /// Use this list to mark setting ids as inactive to ensure they are never ever applied even
        /// if a previously active setting is being deactivated in a new project version.
        /// </summary>
        [System.NonSerialized]
        public static List<string> DeactivateBeforeInit = new List<string>();

        /// <summary>
        /// Use this list to mark setting ids as inactive to ensure they are never ever applied even
        /// if a previously active setting is being deactivated in a new project version.
        /// </summary>
        /// <param name="ids"></param>
        public static void AddToDeactivateBeforeInit(params string[] ids)
        {
            if (ids == null)
                return;

            for (int i = 0; i < ids.Length; i++)
            {
                DeactivateBeforeInit.Add(ids[i]);
            }
        }
        
        public delegate void CustomStorageMethod(string key, Settings settings);

        /// <summary>
        /// If set then this will be called when saving the settings.<br />
        /// The first parameter (string) is the playerPrefsKey.<br />
        /// The second parameter (Settings) is the current Settings instance.
        /// <br /><br />
        /// NOTICE: You have to set these BEFORE the Settings are used for the
        /// very first time or else it will still use the default method.
        /// </summary>
        [System.NonSerialized]
        public static CustomStorageMethod CustomSaveMethod;

        /// <summary>
        /// If set then this will be called when loading the settings.<br />
        /// The first parameter (string) is the playerPrefsKey.<br />
        /// The second parameter (Settings) is the current Settings instance.
        /// <br /><br />
        /// NOTICE: You have to set these BEFORE the Settings are used for the
        /// very first time or else it will still use the default method.
        /// </summary>
        [System.NonSerialized]
        public static CustomStorageMethod CustomLoadMethod;

        /// <summary>
        /// If set then this will be called when deleting all the settings.<br />
        /// The first parameter (string) is the playerPrefsKey.<br />
        /// The second parameter (Settings) is the current Settings instance.
        /// <br /><br />
        /// NOTICE: You have to set these BEFORE the Settings are used for the
        /// very first time or else it will still use the default method.
        /// </summary>
        [System.NonSerialized]
        public static CustomStorageMethod CustomDeleteMethod;
        
#if UNITY_EDITOR
        [System.NonSerialized]
        protected SerializedObject _serializedObject;

        public SerializedObject SerializedObject
        {
            get
            {
                if (_serializedObject == null)
                {
                    _serializedObject = new SerializedObject(this);
                }

                return _serializedObject;
            }
        }

        public SerializedProperty GetSettingAsSerializedProperty(string id)
        {
            var setting = GetSetting(id);
            if (setting == null)
                return null;
            
            SerializedProperty property = SerializedObject.GetIterator();
            do
            {
                if (property.type.StartsWith("Setting"))
                {
                    var idProp = property.FindPropertyRelative(SettingWithValue<float>._IdFieldName);
                    if (idProp != null && idProp.stringValue == id)
                    {
                        return property;
                    }
                }
            }
            while (property.Next(true));

            return null;
        }
#endif

        // This is called whenever a new setting was added (that includes after serialization or after loading).
        public void RebuildSettingsCache()
        {
            _settingsCache.Clear();

            foreach (var setting in _bools)
                if(setting != null)
                    _settingsCache.Add(setting); 

            foreach (var setting in _options)
                if (setting != null)
                    _settingsCache.Add(setting);

            foreach (var setting in _integers)
                if (setting != null)
                    _settingsCache.Add(setting);

            foreach (var setting in _floats)
                if (setting != null)
                    _settingsCache.Add(setting);

            foreach (var setting in _strings)
                if (setting != null)
                    _settingsCache.Add(setting);

            foreach (var setting in _colors)
                if (setting != null)
                    _settingsCache.Add(setting);

            foreach (var setting in _colorOptions)
                if (setting != null)
                    _settingsCache.Add(setting);

            foreach (var setting in _keyCombinations)
                if (setting != null)
                    _settingsCache.Add(setting);

            // Add change listener
            foreach (var setting in _settingsCache)
            {
                setting.OnSettingChanged -= onSettingChanged;
                setting.OnSettingChanged += onSettingChanged;
            }
        }

        public List<ISetting> GetAllSettings()
        {
            if (_settingsCache.IsNullOrEmpty())
                RebuildSettingsCache();
            
            return _settingsCache;
        }
        
        /// <summary>
        /// Gets all unapplied settings and stores them in the results list (clears the list before adding).
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public List<ISetting> GetUnappliedSettings(List<ISetting> results = null)
        {
            if (results == null)
                results = new List<ISetting>();
            else
                results.Clear();

            var settings = GetAllSettings();
            foreach (var setting in settings)
            {
                if (!setting.IsActive)
                    continue;
                
                if (setting.HasUnappliedChanges())
                    results.Add(setting);
            }

            return results;
        }
        
        public bool HasUnappliedSettings()
        {
            var settings = GetAllSettings();
            foreach (var setting in settings)
            {
                if (!setting.IsActive)
                    continue;

                if (setting.HasUnappliedChanges())
                    return true;
            }

            return false;
        }
        
        protected void onSettingChanged(ISetting setting)
        {
            if (!_isLoading && setting.IsActive)
            {
                OnSettingChanged?.Invoke(setting);
            }
        }

        public void RemoveSetting(ISetting setting)
        {
            RemoveSetting(setting.GetID());
        }

        public void RemoveSetting(string id)
        {
            removeSetting(_bools, id);
            removeSetting(_options, id);
            removeSetting(_integers, id);
            removeSetting(_floats, id);
            removeSetting(_strings, id);
            removeSetting(_colors, id);
            removeSetting(_colorOptions, id);
            removeSetting(_keyCombinations, id);

            removeSetting(_settingsCache, id);
        }

        protected void removeSetting<T>(List<T> list, string id) where T : ISetting
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].MatchesID(id))
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }

        public void OnBeforeSerialize()
        {
            RebuildSettingsCache();

            foreach (var setting in _settingsCache)
            {
                setting.OnBeforeSerialize();
            }
        }

        public void OnAfterDeserialize()
        {
            RebuildSettingsCache();

            foreach (var setting in _settingsCache)
            {
                setting.OnAfterDeserialize();
            }
        }

        public void Load(string key, SettingsSaverBase settingsSaver)
        {
            Load(key, settingsSaver, removeUnknownSettingsAfterLoad: false);
        }

        private static List<string> _tmpExistingIdsBeforeLoad = new List<string>(20);
        
        /// <summary>
        /// Loads the settings from storage into memory.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="settingsSaver"></param>
        /// <param name="removeUnknownSettingsAfterLoad">If enabled then settings that are added via code need to be added before loading (otherwise those would be removed too).</param>
        public void Load(string key, SettingsSaverBase settingsSaver, bool removeUnknownSettingsAfterLoad)
        {
            _isLoading = true;

            if (removeUnknownSettingsAfterLoad)
            {
                // Gather list of know setting ids.
                // We assume the settings that should have been added via code have already been added at this point.
                RebuildSettingsCache();
                _tmpExistingIdsBeforeLoad.Clear();
                foreach (var setting in _settingsCache)
                {
                    _tmpExistingIdsBeforeLoad.Add(setting.GetID());
                }
            }

            if (CustomLoadMethod != null)
            {
                CustomLoadMethod.Invoke(key, this);
            }
            else
            {
                settingsSaver.LoadInto(key, this);
            }
            
            if (removeUnknownSettingsAfterLoad)
            {
                // Check if the loaded IDs have existed before. If not then remove them.
                RebuildSettingsCache();
                for (int i = _settingsCache.Count-1; i >= 0; i--)
                {
                    if (!_tmpExistingIdsBeforeLoad.Contains(_settingsCache[i].GetID()))
                        RemoveSetting(_settingsCache[i]);
                }
                _tmpExistingIdsBeforeLoad.Clear();
                RebuildSettingsCache();
            }

            postLoad();

            _isLoading = false;
        }

        /// <summary>
        /// Initialized connections, marks all settings as changed and the applied all settings.
        /// <br /><br />
        /// It is important that this is called BEFORE the first Connection.SET() call on any setting
        /// because this fetches the default values from the connections.
        /// </summary>
        protected void postLoad()
        {
            deactivateBeforeInitialization();

            RebuildSettingsCache();

            // Pull the initial default values from connections (only if no user setting was saved).
            // This also fetches the default value from the connection.
            foreach (var setting in _settingsCache)
            {
                // Skip inactive settings
                if (!setting.IsActive)
                    continue;

                // Set settings reference if connection is an IConnectionWithSettingsAccess
                if (setting.HasConnection())
                {
                    var connection = setting.GetConnectionInterface();
                    var connectionWithSettingsAccess = connection as IConnectionWithSettingsAccess;
                    if (connectionWithSettingsAccess != null)
                    {
                        connectionWithSettingsAccess.SetSettings(this);
                    }
                }

                setting.InitializeConnection(); // auto initializes default value for settings with connections

                // Explicitly initialize value with default if a setting does not yet have user data.
                if (!setting.HasConnection() && !setting.HasUserData())
                {
                    setting.ResetToDefault();
                }
            }

            // Mark all as changed (important for Connections which effect many other settings, example: QualityConnection).
            foreach (var setting in _settingsCache)
            {
                // Skip inactive settings
                if (!setting.IsActive)
                    continue;

                setting.MarkAsChanged();
            }

            // Apply the settings after loading them (that's what most people would expect).
            Apply(changedOnly: true);

            // Initially this does nothing in the current setup as all resolvers
            // use the provider which loads the settings synchronously and
            // thus no resolver can be registered at this time.
            // However it does refresh if it's called after the initial load.
            RefreshRegisteredResolvers();
        }

        protected void deactivateBeforeInitialization()
        {
            foreach (var id in DeactivateBeforeInit)
            {
                var setting = GetSetting(id);
                if (setting != null)
                    setting.IsActive = false;
            }
        }

        public void Save(string key, SettingsSaverBase settingsSaver)
        {
            if (CustomSaveMethod != null)
            {
                CustomSaveMethod.Invoke(key, this);
            }
            else
            {
                settingsSaver.Save(key, this);
            }
        }

        public void Delete(string key, SettingsSaverBase settingsSaver)
        {
            if (CustomDeleteMethod != null)
            {
                CustomDeleteMethod.Invoke(key, this);
            }
            else
            {
                settingsSaver.Delete(key);
            }
        }

        public static void DeletePlayerPrefs(string playerPrefsKey)
        {
            PlayerPrefs.DeleteKey(playerPrefsKey);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Applies the settings.<br />
        /// This means that if a setting has a connection it will be pushed and then pulled.
        /// </summary>
        /// <param name="changedOnly">Apply only those which have changed.</param>
        /// <param name="triggerChangeEvents">Since apply usually only triggers a push to connections you can use this to also trigger the change event on all settings without connections.</param>
        public void Apply(bool changedOnly = true, bool triggerChangeEvents = false)
        {
            var sortedSettings = getSettingsOrderedByConnectionOrderASC(_settingsCache);
            ISetting setting;

            // If all settings should be applied then mark them all as changed first.
            // We do this so that even if a setting implements Apply() as being executed only
            // if changed it will still execute.
            if (!changedOnly)
            {
                for (int i = 0; i < sortedSettings.Count; i++)
                {
                    setting = sortedSettings[i];

                    // Skip inactive settings
                    if (!setting.IsActive)
                        continue;

                    setting.MarkAsChanged();
                }
            }

            for (int i = 0; i < sortedSettings.Count; i++)
            {
                setting = sortedSettings[i];

                // Skip inactive settings
                if (!setting.IsActive)
                    continue;

                if (changedOnly && !setting.HasUnappliedChanges())
                    continue;

                setting.Apply();
            }

            if (triggerChangeEvents)
            {
                TriggerChangeEvent(skipSettingsWithConnections: true);
            }
        }
        
        /// <summary>
        /// Triggers the change event on each setting. By default only on settings that have NO connection. For settings with connections use Apply().
        /// </summary>
        /// <param name="skipSettingsWithConnections"></param>
        public void TriggerChangeEvent(bool skipSettingsWithConnections = true)
        {
            var sortedSettings = getSettingsOrderedByConnectionOrderASC(_settingsCache);
            ISetting setting;
            for (int i = 0; i < sortedSettings.Count; i++)
            {
                setting = sortedSettings[i];

                // Skip inactive settings
                if (!setting.IsActive)
                    continue;

                // Skip settings with connections
                if (skipSettingsWithConnections && setting.HasConnection())
                    continue;
                
                setting.OnChanged();
            }
        }

        /// <summary>
        /// Makes all settings that use this connection pull their values from it.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="exceptUnapplied">Set to TRUE if you do not want values to be pulled for settings which still have unapplied changes.</param>
        /// <param name="propagateChange">Set to TRUE then this will trigger the onSettingChanged event.</param>
        public void PullFromConnection(IConnection connection, bool exceptUnapplied = false, bool propagateChange = false)
        {
            var sortedSettings = getSettingsOrderedByConnectionOrderASC(_settingsCache);
            ISetting setting;
            for (int i = 0; i < sortedSettings.Count; i++)
            {
                setting = sortedSettings[i];

                // Skip inactive settings
                if (!setting.IsActive)
                    continue;

                if (!setting.HasConnection())
                    continue;

                if (exceptUnapplied && setting.HasUnappliedChanges())
                    continue;

                if (setting.GetConnectionInterface() == connection)
                {
                    setting.PullFromConnection(propagateChange);
                }
            }
        }

        /// <summary>
        /// Makes all settings that use this connection push theor value to it.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="exceptUnapplied">Set to TRUE if you do not want values to be pulled for settings which still have unapplied changes.</param>
        public void PushToConnection(IConnection connection, bool exceptUnapplied = false)
        {
            var sortedSettings = getSettingsOrderedByConnectionOrderASC(_settingsCache);
            ISetting setting;
            for (int i = 0; i < sortedSettings.Count; i++)
            {
                setting = sortedSettings[i];

                // Skip inactive settings
                if (!setting.IsActive)
                    continue;

                if (!setting.HasConnection())
                    continue;

                if (exceptUnapplied && setting.HasUnappliedChanges())
                    continue;

                if (setting.GetConnectionInterface() == connection)
                {
                    setting.PushToConnection();
                }
            }
        }

        /// <summary>
        /// Pulls all values from the connections.
        /// </summary>
        /// <param name="exceptUnapplied">Set to TRUE if you do not want values to be pulled for settings which still have unapplied changes.</param>
        /// <param name="propagateChange">Set to TRUE then this will trigger the onSettingChanged event.</param>
        public void PullFromConnections(bool exceptUnapplied = false, bool propagateChange = false)
        {
            var sortedSettings = getSettingsOrderedByConnectionOrderASC(_settingsCache);
            ISetting setting;
            for (int i = 0; i < sortedSettings.Count; i++)
            {
                setting = sortedSettings[i];

                // Skip inactive settings
                if (!setting.IsActive)
                    continue;

                if (!setting.HasConnection())
                    continue;

                if (exceptUnapplied && setting.HasUnappliedChanges())
                    continue;
                
                setting.PullFromConnection(propagateChange);
            }
        }

        public void PushToConnections()
        {
            var sortedSettings = getSettingsOrderedByConnectionOrderASC(_settingsCache);
            ISetting setting;
            for (int i = 0; i < sortedSettings.Count; i++)
            {
                setting = sortedSettings[i];

                // Skip inactive settings
                if (!setting.IsActive)
                    continue;

                if (setting.HasConnection())
                {
                    setting.PushToConnection();
                }
            }
        }

        public void PushToConnections(params string[] groups)
        {
            var sortedSettings = getSettingsOrderedByConnectionOrderASC(_settingsCache);
            ISetting setting;
            for (int i = 0; i < sortedSettings.Count; i++)
            {
                setting = sortedSettings[i];

                // Skip inactive settings
                if (!setting.IsActive)
                    continue;

                if (setting.MatchesAnyGroup(groups))
                {
                    if (setting.HasConnection())
                    {
                        setting.PushToConnection();
                    }
                }
            }
        }

        #region sort by connection order
        protected List<ISetting> _tmpSettingsSortedByConnectionOrder;

        protected List<ISetting> getSettingsOrderedByConnectionOrderASC(IEnumerable<ISetting> settings)
        {
            if (_tmpSettingsSortedByConnectionOrder == null)
                _tmpSettingsSortedByConnectionOrder = new List<ISetting>();
            _tmpSettingsSortedByConnectionOrder.Clear();

            foreach (var setting in settings)
            {
                // Skip inactive settings
                if (!setting.IsActive)
                    continue;

                _tmpSettingsSortedByConnectionOrder.Add(setting);
            }

            _tmpSettingsSortedByConnectionOrder.Sort(compartByConnectionOrder);

            return _tmpSettingsSortedByConnectionOrder;
        }

        protected int compartByConnectionOrder(ISetting a, ISetting b)
        {
            return a.GetConnectionOrder() - b.GetConnectionOrder();
        }
#endregion

#region sort by name/id
        protected List<ISetting> _tmpSettingsSortedByName;

        protected List<ISetting> getSettingsOrderedByID(IEnumerable<ISetting> settings)
        {
            if (_tmpSettingsSortedByName == null)
                _tmpSettingsSortedByName = new List<ISetting>();
            _tmpSettingsSortedByName.Clear();

            foreach (var setting in settings)
            {
                _tmpSettingsSortedByName.Add(setting);
            }

            _tmpSettingsSortedByName.Sort(compareByID);

            return _tmpSettingsSortedByName;
        }

        protected int compareByID(ISetting a, ISetting b)
        {
            return string.Compare(a.GetID(), b.GetID());
        }
        #endregion

        public bool HasID(string id)
        {
            return GetSetting(id) != null;
        }

        public bool HasActiveID(string id)
        {
            return GetActiveSetting(id) != null;
        }

        public ISetting GetSetting(string id)
        {
            foreach (var setting in _settingsCache)
            {
                if (setting.GetID() == id)
                {
                    return setting;
                }
            }

            return null;
        }

        public ISetting GetActiveSetting(string id)
        {
            var setting = GetSetting(id);
            if (setting != null && setting.IsActive)
                return setting;
            else
                return null;
        }

        protected bool doesOtherSettingExist(string id, SettingData.DataType dataType)
        {
            var setting = GetSetting(id);
            if (setting != null && setting.GetDataType() != dataType)
            {
                Debug.LogError("You are trying to create '"+id+"' (type: '"+dataType+"') but another '"+id+"' with a DIFFERENT type ('" + setting.GetDataType() + "') already exists. Aborting creation. Duplicate IDs are not allowed.");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Search for the id in the settings.
        /// <br />
        /// If a setting with that id is found then it will be returned.
        /// <br />
        /// If no setting is found then a new setting with the id will be created.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataType">SettingData.DataType.Unknown will always lead to a return value of null. No setting will be created.</param>
        /// <returns></returns>
        public ISetting GetOrCreate(string id, SettingData.DataType dataType)
        {
            switch (dataType)
            {
                case SettingData.DataType.Int:
                    return GetOrCreateInt(id);

                case SettingData.DataType.Float:
                    return GetOrCreateFloat(id);

                case SettingData.DataType.Bool:
                    return GetOrCreateBool(id);

                case SettingData.DataType.String:
                    return GetOrCreateString(id);

                case SettingData.DataType.Color:
                    return GetOrCreateColor(id, Color.black);

                case SettingData.DataType.KeyCombination:
                    return GetOrCreateKeyCombination(id, new KeyCombination(UGUIComponentsForSettings.UniversalKeyCode.None));

                case SettingData.DataType.Option:
                    return GetOrCreateOption(id);

                case SettingData.DataType.ColorOption:
                    return GetOrCreateColorOption(id);

                case SettingData.DataType.Unknown:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Search for the id in the settings.
        /// <br /><br />
        /// If a setting with that id is found then it will
        /// override the connection and groups of that setting and return it.<br />
        /// The "defaultValue" will be ignored if the settings already exists
        /// or if a connection is specified.
        /// <br /><br />
        /// If no setting is found then a new setting with the id,
        /// defaultValue, groups and connection will be created.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultValue">Default value if no connection is set.</param>
        /// <param name="groups"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public SettingBool GetOrCreateBool(string id, bool defaultValue = false, List<string> groups = null, IConnection<bool> connection = null)
        {
            // Try to find
            var setting = GetBool(id);

            // If not found then create
            if (setting == null)
            {
                setting = addBool(id, defaultValue, groups);
            }
            else
            {
                if (groups != null)
                {
                    setting.SetGroups(groups);
                }
            }

            initConnectionForSetting(setting, connection);

            return setting;
        }

        protected void initConnectionForSetting<T>(ISettingWithConnection<T> setting, IConnection<T> connection)
        {
            if (connection != null)
            {
                // Set settings reference if connection is an IConnectionWithSettingsAccess
                var connectionWithSettingsAccess = connection as IConnectionWithSettingsAccess;
                if (connectionWithSettingsAccess != null)
                {
                    connectionWithSettingsAccess.SetSettings(this);
                }

                // Override connection (if one was given)
                setting.SetConnection(connection);
            }
        }

        public SettingBool GetBool(string id)
        {
            foreach (var setting in _bools)
            {
                if (setting.GetID() == id)
                {
                    return setting;
                }
            }

            return null;
        }

        protected SettingBool addBool(string id, bool value, List<string> groups = null)
        {
            if (doesOtherSettingExist(id, SettingData.DataType.Bool))
                return null;

            var setting = new SettingBool(id, value, groups);
            _bools.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        public SettingBool AddBoolFromSerializedData(SettingData data, List<string> groups = null)
        {
            if (doesOtherSettingExist(data.ID, SettingData.DataType.Bool))
                return null;

            var setting = new SettingBool(data, groups);
            _bools.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        /// <summary>
        /// Search for the id in the settings.
        /// <br /><br />
        /// If a setting with that id is found then it will
        /// override the connection and groups of that setting and return it.<br />
        /// The "defaultValue" will be ignored if the settings already exists
        /// or if a connection is specified.
        /// <br /><br />
        /// If no setting is found then a new setting with the id,
        /// defaultValue, groups and connection will be created.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultValue">Default value if no connection is set.</param>
        /// <param name="groups"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public SettingColor GetOrCreateColor(string id, Color defaultValue, List<string> groups = null, IConnection<Color> connection = null)
        {
            var setting = GetColor(id);
            if (setting == null)
            {
                setting = addColor(id, defaultValue, groups);
            }
            else
            {
                if (groups != null)
                {
                    setting.SetGroups(groups);
                }
            }

            initConnectionForSetting(setting, connection);

            return setting;
        }

        public SettingColor GetColor(string id)
        {
            foreach (var setting in _colors)
            {
                if (setting.GetID() == id)
                {
                    return setting;
                }
            }

            return null;
        }

        protected SettingColor addColor(string id, Color value, List<string> groups = null)
        {
            if (doesOtherSettingExist(id, SettingData.DataType.Color))
                return null;

            var setting = new SettingColor(id, value, groups);
            _colors.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        public SettingColor AddColorFromSerializedData(SettingData data, List<string> groups = null)
        {
            if (doesOtherSettingExist(data.ID, SettingData.DataType.Color))
                return null;

            var setting = new SettingColor(data, groups);
            _colors.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        /// <summary>
        /// Search for the id in the settings.
        /// <br /><br />
        /// If a setting with that id is found then it will
        /// override the connection, options and groups of that setting and return it.<br />
        /// The "defaultValue" will be ignored if the settings already exists
        /// or if a connection is specified.
        /// <br /><br />
        /// If no setting is found then a new setting with the id,
        /// defaultValue, groups and connection will be created.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultOption">Default value if no connection is set.</param>
        /// <param name="groups"></param>
        /// <param name="options"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public SettingColorOption GetOrCreateColorOption(string id, int defaultOption = 0, List<string> groups = null, List<Color> options = null, IConnectionWithOptions<Color> connection = null)
        {
            var setting = GetColorOption(id);
            if (setting == null)
            {
                setting = addColorOption(id, defaultOption, groups, options);
            }
            else
            {
                if (groups != null && groups.Count > 0)
                {
                    setting.SetGroups(groups);
                }

                if (options != null && options.Count > 0)
                {
                    setting.SetOptionLabels(options);
                    RefreshRegisteredResolvers(id);
                }
            }

            initConnectionForSetting(setting, connection);

            return setting;
        }

        public SettingColorOption GetColorOption(string id)
        {
            foreach (var setting in _colorOptions)
            {
                if (setting.GetID() == id)
                {
                    return setting;
                }
            }

            return null;
        }

        protected SettingColorOption addColorOption(string id, int selectedIndex, List<string> groups = null, List<Color> options = null)
        {
            if (doesOtherSettingExist(id, SettingData.DataType.ColorOption))
                return null;

            var setting = new SettingColorOption(id, selectedIndex, groups, options);
            _colorOptions.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        public SettingColorOption AddColorOptionFromSerializedData(SettingData data, List<string> groups = null, List<Color> options = null)
        {
            if (doesOtherSettingExist(data.ID, SettingData.DataType.ColorOption))
                return null;

            var setting = new SettingColorOption(data, groups, options);
            _colorOptions.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        /// <summary>
        /// Search for the id in the settings.
        /// <br /><br />
        /// If a setting with that id is found then it will
        /// override the connection and groups of that setting and return it.<br />
        /// The "defaultValue" will be ignored if the settings already exists
        /// or if a connection is specified.
        /// <br /><br />
        /// If no setting is found then a new setting with the id,
        /// defaultValue, groups and connection will be created.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultValue">Default value if no connection is set.</param>
        /// <param name="groups"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public SettingFloat GetOrCreateFloat(string id, float defaultValue = 0f, List<string> groups = null, IConnection<float> connection = null)
        {
            var setting = GetFloat(id);
            if (setting == null)
            {
                setting = addFloat(id, defaultValue, groups);
            }
            else
            {
                if (groups != null)
                {
                    setting.SetGroups(groups);
                }
            }

            initConnectionForSetting(setting, connection);

            return setting;
        }

        public SettingFloat GetFloat(string id)
        {
            foreach (var setting in _floats)
            {
                if (setting.GetID() == id)
                {
                    return setting;
                }
            }

            return null;
        }

        protected SettingFloat addFloat(string id, float value, List<string> groups = null)
        {
            if (doesOtherSettingExist(id, SettingData.DataType.Float))
                return null;

            var setting = new SettingFloat(id, value, groups);
            _floats.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        public SettingFloat AddFloatFromSerializedData(SettingData data, List<string> groups = null)
        {
            if (doesOtherSettingExist(data.ID, SettingData.DataType.Float))
                return null;

            var setting = new SettingFloat(data, groups);
            _floats.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        /// <summary>
        /// Search for the id in the settings.
        /// <br /><br />
        /// If a setting with that id is found then it will
        /// override the connection and groups of that setting and return it.<br />
        /// The "defaultValue" will be ignored if the settings already exists
        /// or if a connection is specified.
        /// <br /><br />
        /// If no setting is found then a new setting with the id,
        /// defaultValue, groups and connection will be created.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultValue">Default value if no connection is set.</param>
        /// <param name="groups"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public SettingInt GetOrCreateInt(string id, int defaultValue = 0, List<string> groups = null, IConnection<int> connection = null)
        {
            var setting = GetInt(id);
            if (setting == null)
            {
                setting = addInt(id, defaultValue, groups);
            }
            else
            {
                if (groups != null)
                {
                    setting.SetGroups(groups);
                }
            }

            initConnectionForSetting(setting, connection);

            return setting;
        }

        public SettingInt GetInt(string id)
        {
            foreach (var setting in _integers)
            {
                if (setting.GetID() == id)
                {
                    return setting;
                }
            }

            return null;
        }

        protected SettingInt addInt(string id, int value, List<string> groups = null)
        {
            if (doesOtherSettingExist(id, SettingData.DataType.Int))
                return null;

            var setting = new SettingInt(id, value, groups);
            _integers.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        public SettingInt AddIntFromSerializedData(SettingData data, List<string> groups = null)
        {
            if (doesOtherSettingExist(data.ID, SettingData.DataType.Int))
                return null;

            var setting = new SettingInt(data, groups);
            _integers.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        /// <summary>
        /// Search for the id in the settings.
        /// <br /><br />
        /// If a setting with that id is found then it will
        /// override the connection and groups of that setting and return it.<br />
        /// The "defaultValue" will be ignored if the settings already exists
        /// or if a connection is specified.
        /// <br /><br />
        /// If no setting is found then a new setting with the id,
        /// defaultValue, groups and connection will be created.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultValue">Default value if no connection is set.</param>
        /// <param name="groups"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public SettingKeyCombination GetOrCreateKeyCombination(string id, KeyCombination defaultValue, List<string> groups = null, IConnection<KeyCombination> connection = null)
        {
            var setting = GetKeyCombination(id);
            if (setting == null)
            {
                setting = addKeyCombination(id, defaultValue, groups);
            }
            else
            {
                if (groups != null)
                {
                    setting.SetGroups(groups);
                }
            }

            initConnectionForSetting(setting, connection);

            return setting;
        }

        protected SettingKeyCombination addKeyCombination(string id, KeyCombination value, List<string> groups = null)
        {
            if (doesOtherSettingExist(id, SettingData.DataType.KeyCombination))
                return null;

            var setting = new SettingKeyCombination(id, value, groups);
            _keyCombinations.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        public SettingKeyCombination AddKeyCombinationFromSerializedData(SettingData data, List<string> groups = null)
        {
            if (doesOtherSettingExist(data.ID, SettingData.DataType.KeyCombination))
                return null;

            var setting = new SettingKeyCombination(data, groups);
            _keyCombinations.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        public SettingKeyCombination GetKeyCombination(string id)
        {
            foreach (var setting in _keyCombinations)
            {
                if (setting.GetID() == id)
                {
                    return setting;
                }
            }

            return null;
        }

        /// <summary>
        /// Search for the id in the settings.
        /// <br /><br />
        /// If a setting with that id is found then it will
        /// override the connection, options and groups of that setting and return it.<br />
        /// The "defaultValue" will be ignored if the settings already exists
        /// or if a connection is specified.
        /// <br /><br />
        /// If no setting is found then a new setting with the id,
        /// defaultValue, groups and connection will be created.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultOption">Default value if no connection is set.</param>
        /// <param name="groups"></param>
        /// <param name="options"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public SettingOption GetOrCreateOption(string id, int defaultOption = 0, List<string> groups = null, List<string> options = null, IConnectionWithOptions<string> connection = null)
        {
            var setting = GetOption(id);
            if (setting == null)
            {
                setting = addOption(id, defaultOption, groups, options);
            }
            else
            {
                if (groups != null && groups.Count > 0)
                {
                    setting.SetGroups(groups);
                }

                if (options != null && options.Count > 0)
                {
                    setting.SetOptionLabels(options);
                    RefreshRegisteredResolvers(id);
                }
            }

            initConnectionForSetting(setting, connection);

            return setting;
        }

        public SettingOption GetOption(string id)
        {
            foreach (var setting in _options)
            {
                if (setting.GetID() == id)
                {
                    return setting;
                }
            }

            return null;
        }

        protected SettingOption addOption(string id, int selectedIndex, List<string> groups = null, List<string> options = null)
        {
            if (doesOtherSettingExist(id, SettingData.DataType.Option))
                return null;

            var setting = new SettingOption(id, selectedIndex, groups, options);
            _options.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        public SettingOption AddOptionFromSerializedData(SettingData data, List<string> groups = null, List<string> options = null)
        {
            if (doesOtherSettingExist(data.ID, SettingData.DataType.Option))
                return null;

            var setting = new SettingOption(data, groups, options);
            _options.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        /// <summary>
        /// Search for the id in the settings.
        /// <br /><br />
        /// If a setting with that id is found then it will
        /// override the connection and groups of that setting and return it.<br />
        /// The "defaultValue" will be ignored if the settings already exists
        /// or if a connection is specified.
        /// <br /><br />
        /// If no setting is found then a new setting with the id,
        /// defaultValue, groups and connection will be created.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultValue">Default value if no connection is set.</param>
        /// <param name="groups"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public SettingString GetOrCreateString(string id, string defaultValue = "", List<string> groups = null, IConnection<string> connection = null)
        {
            var setting = GetString(id);
            if (setting == null)
            {
                setting = addString(id, defaultValue, groups);
            }
            else
            {
                if (groups != null)
                {
                    setting.SetGroups(groups);
                }
            }

            initConnectionForSetting(setting, connection);

            return setting;
        }

        public SettingString GetString(string id)
        {
            foreach (var setting in _strings)
            {
                if (setting.GetID() == id)
                {
                    return setting;
                }
            }

            return null;
        }

        protected SettingString addString(string id, string value, List<string> groups = null)
        {
            if (doesOtherSettingExist(id, SettingData.DataType.String))
                return null;

            var setting = new SettingString(id, value, groups);
            _strings.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        public SettingString AddStringFromSerializedData(SettingData data, List<string> groups = null)
        {
            if (doesOtherSettingExist(data.ID, SettingData.DataType.String))
                return null;

            var setting = new SettingString(data, groups);
            _strings.Add(setting);
            RebuildSettingsCache();
            return setting;
        }

        public object GetValue(string id)
        {
            var setting = GetSetting(id);
            if (setting != null)
                return setting.GetValueAsObject();
            else
                return null;
        }

        public T GetValue<T>(string id)
        {
            var val = GetValue(id);
            if (val != null)
            {
                if (val is T)
                {
                    return (T)val;
                }
                else
                {
                    Debug.LogError(
                        "SGSettings: The value for id '" + id + "' could not be read because of a type mismatch.\n" +
                        "The type you requested (" + typeof(T).Name.Replace("Single", "Float") + ") does not match " +
                        "the '" + id + "' field in Settings (" + val.GetType().Name.Replace("Single", "Float") + ").\n" +
                        "You may also get an ArgumentException if you try to set this value."
                        );
                    return default(T);
                }
            }

            return default(T);
        }

        public void SetValue(string id, object value)
        {
            var setting = GetSetting(id);
            if (setting != null)
            {
                setting.SetValueFromObject(value);
            }
        }

        public void SetActive(string id, bool active)
        {
            var setting = GetSetting(id);
            if (setting != null)
            {
                setting.IsActive = active;
            }
        }

        public void SetAllActive(bool active)
        {
            foreach (var setting in _settingsCache)
            {
                setting.IsActive = active;
            }
        }

        /// <summary>
        /// Use this to notify settings about a change in QualitySettings.<br />
        /// The default "QualityConnection" implementation uses this.
        /// </summary>
        /// <param name="qualityLevel"></param>
        /// <param name="excludeChanged"></param>
        public void OnQualityChanged(int qualityLevel, bool excludeChanged = false)
        {
            var sortedSettings = getSettingsOrderedByConnectionOrderASC(_settingsCache);
            ISetting setting;
            for (int i = 0; i < sortedSettings.Count; i++)
            {
                setting = sortedSettings[i];

                // Skip inactive settings
                if (!setting.IsActive)
                    continue;

                if (excludeChanged && setting.HasUnappliedChanges())
                    continue;

                setting.OnQualityChanged(qualityLevel);
            }
        }

        public string[] GetSettingIDsOrderedByName(bool filterByDataType = false, params SettingData.DataType[] dataTypes)
        {
            var settings = getSettingsOrderedByID(_settingsCache);
            var result = settings
                .Where(s => !filterByDataType || dataTypes.Contains(s.GetDataType()))
                .Select(s => s.GetID());
            return result.ToArray();
        }

        public IList<TSetting> GetSettingsWithConnectionByType<TSetting, TConnection>(IList<TSetting> results = null)
            where TSetting : class, ISetting
            where TConnection : class, IConnection
        {
            if (results == null)
                results = new List<TSetting>();
            else
                results.Clear();

            foreach (var s in _settingsCache)
            {
                var setting = s as TSetting;
                if (setting != null)
                {
                    var connection = setting.GetConnectionInterface() as TConnection;
                    if (connection != null)
                    {
                        results.Add(setting);
                    }
                }
            }

            return results;
        }
        
        public TSetting GetFirstSettingWithConnectionByType<TSetting, TConnection>()
            where TSetting : class, ISetting
            where TConnection : class, IConnection
        {
            foreach (var s in _settingsCache)
            {
                var setting = s as TSetting;
                if (setting != null)
                {
                    var connection = setting.GetConnectionInterface() as TConnection;
                    if (connection != null)
                    {
                        return setting;
                    }
                }
            }

            return null;
        }
        
        public TConnection GetFirstConnectionByType<TConnection>()
            where TConnection : class, IConnection
        {
            foreach (var setting in _settingsCache)
            {
                if (setting != null && setting.HasConnection() && setting.GetConnectionInterface() is TConnection connection)
                {
                    return connection;
                }
            }

            return null;
        }
        
        public IList<TConnection> GetConnectionsByType<TConnection>(IList<TConnection> results = null)
            where TConnection : class, IConnection
        {
            if (results == null)
                results = new List<TConnection>();
            else
                results.Clear();
            
            foreach (var setting in _settingsCache)
            {
                if (setting != null && setting.HasConnection() && setting.GetConnectionInterface() is TConnection connection)
                {
                    if (!results.Contains(connection))
                        results.Add(connection);
                }
            }

            return results;
        }

        public IList<TSetting> GetSettingsWithConnection<TSetting>(IConnection connection, IList<TSetting> results = null)
            where TSetting : class, ISetting
        {
            if (results == null)
                results = new List<TSetting>();
            else
                results.Clear();

            foreach (var s in _settingsCache)
            {
                var setting = s as TSetting;
                if (setting != null)
                {
                    var settingConnection = setting.GetConnectionInterface();
                    if (settingConnection == connection)
                    {
                        results.Add(setting);
                    }
                }
            }

            return results;
        }

        public IList<ISetting> GetSettingsWithConnection(IConnection connection, IList<ISetting> results = null)
        {
            if (results == null)
                results = new List<ISetting>();
            else
                results.Clear();

            foreach (var setting in _settingsCache)
            {
                var settingConnection = setting.GetConnectionInterface();
                if (settingConnection == connection)
                {
                    results.Add(setting);
                }
            }

            return results;
        }

        public ISetting GetFirstSettingWithConnectionSO(ConnectionSO connectionSO)
        {
            foreach (var setting in _settingsCache)
            {
                if (setting.GetConnectionSO() == connectionSO)
                {
                    return setting;
                }
            }

            return null;
        }
        
        private static List<SettingOption> s_tmpRefreshSettingOptionConnectionAndResolversList = new List<SettingOption>(); 

        /// <summary>
        /// Updates the labels and state of all the settings that have the given connection type.
        /// </summary>
        /// <typeparam name="TConnection">The connection type which has to be of type ConnectionWithOptions&lt;string&gt;</typeparam>
        /// <param name="refreshResolvers">If true then the UI (the resolvers) are also refreshed. TRUE by default, usually you want this to happen.</param>
        public void RefreshSettingOptionConnectionAndResolvers<TConnection>(bool refreshResolvers = true) where TConnection : ConnectionWithOptions<string>
        {
            RefreshSettingOptionConnectionAndResolvers<TConnection, string>(refreshResolvers);
        }
        

        /// <summary>
        /// Updates the labels and state of all the settings that have the give connection type.
        /// </summary>
        /// <typeparam name="TConnection">The connection type which has to be of type ConnectionWithOptions&lt;string&gt;</typeparam>
        /// <typeparam name="TOption">Usually string but could also be anything else.</typeparam>
        /// <param name="refreshResolvers">If true then the UI (the resolvers) are also refreshed. TRUE by default, usually you want this to happen.</param>
        public void RefreshSettingOptionConnectionAndResolvers<TConnection, TOption>(bool refreshResolvers = true) where TConnection : ConnectionWithOptions<TOption>
        {
            GetSettingsWithConnectionByType<SettingOption, TConnection>(s_tmpRefreshSettingOptionConnectionAndResolversList);
            foreach (var setting in s_tmpRefreshSettingOptionConnectionAndResolversList)
            {
                if (setting.HasConnection())
                {
                    var connection = setting.GetConnectionInterface() as TConnection;
                        connection.RefreshOptionLabels();
                    setting.PullFromConnection();
                }
            }

            if (refreshResolvers)
            {
                RefreshRegisteredResolversWithConnection<TConnection>();
            }
            
            s_tmpRefreshSettingOptionConnectionAndResolversList.Clear();
        }

#if ENABLE_INPUT_SYSTEM
        public void SetInputActionAsset(UnityEngine.InputSystem.InputActionAsset asset, bool applyImmediately = true)
        {
            // Update the connection asset in all settings with InputBindingConnections.
            var settingsWithBindings = GetSettingsWithConnectionByType<SettingString, InputBindingConnection>();
            foreach (var setting in settingsWithBindings)
            {
                var connection = setting.GetConnectionInterface() as InputBindingConnection;
                if (connection != null)
                {
                    connection.SetInputActionAsset(asset);

                    if (applyImmediately)
                    {
                        setting.Apply();
                    }
                }
            }
        }

        public UnityEngine.InputSystem.InputActionAsset GetInputActionAsset()
        {
            var settingsWithBindings = GetSettingsWithConnectionByType<SettingString, InputBindingConnection>();
            foreach (var setting in settingsWithBindings)
            {
                var connection = setting.GetConnectionInterface() as InputBindingConnection;
                if (connection != null)
                {
                    var asset = connection.GetInputActionAsset();
                    if (asset != null)
                        return asset;
                }
            }

            return null;
        }
#endif
    }
}
