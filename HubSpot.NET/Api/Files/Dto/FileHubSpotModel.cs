﻿using System;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Files.Dto
{
    /// <summary>
    /// Models a file, used for the COS files API
    /// </summary>
    public class FileHubSpotModel : IHubSpotModel
    {
        [DataMember(Name="id")]
        public long Id { get;set; }

        [IgnoreDataMember]
        public bool Overwrite { get; set; }

        [IgnoreDataMember]
        public bool Hidden { get;set; }

        [IgnoreDataMember]
        public byte[] File { get;set; }

        [DataMember(Name="name")]
        public string Name { get;set; }

        [DataMember(Name="type")]
        public string Type { get; set; }

        [DataMember(Name="folder_paths")]
        public string FolderPaths { get; set; }

        public bool IsNameValue { get; }

        public void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
        }

        public void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }

        public string HubSpotObjectType => throw new NotImplementedException();
        public string HubSpotObjectTypePlural => throw new NotImplementedException();
        
        public string RouteBasePath => "/filemanager/api/v2";
    }
}
