using System;
using System.Threading.Tasks;
using NetVimClient;

namespace SddcHostBrowser
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "https://vcenter.sddc-34-199-139-62.vmwarevmc.com";
            string username = "";
            string password = Environment.GetEnvironmentVariable("VCENTER_PWD");
            string datastoreBrowserKey = "datastoreBrowser-datastore-59";
            string path = "[WorkloadDatastore] 834dde5e-cbd6-413d-c0f8-122cc9c14f4d/00000000-0000-0000-0000-000000000000";
            string searchRegex = "*.vmdk";

            var dsbMOR = new ManagedObjectReference
            {
                type = "HostDatastoreBrowser",
                Value = datastoreBrowserKey
            };

            var vim = new VimPortTypeClient(
                VimPortTypeClient.EndpointConfiguration.VimPort,
                $"{url}/sdk"
            );

            var _sic = vim.RetrieveServiceContentAsync(
                new ManagedObjectReference
                {
                    type = "ServiceInstance",
                    Value = "ServiceInstance"
                }
            ).Result;

            var _props = _sic.propertyCollector;

            var _session = vim.LoginAsync(
                _sic.sessionManager,
                username,
                password,
                null
            ).Result;

            // search datastore
            var searchSpec = new HostDatastoreBrowserSearchSpec
            {
                matchPattern = new string[] { searchRegex },
            };

            // var task = vim.SearchDatastore_TaskAsync(
            var task = vim.SearchDatastoreSubFolders_TaskAsync(
                dsbMOR, path, searchSpec
            ).Result;

            Console.WriteLine($"{task.type} {task.Value}");
            Console.WriteLine($"{url}/mob/?moid={task.Value}");
            Console.WriteLine();
            Console.WriteLine("Sent request, awaiting response...");

            Task.Delay(2000).Wait();

            var response = vim.RetrievePropertiesAsync(
                _props,
                new PropertyFilterSpec[] {
                    new PropertyFilterSpec {
                        propSet = new PropertySpec[] {
                            new PropertySpec {
                                type = "Task",
                                pathSet = new string[] {"info"}
                            }
                        },
                        objectSet = new ObjectSpec[] {
                            new ObjectSpec {
                                obj = task
                            }
                        }
                    }
                }
            ).Result;

            ObjectContent[] oc = response.returnval;

            var info = (TaskInfo)oc[0]?.propSet[0]?.val;

            var results = info?.result as HostDatastoreBrowserSearchResults[];

            foreach (var result in results)
            {
                foreach (var file in result.file)
                {
                    Console.WriteLine(result.folderPath, file.path);
                }
            }

            Console.WriteLine("\nDone.");
        }
    }
}
