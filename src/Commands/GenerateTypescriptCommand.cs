using System.IO;
using System.Windows;
using Microsoft.Win32;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;

namespace JsonSchemaGenerator
{
    [Command(PackageIds.GenerateTypeScript)]
    internal class GenerateTypescriptCommand : BaseCommand<GenerateTypescriptCommand>
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

            SaveFileDialog dialog = new()
            {
                DefaultExt = ".ts",
                FileName = "sample.ts",
                InitialDirectory = Path.GetDirectoryName(docView.FilePath)
            };

            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                TypeScriptGenerator generator = new(this, new TypeScriptGeneratorSettings());

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
