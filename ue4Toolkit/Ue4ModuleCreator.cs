using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using ue4Toolkit.Properties;

namespace ue4Toolkit
{
    class Ue4ModuleCreator
    {
        private readonly string parentPath;
        private readonly string parentName;
        private readonly string moduleName;
        private readonly Ue4ProjectController projectController;
        private readonly Ue4EngineUtilities engineUtilities;
        private string parentSourceFolderPath;
        private string moduleFolderPath;

        public Ue4ModuleCreator(Ue4ProjectController projectController, string moduleName, string selectedParent, Ue4EngineUtilities engineUtilities)
        {
            this.projectController = projectController;
            parentPath = projectController.GetPathForModuleLocation(selectedParent);
            parentName = selectedParent;
            this.moduleName = moduleName;
            this.engineUtilities = engineUtilities;
            FindSourceFolder();
        }

        private void FindSourceFolder()
        {
            string sourceFolder = Path.Combine(parentPath, "Source");
            parentSourceFolderPath = Directory.Exists(sourceFolder) ? sourceFolder : "";
        }

        public void CreateModule()
        {
            CreateModuleFolderStructure();
            CreateBuildCs();
            CreateModuleHeader();
            CreateMdouleSource();
            RegisterToParent();
            GenerateProjectFiles();
            //ToDo: Create Success Popup
            Application.Exit();
        }

        private void CreateModuleFolderStructure()
        {
            moduleFolderPath = Path.Combine(parentSourceFolderPath, moduleName);
            Directory.CreateDirectory(moduleFolderPath);

            string modulePrivateFolderPath = Path.Combine(moduleFolderPath, "Private");
            Directory.CreateDirectory(modulePrivateFolderPath);

            string modulePublicFolderPath = Path.Combine(moduleFolderPath, "Public");
            Directory.CreateDirectory(modulePublicFolderPath);
        }
        
        private void CreateBuildCs()
        {
            string buildCsPath = Path.Combine(moduleFolderPath, moduleName + ".Build.cs");

            string buildCsContent = ReplacePlaceholdersWithModuleData(Resources.moduleCreator_buildCs);
            File.WriteAllText(buildCsPath, buildCsContent);
        }

        private string ReplacePlaceholdersWithModuleData(string contentString)
        {
            contentString = contentString.Replace("%COPYRIGHT%", Resources.moduleCreator_copyright);
            contentString = contentString.Replace("%MODULENAME%", moduleName);
            return contentString;
        }

        private void CreateModuleHeader()
        {
            string headerPath = Path.Combine(moduleFolderPath, "Public", moduleName + ".h" );
            string headerContent = ReplacePlaceholdersWithModuleData(Resources.moduleCreator_moduleHeader);
            File.WriteAllText(headerPath, headerContent);
        }

        private void CreateMdouleSource()
        {
            string sourcePath = Path.Combine(moduleFolderPath, "Private", moduleName + ".cpp");
            string sourceContent = ReplacePlaceholdersWithModuleData(Resources.moduleCreator_moduleSource);
            File.WriteAllText(sourcePath, sourceContent);
        }

        private void RegisterToParent()
        {
            if (projectController.IsMainGameModuleSelected(parentName))
            {
                RegisterToMainModule();
            }
            else
            {
                RegisterToPlugin();
            }
        }

        private void RegisterToPlugin()
        {
            string uPluginPath = Path.Combine(parentPath, parentName + ".uplugin");
            string uPluginContent = File.ReadAllText(uPluginPath);
            dynamic uPluginJson = JObject.Parse(uPluginContent);

            JArray modulesArray = uPluginJson.Modules;
            string moduleJsonObject = ReplacePlaceholdersWithModuleData(Resources.moduleCreator_pluginJson);
            modulesArray.Add(JObject.Parse(moduleJsonObject));
            uPluginJson.Modules = modulesArray;

            // ReSharper disable once UseObjectOrCollectionInitializer
            FileInfo fileInfo = new FileInfo(uPluginPath);
            fileInfo.IsReadOnly = false;

            File.WriteAllText(uPluginPath, uPluginJson.ToString());
        }

        private void RegisterToMainModule()
        {
            string mainModulePath = Path.Combine(parentSourceFolderPath, parentName, parentName + ".Build.cs");
            string mainModuleBuildCsContent = File.ReadAllText(mainModulePath);

            string publicDependencyFunctionName = "PublicDependencyModuleNames";
            int publicDependencyStartIndex = mainModuleBuildCsContent.IndexOf(publicDependencyFunctionName, StringComparison.Ordinal);
            int moduleNameInsertIndex = mainModuleBuildCsContent.IndexOf("});", publicDependencyStartIndex, StringComparison.Ordinal);

            //Might destroy the formating of Build CS
            //Don't have a better idea :(
            string moduleNameString = ",\"" + moduleName + "\"";

            mainModuleBuildCsContent = mainModuleBuildCsContent.Insert(moduleNameInsertIndex, moduleNameString);

            // ReSharper disable once UseObjectOrCollectionInitializer
            FileInfo fileInfo = new FileInfo(mainModulePath);
            fileInfo.IsReadOnly = false;

            File.WriteAllText(mainModulePath, mainModuleBuildCsContent);
        }

        private void GenerateProjectFiles()
        {
            engineUtilities.GenerateProjectFiles(projectController.GetUprojectPath());
        }
    }
}
