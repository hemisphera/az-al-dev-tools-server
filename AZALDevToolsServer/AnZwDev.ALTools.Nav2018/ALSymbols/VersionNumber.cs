﻿/****************************************************************
 *                                                              *
 * Legacy version of the library maintained to support Nav 2018 *
 *                                                              *
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnZwDev.ALTools.Nav2018.ALSymbols
{
    public class VersionNumber
    {

        public string Version { get; set; }
        public int[] Parts { get; private set; }

        public VersionNumber(params int[] versionNumberParts)
        {
            this.Parts = versionNumberParts;
            string versionText = "";
            for (int i=0; i< this.Parts.Length;i++)
            {
                if (i > 0)
                    versionText = versionText + ".";
                versionText = versionText + this.Parts[i].ToString();
            }
            this.Version = versionText;
        }

        public VersionNumber() : this(0,0,0,0)
        {
        }

        public VersionNumber(string versionText)
        {
            this.Version = versionText;
            this.ParseVersion(); 
        }

        private void ParseVersion()
        {
            int partValue;
            char[] split = { '.' };
            string[] textParts = this.Version.Split(split);
            this.Parts = new int[textParts.Length];
            for (int i=0; i<textParts.Length; i++)
            {
                if (Int32.TryParse(textParts[i], out partValue))
                    this.Parts[i] = partValue;
                else
                    this.Parts[i] = 0;
            }
        }

        public int Compare(VersionNumber value)
        {
            int maxCount = Math.Min(this.Parts.Length, value.Parts.Length);
            for (int i=0; i<maxCount; i++)
            {
                if (this.Parts[i] > value.Parts[i])
                    return 1;
                if (this.Parts[i] < value.Parts[i])
                    return -1;
            }
            if (this.Parts.Length > value.Parts.Length)
                return 1;
            if (this.Parts.Length < value.Parts.Length)
                return -1;
            return 0;
        }

        public bool Equal(VersionNumber value)
        {
            return (this.Compare(value) == 0);
        }

        public bool Greater(VersionNumber value)
        {
            return (this.Compare(value) > 0);
        }

        public bool Lower(VersionNumber value)
        {
            return (this.Compare(value) < 0);
        }

        public bool GreaterOrEqual(VersionNumber value)
        {
            return (this.Compare(value) >= 0);
        }

        public bool LowerOrEqual(VersionNumber value)
        {
            return (this.Compare(value) <= 0);
        }

        public bool Different(VersionNumber value)
        {
            return (this.Compare(value) != 0);
        }


    }
}
