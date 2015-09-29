using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.DirectoryServices.AccountManagement;

namespace AdApiService
{
    [DataContract]
    public class AdComputer
    {

        #region constructors

        /// <summary>
        /// Default constructor that initializes attributes with empty values.
        /// </summary>
        public AdComputer()
        {
            this.AccountExpirationDate = String.Empty;
            this.AccountLockouttime = String.Empty;
            this.BadLogonCount = 0;
            this.DelegationPermitted = false;
            this.Description = String.Empty;
            this.DisplayName = String.Empty;
            this.DistinguishedName = String.Empty;
            this.Enabled = false;
            this.Guid = String.Empty;
            this.HomeDirectory = String.Empty;
            this.HomeDrive = String.Empty;
            this.LastBadPasswordAttempt = String.Empty;
            this.Name = String.Empty;
            this.PasswordNeverExpires = false;
            this.PasswordNotRequired = false;
            this.SamAccountName = String.Empty;
            this.ScriptPath = String.Empty;
            this.ServicePrincipalNames = null;
            this.Sid = String.Empty;
            this.SmartcardLogonRequired = false;
            this.UserCannotChangePassword = false;
            this.UserPrincipalName = String.Empty;
        }

        /// <summary>
        /// Constructor that instantiates an AdComputer object with values of the attributes of the
        /// specified ComputerPrincipal object.
        /// </summary>
        /// <param name="computer"></param>
        public AdComputer(ComputerPrincipal computer)
        {
            this.AccountExpirationDate = computer.AccountExpirationDate.ToString();
            this.AccountLockouttime = computer.AccountLockoutTime.ToString();
            this.BadLogonCount = computer.BadLogonCount;
            this.DelegationPermitted = computer.DelegationPermitted;
            this.Description = computer.Description;
            this.DisplayName = computer.DisplayName;
            this.DistinguishedName = computer.DistinguishedName;
            this.Enabled = computer.Enabled;
            this.Guid = computer.Guid.ToString();
            this.HomeDirectory = computer.HomeDirectory;
            this.HomeDrive = computer.HomeDrive;
            this.LastBadPasswordAttempt = computer.LastBadPasswordAttempt.ToString();
            this.Name = computer.Name;
            this.PasswordNeverExpires = computer.PasswordNeverExpires;
            this.PasswordNotRequired = computer.PasswordNotRequired;
            this.SamAccountName = computer.SamAccountName;
            this.ScriptPath = computer.ScriptPath;

            List<string> svcPNs = new List<string>();
            foreach (var spn in computer.ServicePrincipalNames)
            {
                svcPNs.Add(spn);
            }
            this.ServicePrincipalNames = svcPNs;

            this.Sid = computer.Sid.Value;
            this.SmartcardLogonRequired = computer.SmartcardLogonRequired;
            this.UserCannotChangePassword = computer.UserCannotChangePassword;
            this.UserPrincipalName = computer.UserPrincipalName;
        }

        #endregion constructors

        #region DataMembers

        [DataMember]
        public string AccountExpirationDate { get; set; }

        [DataMember]
        public string AccountLockouttime { get; set; }

        [DataMember]
        public int BadLogonCount { get; set; }

        [DataMember]
        public Boolean DelegationPermitted { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string DistinguishedName { get; set; }

        [DataMember]
        public Boolean? Enabled { get; set; }

        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public string HomeDirectory { get; set; }

        [DataMember]
        public string HomeDrive { get; set; }

        [DataMember]
        public string LastBadPasswordAttempt { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Boolean PasswordNeverExpires { get; set; }

        [DataMember]
        public Boolean PasswordNotRequired { get; set; }

        [DataMember]
        public string SamAccountName { get; set; }

        [DataMember]
        public string ScriptPath { get; set; }

        [DataMember]
        public List<string> ServicePrincipalNames { get; set; }

        [DataMember]
        public string Sid { get; set; }

        [DataMember]
        public Boolean SmartcardLogonRequired { get; set; }

        [DataMember]
        public Boolean UserCannotChangePassword { get; set; }

        [DataMember]
        public string UserPrincipalName { get; set; }

        #endregion DataMembers

        #region methods

        /// <summary>
        /// This method returns a list of AdComputer objects that are compiled from the specified
        /// list of ComputerPrincipal objects.
        /// </summary>
        /// <param name="computers"></param>
        /// <returns>A list of AdComputerObjects.</returns>
        public static List<AdComputer> GetAdComputers(List<ComputerPrincipal> computers)
        {
            List<AdComputer> results = new List<AdComputer>();

            foreach (ComputerPrincipal c in computers)
            {
                results.Add(new AdComputer(c));
            }

            return results;
        }

        /// <summary>
        /// Thie method returns a list of user SamAccountNames that is compiled from a list of
        /// ComputerPrincipal objects. The SamAccountNames are the values of the ComputerPrincipals'
        /// SamAccountName attribute.
        /// </summary>
        /// <param name="computers"></param>
        /// <returns>A list of strings that are the SamAccountNames of computers.</returns>
        public static List<string> GetAdComputerSamAccountNames(List<ComputerPrincipal> computers)
        {
            return GetAdComputers(computers).Select(x => x.SamAccountName).OrderBy(x => x).ToList();
        }

        /// <summary>
        /// This method returns a list of strings that are the names of the specified ComputerPrincipals.
        /// </summary>
        /// <param name="computers"></param>
        /// <returns>A list of strings that are computer names.</returns>
        public static List<string> GetAdComputerNames(List<ComputerPrincipal> computers)
        {
            return GetAdComputers(computers).Select(x => x.Name).OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Override to string method.
        /// </summary>
        /// <returns>A string that represents this AdComputer object.</returns>
        public new string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}", this.SamAccountName, this.DisplayName, this.DistinguishedName, this.Description);
        }

        #endregion methods

    }
}