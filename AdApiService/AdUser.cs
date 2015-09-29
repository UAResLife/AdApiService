using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.DirectoryServices.AccountManagement;

namespace AdApiService
{

    [DataContract]
    public class AdUser
    {

        #region constructors

        /// <summary>
        /// Default constructor that initializes attributes with empty values.
        /// </summary>
        public AdUser()
        {
            this.AccountExpirationData = String.Empty;
            this.AccountLockoutTime = String.Empty;
            this.AllowReversiblePasswordEncryption = false;
            this.BadLogonCount = 0;
            this.Company = String.Empty;
            this.DelegationPermitted = false;
            this.Department = String.Empty;
            this.Description = String.Empty;
            this.DisplayName = String.Empty;
            this.DistinguishedName = String.Empty;
            this.EmailAddress = String.Empty;
            this.EmployeeId = String.Empty;
            this.Enabled = false;
            this.GivenName = String.Empty;
            this.Guid = String.Empty;
            this.HomeDirectory = String.Empty;
            this.HomeDrive = String.Empty;
            this.LastBadPasswordAttempt = String.Empty;
            this.LastLogon = String.Empty;
            this.LastPasswordSet = String.Empty;
            this.Manager = String.Empty;
            this.MemberOf = null;
            this.MiddleName = String.Empty;
            this.Mobile = String.Empty;
            this.Name = String.Empty;
            this.PasswordNeverExpires = false;
            this.PasswordNotRequired = false;
            this.ProfilePath = String.Empty;
            this.SamAccountName = String.Empty;
            this.ScriptPath = String.Empty;
            this.Sid = String.Empty;
            this.SmartCardLogonRequired = false;
            this.Surname = String.Empty;
            this.UserCannotChangePassword = false;
            this.UserPrincipalName = String.Empty;
            this.VoiceTelephoneNumber = String.Empty;
        }

        /// <summary>
        /// Constructor that instantiates an AdUser object with the values of the specified
        /// UserPrincipal's attributes.
        /// </summary>
        /// <param name="user"></param>
        public AdUser(UserPrincipal user)
        {
            this.AccountExpirationData = user.AccountExpirationDate.ToString();
            this.AccountLockoutTime = user.AccountLockoutTime.ToString();
            this.AllowReversiblePasswordEncryption = user.AllowReversiblePasswordEncryption;
            this.BadLogonCount = user.BadLogonCount;
            this.Company = GetCompany(user);
            this.DelegationPermitted = user.DelegationPermitted;
            this.Department = GetDepartment(user);
            this.Description = user.Description;
            this.DisplayName = user.DisplayName;
            this.DistinguishedName = user.DistinguishedName;
            this.EmailAddress = user.EmailAddress;
            this.EmployeeId = user.EmployeeId;
            this.Enabled = user.Enabled;
            this.GivenName = user.GivenName;
            this.Guid = user.Guid.ToString();
            this.HomeDirectory = user.HomeDirectory;
            this.HomeDrive = user.HomeDrive;
            this.LastBadPasswordAttempt = user.LastBadPasswordAttempt.ToString();
            this.LastLogon = user.LastLogon.ToString();
            this.LastPasswordSet = user.LastPasswordSet.ToString();
            this.Manager = GetManager(user);
            this.MemberOf = AdGroup.GetAdGroupSamAccountNames(user.GetGroups());
            this.MiddleName = user.MiddleName;
            this.Mobile = GetMobile(user);
            this.Name = user.Name;
            this.PasswordNeverExpires = user.PasswordNeverExpires;
            this.PasswordNotRequired = user.PasswordNotRequired;
            this.ProfilePath = AdToolkit.GetProperty(user, "profilePath");
            this.SamAccountName = user.SamAccountName;
            this.ScriptPath = user.ScriptPath;
            this.Sid = user.Sid.Value;
            this.SmartCardLogonRequired = user.SmartcardLogonRequired;
            this.Surname = user.Surname;
            this.UserCannotChangePassword = user.UserCannotChangePassword;
            this.UserPrincipalName = user.UserPrincipalName;
            this.VoiceTelephoneNumber = user.VoiceTelephoneNumber;

            //user.GetGroups
        }

