using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using GUI.Utils;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;
using ValveResourceFormat.Utils;

namespace GUI.Types.Renderer
{
    internal class WorldLoader
    {
        private readonly World world;
        private readonly VrfGuiContext guiContext;

        // Contains metadata that can't be captured by manipulating the scene itself. Returned from Load().
        public class LoadResult
        {
            public HashSet<string> DefaultEnabledLayers { get; } = new HashSet<string>();

            public IDictionary<string, Matrix4x4> CameraMatrices { get; } = new Dictionary<string, Matrix4x4>();

            public Vector3? GlobalLightPosition { get; set; }

            public World Skybox { get; set; }
            public float SkyboxScale { get; set; } = 1.0f;
            public Vector3 SkyboxOrigin { get; set; } = Vector3.Zero;
        }

        public WorldLoader(VrfGuiContext vrfGuiContext, World world)
        {
            this.world = world;
            guiContext = vrfGuiContext;
        }

        public LoadResult Load(Scene scene)
        {
            var result = new LoadResult();
            result.DefaultEnabledLayers.Add("Entities");

            // Output is World_t we need to iterate m_worldNodes inside it.
            var worldNodes = world.GetWorldNodeNames();
            foreach (var worldNode in worldNodes)
            {
                if (worldNode != null)
                {
                    var newResource = guiContext.LoadFileByAnyMeansNecessary(worldNode + ".vwnod_c");
                    if (newResource == null)
                    {
                        continue;
                    }

                    var subloader = new WorldNodeLoader(guiContext, (WorldNode)newResource.DataBlock);
                    subloader.Load(scene);
                }
            }

            foreach (var lumpName in world.GetEntityLumpNames())
            {
                if (lumpName == null)
                {
                    continue;
                }

                var newResource = guiContext.LoadFileByAnyMeansNecessary(lumpName + "_c");

                if (newResource == null)
                {
                    continue;
                }

                var entityLump = (EntityLump)newResource.DataBlock;
                LoadEntitiesFromLump(scene, result, entityLump, "world_layer_base"); // TODO
            }

            return result;
        }

