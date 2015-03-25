﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Xbim.COBieLiteUK.Converters;
using Formatting = System.Xml.Formatting;

namespace Xbim.COBieLiteUK
{
    public partial class Facility
    {
        #region Enumerations

        public AreaUnit AreaUnits
        {
            get
            {
                if (String.IsNullOrEmpty(AreaUnitsCustom)) return AreaUnit.notdefined;

                //try to parse string value
                AreaUnit result;
                if (Enum.TryParse(AreaUnitsCustom, true, out result))
                    return result;

                //try to use aliases
                var enumMembers = typeof (AreaUnit).GetFields();
                foreach (var member in from member in enumMembers
                    let alias = member.GetCustomAttributes<AliasAttribute>()
                        .FirstOrDefault(
                            a => String.Equals(a.Value, AreaUnitsCustom, StringComparison.CurrentCultureIgnoreCase))
                    where alias != null
                    select member)
                    return (AreaUnit) member.GetValue(result);

                //if nothing fits it is a user defined value
                return AreaUnit.userdefined;
            }
            set
            {
                switch (value)
                {
                    case AreaUnit.notdefined:
                        AreaUnitsCustom = null;
                        break;
                    case AreaUnit.userdefined:
                        break;
                    default:
                        AreaUnitsCustom = Enum.GetName(typeof (AreaUnit), value);
                        break;
                }
            }
        }

        public LinearUnit LinearUnits
        {
            get
            {
                if (String.IsNullOrEmpty(LinearUnitsCustom)) return LinearUnit.notdefined;

                //try to parse string value
                LinearUnit result;
                if (Enum.TryParse(LinearUnitsCustom, true, out result))
                    return result;

                //try to use aliases
                var enumMembers = typeof (LinearUnit).GetFields();
                foreach (var member in from member in enumMembers
                    let alias = member.GetCustomAttributes<AliasAttribute>()
                        .FirstOrDefault(
                            a => String.Equals(a.Value, LinearUnitsCustom, StringComparison.CurrentCultureIgnoreCase))
                    where alias != null
                    select member)
                    return (LinearUnit) member.GetValue(result);

                //if nothing fits it is a user defined value
                return LinearUnit.userdefined;
            }
            set
            {
                switch (value)
                {
                    case LinearUnit.notdefined:
                        LinearUnitsCustom = null;
                        break;
                    case LinearUnit.userdefined:
                        break;
                    default:
                        LinearUnitsCustom = Enum.GetName(typeof (LinearUnit), value);
                        break;
                }
            }
        }

        public VolumeUnit VolumeUnits
        {
            get
            {
                if (String.IsNullOrEmpty(VolumeUnitsCustom)) return VolumeUnit.notdefined;

                //try to parse string value
                VolumeUnit result;
                if (Enum.TryParse(VolumeUnitsCustom, true, out result))
                    return result;

                //try to use aliases
                var enumMembers = typeof(VolumeUnit).GetFields();
                foreach (var member in from member in enumMembers
                                       let alias = member.GetCustomAttributes<AliasAttribute>()
                                           .FirstOrDefault(
                                               a => String.Equals(a.Value, VolumeUnitsCustom, StringComparison.CurrentCultureIgnoreCase))
                                       where alias != null
                                       select member)
                    return (VolumeUnit)member.GetValue(result);

                //if nothing fits it is a user defined value
                return VolumeUnit.userdefined;
            }
            set
            {
                switch (value)
                {
                    case VolumeUnit.notdefined:
                        VolumeUnitsCustom = null;
                        break;
                    case VolumeUnit.userdefined:
                        break;
                    default:
                        VolumeUnitsCustom = Enum.GetName(typeof(VolumeUnit), value);
                        break;
                }
            }
        }

        public CurrencyUnit CurrencyUnit
        {
            get
            {
                if (String.IsNullOrEmpty(CurrencyUnitCustom)) return CurrencyUnit.notdefined;

                //try to parse string value
                CurrencyUnit result;
                if (Enum.TryParse(CurrencyUnitCustom, true, out result))
                    return result;

                //try to use aliases
                var enumMembers = typeof(CurrencyUnit).GetFields();
                foreach (var member in from member in enumMembers
                                       let alias = member.GetCustomAttributes<AliasAttribute>()
                                           .FirstOrDefault(
                                               a => String.Equals(a.Value, CurrencyUnitCustom, StringComparison.CurrentCultureIgnoreCase))
                                       where alias != null
                                       select member)
                    return (CurrencyUnit)member.GetValue(result);

                //if nothing fits it is a user defined value
                return CurrencyUnit.userdefined;
            }
            set
            {
                switch (value)
                {
                    case CurrencyUnit.notdefined:
                        CurrencyUnitCustom = null;
                        break;
                    case CurrencyUnit.userdefined:
                        break;
                    default:
                        CurrencyUnitCustom = Enum.GetName(typeof(CurrencyUnit), value);
                        break;
                }
            }
        }

        #endregion

        #region XML serialization

        private static XmlSerializer GetXmlSerializer()
        {
            return new XmlSerializer(typeof (Facility));
        }

        public void WriteXml(string path, bool indented = false)
        {
            using (var stream = File.Create(path))
            {
                WriteXml(stream, indented);
                stream.Close();
            }
        }

