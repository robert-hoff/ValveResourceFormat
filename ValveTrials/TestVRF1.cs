using SteamDatabase.ValvePak;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ValveResourceFormat;
using ValveResourceFormat.IO;
using ValveResourceFormat.ResourceTypes;

namespace ValveTrials {
	class TestVRF1 {

		public static void doIt() {

			string vpkfile = @"Z:\git\vcs-decompile\vrf-decompiler-bat\pak01.vpk";
			var package = new SteamDatabase.ValvePak.Package();
			package.Read(vpkfile);

			string huskar_filename = "armor_of_reckless_vigor_weapon.vmdl_c";
			string huskar_model = "models/items/huskar/armor_of_reckless_vigor_weapon/armor_of_reckless_vigor_weapon.vmdl_c";
			PackageEntry model_entry = package.FindEntry(huskar_model);

			var resource = new ValveResourceFormat.Resource {
				FileName = huskar_filename
			};


			package.ReadEntry(model_entry, out byte[] output);
			resource.Read(new MemoryStream(output));

		}



		static ISet<ResourceType> ResourceTypesThatAreGltfExportable = new HashSet<ResourceType>()
		{ ResourceType.Mesh, ResourceType.Model, ResourceType.WorldNode, ResourceType.World };


		public static void Export(string fileName, ExportFileType FileType, Resource resource) {
			var extension = FileExtract.GetExtension(resource);

			if (extension == null) {
				Console.WriteLine($"Export for \"{fileName}\" has no suitable extension");
				return;
			}

			var filter = $"{extension} file|*.{extension}";

			if (ResourceTypesThatAreGltfExportable.Contains(resource.ResourceType)) {
				if (FileType == ExportFileType.GLB) {
					extension = "glb";
					filter = $"GLB file|*.glb|{filter}";
				} else {
					extension = "gltf";
					filter = $"glTF file|*.gltf|{filter}";
				}
			}


			// -- this line just adds the gltf ending, FileName will be
			// "armor_of_reckless_vigor_weapon.gltf"
			var dialog = new SaveFileDialog {
				FileName = Path.GetFileName(Path.ChangeExtension(fileName, extension)),
				DefaultExt = extension,
				Filter = filter,
			};


			// output directory
			// string FileName = "armor_of_reckless_vigor_weapon.gltf";

			// results indicates if the dialog was successful
			// var result = dialog.ShowDialog();
			// if (result != DialogResult.OK)
			// {
			//	Console.WriteLine($"Export for \"{fileName}\" cancelled");
			//	return;
			// }

			Console.WriteLine($"Export for \"{fileName}\" started to \"{extension}\"");
			// Settings.Config.SaveDirectory = Path.GetDirectoryName(dialog.FileName);
			// Settings.Save();

			var extractDialog = new GenericProgressForm();



			/*
			extractDialog.OnProcess += (_, __) => {
				if (dialog.FilterIndex == 1 && ResourceTypesThatAreGltfExportable.Contains(resource.ResourceType)) {
					var exporter = new GltfModelExporter {
						ProgressReporter = new Progress<string>(extractDialog.SetProgress),
						FileLoader = exportData.VrfGuiContext.FileLoader,
					};
					switch (resource.ResourceType) {
						case ResourceType.Mesh:
							exporter.ExportToFile(fileName, dialog.FileName, new Mesh(resource));
							break;
						case ResourceType.Model:
							exporter.ExportToFile(fileName, dialog.FileName, (Model)resource.DataBlock);
							break;
						case ResourceType.WorldNode:
							exporter.ExportToFile(fileName, dialog.FileName, (WorldNode)resource.DataBlock);
							break;
						case ResourceType.World:
							exporter.ExportToFile(fileName, dialog.FileName, (World)resource.DataBlock);
							break;
						default:
							break;
					}
				} else {
					var data = FileExtract.Extract(resource).ToArray();
					using var stream = dialog.OpenFile();
					stream.Write(data, 0, data.Length);
				}

				Console.WriteLine($"Export for \"{fileName}\" completed");
			};
			extractDialog.ShowDialog();
			*/



		}



		public enum ExportFileType {
			Auto,
			GLB
		}










	}
}
