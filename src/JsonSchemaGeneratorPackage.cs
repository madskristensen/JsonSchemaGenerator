global using System;
global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Threading;

namespace JsonSchemaGenerator
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.JsonSchemaGeneratorString)]
    [ProvideUIContextRule(PackageGuids.JsonFileString,
        name: "JSON file",
        expression: "json",
        termNames: new[] { "json" },
        termValues: new[] { "HierSingleSelectionName:.json$" })]
    public sealed class JsonSchemaGeneratorPackage : ToolkitPackage
    {
        public static RatingPrompt RatingPrompt { get; private set;  } 
        
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            RatingPrompt = new("MadsKristensen.JSONSchemaGenerator2", Vsix.Name, await General.GetLiveInstanceAsync(), 2);

            await this.RegisterCommandsAsync();
        }
    }
}