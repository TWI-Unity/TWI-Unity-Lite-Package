namespace TWILite.Utilities.MenuItemAutogen
{
    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Linq;
    using System.Text;
    using TWILite.Extensions;

    [Serializable]
    public sealed class MenuItemAutogenItem_CreateFile : MenuItemAutogenItem
    {
        public enum DataSources
        {
            RawString,
            CopyFile,
            Base64String,
        }

        public const string NAMEOF_FILE_NAME = nameof(FileName);
        public const string NAMEOF_DATA_SOURCE = nameof(DataSource);
        public const string NAMEOF_DATA = nameof(Data);

        public override bool UseMethodLambdaOperator => true;

        public string FileName;
        public DataSources DataSource;
        public string Data;

        public MenuItemAutogenItem_CreateFile() { }
        public MenuItemAutogenItem_CreateFile(string menuPath, string methodName, string fileName = null, DataSources dataFormat = DataSources.RawString, string data = null) : base(menuPath, methodName)
        {
            FileName = fileName;
            DataSource = dataFormat;
            Data = data;
        }

        public override void CompileMethod(IndentedTextWriter writer) => writer.WriteLine($"Create(GetSelectedPath(\"New {FileName}\"), Convert.FromBase64String(\"{GetDataAsBase64String()}\"), CreateOptions.UniqueID, LoadOptions.Rename);");

        public string GetDataAsBase64String()
        {
            switch (DataSource)
            {
                case DataSources.Base64String: return string.IsNullOrWhiteSpace(Data) || !Data.IsBase64String() ? string.Empty : Data;
                case DataSources.CopyFile: return File.Exists(Data) ? Convert.ToBase64String(File.ReadAllBytes(Data)) : string.Empty;
                case DataSources.RawString: return Convert.ToBase64String(Encoding.Default.GetBytes(Data ?? string.Empty));
                default: throw new NotImplementedException();
            }
        }

        public override string Validate()
        {
            if (base.Validate() is string warning) return warning;
            else if (string.IsNullOrWhiteSpace(FileName)) return "The property 'FileName' was not specified.";
            else if (string.IsNullOrWhiteSpace(FileName.Replace(".", string.Empty))) return "The property 'FileName' has no file name or extension specified.";
            else if (FileName.Any(Path.GetInvalidFileNameChars().Contains)) return "The property 'FileName' contains invalid characters.";
            else if (Path.GetFileName(Path.GetFullPath(FileName)) != FileName) return "The property 'FileName' is invalid.";

            switch (DataSource)
            {
                case DataSources.Base64String:
                    if (Data.IsBase64String()) return null;
                    else return "The property 'Data' is an invalid Base64 string.";
                case DataSources.CopyFile:
                    if (File.Exists(FileUtilities.GetFullPath(Data))) return null;
                    else return "The property 'Data' specifies a file that could not be found.";
                case DataSources.RawString:
                    return null;
                default: 
                    throw new NotImplementedException();
            }
        }
    }
}