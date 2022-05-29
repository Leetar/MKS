﻿using System;
using System.Collections.Generic;
using System.Linq;
using KolonyTools;
using UnityEngine;



namespace KolonyTools
{
    public class KolonizationManager : MonoBehaviour
    {
        // Static singleton instance
        private static KolonizationManager instance;

        // Static singleton property
        public static KolonizationManager Instance
        {
            get { return instance ?? (instance = new GameObject("KolonizationManager").AddComponent<KolonizationManager>()); }
        }

        //Backing variables
        private List<KolonizationEntry> _KolonizationInfo;

        public void ResetCache()
        {
            _KolonizationInfo = null;
        }

        public List<KolonizationEntry> KolonizationInfo
        {
            get
            {
                if (_KolonizationInfo == null)
                {
                    _KolonizationInfo = new List<KolonizationEntry>();
                    _KolonizationInfo.AddRange(KolonizationScenario.Instance.settings.GetStatusInfo());
                }
                return _KolonizationInfo;
            }
        }

        public bool DoesLogEntryExist(string vesselId, int body)
        {
            //Does a node exist?
            return KolonizationInfo.Any(n => n.VesselId == vesselId
                && n.BodyIndex == body);
        }

        public void RemoveLogEntry(string vesselId, int body)
        {
            if (!DoesLogEntryExist(vesselId, body))
                return;
            var logEntry = KolonizationInfo.First(n => n.VesselId == vesselId
                && n.BodyIndex == body);
            KolonizationInfo.Remove(logEntry);
            //For saving to our scenario data
            KolonizationScenario.Instance.settings.DeleteStatusNode(logEntry);
        }

        public List<KolonizationEntry> FetchEntriesForPlanet(int body)
        {
            var logEntries = KolonizationInfo.Where(n => n.BodyIndex == body).ToList();
            return logEntries;
        }

        public KolonizationEntry FetchLogEntry(string vesselId, int body)
        {
            if (!DoesLogEntryExist(vesselId, body))
            {
                var k = new KolonizationEntry();
                k.VesselId = vesselId;
                k.BodyIndex = body;
                k.LastUpdate = Planetarium.GetUniversalTime();
                k.KolonyDate = Planetarium.GetUniversalTime();
                k.GeologyResearch = 0d;
                k.BotanyResearch = 0d;
                k.KolonizationResearch = 0d;
                k.Science = 0d;
                k.Rep = 0d;
                k.Funds = 0d;
                TrackLogEntry(k);
            }

            var logEntry = KolonizationInfo.FirstOrDefault(n => n.VesselId == vesselId
                && n.BodyIndex == body);
            return logEntry;
        }

        public void TrackLogEntry(KolonizationEntry logEntry)
        {
            KolonizationEntry newEntry =
                KolonizationInfo.FirstOrDefault(n => n.VesselId == logEntry.VesselId
                && n.BodyIndex == logEntry.BodyIndex);
            if (newEntry == null)
            {
                newEntry = new KolonizationEntry();
                newEntry.VesselId = logEntry.VesselId;
                newEntry.BodyIndex = logEntry.BodyIndex;
                KolonizationInfo.Add(newEntry);
            }
            newEntry.LastUpdate = logEntry.LastUpdate;
            newEntry.KolonyDate = logEntry.KolonyDate;
            newEntry.GeologyResearch = logEntry.GeologyResearch;
            newEntry.BotanyResearch = logEntry.BotanyResearch;
            newEntry.KolonizationResearch = logEntry.KolonizationResearch;
            newEntry.Science = logEntry.Science;
            newEntry.Funds = logEntry.Funds;
            newEntry.Rep = logEntry.Rep; 
            newEntry.RepBoosters = logEntry.RepBoosters;
            newEntry.ScienceBoosters = logEntry.ScienceBoosters;
            newEntry.FundsBoosters = logEntry.FundsBoosters;
            KolonizationScenario.Instance.settings.SaveLogEntryNode(newEntry);
        }

        public static float GetGeologyResearchBonus(int bodyIndex)
        {
            var researchTotal = Instance.KolonizationInfo.Where(k => k.BodyIndex == bodyIndex).Sum(k => k.GeologyResearch);
            return ConvertResearchToBonus(researchTotal);
        }
        public static float GetBotanyResearchBonus(int bodyIndex)
        {
            var researchTotal = Instance.KolonizationInfo.Where(k => k.BodyIndex == bodyIndex).Sum(k => k.BotanyResearch);
            return ConvertResearchToBonus(researchTotal);
        }
        public static float GetKolonizationResearchBonus(int bodyIndex)
        {
            var researchTotal = Instance.KolonizationInfo.Where(k => k.BodyIndex == bodyIndex).Sum(k => k.KolonizationResearch);
            return ConvertResearchToBonus(researchTotal);
        }
        public static int GetGeologyResearchBoosters(int bodyIndex)
        {
            return Instance.KolonizationInfo.Where(k => k.BodyIndex == bodyIndex).Sum(k => k.FundsBoosters);
        }
        public static int GetBotanyResearchBoosters(int bodyIndex)
        {
            return Instance.KolonizationInfo.Where(k => k.BodyIndex == bodyIndex).Sum(k => k.ScienceBoosters);
        }
        public static int GetKolonizationResearchBoosters(int bodyIndex)
        {
            return Instance.KolonizationInfo.Where(k => k.BodyIndex == bodyIndex).Sum(k => k.RepBoosters);
        }
        private static float ConvertResearchToBonus(double researchTotal)
        {
            var progress = Math.Sqrt(researchTotal) / KolonizationSetup.Instance.Config.EfficiencyMultiplier;
            var candidateBonus = KolonizationSetup.Instance.Config.StartingBaseBonus + (float)progress;
            return Math.Max(KolonizationSetup.Instance.Config.MinBaseBonus, candidateBonus);

        }


    }
}

