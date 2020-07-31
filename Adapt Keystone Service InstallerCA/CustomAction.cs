using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Microsoft.Deployment.WindowsInstaller;


namespace Adapt_Keystone_Service_InstallerCA
{
    public class CustomActions
    {
        private static Dictionary<string, string> addressEntries = new Dictionary<string, string>();

        [CustomAction]
        public static ActionResult CheckForExistingConfigFile(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            session.Log("CheckForOldLocation");
            string configFilePath = session["SERVICE_OLD_PATH"] + "ConsoleApp1.exe.config";
            if (File.Exists(configFilePath))
            {
                GetExistingConfigSettings(configFilePath, session);
            }
            else
            {
                configFilePath = session["ProgramFilesFolder"] + "Adapt Telephony, LLC\\ConsoleApp1\\" + "ConsoleApp1.exe.config";
                if (File.Exists(configFilePath))
                {
                    GetExistingConfigSettings(configFilePath, session);
                }
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveOriginalServiceFolder(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            session.Log("RemoveOriginalServiceFolder");
            string oldServiceFolder = session["SERVICE_OLD_PATH"];
            if (!string.IsNullOrEmpty(oldServiceFolder.Trim()) && Directory.Exists(oldServiceFolder))
            {
                Directory.Delete(oldServiceFolder, true);
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveRenamedServiceExe(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            session.Log("RemoveRenamedServiceExe");

            string renameFilePath = session["ProgramFilesFolder"] + "Adapt Telephony, LLC\\ConsoleApp1\\" + "ConsoleApp1.exe.save";
            if (File.Exists(renameFilePath))
            {
                File.Delete(renameFilePath);
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RenameExistingServiceExe(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            session.Log("RenameOriginalServiceExe");

            string existingFilePath = session["ProgramFilesFolder"] + "Adapt Telephony, LLC\\ConsoleApp1\\" + "ConsoleApp1.exe";
            if (File.Exists(existingFilePath))
            {
                string renameFilePath = existingFilePath + ".save";
                if (File.Exists(renameFilePath)) { File.Delete(renameFilePath); }
                File.Move(existingFilePath, renameFilePath);
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult UpdateConfigFile(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            session.Log("UpdateConfigFile");

            string configFilePath = session.CustomActionData["InstallFolder"] + "ConsoleApp1.exe.config";
            XmlDocument configXml = new XmlDocument();
            configXml.Load(configFilePath);

            XmlNode nodeKeystone_URL = configXml.SelectSingleNode("/configuration/appSettings/add[@key='Keystone_URL']");
            XmlNode nodeKeystone_UserName = configXml.SelectSingleNode("/configuration/appSettings/add[@key='Keystone_UserName']");
            XmlNode nodeKeystone_Password = configXml.SelectSingleNode("/configuration/appSettings/add[@key='Keystone_Password']");
            XmlNode nodeKeystone_DeviceName = configXml.SelectSingleNode("/configuration/appSettings/add[@key='Keystone_DeviceName']");
            XmlNode nodeKeystone_DefaultTimeZone = configXml.SelectSingleNode("/configuration/appSettings/add[@key='Keystone_DefaultTimeZone']");

            nodeKeystone_URL.Attributes["value"].Value = session.CustomActionData["KeystoneURL"];
            nodeKeystone_UserName.Attributes["value"].Value = session.CustomActionData["KeystoneUserName"];
            nodeKeystone_Password.Attributes["value"].Value = session.CustomActionData["KeystonePassword"];
            nodeKeystone_DeviceName.Attributes["value"].Value = session.CustomActionData["KeystoneDeviceName"];
            nodeKeystone_DefaultTimeZone.Attributes["value"].Value = session.CustomActionData["KeystoneDefaultTimeZone"];

            configXml.Save(configFilePath);

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult VerifyConfigFileData(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            session.Log("VerifyConfigFileData");

            session["CONFIG_DATA_VALID"] = "0";

            //  Keystone URL 
            if (String.IsNullOrEmpty(session["KEYSTONEURL"].Trim()))
            {
                session["CONFIG_DATA_VALID"] = "1";
                return ActionResult.Success;
            }
            //  Keystone User Name 
            if (String.IsNullOrEmpty(session["KEYSTONEUSERNAME"].Trim()))
            {
                session["CONFIG_DATA_VALID"] = "2";
                return ActionResult.Success;
            }
            //  Keystone Password
            if (String.IsNullOrEmpty(session["KEYSTONEPASSWORD"].Trim()))
            {
                session["CONFIG_DATA_VALID"] = "3";
                return ActionResult.Success;
            }
            //   Keystine Device Name
            if (String.IsNullOrEmpty(session["KEYSTONEDEVICENAME"].Trim()))
            {
                session["CONFIG_DATA_VALID"] = "4";
                return ActionResult.Success;
            }
            //  Keystone Defuatl Time Zone
            if (String.IsNullOrEmpty(session["KEYSTONEDEFAULTTIMEZONE"].Trim()))
            {
                session["CONFIG_DATA_VALID"] = "5";
                return ActionResult.Success;
            }
            return ActionResult.Success;
        }

        private static void GetExistingConfigSettings(string configFilePath, Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            XmlDocument configXml = new XmlDocument();
            configXml.Load(configFilePath);
            XmlNode nodeKeystone_URL = configXml.SelectSingleNode("/configuration/appSettings/add[@key='Keystone_URL']");
            XmlNode nodeKeystone_UserName = configXml.SelectSingleNode("/configuration/appSettings/add[@key='Keystone_UserName']");
            XmlNode nodeKeystone_Password = configXml.SelectSingleNode("/configuration/appSettings/add[@key='Keystone_Password']");
            XmlNode nodeKeystone_DeviceName = configXml.SelectSingleNode("/configuration/appSettings/add[@key='Keystone_DeviceName']");
            XmlNode nodeKeystone_DefaultTimeZone = configXml.SelectSingleNode("/configuration/appSettings/add[@key='Keystone_DefaultTimeZone']");

            session["KEYSTONEURL"] = nodeKeystone_URL.Attributes["value"].Value.ToString();
            session["KEYSTONEUSERNAME"] = nodeKeystone_UserName.Attributes["value"].Value.ToString();
            session["KEYSTONEPASSWORD"] = nodeKeystone_Password.Attributes["value"].Value.ToString();
            session["KEYSTONEDEVICENAME"] = nodeKeystone_DeviceName.Attributes["value"].Value.ToString();
            session["KEYSTONEDEFAULTTIMEZONE"] = nodeKeystone_DefaultTimeZone.Attributes["value"].Value.ToString();

        }
    
    }
}
