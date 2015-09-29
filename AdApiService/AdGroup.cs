using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.DirectoryServices.AccountManagement;

namespace AdApiService
{
    [DataContract]
    public class AdGroup
    {

        #region constructors

        public AdGroup()
        {
            this.Description = String.Empty;
            this.DisplayName = String.Empty;
            this.DistinguishedName = String.Empty;
            this.GroupScope = String.Empty;
            this.Guid = String.Empty;
            this.IsSecurityGroup = false;
            this.Members = null;
            this.Name = String.Empty;
            this.SamAccountName = String.Empty;
            this.Sid = String.Empty;
            this.UserPrincipalName = String.Empty;
        }

        public AdGroup(GroupPrincipal group)
        {
            this.Description = group.Description;
            this.DisplayName = group.DisplayName;
            this.DistinguishedName = group.DistinguishedName;
            this.GroupScope = group.GroupScope.ToString();
            this.Guid = group.Guid.ToString();
            this.IsSecurityGroup = group.IsSecurityGroup;
            this.Members = AdMember.GetAdMembers(group.Members);
            this.Name = group.Name;
            this.SamAccountName = group.SamAccountName;
            this.Sid = group.Sid.Value;
            this.UserPrincipalName = group.UserPrincipalName;
        }

        #endregion constructors

        #region DataMembers

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string DistinguishedName { get; set; }

        [DataMember]
        public string GroupScope { get; set; }

        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public Boolean? IsSecurityGroup { get; set; }

        [DataMember]
        public List<AdMember> Members { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string SamAccountName { get; set; }

        [DataMember]
        public string Sid { get; set; }

        [DataMember]
        public string UserPrincipalName { get; set; }

        #endregion DataMembers

        #region methods

        /// <summary>
        /// This method returns a list of AdGroup objects that are compiled from the specified list
        /// of GroupPrincipal objects.
        /// </summary>
        /// <param name="groups"></param>
        /// <returns>A list of AdGroup objects.</returns>
        public static List<AdGroup> GetAdGroups(List<GroupPrincipal> groups)
        {
            List<AdGroup> results = new List<AdGroup>();
            
            foreach (GroupPrincipal g in groups)
            {
                results.Add(new AdGroup(g));
            }

            return results;
        }

        /// <summary>
        /// This method returns a list of SamAccountNames compiled from the specified list
        /// of GroupPrincipal objects.
        /// </summary>
        /// <param name="groups"></param>
        /// <returns>A list of strings that are group SamAccountNames.</returns>
        public static List<string> GetAdGroupsSamAccountNames(List<GroupPrincipal> groups) 
        {
            return GetAdGroups(groups).Select(x => x.SamAccountName).OrderBy(x => x).ToList();
        }

        public static List<string> GetAdGroupSamAccountNames(PrincipalSearchResult<Principal> groups)
        {
            List<string> results = new List<string>();

            foreach (Principal p in groups)
            {
                results.Add(p.SamAccountName);
            }

            return results;
        }

        /// <summary>
        /// Override to string method.
        /// </summary>
        /// <returns>A string representing this AdGroup object.</returns>
        public new string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}", this.SamAccountName, this.DisplayName, this.DistinguishedName, this.Description);
        }

        #endregion methods
    }
}