using CSCM.DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCM.Util
{
    internal class refreshCSCM
    {
        public async static void reface(string auth)
        {
            await Task.Run(async () =>
            {
                csdpEntities csdp = new csdpEntities();
                using (StreamReader file = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Z-Factory\\db.json"))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(file))
                    {
                        JObject o = (JObject)JToken.ReadFrom(jsonReader);
                        JArray groups = (JArray)o["group"];
                        foreach (JObject group in groups)
                        {
                            var packageId = group["id"].ToString();
                            CSCMPackage package = csdp.CSCMPackage.Find(packageId);
                            if (package == null)
                            {
                                package = new CSCMPackage();
                                package.id = packageId;
                                package.name = group["name"].ToString();
                                package.version = group["version"].ToString();
                                package.introduction = group["introduction"].ToString();
                                package.description = group["description"].ToString();
                                package.lastModifyTime = group["lastModifyTime"].ToString();
                                package.registry = group["registry"].ToString();
                                package.auth = auth;
                                csdp.CSCMPackage.Add(package);
                                csdp.SaveChanges();
                                Debug.WriteLine($"更新了组件包:{package.name}");
                            }
                            else
                            {
                                package.id = packageId;
                                package.name = group["name"].ToString();
                                package.version = group["version"].ToString();
                                package.introduction = group["introduction"].ToString();
                                package.description = group["description"].ToString();
                                package.lastModifyTime = group["lastModifyTime"].ToString();
                                package.registry = group["registry"].ToString();
                                package.auth = auth;
                                csdp.CSCMPackage.Add(package);
                                csdp.SaveChanges();
                                Debug.WriteLine($"添加了组件包:{package.name}");
                            }
                            JObject dependencies = (JObject)group["dependencies"];
                            foreach(var item in dependencies)
                            {
                                
                                string cscmId = item.Value["id"].ToString();
                                CSCMDependencies cSCMDependencies =csdp.CSCMDependencies.Find(cscmId);
                                if(cSCMDependencies == null)
                                {
                                    cSCMDependencies = new CSCMDependencies();
                                    cSCMDependencies.id = cscmId;
                                    cSCMDependencies.packageId= packageId;
                                    cSCMDependencies.name = item.Value["name"].ToString();
                                    cSCMDependencies.version = item.Value["version"].ToString();
                                    cSCMDependencies.type = item.Value["type"].ToString();
                                    cSCMDependencies.state = item.Value["state"].ToString();
                                    cSCMDependencies.descriptioin = item.Value["description"].ToString();
                                    cSCMDependencies.message0 = item.Value["message0"].ToString();
                                    cSCMDependencies.componentType = item.Value["componentType"].ToString();
                                    cSCMDependencies.lastModifyTime = item.Value["lastModifyTime"].ToString();
                                    csdp.SaveChanges();
                                    Debug.WriteLine($"更新了组件列表:{cSCMDependencies.name}");
                                }
                                else
                                {
                                   // cSCMDependencies = new CSCMDependencies();
                                    cSCMDependencies.id = cscmId;
                                    cSCMDependencies.packageId = packageId;
                                    cSCMDependencies.name = item.Value["name"].ToString();
                                    cSCMDependencies.version = item.Value["version"].ToString();
                                    cSCMDependencies.type = item.Value["type"].ToString();
                                    cSCMDependencies.state = item.Value["state"].ToString();
                                    cSCMDependencies.descriptioin = item.Value["description"].ToString();
                                    cSCMDependencies.message0 = item.Value["message0"].ToString();
                                    cSCMDependencies.componentType = item.Value["componentType"].ToString();
                                    cSCMDependencies.lastModifyTime = item.Value["lastModifyTime"].ToString();
                                    csdp.SaveChanges();
                                    Debug.WriteLine($"添加了组件列表:{cSCMDependencies.name}");
                                }
                            }
                            
                        }
                    }
                }
                foreach (string folderName in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Z-Factory\\task-component\\components"))
                {
                    foreach (string versionPath in Directory.GetDirectories(folderName))
                    {
                        using (System.IO.StreamReader file = System.IO.File.OpenText(versionPath + "\\taskInfo.json"))
                        {
                            using (JsonTextReader jsonReader = new JsonTextReader(file))
                            {
                                JObject o = (JObject)JToken.ReadFrom(jsonReader);
                                string taskId = o["id"].ToString();
                                string taskVersion = o["version"].ToString();
                                cscmVersion version = csdp.cscmVersion.Where(p => p.taskId == taskId & p.taskVersion == taskVersion).FirstOrDefault();
                                if (taskVersion != null)
                                {
                                    continue;
                                }
                                else
                                {
                                    version = new cscmVersion();
                                    version.id = System.Guid.NewGuid().ToString();
                                    version.taskId = taskId;
                                    version.message0 = o["component"]["definition"]["message0"].ToString();
                                    version.args0 = o["component"]["definition"]["args0"].ToString();
                                    version.taskVersion = o["taskVersion"].ToString();
                                    csdp.cscmVersion.Add(version);
                                    csdp.SaveChanges();
                                    Debug.WriteLine($"添加了版本信息{version.args0}");

                                }

                            }
                        }
                    }
                }
               
            });
        }
        public async static void refaceFile()
        {
            await Task.Run((Action)(() =>
            {
                csdpEntities csdp = new csdpEntities();
                
            }));
        }
    }
}
