#if KAMGAM_TND_UPSCALING && (TND_URP || TND_HDRP || TND_BIRP)
using System.Collections.Generic;
using UnityEngine;
using TND.Upscaling.Framework;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// With a big round of updates TheNakedDev unified the API for all upscalers.<br />
    /// This is the connection class for that and it replaces all the upscaler specific ones.<br />
    /// DLSS since version 1.5.0.<br />
    /// FSR 3 since version 1.8.0.<br />
    /// SGSR 1 Mobile since version 1.3.0.<br />
    /// ..
    /// </summary>
    public class TheNakedDevUpscalerConnection : ConnectionWithOptions<string>
    {
        protected List<string> _labels;
        
        /// <summary>
        /// If enabled then the camera detection will search (an prefer) cameras with the SettingsMainCameraMarker component on it.
        /// </summary>
        public bool CheckForCameraMarker = true;

        public TheNakedDevUpscalerConnection()
        {
        }
        
#if KAMGAM_TND_UPSCALING
        public TNDUpscaler GetUtils()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                return null;
#endif
            
            TNDUpscaler utils = null;
            
            var cam = RenderUtils.GetCurrentRenderingCamera(CheckForCameraMarker);
            if (cam != null)
                utils = cam.GetComponent<TNDUpscaler>();
            if (utils != null)
                return utils;

            cam = Camera.current;
            if (cam != null)
                utils = cam.GetComponent<TNDUpscaler>();

            if (utils == null)
            {
                Logger.LogWarning("TheNakedDevUpscalerConnection: Could not find the TNDUpscaler component on the current camera. Please make sure you have it added (ignoring all upscaler settings for now).");
            }

            return utils;
        }
#endif

        public override List<string> GetOptionLabels()
        {
#if KAMGAM_TND_UPSCALING
            if (_labels == null)
            {
                _labels = new List<string>();
                var qualityNames = System.Enum.GetNames(typeof(UpscalerQuality));
                foreach (var name in qualityNames)
                {
                    _labels.Add(name);
                }

                // The first option is "Custom" which we do not want to show to the user.
                // But instead we want to show an "Off" option so we replace "Custom" with "Off".
                _labels[0] = "Off";
            }
#else
            Logger.LogWarning("TheNakedDevUpscalerConnection: TheNakedDev upscalers are not yet set up. Please consult the TheNakedDev manual for more info and support.");
#endif
            return _labels;
        }

        /// <summary>
        /// Be aware that option 0 always has to be Off and indices 1+ have to match UpScalerQuality enum values.
        /// </summary>
        /// <param name="optionLabels"></param>
        public override void SetOptionLabels(List<string> optionLabels)
        {
            if (optionLabels == null)
            {
                Debug.LogError("Invalid new labels. Need to be four.");
                return;
            }

            _labels = optionLabels;
        }

        public override void RefreshOptionLabels()
        {
            _labels = null;
            GetOptionLabels();
        }

        public override int Get()
        {
#if KAMGAM_TND_UPSCALING
            var utils = GetUtils();
            if (utils != null)
            {
                return (int)GetFieldValue<UpscalerQuality>(utils, "qualityMode");
            }
            else
            {
                return 0;
            }
#else
            Logger.LogWarning("AlteregoFSR2Connection: Alterego FSR2 is not yet set up. Please consult the Alterego Games Manual for more info and support.");
            return 0;
#endif
        }
        
        public static T GetFieldValue<T>(object obj, string fieldName)
        {
            System.Type type = obj.GetType();

            var fieldInfo = type.GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldInfo == null)
            {
                Logger.LogError("TheNakedDev UpscalerController.qualityMode was not found. Maybe the internal API changed. Please contact TheNakedDev for support.");
                return default;
            }
            
            object value = fieldInfo.GetValue(obj);
            if (value is T typedValue)
            {
                return typedValue;
            }

            return default;
        }

        public override void Set(int index)
        {
#if KAMGAM_TND_UPSCALING
            var utils = GetUtils();
            if (utils != null)
            {
                // We use 0 as the value for disabling it completely.
                utils.enabled = index != 0;
                // But we also allow setting it 0 "Custom" which usually is not exposed to the user so we use it as a fallback for "off".
                utils.SetQuality((UpscalerQuality)index);
                
            }
#endif
            NotifyListenersIfChanged(index);
        }
    }
}
#endif