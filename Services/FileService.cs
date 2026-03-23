using RandomStudentSelectorArturW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStudentSelectorArturW.Services
{
    internal class FileService
    {
        private const string CLASSES_FILE = "classes.txt";

        public static string GetPath(string className)
        {
            try
            {
                string appDataPath = FileSystem.AppDataDirectory;
                string fileName = $"{className}.txt";
                string fullPath = Path.Combine(appDataPath, fileName);
                System.Diagnostics.Debug.WriteLine($"FileService Path: {fullPath}");
                return fullPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetPath Error: {ex.Message}");
                throw;
            }
        }

        private static string GetClassesPath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, CLASSES_FILE);
        }

        public static List<string> GetAllClasses()
        {
            try
            {
                string path = GetClassesPath();

                if (!File.Exists(path))
                {
                    System.Diagnostics.Debug.WriteLine("Classes file doesn't exist, returning empty list");
                    return new List<string>();
                }

                var classes = File.ReadAllLines(path)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Trim())
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Loaded {classes.Count} classes");
                return classes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllClasses Error: {ex.Message}");
                return new List<string>();
            }
        }

        public static void AddClass(string className)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(className))
                {
                    System.Diagnostics.Debug.WriteLine("Cannot add empty class name");
                    return;
                }

                className = className.Trim();
                var classes = GetAllClasses();

                if (classes.Contains(className))
                {
                    System.Diagnostics.Debug.WriteLine($"Class {className} already exists");
                    return;
                }

                classes.Add(className);
                SaveClassesList(classes);
                System.Diagnostics.Debug.WriteLine($"Added class: {className}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddClass Error: {ex.Message}");
            }
        }

        public static void DeleteClass(string className)
        {
            try
            {
                string dataPath = GetPath(className);
                if (File.Exists(dataPath))
                {
                    File.Delete(dataPath);
                    System.Diagnostics.Debug.WriteLine($"Deleted student file: {dataPath}");
                }

                var classes = GetAllClasses();
                classes.Remove(className);
                SaveClassesList(classes);
                System.Diagnostics.Debug.WriteLine($"Deleted class: {className}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteClass Error: {ex.Message}");
            }
        }

        public static void RenameClass(string oldName, string newName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newName))
                {
                    System.Diagnostics.Debug.WriteLine("New class name cannot be empty");
                    return;
                }

                newName = newName.Trim();

                var classes = GetAllClasses();
                if (classes.Contains(newName) && oldName != newName)
                {
                    System.Diagnostics.Debug.WriteLine($"Class {newName} already exists");
                    return;
                }

                string oldPath = GetPath(oldName);
                string newPath = GetPath(newName);

                if (File.Exists(oldPath))
                {
                    File.Move(oldPath, newPath, true);
                    System.Diagnostics.Debug.WriteLine($"Renamed student file: {oldName} -> {newName}");
                }

                if (classes.Contains(oldName))
                {
                    classes.Remove(oldName);
                    classes.Add(newName);
                    SaveClassesList(classes);
                    System.Diagnostics.Debug.WriteLine($"Renamed class: {oldName} -> {newName}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RenameClass Error: {ex.Message}");
            }
        }

        private static void SaveClassesList(List<string> classes)
        {
            try
            {
                string path = GetClassesPath();
                string directory = Path.GetDirectoryName(path)!;

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllLines(path, classes.OrderBy(c => c));
                System.Diagnostics.Debug.WriteLine($"Saved {classes.Count} classes");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveClassesList Error: {ex.Message}");
            }
        }

        public static void Save(ClassRoom classRoom)
        {
            try
            {
                if (classRoom == null)
                {
                    System.Diagnostics.Debug.WriteLine("Cannot save: ClassRoom is null");
                    return;
                }

                if (string.IsNullOrEmpty(classRoom.ClassName))
                {
                    System.Diagnostics.Debug.WriteLine("Cannot save: ClassName is null or empty");
                    return;
                }

                string path = GetPath(classRoom.ClassName);
                string directory = Path.GetDirectoryName(path)!;

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    System.Diagnostics.Debug.WriteLine($"Created directory: {directory}");
                }

                var studentLines = classRoom.Students
                    .Where(s => s != null && !string.IsNullOrWhiteSpace(s.Name))
                    .Select((s, index) =>
                    {
                        s.Number = index + 1;
                        return $"{s.Number}|{s.Name.Trim()}";
                    })
                    .Distinct()
                    .ToArray();

                File.WriteAllLines(path, studentLines);
                System.Diagnostics.Debug.WriteLine($"Saved {studentLines.Length} students to {path}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Save Error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public static ClassRoom Load(string className)
        {
            try
            {
                var room = new ClassRoom { ClassName = className };
                string path = GetPath(className);

                System.Diagnostics.Debug.WriteLine($"Loading from: {path}");

                if (!File.Exists(path))
                {
                    System.Diagnostics.Debug.WriteLine($"File does not exist, returning empty class: {path}");
                    return room;
                }

                var lines = File.ReadAllLines(path);
                System.Diagnostics.Debug.WriteLine($"Read {lines.Length} lines from file");

                int studentNumber = 1;
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var parts = line.Split('|');
                        string name = parts.Length > 1 ? parts[1].Trim() : line.Trim();

                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            room.Students.Add(new Student { Number = studentNumber, Name = name });
                            studentNumber++;
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Loaded {room.Students.Count} students for class {className}");
                return room;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load Error: {ex.Message}\n{ex.StackTrace}");
                return new ClassRoom { ClassName = className };
            }
        }
    }
}

