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
            DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();

            JsonSchema schema = await JsonSchema.FromFileAsync(docView.FilePath, Package.DisposalToken);

            if (string.IsNullOrEmpty(schema.SchemaVersion))
            {
                await VS.MessageBox.ShowErrorAsync("The selected JSON file is not a valid schema.");
                return;
            }

            SaveFileDialog dialog = new()
            {
                DefaultExt = ".json",
                FileName = "sample.json",
                InitialDirectory = Path.GetDirectoryName(docView.FilePath)
            };

            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                File.WriteAllText(dialog.FileName, schema.ToSampleJson().ToString());

                if (await VS.Solutions.GetActiveProjectAsync() is Project project)
                {
                    await project.AddExistingFilesAsync(dialog.FileName);
                }

                await VS.Documents.OpenAsync(dialog.FileName);

                // Apply schema to document
                string relativeSchemaPath = PackageUtilities.MakeRelative(dialog.FileName, docView.FilePath);
                IDataObject oldClipboard = Clipboard.GetDataObject();
                Clipboard.SetText(relativeSchemaPath);
                System.Windows.Forms.SendKeys.SendWait($"{{F4}}^v~");
                Clipboard.SetDataObject(oldClipboard, true);

                JsonSchemaGeneratorPackage.RatingPrompt.RegisterSuccessfulUsage();
            }
        }
    }
}
