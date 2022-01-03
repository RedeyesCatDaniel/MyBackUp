using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
//--------------------------------------------------------------------------------------------------------------------------
//-------------------------------------- GUI Utilities ---------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------


[Serializable]
public class SubsurfaceScatteringProfileManager
{
    public const int MAX_PROFILES = 7;

    public static bool needToUpdateGUI = true;
    public static bool needToUpdateProfiles = false;

    public static SubsurfaceScatteringProfile[] profiles;
    [SerializeField] public SubsurfaceScatteringProfile[] profilesInGUI = new SubsurfaceScatteringProfile[MAX_PROFILES];

    public static SubsurfaceScatteringModel subsurfaceScatteringModel;

    public static SubsurfaceScatteringProfile GetSubsurfaceProfile(int index)
    {
        if (profiles == null || index < 0)
            return null;

        return profiles.GetValue(index) as SubsurfaceScatteringProfile;
    }

    public static int GetSubsurfaceProfileIndex(SubsurfaceScatteringProfile profile)
    {
        if (profiles == null)
            return -1;

        return Array.IndexOf(profiles, profile);
    }

    public static int AddSubsurfaceProfile(SubsurfaceScatteringProfile profile)
    {
        if (profiles == null)
            return -1;

        int index = GetSubsurfaceProfileIndex(profile);
        if (index > 0)
            return index;


        //Add a new profile to m_subsurfaceScatteringModel
        for (int i = 0; i < profiles.Length; i++)
        {
            if (profiles[i] == null)
            {
                profiles[i] = profile;
                needToUpdateGUI = true;
                ApplyProfilesToSSModel();

                return i;
            }
        }

        //Failed on setSSprofile
        Debug.LogError("Subsurface Scattering Render Feature reached maximum profile number: " + MAX_PROFILES);
        return -1;
    }

    public void CheckDuplicateProfile()
    {
        for (int i = 0; i < profiles.Length; i++)
        {
            if (profiles[i] == null)
                continue;

            for (int j = i + 1; j < profiles.Length; j++)
            {
                if (profiles[j] == null)
                    continue;

                if (profiles[i] == profiles[j])
                {
                    profiles[j] = null;
                }

            }
        }
    }

    public void CheckDuplicateProfileInGUI()
    {
        profilesInGUI = profilesInGUI.Distinct().ToArray();
    }

    public void UpdateGUI()
    {
        Init();

        CheckDuplicateProfile();

        if (needToUpdateGUI)
        {
            // List -> GUI

            CheckDuplicateProfileInGUI();

            //Check if GUI need to add profiles
            foreach(var i in profiles)
            {
                if (i == null)
                    continue;

                if(Array.IndexOf(profilesInGUI, i) < 0)
                {
                    for (int j = 0; j < profilesInGUI.Length; j++)
                    {
                        if (profilesInGUI[j] == null)
                        {
                            profilesInGUI[j] = i;

                            break;
                        }

                        if(j == profilesInGUI.Length - 1)
                        {
                            Array.Resize(ref profilesInGUI, profilesInGUI.Length + 1);
                            profilesInGUI[profilesInGUI.Length - 1] = i;

                            break;
                        }
                    }
                }
            }

            //Check if GUI need to delete profiles
            foreach (var i in profilesInGUI)
            {
                if (i == null)
                    continue;

                if (Array.IndexOf(profiles, i) < 0)
                {
                    int index = Array.IndexOf(profilesInGUI, i);
                    profilesInGUI[index] = null;
                }
            }
        }

        needToUpdateGUI = false;

    }

    //GUI -> profiles
    public void UpdateProfileListFromGUI()
    {
        if (needToUpdateProfiles)
        {
            
            //check if profile need to delete profiles

            foreach (var i in profiles)
            {
                if (i == null)
                    continue;

                if (Array.IndexOf(profilesInGUI, i) < 0)
                {
                    int index = Array.IndexOf(profiles, i);
                    profiles[index] = null;
                }
            }

            //check if profile need to add profiles

            foreach (var i in profilesInGUI)
            {
                if (i == null)
                    continue;

                if (Array.IndexOf(profiles, i) < 0)
                {
                    for (int j = 0; j < profiles.Length; j++)
                    {
                        if (profiles[j] == null)
                        {
                            profiles[j] = i;
                            break;
                        }

                        if(j == profiles.Length - 1)
                            Debug.LogWarning("Subsurface Scattering Render Feature reached maximum profile number");
                    }
                }
            }

            ApplyProfilesToSSModel();

        }
        else
        {
            UpdateGUI();
            
        }
        needToUpdateProfiles = false;
    }

    public void Init()
    {
        if (subsurfaceScatteringModel == null)
        {
            subsurfaceScatteringModel = new SubsurfaceScatteringModel();
            SubsurfaceScatteringProfile[] subsurfaceScatteringProfiles = new SubsurfaceScatteringProfile[MAX_PROFILES];
            SubsurfaceScatteringModel.Settings m_Settings = SubsurfaceScatteringModel.Settings.defaultSettings;
            m_Settings.profiles = subsurfaceScatteringProfiles;
            m_Settings.useDisneySSS = true;
            subsurfaceScatteringModel.settings = m_Settings;
        }

        subsurfaceScatteringModel.settings.OnValidate();

    }

    public static void ApplyProfilesToSSModel()
    {
        if (subsurfaceScatteringModel != null)
        {
            for (int i = 0; i < MAX_PROFILES; i++)
            {
                if (profiles[i] != null)
                {
                    subsurfaceScatteringModel.settings.profiles[i] = profiles[i];
                    profiles[i].hash = (uint)i;
                }
                else
                {
                    subsurfaceScatteringModel.settings.profiles[i] = null;
                }

            }
            subsurfaceScatteringModel.settings.OnValidate();

        }
    }
}