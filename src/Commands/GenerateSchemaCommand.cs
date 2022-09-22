using System.IO;
using System.Windows;
using Microsoft.Win32;
using NJsonSchema;

namespace JsonSchemaGenerator
{
    [Command(PackageIds.GenerateSchema)]
    internal sealed class GenerateSchemaCommand : BaseCommand<GenerateSchemaCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();

            JsonSchema schema = JsonSchema.FromSampleJson(File.ReadAllText(docView.FilePath));
            string schemaFileName = Path.GetFileNameWithoutExtension(docView.FilePath) + ".schema.json";

            SaveFileDialog dialog = new()
            {
                DefaultExt = ".json",
                FileName = schemaFileName,
                InitialDirectory = Path.GetDirectoryName(docView.FilePath)
            };

            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                File.WriteAllText(dialog.FileName, schema.ToJson());

                if (await VS.Solutions.GetActiveProjectAsync() is Project project)
                {
                    await project.AddExistingFilesAsync(dialog.FileName);
                }

                // Apply schema to document
                string relativeSchemaPath = PackageUtilities.MakeRelative(docView.FilePath, dialog.FileName);
                IDataObject oldClipboard = Clipboard.GetDataObject();
                Clipboard.SetText(relativeSchemaPath);
                System.Windows.Forms.SendKeys.SendWait($"{{F4}}^v~");
                Clipboard.SetDataObject(oldClipboard, true);

                await VS.Documents.OpenAsync(dialog.FileName);
                JsonSchemaGeneratorPackage.RatingPrompt.RegisterSuccessfulUsage();
            }
        }
    }
}
