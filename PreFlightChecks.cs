using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace PreFlightChecks
{
    public class Begin
    {
        private static bool updateAvaiable, updateCheck;
        private static string _updateVersionSource, _updateFileSource, _updateFileName;
        private Dictionary<string, string> hashList = new Dictionary<string, string>();

        public bool Checks(string configPath, string versionOfHost)
        {

            if (ConfigLoader(configPath))
            {
                return false;

            }

            foreach (KeyValuePair<string, String> hash in hashList)
            {
                if (!CalculateMD5(hash.Key).Equals(hash.Value))
                {
                    return false;
                }
            }

            //return (true); //add variable checks
            var downloadClient = new WebClient();
            try
            {
                if (!downloadClient.DownloadString(_updateVersionSource).Equals(versionOfHost)) ;


                {
                    downloadClient.DownloadFile(_updateFileSource, _updateFileName);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool ConfigLoader(string configPath)
        {
            if (!File.Exists(configPath))
            {
                return false;
            }
            
                using (var file = new StreamReader(configPath))
                {
                    var line = file.ReadLine();
                    while (line != null)
                    {
                        line = file.ReadLine();
                        
                            if (line.Contains("updateCheck"))
                                if (line.Contains("true"))
                                {
                                    updateCheck = true;
                                }

                                
                            else if (line.Contains("updateFileSource"))

                        {
                            try
                            {

                                _updateFileSource = line.Split('=')[1];
                            }
                            catch (Exception)
                            {
                                //required
                            }
                        }

                        
                            else if (line.Contains( "updateVersionSource")){
                            try
                            {
                                _updateVersionSource = line.Split('=')[1];
                            }
                            catch (Exception)
                            {
                                //required
                            }
                        }
                        else if (line.Contains("hash"))
                                {
                                    try
                                    {
                                        var split = line.Split('=');
                                        var temp = split[1].Split(',');
                                        hashList.Add(temp[0], temp[1]);
                                    }
                                    catch(Exception){}

                                }
                                else if (line.Contains("updateFileName"))
                                {
                                    try
                                    {
                                        _updateFileName = line.Split('=')[1];
                                        
                                    }
                                    catch(Exception){}

                                }




                    }
                    return true;
                }
            
            }
        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        }
    }
