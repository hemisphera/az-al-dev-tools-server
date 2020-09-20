﻿/****************************************************************
 *                                                              *
 * Legacy version of the library maintained to support Nav 2018 *
 *                                                              *
 ****************************************************************/
using AnZwDev.ALTools.Nav2018.ALSymbols;
using AnZwDev.ALTools.Nav2018.ALSymbols.Internal;
using AnZwDev.ALTools.Nav2018.Extensions;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AnZwDev.ALTools.Nav2018.CodeTransformations
{
    public class SortProceduresSyntaxRewriter: ALSyntaxRewriter
    {

        #region Member sort info

        protected class MethodSortInfo<T> where T: SyntaxNode
        {
            public string Name { get; set; }
            public ALSymbolKind Kind { get; set; }
            public int Index { get; set; }
            public T Node { get; set; }

            public MethodSortInfo(T node, int index)
            {
                this.Node = node;
                this.Index = index;
                this.Name = node.GetNameStringValue();
                this.Kind = GetKind(node);
            }

            private ALSymbolKind GetKind(SyntaxNode node)
            {
                ConvertedSyntaxKind kind = node.Kind.ConvertToLocalType();
                switch (kind)
                {
                    case ConvertedSyntaxKind.TriggerDeclaration:
                        return ALSymbolKind.TriggerDeclaration;
                    case ConvertedSyntaxKind.MethodDeclaration:
                        MethodDeclarationSyntax methodNode = node as MethodDeclarationSyntax;
                        foreach (MemberAttributeSyntax att in methodNode.Attributes)
                        {
                            ALSymbolKind alKind = ALSyntaxHelper.MemberAttributeToMethodKind(att.GetNameStringValue());
                            if (alKind != ALSymbolKind.Undefined)
                                return alKind;
                        }
                        return ALSymbolKind.MethodDeclaration;
                }
                return ALSymbolKind.Undefined;
            }

            public static List<MethodSortInfo<T>> FromSyntaxList(SyntaxList<T> nodeList)
            {
                List<MethodSortInfo<T>> list = new List<MethodSortInfo<T>>();
                for (int i=0; i<nodeList.Count; i++)
                {
                    list.Add(new MethodSortInfo<T>(nodeList[i], i));
                }
                return list;
            }

            public static SyntaxList<T> ToSyntaxList(List<MethodSortInfo<T>> sortInfoList)
            {
                List<T> nodeList = new List<T>();
                for (int i=0; i<sortInfoList.Count; i++)
                {
                    nodeList.Add(sortInfoList[i].Node);
                }
                return SyntaxFactory.List<T>(nodeList);
            }

        }

        #endregion

        #region Method info comparer

        protected class MethodSortInfoComparer<T> : IComparer<MethodSortInfo<T>> where T: SyntaxNode
        {
            protected static Dictionary<ALSymbolKind, int> _typePriority;
            protected static int UndefinedPriority = -1;

            public MethodSortInfoComparer()
            {
                InitTypePriority();
            }

            private void InitTypePriority()
            {
                if (_typePriority == null)
                {
                    ALSymbolKind[] types = {
                        ALSymbolKind.TestDeclaration,
                        ALSymbolKind.ConfirmHandlerDeclaration,
                        ALSymbolKind.FilterPageHandlerDeclaration,
                        ALSymbolKind.HyperlinkHandlerDeclaration,
                        ALSymbolKind.MessageHandlerDeclaration,
                        ALSymbolKind.ModalPageHandlerDeclaration,
                        ALSymbolKind.PageHandlerDeclaration,
                        //ALSymbolKind.RecallNotificationHandler, // is missing
                        ALSymbolKind.ReportHandlerDeclaration,
                        ALSymbolKind.RequestPageHandlerDeclaration,
                        ALSymbolKind.SendNotificationHandlerDeclaration,
                        ALSymbolKind.SessionSettingsHandlerDeclaration,
                        ALSymbolKind.StrMenuHandlerDeclaration,
                        ALSymbolKind.MethodDeclaration,
                        ALSymbolKind.LocalMethodDeclaration,
                        ALSymbolKind.EventSubscriberDeclaration,
                        ALSymbolKind.EventDeclaration,
                        ALSymbolKind.BusinessEventDeclaration,
                        ALSymbolKind.IntegrationEventDeclaration
                    };
                    _typePriority = new Dictionary<ALSymbolKind, int>();
                    for (int i = 0; i < types.Length; i++)
                    {
                        _typePriority.Add(types[i], i);
                    }
                }
            }

            protected int GetTypePriority(ALSymbolKind kind)
            {
                if (_typePriority.ContainsKey(kind))
                    return _typePriority[kind];
                return UndefinedPriority;
            }

            public int Compare(MethodSortInfo<T> x, MethodSortInfo<T> y)
            {
                //check type
                int xTypePriority = this.GetTypePriority(x.Kind);
                int yTypePriority = this.GetTypePriority(y.Kind);
                if (xTypePriority != yTypePriority)
                    return xTypePriority - yTypePriority;
                //for known types check name
                if (yTypePriority != UndefinedPriority)
                {
                    int val = x.Name.CompareTo(y.Name);
                    if (val != 0)
                        return val;
                }
                //check old index
                return x.Index - y.Index;
            }
        }


        #endregion

        public SortProceduresSyntaxRewriter()
        {
        }

        #region Visit objects

        public override SyntaxNode VisitTable(TableSyntax node)
        {
            if ((node.Members != null) && (node.Members.Count > 0))
                node = node.WithMembers(this.Sort(node.Members));
            return base.VisitTable(node);
        }

        public override SyntaxNode VisitPage(PageSyntax node)
        {
            if ((node.Members != null) && (node.Members.Count > 0))
                node = node.WithMembers(this.Sort(node.Members));
            return base.VisitPage(node);
        }

        public override SyntaxNode VisitReport(ReportSyntax node)
        {
            if ((node.Members != null) && (node.Members.Count > 0))
                node = node.WithMembers(this.Sort(node.Members));
            return base.VisitReport(node);
        }

        public override SyntaxNode VisitXmlPort(XmlPortSyntax node)
        {
            if ((node.Members != null) && (node.Members.Count > 0))
                node = node.WithMembers(this.Sort(node.Members));
            return base.VisitXmlPort(node);
        }

        public override SyntaxNode VisitCodeunit(CodeunitSyntax node)
        {
            if ((node.Members != null) && (node.Members.Count > 0))
                node = node.WithMembers(this.Sort(node.Members));
            return base.VisitCodeunit(node);
        }

        public override SyntaxNode VisitQuery(QuerySyntax node)
        {
            if ((node.Members != null) && (node.Members.Count > 0))
                node = node.WithMembers(this.Sort(node.Members));
            return base.VisitQuery(node);
        }

        public override SyntaxNode VisitTableExtension(TableExtensionSyntax node)
        {
            if ((node.Members != null) && (node.Members.Count > 0))
                node = node.WithMembers(this.Sort(node.Members));
            return base.VisitTableExtension(node);
        }

        public override SyntaxNode VisitPageExtension(PageExtensionSyntax node)
        {
            if ((node.Members != null) && (node.Members.Count > 0))
                node = node.WithMembers(this.Sort(node.Members));
            return base.VisitPageExtension(node);
        }

        public override SyntaxNode VisitPageCustomization(PageCustomizationSyntax node)
        {
            if ((node.Members != null) && (node.Members.Count > 0))
                node = node.WithMembers(this.Sort(node.Members));
            return base.VisitPageCustomization(node);
        }

        public override SyntaxNode VisitProfile(ProfileSyntax node)
        {
            if ((node.Members != null) && (node.Members.Count > 0))
                node = node.WithMembers(this.Sort(node.Members));
            return base.VisitProfile(node);
        }

        #endregion

        #region Visit nodes with triggers

        public override SyntaxNode VisitField(FieldSyntax node)
        {
            if ((node.Triggers != null) && (node.Triggers.Count > 0))
                node = node.WithTriggers(this.Sort(node.Triggers));
            return base.VisitField(node);
        }

        public override SyntaxNode VisitPageField(PageFieldSyntax node)
        {
            if ((node.Triggers != null) && (node.Triggers.Count > 0))
                node = node.WithTriggers(this.Sort(node.Triggers));
            return base.VisitPageField(node);
        }

        public override SyntaxNode VisitPageAction(PageActionSyntax node)
        {
            if ((node.Triggers != null) && (node.Triggers.Count > 0))
                node = node.WithTriggers(this.Sort(node.Triggers));
            return base.VisitPageAction(node);
        }

        public override SyntaxNode VisitXmlPortFieldAttribute(XmlPortFieldAttributeSyntax node)
        {
            if ((node.Triggers != null) && (node.Triggers.Count > 0))
                node = node.WithTriggers(this.Sort(node.Triggers));
            return base.VisitXmlPortFieldAttribute(node);
        }

        public override SyntaxNode VisitXmlPortFieldElement(XmlPortFieldElementSyntax node)
        {
            if ((node.Triggers != null) && (node.Triggers.Count > 0))
                node = node.WithTriggers(this.Sort(node.Triggers));
            return base.VisitXmlPortFieldElement(node);
        }

        public override SyntaxNode VisitXmlPortTableElement(XmlPortTableElementSyntax node)
        {
            if ((node.Triggers != null) && (node.Triggers.Count > 0))
                node = node.WithTriggers(this.Sort(node.Triggers));
            return base.VisitXmlPortTableElement(node);
        }

        public override SyntaxNode VisitXmlPortTextAttribute(XmlPortTextAttributeSyntax node)
        {
            if ((node.Triggers != null) && (node.Triggers.Count > 0))
                node = node.WithTriggers(this.Sort(node.Triggers));
            return base.VisitXmlPortTextAttribute(node);
        }

        public override SyntaxNode VisitXmlPortTextElement(XmlPortTextElementSyntax node)
        {
            if ((node.Triggers != null) && (node.Triggers.Count > 0))
                node = node.WithTriggers(this.Sort(node.Triggers));
            return base.VisitXmlPortTextElement(node);
        }

        #endregion

        private SyntaxList<T> Sort<T>(SyntaxList<T> members) where T: SyntaxNode
        {
            List<MethodSortInfo<T>> list = MethodSortInfo<T>.FromSyntaxList(members);
            list.Sort(new MethodSortInfoComparer<T>());
            return MethodSortInfo<T>.ToSyntaxList(list);
        }

    }
}
