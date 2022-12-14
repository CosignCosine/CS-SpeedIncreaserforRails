using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace CS_RailTrackSpeedIncreaser
{
    public class SIRMod : LoadingExtensionBase, IUserMod
    {
        public string Name => "Speed Increaser for Rails";
        public string Description => "Increases maximum rail speed for all assets containing tracks to a configurable maximum speed.";

        public SavedFloat maxSpeed => new SavedFloat("MaxSpeed", "SIR4Mod", 11.25f, true);

        public UILabel optionsLabel;

        public void ResolveGameSettings()
        {
            if (GameSettings.FindSettingsFileByName("SIR4Mod") == null)
            {
                GameSettings.AddSettingsFile(new SettingsFile[] { new SettingsFile() { fileName = "SIR4Mod" } });
            }
        }
        public void OnEnable()
        {
            ResolveGameSettings();
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            ResolveGameSettings();

            UIComponent baseEl = (helper.AddSlider("Maximum Track Speed: " + (((float)maxSpeed) * 40f) + " (km/h)", 10f, 2000f, 10f, ((float)maxSpeed)*40f, (mxsp) =>
            {
                maxSpeed.value = (mxsp/40f);

                Debug.Log("Rail track speed is now " + mxsp + " km/h.");
                if (optionsLabel != null) optionsLabel.text = "Maximum Track Speed: " + mxsp + " (km/h)";

                SetSpeeds();
            }) as UIComponent);

            optionsLabel = baseEl.parent.Find("Label") as UILabel;
        }

        public void SetSpeeds()
        {
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                NetInfo network = PrefabCollection<NetInfo>.GetLoaded(i);
                if (network.m_lanes == null) continue;

                for (int j = 0; j < network.m_lanes.Length; j++)
                {
                    var lane = network.m_lanes[j];
                    if (lane != null && lane.m_laneType == NetInfo.LaneType.Vehicle && (lane.m_vehicleType & (VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Metro | VehicleInfo.VehicleType.Tram)) != 0)
                    {
                        lane.m_speedLimit = maxSpeed;
                    }
                }
                network.m_averageVehicleLaneSpeed = maxSpeed;
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if ((mode & (LoadMode.LoadGame | LoadMode.NewGame | LoadMode.NewGameFromScenario | LoadMode.NewScenarioFromGame | LoadMode.NewScenarioFromMap)) == 0) return;

            SetSpeeds();
        }

    }
}