        private void LoadEntitiesFromLump(Scene scene, LoadResult result, EntityLump entityLump, string layerName = null)
        {
            var childEntities = entityLump.GetChildEntityNames();

            foreach (var childEntityName in childEntities)
            {
                var newResource = guiContext.LoadFileByAnyMeansNecessary(childEntityName + "_c");

                if (newResource == null)
                {
                    continue;
                }

                var childLump = (EntityLump)newResource.DataBlock;
                var childName = childLump.Data.GetProperty<string>("m_name");

                LoadEntitiesFromLump(scene, result, childLump, childName);
            }

            var worldEntities = entityLump.GetEntities();

            foreach (var entity in worldEntities)
            {
                var classname = entity.GetProperty<string>("classname");

                if (classname == "info_world_layer")
                {
                    var spawnflags = entity.GetProperty<uint>("spawnflags");
                    var layername = entity.GetProperty<string>("layername");

                    // Visible on spawn flag
                    if ((spawnflags & 1) == 1)
                    {
                        result.DefaultEnabledLayers.Add(layername);
                    }
                }
                else if (classname == "skybox_reference")
                {
                    // var worldgroupid = entity.GetProperty<string>("worldgroupid");
                    _ = entity.GetProperty<string>("worldgroupid");
                    var targetmapname = entity.GetProperty<string>("targetmapname");

                    var skyboxWorldPath = $"maps/{Path.GetFileNameWithoutExtension(targetmapname)}/world.vwrld_c";
                    var skyboxPackage = guiContext.LoadFileByAnyMeansNecessary(skyboxWorldPath);

                    if (skyboxPackage == null && guiContext.ParentGuiContext != null)
                    {
                        var mapName = Path.GetFileNameWithoutExtension(guiContext.ParentGuiContext.FileName);
                        var mapsFolder = Path.GetDirectoryName(guiContext.ParentGuiContext.FileName);
                        var skyboxVpk = Path.Join(mapsFolder, mapName, $"{Path.GetFileNameWithoutExtension(targetmapname)}.vpk");

                        if (File.Exists(skyboxVpk))
                        {
                            var skyboxNewPackage = new SteamDatabase.ValvePak.Package();
                            skyboxNewPackage.Read(skyboxVpk);

                            guiContext.ParentGuiContext.FileLoader.AddPackageToSearch(skyboxNewPackage);

                            skyboxWorldPath = $"maps/{mapName}/{Path.GetFileNameWithoutExtension(targetmapname)}/world.vwrld_c";
                            skyboxPackage = guiContext.LoadFileByAnyMeansNecessary(skyboxWorldPath);
                        }
                    }

                    if (skyboxPackage != null)
                    {
                        result.Skybox = (World)skyboxPackage.DataBlock;
                    }
                }

                var scale = entity.GetProperty<string>("scales");
                var position = entity.GetProperty<string>("origin");
                var angles = entity.GetProperty<string>("angles");
                var model = entity.GetProperty<string>("model");
                var skin = entity.GetProperty<string>("skin");
                var particle = entity.GetProperty<string>("effect_name");
                var animation = entity.GetProperty<string>("defaultanim");

                if (scale == null || position == null || angles == null)
                {
                    continue;
                }

                var isGlobalLight = classname == "env_global_light" || classname == "light_environment";
                var isCamera =
                    classname == "sky_camera" ||
                    classname == "point_devshot_camera" ||
                    classname == "point_camera";
                var isTrigger =
                    classname.Contains("trigger", StringComparison.InvariantCulture) ||
                    classname == "post_processing_volume";

                var positionVector = EntityTransformHelper.ParseVector(position);

                var transformationMatrix = EntityTransformHelper.CalculateTransformationMatrix(entity);

                if (classname == "sky_camera")
                {
                    result.SkyboxScale = entity.GetProperty<ulong>("scale");
                    result.SkyboxOrigin = positionVector;
                }

                if (particle != null)
                {
                    var particleResource = guiContext.LoadFileByAnyMeansNecessary(particle + "_c");

                    if (particleResource != null)
                    {
                        var particleSystem = (ParticleSystem)particleResource.DataBlock;
                        var origin = new Vector3(positionVector.X, positionVector.Y, positionVector.Z);

                        try
                        {
                            var particleNode = new ParticleSceneNode(scene, particleSystem)
                            {
                                Transform = Matrix4x4.CreateTranslation(origin),
                                LayerName = layerName,
                            };
                            scene.Add(particleNode, true);
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine($"Failed to setup particle '{particle}': {e}");
                        }
                    }

                    continue;
                }

                if (isCamera)
                {
                    var name = entity.GetProperty<string>("targetname") ?? string.Empty;
                    var cameraName = string.IsNullOrEmpty(name)
                        ? classname
                        : name;

                    result.CameraMatrices.Add(cameraName, transformationMatrix);
                }
                else if (isGlobalLight)
                {
                    result.GlobalLightPosition = positionVector;
                }

                var objColor = Vector4.One;

                // Parse colour if present
                var colour = entity.GetProperty("rendercolor");

                // HL Alyx has an entity that puts rendercolor as a string instead of color255
                if (colour != default && colour.Type == EntityFieldType.Color32)
                {
                    var colourBytes = (byte[])colour.Data;
                    objColor.X = colourBytes[0] / 255.0f;
                    objColor.Y = colourBytes[1] / 255.0f;
                    objColor.Z = colourBytes[2] / 255.0f;
                    objColor.W = colourBytes[3] / 255.0f;
                }

                if (!isTrigger && model == null)
                {
                    AddToolModel(scene, classname, transformationMatrix, positionVector);
                    continue;
                }

                var newEntity = guiContext.LoadFileByAnyMeansNecessary(model + "_c");

                if (newEntity == null)
                {
                    var errorModelResource = guiContext.LoadFileByAnyMeansNecessary("models/dev/error.vmdl_c");

                    if (errorModelResource != null)
                    {
                        var errorModel = new ModelSceneNode(scene, (Model)errorModelResource.DataBlock, skin, false)
                        {
                            Transform = transformationMatrix,
                            LayerName = layerName,
                        };
                        scene.Add(errorModel, false);
                    }

                    continue;
                }

                var newModel = (Model)newEntity.DataBlock;

                var modelNode = new ModelSceneNode(scene, newModel, skin, false)
                {
                    Transform = transformationMatrix,
                    Tint = objColor,
                    LayerName = layerName,
                    Name = model,
                };

                if (animation != default)
                {
                    modelNode.LoadAnimations();
                    modelNode.SetAnimation(animation);

                    if (entity.GetProperty<bool>("holdanimation"))
                    {
                        modelNode.AnimationController.PauseLastFrame();
                    }
                }

                var bodyHash = StringToken.Get("body");
                if (entity.Properties.ContainsKey(bodyHash))
                {
                    var groups = modelNode.GetMeshGroups();
                    var body = entity.Properties[bodyHash].Data;
                    var bodyGroup = -1;

                    if (body is ulong bodyGroupLong)
                    {
                        bodyGroup = (int)bodyGroupLong;
                    }
                    else if (body is string bodyGroupString)
                    {
                        if (!int.TryParse(bodyGroupString, out bodyGroup))
                        {
                            bodyGroup = -1;
                        }
                    }

                    modelNode.SetActiveMeshGroups(groups.Skip(bodyGroup).Take(1));
                }

                scene.Add(modelNode, animation != default);

                var phys = newModel.GetEmbeddedPhys();
                if (phys == null)
                {
                    var refPhysicsPaths = newModel.GetReferencedPhysNames();
                    if (refPhysicsPaths.Any())
                    {
                        var newResource = guiContext.LoadFileByAnyMeansNecessary(refPhysicsPaths.First() + "_c");
                        if (newResource != null)
                        {
                            phys = (PhysAggregateData)newResource.DataBlock;
                        }
                    }
                }

                if (phys != null)
                {
                    var physSceneNode = new PhysSceneNode(scene, phys)
                    {
                        Transform = transformationMatrix,
                        IsTrigger = isTrigger,
                        LayerName = layerName
                    };
                    scene.Add(physSceneNode, false);
                }
            }
        }

        private void AddToolModel(Scene scene, string classname, Matrix4x4 transformationMatrix, Vector3 position)
        {
            var filename = HammerEntities.GetToolModel(classname);
            var resource = guiContext.LoadFileByAnyMeansNecessary(filename + "_c");

            if (resource == null)
            {
                // TODO: Create a 16x16x16 box to emulate how Hammer draws them
                resource = guiContext.LoadFileByAnyMeansNecessary("materials/editor/obsolete.vmat_c");

                if (resource == null)
                {
                    return;
                }
            }

            if (resource.ResourceType == ResourceType.Model)
            {
                var modelNode = new ModelSceneNode(scene, (Model)resource.DataBlock, null, false)
                {
                    Transform = transformationMatrix,
                    LayerName = "Entities",
                };
                scene.Add(modelNode, false);
            }
            else if (resource.ResourceType == ResourceType.Material)
            {
                var spriteNode = new SpriteSceneNode(scene, guiContext, resource, position)
                {
                    LayerName = "Entities",
                };
                scene.Add(spriteNode, false);
            }
            else
            {
                throw new InvalidDataException($"Got resource {resource.ResourceType} for class \"{classname}\"");
            }
        }
    }
}
