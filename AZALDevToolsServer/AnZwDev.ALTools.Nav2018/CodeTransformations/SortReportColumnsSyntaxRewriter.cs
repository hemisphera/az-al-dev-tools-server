﻿/****************************************************************
 *                                                              *
 * Legacy version of the library maintained to support Nav 2018 *
 *                                                              *
 ****************************************************************/
using AnZwDev.ALTools.Nav2018.ALSymbols.Internal;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnZwDev.ALTools.Nav2018.CodeTransformations
{
    public class SortReportColumnsSyntaxRewriter: ALSyntaxRewriter
    {

        protected class ReportElementComparer : IComparer<SyntaxNodeSortInfo<ReportDataItemElementSyntax>>
        {

            public ReportElementComparer()
            {
            }

            public int Compare(SyntaxNodeSortInfo<ReportDataItemElementSyntax> x, SyntaxNodeSortInfo<ReportDataItemElementSyntax> y)
            {
                if ((x.Kind == y.Kind) && (x.Kind == ConvertedSyntaxKind.ReportColumn))
                    return x.Name.CompareTo(y.Name);
                if (x.Kind == ConvertedSyntaxKind.ReportColumn)
                    return -1;
                if (y.Kind == ConvertedSyntaxKind.ReportColumn)
                    return 1;
                return (x.Index - y.Index);
            }
        }

        public SortReportColumnsSyntaxRewriter()
        {
        }

        public override SyntaxNode VisitReportDataItem(ReportDataItemSyntax node)
        {
            if ((this.NodeInSpan(node)) && (!node.ContainsDiagnostics))
            {
                List<SyntaxNodeSortInfo<ReportDataItemElementSyntax>> list =
                SyntaxNodeSortInfo<ReportDataItemElementSyntax>.FromSyntaxList(node.Elements);
                if (list.Count > 1)
                {
                    list.Sort(new ReportElementComparer());
                    List<ReportDataItemElementSyntax> elementsList = new List<ReportDataItemElementSyntax>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        elementsList.Add(list[i].Node);
                    }
                    node = node.WithElements(SyntaxFactory.List<ReportDataItemElementSyntax>(elementsList));
                }
            }
            return base.VisitReportDataItem(node);
        }

    }
}
