using System.IO;
using System.Windows;
using Microsoft.Win32;
using NJsonSchema;

namespace JsonSchemaGenerator
{
    [Command(PackageIds.GenerateSample)]
    internal sealed class GenerateSampleCommand : BaseCommand<GenerateSampleCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            SolutionItem file = await VS.Solutions.GetActiveItemAsync();

            JsonSchema schema = await JsonSchema.FromFileAsync(file.FullPath, Package.DisposalToken);

            if (string.IsNullOrEmpty(schema.SchemaVersion))
            {
                await VS.MessageBox.ShowErrorAsync("The selected JSON file is not a valid schema.");
                return;
            }

            SaveFileDialog dialog = new()
            {
                DefaultExt = ".json",
                FileName = "sample.json",
                InitialDirectory = Path.GetDirectoryName(file.FullPath)
            };

            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                File.WriteAllText(dialog.FileName, schema.ToSampleJson().ToString());

                if (file.FindParent(SolutionItemType.Project) is Project project)
                {
                    await project.AddExistingFilesAsync(dialog.FileName);
                }

                await VS.Documents.OpenAsync(dialog.FileName);
                JsonSchemaGeneratorPackage.RatingPrompt.RegisterSuccessfulUsage();
            }
        }
    }
}
