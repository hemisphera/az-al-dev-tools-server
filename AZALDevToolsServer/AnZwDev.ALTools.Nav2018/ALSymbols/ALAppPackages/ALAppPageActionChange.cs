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

namespace AnZwDev.ALTools.Nav2018.ALSymbols.ALAppPackages
{
    public class ALAppPageActionChange : ALAppElementWithName
    {

        public ALAppElementsCollection<ALAppPageAction> Actions { get; set; }

        public ALAppPageActionChange()
        {
        }

        protected override ALSymbolKind GetALSymbolKind()
        {
            return ALSymbolKind.ActionModifyChange;
        }

        protected override void AddChildALSymbols(ALSymbolInformation symbol)
        {
            this.Actions?.AddToALSymbol(symbol);
            base.AddChildALSymbols(symbol);
        }

    }
}
