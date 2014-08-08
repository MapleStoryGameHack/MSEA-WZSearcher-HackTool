/*   MSEA-WZSearcher-HackTool - A handy tool for MapleStory packet editing
    Copyright (C) 2012~2014 eaxvac/lastBattle https://github.com/eaxvac

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MSEAHackUtility
{
    public static class WZFactory {
    
        public static Dictionary<int, string> map_name = new Dictionary<int, string>();
        public static Dictionary<int, KeyValuePair<string, string>>
            Item_EQ = new Dictionary<int, KeyValuePair<string, string>>(),
            Item_Use = new Dictionary<int, KeyValuePair<string, string>>(),
            Item_Setup = new Dictionary<int, KeyValuePair<string, string>>(),
            Item_ETC = new Dictionary<int, KeyValuePair<string, string>>(),
            Item_Cash = new Dictionary<int, KeyValuePair<string, string>>();
        public static bool isLoaded = false;

        public static String getMapName(int mapid)
        {
            if (isLoaded)
            {
                return map_name.ContainsKey(mapid) ? map_name[mapid] : "NULL";
            }
            CacheMapData();
            isLoaded = true;
            return map_name.ContainsKey(mapid) ? map_name[mapid] : "NULL";
        }

        public static string getMapNameEx(int mapid)
        {
            if (isLoaded)
            {
                return map_name.ContainsKey(mapid) ? map_name[mapid] : null;
            }
            CacheMapData();
            isLoaded = true;
            return map_name.ContainsKey(mapid) ? map_name[mapid] : null;
        }

        public static int getIndexOfMap(int index)
        {
            return map_name.ElementAt(index).Key;
        }

        public static void CacheMapData()
        {
            if (map_name.Count != 0)
            {
                return;
            }
            Assembly _assembly;
            StreamReader _textStreamReader = null;

            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("MSEAHackUtility."+Program.gateway+".Map.img.xml"));
            }
            catch (Exception e)
            {
  //              Debug.WriteLine(LogLevel.Error, "Error accessing Map.xml resourrce file {0}", e.StackTrace);
                return;
            }
            try
            {
                XmlDocument mapxml = new XmlDocument();
                mapxml.Load(_textStreamReader);
                foreach (XmlNode map in mapxml.DocumentElement.SelectNodes("/imgdir/imgdir/imgdir"))
                {
                    int id = int.Parse(map.Attributes["name"].InnerText);

                    XmlNode Node_StreetName = map.SelectSingleNode("string[@name='streetName']");
                    XmlNode Node_MapName = map.SelectSingleNode("string[@name='mapName']");

                    XmlAttribute streetName = Node_StreetName != null ? Node_StreetName.Attributes["value"] : null;
                    XmlAttribute mapName = Node_MapName != null ? Node_MapName.Attributes["value"] : null;

                    string mapname = String.Format("{0}: {1}",
                        streetName != null ? streetName.InnerText.Trim() : "",
                        mapName != null ? mapName.InnerText.Trim() : "");
                    // stuff it into dictionary
                    //            System.Windows.Forms.MessageBox.Show("id : "+id+", map : "+mapname+"");
                    if (!map_name.ContainsKey(id))
                    {
                        map_name.Add(id, mapname);
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        #region Eqp

        public static void SearchEQ(string data, Dictionary<int, KeyValuePair<string, string>> HexJumpList)
        {
            CacheEqData();

            foreach (KeyValuePair<int, KeyValuePair<string, string>> mapname in Item_EQ)
            {
                KeyValuePair<string, string> val = mapname.Value;
                if (val.Key.Contains(data))
                {
                    HexJumpList.Add(mapname.Key, val);
                }
            }
        }

        private static void CacheEqData()
        {
            if (Item_EQ.Count != 0)
            {
                return;
            }
            Assembly _assembly;
            StreamReader _textStreamReader = null;

            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("MSEAHackUtility." + Program.gateway + ".Eqp.img.xml"));
            }
            catch (Exception e)
            {
                //              Debug.WriteLine(LogLevel.Error, "Error accessing Map.xml resourrce file {0}", e.StackTrace);
                return;
            }
            XmlDocument mapxml = new XmlDocument();
            mapxml.Load(_textStreamReader);
            foreach (XmlNode map in mapxml.DocumentElement.SelectNodes("/imgdir/imgdir/imgdir/imgdir"))
            {
                int id = int.Parse(map.Attributes["name"].InnerText);
                XmlNode node = map.SelectSingleNode("string[@name='name']");
                XmlNode nodedesc = map.SelectSingleNode("string[@name='desc']");

                Item_EQ.Add(id, new KeyValuePair<string, String>(
                    node == null ? "NULL" : node.Attributes["value"].InnerText.ToLower(),
                    nodedesc == null ? "NULL" : nodedesc.Attributes["value"].InnerText));
            }
        }
        #endregion

        #region Use

        public static void SearchUse(string data, Dictionary<int, KeyValuePair<string, string>> HexJumpList)
        {
            CacheUseData();

            foreach (KeyValuePair<int, KeyValuePair<string, string>> mapname in Item_Use)
            {
                KeyValuePair<string, string> val = mapname.Value;
                if (val.Key.Contains(data))
                {
                    HexJumpList.Add(mapname.Key, val);
                }
            }
        }

        private static void CacheUseData()
        {
            if (Item_Use.Count != 0)
            {
                return;
            }
            Assembly _assembly;
            StreamReader _textStreamReader = null;

            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("MSEAHackUtility." + Program.gateway + ".Consume.img.xml"));
            }
            catch (Exception e)
            {
                //              Debug.WriteLine(LogLevel.Error, "Error accessing Map.xml resourrce file {0}", e.StackTrace);
                return;
            }
            XmlDocument mapxml = new XmlDocument();
            mapxml.Load(_textStreamReader);
            foreach (XmlNode map in mapxml.DocumentElement.SelectNodes("/imgdir/imgdir"))
            {
                int id = int.Parse(map.Attributes["name"].InnerText);
                XmlNode node = map.SelectSingleNode("string[@name='name']");
                XmlNode nodedesc = map.SelectSingleNode("string[@name='desc']");

                Item_Use.Add(id, new KeyValuePair<string, String>(
                    node == null ? "NULL" : node.Attributes["value"].InnerText.ToLower(),
                    nodedesc == null ? "NULL" : nodedesc.Attributes["value"].InnerText));
            }
        }
        #endregion

        #region setup

        public static void SearchSetup(string data, Dictionary<int, KeyValuePair<string, string>> HexJumpList)
        {
            CacheSetupData();

            foreach (KeyValuePair<int, KeyValuePair<string, string>> mapname in Item_Setup)
            {
                KeyValuePair<string, string> val = mapname.Value;
                if (val.Key.Contains(data))
                {
                    HexJumpList.Add(mapname.Key, val);
                }
            }
        }

        private static void CacheSetupData()
        {
            if (Item_Setup.Count != 0)
            {
                return;
            }
            Assembly _assembly;
            StreamReader _textStreamReader = null;

            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("MSEAHackUtility." + Program.gateway + ".Ins.img.xml"));
            }
            catch (Exception e)
            {
                //              Debug.WriteLine(LogLevel.Error, "Error accessing Map.xml resourrce file {0}", e.StackTrace);
                return;
            }
            XmlDocument mapxml = new XmlDocument();
            mapxml.Load(_textStreamReader);
            foreach (XmlNode map in mapxml.DocumentElement.SelectNodes("/imgdir/imgdir"))
            {
                int id = int.Parse(map.Attributes["name"].InnerText);
                XmlNode node = map.SelectSingleNode("string[@name='name']");
                XmlNode nodedesc = map.SelectSingleNode("string[@name='desc']");

                Item_Setup.Add(id, new KeyValuePair<string, String>(
                    node == null ? "NULL" : node.Attributes["value"].InnerText.ToLower(),
                    nodedesc == null ? "NULL" : nodedesc.Attributes["value"].InnerText));
            }
        }
        #endregion

        #region etc

        public static void SearchETC(string data,  Dictionary<int, KeyValuePair<string, string>> HexJumpList)
        {
            CacheETCData();

            foreach (KeyValuePair<int, KeyValuePair<string, string>> mapname in Item_ETC)
            {
                KeyValuePair<string, string> val = mapname.Value;
                if (val.Key.Contains(data))
                {
                    HexJumpList.Add(mapname.Key, val);
                }
            }
        }

        private static void CacheETCData()
        {
            if (Item_ETC.Count != 0)
            {
                return;
            }
            Assembly _assembly;
            StreamReader _textStreamReader = null;

            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("MSEAHackUtility." + Program.gateway + ".Etc.img.xml"));
            }
            catch (Exception e)
            {
                //              Debug.WriteLine(LogLevel.Error, "Error accessing Map.xml resourrce file {0}", e.StackTrace);
                return;
            }
            XmlDocument mapxml = new XmlDocument();
            mapxml.Load(_textStreamReader);
            foreach (XmlNode map in mapxml.DocumentElement.SelectNodes("/imgdir/imgdir/imgdir"))
            {
                int id = int.Parse(map.Attributes["name"].InnerText);
                XmlNode node = map.SelectSingleNode("string[@name='name']");
                XmlNode nodedesc = map.SelectSingleNode("string[@name='desc']");

                Item_ETC.Add(id, new KeyValuePair<string, String>(
                    node == null ? "NULL" : node.Attributes["value"].InnerText.ToLower(),
                    nodedesc == null ? "NULL" : nodedesc.Attributes["value"].InnerText));
            }
        }
        #endregion

        #region cash

        public static void SearchCash(string data, Dictionary<int, KeyValuePair<string, string>> HexJumpList)
        {
            CacheCashData();

            foreach (KeyValuePair<int, KeyValuePair<string, string>> mapname in Item_Cash)
            {
                KeyValuePair<string, string> val = mapname.Value;
                if (val.Key.Contains(data))
                {
                    HexJumpList.Add(mapname.Key, val);
                }
            }
        }

        private static void CacheCashData()
        {
            if (Item_Cash.Count != 0)
            {
                return;
            }
            Assembly _assembly;
            StreamReader _textStreamReader = null;

            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("MSEAHackUtility." + Program.gateway + ".Cash.img.xml"));
            }
            catch (Exception e)
            {
                //              Debug.WriteLine(LogLevel.Error, "Error accessing Map.xml resourrce file {0}", e.StackTrace);
                return;
            }
            XmlDocument mapxml = new XmlDocument();
            mapxml.Load(_textStreamReader);
            foreach (XmlNode map in mapxml.DocumentElement.SelectNodes("/imgdir/imgdir"))
            {
                int id = int.Parse(map.Attributes["name"].InnerText);
                XmlNode node = map.SelectSingleNode("string[@name='name']");
                XmlNode nodedesc = map.SelectSingleNode("string[@name='desc']");

                Item_Cash.Add(id, new KeyValuePair<string, String>(
                    node == null ? "NULL" : node.Attributes["value"].InnerText.ToLower(),
                    nodedesc == null ? "NULL" : nodedesc.Attributes["value"].InnerText));
            }
        }
        #endregion
    }
}
