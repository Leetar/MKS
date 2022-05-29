﻿namespace KolonyTools
{
    public class KolonizationEntry
    {
        public int BodyIndex { get; set; }
        public string VesselId { get; set; }
        public double LastUpdate { get; set; }
        public double KolonyDate { get; set; }
        public double GeologyResearch { get; set; }
        public double BotanyResearch { get; set; }
        public double KolonizationResearch { get; set; }
        public double Science { get; set; }
        public double Rep { get; set; }
        public double Funds { get; set; }
        public int RepBoosters { get; set; }
        public int FundsBoosters { get; set; }
        public int ScienceBoosters { get; set; }
    }
}