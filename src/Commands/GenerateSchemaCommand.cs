using System.IO;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI.OleComponentSupport;
using Microsoft.Win32;
using NJsonSchema;

namespace JsonSchemaGenerator
{
    [Command(PackageIds.GenerateSchema)]
    internal sealed class GenerateSchemaCommand : BaseCommand<GenerateSchemaCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            SolutionItem file = await VS.Solutions.GetActiveItemAsync();

            var schema = JsonSchema.FromSampleJson(File.ReadAllText(file.FullPath));
            var schemaFileName = Path.GetFileNameWithoutExtension(file.FullPath) + ".schema.json";

            SaveFileDialog dialog = new()
            {
                DefaultExt = ".json",
                FileName = schemaFileName,
                InitialDirectory = Path.GetDirectoryName(file.FullPath)
            };

            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                File.WriteAllText(dialog.FileName, schema.ToJson());

                if (file.FindParent(SolutionItemType.Project) is Project project)
                {
                    await project.AddExistingFilesAsync(dialog.FileName);
                }
                
                await VS.Documents.OpenAsync(dialog.FileName);
            }
        }
    }
}