        #endregion constructors

        #region DataMembers

        [DataMember]
        public string AccountExpirationData { get; set; }

        [DataMember]
        public string AccountLockoutTime { get; set; }

        [DataMember]
        public Boolean AllowReversiblePasswordEncryption { get; set; }

        [DataMember]
        public int BadLogonCount { get; set; }

        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public Boolean DelegationPermitted { get; set; }

        [DataMember]
        public string Department { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string DistinguishedName { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public string EmployeeId { get; set; }

        [DataMember]
        public Boolean? Enabled { get; set; }

        [DataMember]
        public string GivenName { get; set; }

        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public string HomeDirectory { get; set; }

        [DataMember]
        public string HomeDrive { get; set; }

        [DataMember]
        public string LastBadPasswordAttempt { get; set; }

        [DataMember]
        public string LastLogon { get; set; }

        [DataMember]
        public string LastPasswordSet { get; set; }

        [DataMember]
        public string Manager { get; set; }

        [DataMember]
        public List<string> MemberOf { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string Mobile { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Boolean PasswordNeverExpires { get; set; }

        [DataMember]
        public Boolean PasswordNotRequired { get; set; }

        [DataMember]
        public string ProfilePath { get; set; }

        [DataMember]
        public string SamAccountName { get; set; }

        [DataMember]
        public string ScriptPath { get; set; }

        [DataMember]
        public string Sid { get; set; }

        [DataMember]
        public Boolean SmartCardLogonRequired { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public Boolean UserCannotChangePassword { get; set; }

        [DataMember]
        public string UserPrincipalName { get; set; }

        [DataMember]
        public string VoiceTelephoneNumber { get; set; }

        #endregion DataMembers

        #region methods

        /// <summary>
        /// The method returns a list of AdUser objects that is compiled from a list
        /// of UserPrincipal objects.
        /// </summary>
        /// <param name="users"></param>
        /// <returns>A list of AdUser objects.</returns>
        public static List<AdUser> GetAdUsers(List<UserPrincipal> users)
        {
            List<AdUser> results = new List<AdUser>();

            foreach (UserPrincipal u in users)
            {
                results.Add(new AdUser(u));
            }

            return results;
        }

        /// <summary>
        /// This method returns a list of user name that is compilied from a list of
        /// UserPrincipal objects. The names returned are the values of the UserPrincipal's
        /// Name attribute.
        /// </summary>
        /// <param name="users"></param>
        /// <returns>A list of strings that are user names.</returns>
        public static List<string> GetAdUserNames(List<UserPrincipal> users)
        {
            return GetAdUsers(users).Select(x => x.Name).OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Thie method returns a list of user SamAccountNames that is compiled from a list of
        /// UserPrincipal objects. The SamAccountNames are the values of the UserPrincipal's
        /// SamAccountName attribute.
        /// </summary>
        /// <param name="users"></param>
        /// <returns>A list of strings that are SamAccountNames of users.</returns>
        public static List<string> GetAdUserSamAccountNames(List<UserPrincipal> users)
        {
            return GetAdUsers(users).Select(x => x.SamAccountName).OrderBy(x => x).ToList();
        }

        public static String GetCompany(Principal user)
        {
            return AdToolkit.GetProperty(user, "company");
        }

        public static String GetDepartment(Principal user)
        {
            return AdToolkit.GetProperty(user, "department");
        }

        public static string GetManager(Principal user)
        {
            string managerDn = AdToolkit.GetProperty(user, "manager");
            return AdToolkit.GetSamAccountName(managerDn);
        }

        public static string GetMobile(Principal user)
        {
            return AdToolkit.GetProperty(user, "mobile");
        }

        /// <summary>
        /// To string override function.
        /// </summary>
        /// <returns>A string representing this AdUser object.</returns>
        public new string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}", this.SamAccountName, this.DisplayName, this.DistinguishedName, this.Description);
        }

        #endregion methods
    }
}