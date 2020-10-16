﻿/****************************************************************
 *                                                              *
 * Legacy version of the library maintained to support Nav 2018 *
 *                                                              *
 ****************************************************************/
using AnZwDev.ALTools.Nav2018;
using AnZwDev.ALTools.Nav2018.ALSymbols;
using AnZwDev.VSCodeLangServer.Protocol.MessageProtocol;
using AnZwDev.ALTools.Server.Nav2018.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnZwDev.ALTools.Server.Nav2018.Handlers
{
    public class ProjectSymbolsRequestHandler : BaseALRequestHandler<ProjectSymbolsRequest, ProjectSymbolsResponse>
    {

        public ProjectSymbolsRequestHandler(ALDevToolsServer server) : base(server, "al/projectsymbols")
        {
        }

        protected override async Task<ProjectSymbolsResponse> HandleMessage(ProjectSymbolsRequest parameters, RequestContext<ProjectSymbolsResponse> context)
        {
            ProjectSymbolsResponse response = new ProjectSymbolsResponse();
            try
            {
                //collect project symbols
                ALProjectSymbolsLibrary library = new ALProjectSymbolsLibrary(this.Server.AppPackagesCache,
                    parameters.includeDependencies, parameters.projectPath, parameters.packagesFolder);
                if (library != null)
                {
                    library.WorkspaceFolders = parameters.workspaceFolders;
                    library.Load(false);
                    response.libraryId = this.Server.SymbolsLibraries.AddLibrary(library);
                    response.root = library.GetObjectsTree();
                }
            }
            catch (Exception e)
            {
                response.root = new ALSymbolInformation();
                response.root.fullName = e.Message;
            }

            return response;
        }

    }
}
