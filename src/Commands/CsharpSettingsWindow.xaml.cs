using System.Collections.Generic;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace JsonSchemaGenerator.Commands
{
    /// <summary>
    /// Interaction logic for CsharpSettingsWindow.xaml
    /// </summary>
    public partial class CSharpSettingsWindow : DialogWindow
    {
        public CSharpSettingsWindow()
        {
            InitializeComponent();

            cbSchemaType.ItemsSource = EnumToDictionary<SchemaType>().Keys;
            cbLibrary.ItemsSource = EnumToDictionary<CSharpJsonLibrary>().Keys;
            cbStyle.ItemsSource = EnumToDictionary<CSharpClassStyle>().Keys;
        }

        public CSharpGeneratorSettings Settings { get; } = new();

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            Settings.GenerateDefaultValues = false;
            Settings.SchemaType = (SchemaType)cbSchemaType.SelectedValue;
            Settings.JsonLibrary = (CSharpJsonLibrary)cbLibrary.SelectedValue;
            Settings.ClassStyle = (CSharpClassStyle)cbStyle.SelectedValue;
            DialogResult = true;
            Close();
        }

        public static Dictionary<T, string> EnumToDictionary<T>()
        where T : struct
        {
            if (typeof(T).BaseType != typeof(Enum))
            {
                throw new ArgumentException("T must be of type System.Enum");
            }

            Dictionary<T, string> enumDL = new();
            
            foreach (T val in Enum.GetValues(typeof(T)))
            {
                enumDL.Add(val, val.ToString());
            }

            return enumDL;
        }
    }
}