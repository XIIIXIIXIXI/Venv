using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Venv.Models
{
    public class XmlDataExtractor
    {
        //private string _folderPath;
        private readonly XmlDocument _xmlDocument;
        private string _xmlFolderPath;

        public XmlDataExtractor()
        {
            _xmlDocument = new XmlDocument();
        }

        private void LoadXml(string xmlFile)
        {
            _xmlDocument.Load(xmlFile);
        }
        public void SetFolderPath(string filePath)
        {
            var _folderPath = filePath;
            _xmlFolderPath = Path.Combine(_folderPath, "XML");
        }
        public bool ValidateConfigurationFolder()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            return File.Exists(xmlFilePath);
        }

        public List<DPU> ExtractDpus()
        {
            List<DPU> plcNumbers = new List<DPU>();
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNodeList plcNodes = _xmlDocument.GetElementsByTagName("PlcList");
            foreach(XmlNode plcListNode in plcNodes )
            {
                foreach(XmlNode plcNode in plcListNode.ChildNodes)
                {
                    if (plcNode.Name == "int" && int.TryParse(plcNode.InnerText, out int plcNumber))
                    {
                        var dpu = new DPU();
                        dpu.Number = plcNumber;
                        dpu.Status = "Stopped";
                        plcNumbers.Add(dpu);
                    }
                }
            }
            return plcNumbers;
        }
        private Dictionary<int, MachineryGroup> ExtractMachineryGroups()
        {
            var machineryGroups = new Dictionary<int, MachineryGroup>();
            XmlNodeList groupNodes = _xmlDocument.GetElementsByTagName("MachineryGroup");
            foreach (XmlNode groupNode in groupNodes)
            {
                var numberNode = groupNode["Number"];
                var nameNode = groupNode["Name"]["string"];

                if (numberNode != null && nameNode != null && int.TryParse(numberNode.InnerText, out int number))
                {
                    var group = new MachineryGroup
                    {
                        Number = number,
                        Name = nameNode.InnerText
                    };
                    machineryGroups[number] = group;
                }

            }
            return machineryGroups;
        }

        public (List<DPU>, List<MachineryGroup>) AssociateDpusWithMachineryGorups()
        {
            List<DPU> dpus = ExtractDpus();
            Dictionary<int, MachineryGroup> machineryGroups = ExtractMachineryGroups();

            string xmlFilePath = Path.Combine(_xmlFolderPath, "ObjectConfig.xml");
            LoadXml(xmlFilePath);

            XmlNodeList objectConfigNodes = _xmlDocument.GetElementsByTagName("ObjectConfig");
            foreach (XmlNode node in objectConfigNodes)
            {
                var unitNumberAttr = node.Attributes["UnitNumber"];
                var machineryGroupAttr = node.Attributes["MachineryGroup"];

                if (unitNumberAttr != null && machineryGroupAttr != null && int.TryParse(unitNumberAttr.Value, out int unitNumber) && int.TryParse(machineryGroupAttr.Value, out int machineryGroupNumber))
                {
                    var dpu = dpus.FirstOrDefault(d => d.Number == unitNumber);
                    if (dpu != null && machineryGroups.TryGetValue(machineryGroupNumber, out MachineryGroup group))
                    {
                        if (!group.DPUs.Any(d => d.Number == dpu.Number))
                        {
                            group.DPUs.Add(dpu);
                        }
                    }
                }
            }
            foreach (var group in machineryGroups.Values)
            {
                group.DPUs = group.DPUs.OrderBy(d => d.Number).ToList();
            }

            return (dpus, machineryGroups.Values.ToList());
        }
        public string GetDatabaseVersion()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNode node = _xmlDocument.SelectSingleNode("//DatabaseVersion");
            return node?.InnerText ?? string.Empty;
        }
        public string GetDPU2010Version() 
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNode node = _xmlDocument.SelectSingleNode("//Dpu2010Version");
            return node?.InnerText ?? string.Empty;
        }
        /* not found
        public int GetPACsAmount()
        {

        }*/
        /* not found
        public int GetPanelsAmount()
        {

        }*/
        public int GetMFDsAmount()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNode node = _xmlDocument.SelectSingleNode("//Number_MFD");
            if (node != null && int.TryParse(node.InnerText, out int amount))
            {
                return amount;
            }
            return 0;
        }
        public string GetVesselName()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNode node = _xmlDocument.SelectSingleNode("//ShipName");
            return node?.InnerText ?? string.Empty;
        }

        public string GetIMONumber()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNode node = _xmlDocument.SelectSingleNode("//ImoNumber");
            return node?.InnerText ?? string.Empty;
        }
    }
}