        public void WriteXml(Stream stream, bool indented = false)
        {
            var serializer = GetXmlSerializer();
            using (var w = new StreamWriter(stream))
            {
                using (
                    var xmlWriter = new XmlTextWriter(w) {Formatting = indented ? Formatting.Indented : Formatting.None}
                    )
                {
                    serializer.Serialize(xmlWriter, this,
                        new XmlSerializerNamespaces(new[]
                        {
                            new XmlQualifiedName("cobielite", "http://openbim.org/schemas/cobieliteuk"),
                            new XmlQualifiedName("xsi", "http://www.w3.org/2001/XMLSchema-instance")
                        }));
                }
            }
        }

        public static Facility ReadXml(Stream stream)
        {
            var serializer = GetXmlSerializer();
            var facility = (Facility) serializer.Deserialize(stream);
            facility.SetFacility(facility);
            return facility;
        }

        public static Facility ReadXml(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var facility = ReadXml(stream);
                stream.Close();
                return facility;
            }
        }

        #endregion

        #region JSON serialization

        private static JsonSerializer GetJsonSerializer(bool indented = false)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
            serializerSettings.Converters.Add(new StringEnumConverter());
            serializerSettings.Converters.Add(new AttributeConverter());
            var serialiser = JsonSerializer.Create(serializerSettings);
            return serialiser;
        }

        public void WriteJson(Stream stream, bool indented = false)
        {
            using (var textWriter = new StreamWriter(stream))
            {
                var serialiser = GetJsonSerializer(indented);
                serialiser.Serialize(textWriter, this);
            }
        }

        public void WriteJson(string path, bool indented = false)
        {
            using (var stream = File.Create(path))
            {
                WriteJson(stream, indented);
                stream.Close();
            }
        }


        public static Facility ReadJson(Stream stream)
        {
            using (var textReader = new StreamReader(stream))
            {
                var serialiser = GetJsonSerializer();
                var facility = (Facility) serialiser.Deserialize(textReader, typeof (Facility));
                facility.SetFacility(facility);
                return facility;
            }
        }

        public static Facility ReadJson(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var facility = ReadJson(stream);
                stream.Close();
                return facility;
            }
        }

        #endregion

        #region Reading COBie Spreadsheet

        [SuppressMessage("ReSharper", "InconsistentNaming")] private StringWriter log = new StringWriter();

        public static Facility ReadCobie(string path, out string message, string version = "UK2012")
        {
            if (path == null) throw new ArgumentNullException("path");
            var ext = Path.GetExtension(path).ToLower().Trim('.');
            if (ext != "xls" && ext != "xlsx") throw new Exception("File must be an MS Excel file.");
            using (var file = File.OpenRead(path))
            {
                var type = ext == "xlsx" ? ExcelTypeEnum.XLSX : ExcelTypeEnum.XLS;
                var result = ReadCobie(file, type, out message, version);
                file.Close();
                return result;
            }
        }

        public static Facility ReadCobie(Stream stream, ExcelTypeEnum type, out string message,
            string version = "UK2012")
        {
            //refresh log for this run

            //use NPOI to open and access spreadsheet data
            IWorkbook workbook;
            switch (type)
            {
                case ExcelTypeEnum.XLS:
                    workbook = new HSSFWorkbook(stream);
                    break;
                case ExcelTypeEnum.XLSX:
                    workbook = new XSSFWorkbook(stream);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
            string msg;
            var flatList = ReadAllCobieObjects(workbook, out msg, version);
            message = msg ?? "";

            //create structure hierarchy
            foreach (var cobieObject in flatList)
            {
                cobieObject.AddToParent(flatList, out msg, version);
                message += msg;
            }


            //return first parent
            var facility = flatList.OfType<Facility>().FirstOrDefault();
            if (facility == null)
            {
                message = "There is no facility in the data. This is an invalid data structure. \n" + msg;
                return null;
            }


            message = msg;
            return facility;
        }

        private static List<CobieObject> ReadAllCobieObjects(IWorkbook workbook, out string message, string version)
        {
            var result = new List<CobieObject>();
            message = "";
            var types =
                typeof (CobieObject).Assembly.GetTypes()
                    .Where(t => !t.IsAbstract && typeof (CobieObject).IsAssignableFrom(t));
            foreach (var type in types)
            {
                string msg;
                result.AddRange(CobieObject.LoadFromCobie(type, workbook, out msg, version));
                message += msg;
            }
            return result;
        }

        #endregion

        internal override IEnumerable<CobieObject> GetChildren()
        {
            //enumerate base
            foreach (var child in base.GetChildren())
                yield return child;

            //enumerate own
            if (Floors != null)
                foreach (var floor in Floors)
                    yield return floor;
            if (AssetTypes != null)
                foreach (var assetType in AssetTypes)
                    yield return assetType;
            if (Contacts != null)
                foreach (var contact in Contacts)
                    yield return contact;
            if (Systems != null)
                foreach (var system in Systems)
                    yield return system;
            if (Zones != null)
                foreach (var zone in Zones)
                    yield return zone;
            if (Resources != null)
                foreach (var resource in Resources)
                    yield return resource;
            if (Stages != null)
                foreach (var stage in Stages)
                    yield return stage;
        }
    }

    public enum ExcelTypeEnum
    {
        // ReSharper disable InconsistentNaming
        XLS,
        XLSX
        // ReSharper restore InconsistentNaming
    }
}