using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Venv.Models
{
    public class XmlDataExtractor
    {
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
        public void EnrichDpusWithLineSetups(List<DPU> dpus)
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);

            XmlNodeList serialObjectDataNodes = _xmlDocument.GetElementsByTagName("SerialObjectData");
            foreach (XmlNode node in serialObjectDataNodes)
            {
                int plcNumber = int.Parse(node["PlcNumber"].InnerText);
                var dpu = dpus.FirstOrDefault(d => d.Number == plcNumber);
                
                //not all dpus have serialLines
                if (dpu != null)
                {
                    int sioChannelNumber = int.Parse(node["SioChannelNumber"].InnerText);
                    // If the sioChannelNumber has already been saved, then skip the entry. 
                    bool alreadyExists = dpu.LineSetups.Any(ls => ls.SioChannelNumber == sioChannelNumber);
                    if (!alreadyExists)
                    {
                        dpu.SioModuleID = node["SioModuleID"].InnerText;
                        var lineSetupNode = node["LineSetup"];
                        var lineSetup = new LineSetup
                        {
                            SioChannelNumber = int.Parse(node["SioChannelNumber"].InnerText),
                            ChannelInfo = lineSetupNode["ChannelInfo"].InnerText,
                            BaudRate = int.Parse(lineSetupNode["BaudRate"].InnerText),
                            LineFormat = lineSetupNode["LineFormat"].InnerText,
                            Handshake = lineSetupNode["Handshake"].InnerText,
                            RsSpecification = lineSetupNode["RSSpecification"].InnerText
                        };
                        dpu.LineSetups.Add(lineSetup);
                    }     
                }
            }
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
            EnrichDpusWithLineSetups(dpus);


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

            List<int> keysToRemove = new List<int>();
            foreach (var entry in machineryGroups)
            {
                if (entry.Value.Name == "Not used")
                {
                    keysToRemove.Add(entry.Key);
                }
            }

            foreach (int key in keysToRemove)
            {
                machineryGroups.Remove(key);
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
        public string GetYardBuildNumber()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNode node = _xmlDocument.SelectSingleNode("//YardBuildNo");
            return node?.InnerText ?? string.Empty;
        }
        public string GetYardName()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNode node = _xmlDocument.SelectSingleNode("//Yard");
            return node?.InnerText ?? string.Empty;
        }
        public int GetSwitchesNumber()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "EthSwitchConfiguration.xml");
            LoadXml(xmlFilePath);
            XmlNodeList node = _xmlDocument.GetElementsByTagName("Switch");
            if (node != null)
            {
                return node.Count;
            }
            return 0;
        }
        public int GetSequenceNumber()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNode node = _xmlDocument.SelectSingleNode("//SequenceNumber");
            if (node != null && int.TryParse(node.InnerText, out int amount))
            {
                return amount;
            }
            return 0;
        }
        public string GetFicVersion()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNode node = _xmlDocument.SelectSingleNode("//FicVersion");
            return node?.InnerText ?? string.Empty;
        }
        public string GetShipOwner()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNode node = _xmlDocument.SelectSingleNode("//ShipOwner");
            return node?.InnerText ?? string.Empty;
        }

        public int GetShipType()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNode node = _xmlDocument.SelectSingleNode("//ShipType");
            if (node != null && int.TryParse(node.InnerText, out int amount))
            {
                return amount;
            }
            return 0;
        }

        public string GetGenerationDate()
        {
            string xmlFilePath = Path.Combine(_xmlFolderPath, "ConfigIfSystem.xml");
            LoadXml(xmlFilePath);
            XmlNode node = _xmlDocument.SelectSingleNode("//GenerationDate");

            if (node != null && DateTime.TryParseExact(node.InnerText, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date.ToString("dd/MM/yyyy");
            }
            return string.Empty;
        }
    }
}
