using System.IO;
using System.Windows;
using JsonSchemaGenerator.Commands;
using Microsoft.Win32;
using NJsonSchema;
using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;

namespace JsonSchemaGenerator
{
    [Command(PackageIds.GenerateCSharp)]
    internal class GenerateCSharpCommand : BaseCommand<GenerateCSharpCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
            JsonSchema schema = await JsonSchema.FromFileAsync(docView.FilePath);

            if (string.IsNullOrEmpty(schema?.SchemaVersion))
            {
                await VS.MessageBox.ShowErrorAsync("The selected JSON file is not a valid schema.");
                return;
            }

            CSharpSettingsWindow window = new();

            if (await window.ShowDialogAsync(WindowStartupLocation.CenterOwner) != true)
            {
                return;
            }

            SaveFileDialog dialog = new()
            {
                DefaultExt = ".cs",
                FileName = "sample.cs",
                InitialDirectory = Path.GetDirectoryName(docView.FilePath)
            };

            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                CSharpGenerator generator = new(this, window.Settings);
                
                File.WriteAllText(dialog.FileName, generator.GenerateFile(schema, Path.GetFileNameWithoutExtension(dialog.FileName)));

                if (await VS.Solutions.GetActiveProjectAsync() is Project project)
                {
                    await project.AddExistingFilesAsync(dialog.FileName);
                }

                await VS.Documents.OpenAsync(dialog.FileName);
                JsonSchemaGeneratorPackage.RatingPrompt.RegisterSuccessfulUsage();
            }
        }
    }
}
