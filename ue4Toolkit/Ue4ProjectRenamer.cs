﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ue4Toolkit
{
    class Ue4ProjectRenamer
    {
        private readonly Ue4EngineUtilities engineUtilities;
        private readonly Ue4ProjectController projectController;
        private readonly string newName;
        private string newProjectPath;

        public Ue4ProjectRenamer(Ue4EngineUtilities engineUtilities, Ue4ProjectController projectController, string newName)
        {
            this.engineUtilities = engineUtilities;
            this.projectController = projectController;
            this.newName = newName;
        }

        //ToDo:
        // Update all module build cs files, as some may depend on main module
        // Change GameInstanceClass in DefaultEngine.ini
        public void Rename()
        {
            SetNewProjectPath();
            DuplicateProject();
            RenameUprojectFile();
            UpdateNewUprojectFile();
            RenameThumbnail();
            RenameTargetCs();
            UpdateNewTargetCsFile();
            RenameEditorTargetCs();
            UpdateNewEditorTargetCsFile();
            RenameMainModuleFolder();
            RenameBuildCs();
            UpdateNewBuildCsFile();
            GenerateProjectFiles();
            UpdateMainModuleCpp();
            UpdateEngineConfigIni();
            UpdateApiInSource();
            FixupRedirects();
            ResavePackages();
            Application.Exit();
        }

        private void SetNewProjectPath()
        {
            newProjectPath = projectController.GetProjectPath();
            //Todo: Can this be safer?
            newProjectPath = newProjectPath.Replace(projectController.GetProjectName(), newName);
        }

        private void DuplicateProject()
        {
            string sourcePath = projectController.GetProjectPath();
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                if (!dirPath.Contains("\\Intermediate") && 
                    !dirPath.Contains("\\Saved") && 
                    !dirPath.Contains("\\.vs") &&
                    !dirPath.Contains("\\Binaries") &&
                    !dirPath.Contains("\\Releases"))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, newProjectPath));
                }
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string sourceFilePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                if (!sourceFilePath.Contains("\\Intermediate\\") && 
                    !sourceFilePath.Contains("\\Saved\\") && 
                    !sourceFilePath.Contains("\\.vs\\") &&
                    !sourceFilePath.Contains("\\Binaries\\") &&
                    !sourceFilePath.Contains("\\Releases\\") &&
                    !sourceFilePath.Contains(projectController.GetProjectName() + ".sln"))
                {
                    string destinationFilePath = sourceFilePath.Replace(sourcePath, newProjectPath);
                    File.Copy(sourceFilePath, sourceFilePath.Replace(sourcePath, newProjectPath), true);
                    File.SetAttributes(destinationFilePath, File.GetAttributes(destinationFilePath) & ~FileAttributes.ReadOnly);
                }
            }
        }

        private void RenameUprojectFile()
        {
            RenameProjectFile("", ".uproject");
        }

        private void UpdateNewUprojectFile()
        {
            UpdateProjectFile("", ".uproject");
        }

        private void RenameThumbnail()
        {
            RenameProjectFile("", ".png");
        }

        private void RenameTargetCs()
        {
            RenameProjectFile("Source", ".Target.cs");
        }

        private void UpdateNewTargetCsFile()
        {
            UpdateProjectFile("Source", ".Target.cs");
        }

        private void RenameEditorTargetCs()
        {
            RenameProjectFile("Source", "Editor.Target.cs");
        }

        private void UpdateNewEditorTargetCsFile()
        {
            UpdateProjectFile("Source", "Editor.Target.cs");
        }

        private void RenameMainModuleFolder()
        {
            string sourcePath = Path.Combine(newProjectPath, "Source");
            string sourceModulePath = Path.Combine(sourcePath, projectController.GetProjectName());
            string destinationModulePath = Path.Combine(sourcePath, newName);

            if (Directory.Exists(destinationModulePath))
            {
                //ToDo: Error there is already a game module with the new name
            }

            Directory.CreateDirectory(destinationModulePath);

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourceModulePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourceModulePath, destinationModulePath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string file in Directory.GetFiles(sourceModulePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(file, file.Replace(sourceModulePath, destinationModulePath), true);
            }

            Directory.Delete(sourceModulePath, true);
        }

        private void RenameBuildCs()
        {
            RenameProjectFile(Path.Combine("Source", newName), ".Build.cs");
        }

        private void UpdateNewBuildCsFile()
        {
            UpdateProjectFile(Path.Combine("Source", newName), ".Build.cs");
        }

        private void GenerateProjectFiles()
        {
            engineUtilities.GenerateProjectFiles(Path.Combine(newProjectPath, newName + ".uproject"));
        }

        private void UpdateProjectFile(string path, string extention)
        {
            string filePath = Path.Combine(newProjectPath, path, newName + extention);
            string fileContent = File.ReadAllText(filePath);

            fileContent = fileContent.Replace(projectController.GetProjectName(), newName);

            File.WriteAllText(filePath, fileContent);
        }

        private void RenameProjectFile(string path, string extention)
        {
            string oldFile = Path.Combine(newProjectPath, path, projectController.GetProjectName() + extention);
            if (File.Exists(oldFile))
            {
                string newFile = Path.Combine(newProjectPath, path, newName + extention);
                File.Move(oldFile, newFile);
            }
        }

        private void UpdateMainModuleCpp()
        {
            string projectName = projectController.GetProjectName();
            string filePath = Path.Combine(newProjectPath, "Source", newName, projectName + ".cpp");
            string fileContent = File.ReadAllText(filePath);

            string implementModuleString = "IMPLEMENT_PRIMARY_GAME_MODULE";

            int implementModuleIndex = fileContent.IndexOf(implementModuleString, StringComparison.Ordinal);

            int firstProjectNameIndex = fileContent.IndexOf(projectName, implementModuleIndex, StringComparison.Ordinal);
            fileContent = fileContent.Remove(firstProjectNameIndex, projectName.Length);
            fileContent = fileContent.Insert(firstProjectNameIndex, newName);

            int secondProjectNameIndex =
                fileContent.IndexOf(projectName, implementModuleIndex, StringComparison.Ordinal);
            fileContent = fileContent.Remove(secondProjectNameIndex, projectName.Length);
            fileContent = fileContent.Insert(secondProjectNameIndex, newName);

            File.WriteAllText(filePath, fileContent);
        }

        private void UpdateEngineConfigIni()
        {
            string filePath = Path.Combine(newProjectPath, "Config", "DefaultEngine.ini");
            string fileContent = File.ReadAllText(filePath);

            fileContent = ReplaceDefaultGameMode(fileContent);
            fileContent = AddGameNameRedirector(fileContent);
            fileContent = AddClassRedirectors(fileContent);

            File.WriteAllText(filePath, fileContent);
        }

        private string ReplaceDefaultGameMode(string fileContent)
        {
            string defaultGameModeString = "GlobalDefaultGameMode=/Script/" + projectController.GetProjectName() + ".";
            string newDefaultGameModeString = "GlobalDefaultGameMode=/Script/" + newName + ".";

            fileContent = fileContent.Replace(defaultGameModeString, newDefaultGameModeString);
            return fileContent;
        }

        private string AddGameNameRedirector(string fileContent)
        {
            string engineIniCategory = "[/Script/Engine.Engine]";

            int categoryIndex = fileContent.IndexOf(engineIniCategory, StringComparison.Ordinal);

            int firstElementIndex = fileContent.IndexOf("+", categoryIndex, StringComparison.Ordinal);

            string gameNameRedirector = "+ActiveGameNameRedirects=(OldGameName=\"/Script/" + projectController.GetProjectName() +
                                        "\",NewGameName=\"/Script/" + newName + "\")\r\n";

            fileContent = fileContent.Insert(firstElementIndex, gameNameRedirector);
            return fileContent;
        }

        private string AddClassRedirectors(string fileContent)
        {
            string sourceFolderPath = Path.Combine(newProjectPath, "Source");

            foreach (string headerPath in Directory.GetFiles(sourceFolderPath, "*.h", SearchOption.AllDirectories))
            {
                //ToDo: create redirector for every class!
            }

            return fileContent;
        }

        private void UpdateApiInSource()
        {
            string oldApiString = projectController.GetProjectName().ToUpper() + "_API";
            string newApiString = newName.ToUpper() + "_API";

            string sourceFolderPath = Path.Combine(newProjectPath, "Source");

            foreach (string headerPath in Directory.GetFiles(sourceFolderPath, "*.h", SearchOption.AllDirectories))
            {
                string headerContent = File.ReadAllText(headerPath);
                int apiStringIndex = headerContent.IndexOf(oldApiString, StringComparison.Ordinal);
                if (apiStringIndex < -1)
                {
                    continue;
                }

                headerContent = headerContent.Replace(oldApiString, newApiString);

                File.WriteAllText(headerPath, headerContent);
            }       
        }

        private void FixupRedirects()
        {
            engineUtilities.FixupRedirects(Path.Combine(newProjectPath, newName + ".uproject"));
        }

        private void ResavePackages()
        {
            engineUtilities.ResavePackages(Path.Combine(newProjectPath, newName + ".uproject"));
        }
    }
}
